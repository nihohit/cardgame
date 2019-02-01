using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ISceneViewModel {
	#region inputs
	void setDoneButtonClicked(IObservable<Unit> observable);
	void setDriveButtonClicked(IObservable<Unit> observable);
	void setStayButtonClicked(IObservable<Unit> observable);
	void setSelectedCardObservation(IObservable<Card> observable);
	void setDeckWasClicked(IObservable<Unit> observable);
	#endregion

	#region outputs
	IObservable<string> MainTextContent { get; }
	IObservable<string> PopulationValue { get; }
	IObservable<string> FuelValue { get; }
	IObservable<string> MaterialsValue { get; }
	IObservable<string> ArmyValue { get; }
	IObservable<int> DeckCount { get; }
	IObservable<int> DiscardPileCount { get; }
	IObservable<bool> DisplayDoneButton { get; }
	IObservable<bool> DisplayStayButton { get; }
	IObservable<bool> DisplayDriveButton { get; }
	IObservable<IEnumerable<CardDisplayModel>> CardsInMultiDisplay { get; }
	IObservable<string> TextForMultiDisplay { get; }
	IObservable<string> TextForDoneButton { get; }
	IObservable<Unit> HideMultiDisplay { get; }
	IObservable<IEnumerable<CardMovementInstruction>> CardMovementInstructions { get; }
	IObservable<IEnumerable<Tradition>> Traditions { get; }
	IObservable<IReadOnlyList<TrainCar>> Train { get; }
	IObservable<bool> ShowEndGameScreen { get; }
	#endregion
}

public class SceneViewModel : ISceneViewModel {
	private readonly ISceneModel model;

	#region ISceneViewModel
	#region inputs
	public void setDoneButtonClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.UserFinishedMode());
	}

	public void setSelectedCardObservation(IObservable<Card> observable) {
		observable.Subscribe(cardWasClicked);
	}

	private void cardWasClicked(Card card) {
		model.PlayCard(card);
	}

	public void setDeckWasClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.TryDrawCard());
	}

	public void setDriveButtonClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.UserChoseToDrive());
	}

	public void setStayButtonClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.UserFinishedMode());
	}

	#endregion
	#region outputs

	public IObservable<string> MainTextContent => model.State
		.CombineLatest(model.PopulationDied, locationsDescription);

	public IObservable<string> PopulationValue => model.State
		.Select(state => $"{state.Train.AvailablePopulation}/{state.Train.TotalPopulation}/{state.Train.LivingSpace}");

	public IObservable<string> FuelValue => model.State
		.Select(state => $"{state.Train.Fuel} ({state.Train.FuelConsumption})");

	public IObservable<string> MaterialsValue => model.State
		.Select(state => $"{state.Train.Materials} ({state.Train.MaterialsConsumption()})");

	public IObservable<string> ArmyValue => model.State
		.Select(state => state.Train.Army.ToString());

	private string locationsDescription(SceneState state, bool populationDied) {
		var end = populationDied ? "\nSome population left you for lack of materials" : "";
		return
			$"{locationDescription(state.Train.CurrentLocation)}\n" +
			$"Next: {locationDescription(state.Train.NextLocation)}" +
			end;
	}

	private string locationContentDescription(LocationContent content) {
		switch (content) {
			case LocationContent.Test:
				break;
			case LocationContent.TrainWreck:
				return "wrecked train";
			case LocationContent.Howitizer:
				return "howitzer";
			case LocationContent.Armory:
				return "armory";
			case LocationContent.Workhouse:
				return "old workshop";
			case LocationContent.OldHouses:
				return "old houses";
			case LocationContent.FuelRefinery:
				return "refinery";
			case LocationContent.Woods:
				return "woods";
			case LocationContent.WildAnimals:
				return "wild animals";
			case LocationContent.LivingPeople:
				return "small settlement";
			case LocationContent.FuelStorage:
				return "fuel storage";
			case LocationContent.Storehouse:
				return "storehouse";
			case LocationContent.Mine:
				return "mines";
			case LocationContent.ArmyBase:
				return "Abandoned Army Base";
		}

		return "";
	}

	private string locationDescription(Location location) {
		var contentDescription = location.Content
			.Distinct()
			.Select(locationContentDescription)
			.ToJoinedString(", ");
		return $"{location.Name} with {contentDescription}";
	}

	public IObservable<int> DeckCount => model.State
		.Select(state => state.Cards.CurrentDeck.Count());

	public IObservable<int> DiscardPileCount => model.State
		.Select(state => state.Cards.DiscardPile.Count());

	public IObservable<IEnumerable<Card>> Hand => model.State
		.Select(state => state.Cards.Hand);

	public IObservable<bool> DisplayDoneButton => model.State
		.Select(state => state.Mode != CardHandlingMode.Event &&
						!state.Train.CanDrive());
	
	public IObservable<bool> DisplayDriveButton => model.State
		.Select(state => state.Mode != CardHandlingMode.Event &&
						state.Train.CanDrive());
	
	public IObservable<bool> DisplayStayButton => model.State
		.Select(state => state.Mode != CardHandlingMode.Event &&
		                 state.Train.CanDrive());

	public IObservable<IEnumerable<CardDisplayModel>> CardsInMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(cardsInMultiCardDisplay);

	// TODO: Move using CardMovementInstructions.
	private IEnumerable<CardDisplayModel> cardsInMultiCardDisplay(SceneState state) {
		if (state.Mode == CardHandlingMode.Event) {
			return state.CurrentEvent.Options.Select(card => displayModel(card));
		}

		return state.Cards.Hand.Select(card => displayModel(card, state.Train));
	}

	public IObservable<string> TextForMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(stateMultiCardDescription);

	private string stateMultiCardDescription(SceneState state) {
		switch (state.Mode) {
			case CardHandlingMode.Exhaust:
				return "Choose cards to exhaust";
			case CardHandlingMode.Discard:
				return "Choose cards to discard";
			case CardHandlingMode.Event:
				return state.CurrentEvent.Name;
			default:
				AssertUtils.UnreachableCode();
				break;
		}
		return null;
	}

	public IObservable<string> TextForDoneButton => model.State
		.Select(state => state.Mode == CardHandlingMode.Regular ? "Stay" : "Done");

	public IObservable<Unit> HideMultiDisplay => model.State
		.Select(state => state.Mode)
		.DistinctUntilChanged()
		.Where(mode => mode == CardHandlingMode.Regular)
		.Select(_ => Unit.Default);

	public IObservable<IEnumerable<CardMovementInstruction>> CardMovementInstructions => model.State
		.StartWith<SceneState>((SceneState)null)
		.DistinctUntilChanged()
		.Pairwise()
		.SelectMany(pair => movementsFromStateDifferences(pair));

	private IObservable<IEnumerable<CardMovementInstruction>> movementsFromStateDifferences(Pair<SceneState> pair) {
		var previousCards = pair.Previous != null ? pair.Previous.Cards : CardsState.NewState(new Card[0]);
		var currentHand = pair.Current.Cards.Hand.ToList();
		var previousHand = previousCards.Hand.ToList();
		return Observable.Create<IEnumerable<CardMovementInstruction>>(obs => {
			// Discarding cards from hand and in-hand movements.
			var initialMovements = new List<CardMovementInstruction>();
			// Cards added to discard and draw piles.
			var midStepMovements = new List<CardMovementInstruction>();
			// Cards added to hand.
			var lastMovements = new List<CardMovementInstruction>();
			Func<Card, CardDisplayModel> makeDisplayModel = card => displayModel(card, pair.Current.Train);
			var discardedCards = new List<Card>();
			var cardsMovedFromDiscardToDeck = new List<Card>();
			var currentHandIndicesToCheck = Enumerable.Range(0, currentHand.Count).ToList();
			for(int i = 0; i < previousHand.Count; i++) {
				var card = previousHand[i];
				var currentIndex = currentHand.FindIndex(currentCard => currentCard == card);
				if (currentIndex == -1) {
					initialMovements.Add(new CardMovementInstruction(makeDisplayModel(card), 
						handLocationFromIndex(i), 
						ScreenLocation.DiscardPile,
						currentHand.Count));
					discardedCards.Add(card);
					continue;
				}
				currentHandIndicesToCheck.Remove(currentIndex);

				initialMovements.Add(new CardMovementInstruction(makeDisplayModel(card), 
					handLocationFromIndex(i), 
					handLocationFromIndex(currentIndex),
					currentHand.Count));
			}
			if (initialMovements.Count > 0) {
				obs.OnNext(initialMovements);
			}

			var newDiscardedCards = pair.Current.Cards.DiscardPile.ToList();
			foreach (var card in discardedCards) {
				newDiscardedCards.Remove(card);
			}
			foreach(var card in previousCards.DiscardPile) {
				if (!newDiscardedCards.Remove(card)) {
					cardsMovedFromDiscardToDeck.Add(card);
				}
			}
			foreach(var card in newDiscardedCards) {
				midStepMovements.Add(new CardMovementInstruction(makeDisplayModel(card), 
					ScreenLocation.Center, 
					ScreenLocation.DiscardPile,
					currentHand.Count));
			}

			var previousDeck = previousCards.CurrentDeck.ToList();
			foreach (var card in pair.Current.Cards.CurrentDeck) {
				if (previousDeck.FindIndex(previousCard => previousCard == card) != -1) {
					continue;
				}
				if (cardsMovedFromDiscardToDeck.FindIndex(discardedCard => discardedCard == card) != -1) {
					midStepMovements.Add(new CardMovementInstruction(displayModel(card, pair.Current.Train), 
					ScreenLocation.DiscardPile, 
					ScreenLocation.Deck,
					currentHand.Count));
					continue;
				}
				midStepMovements.Add(new CardMovementInstruction(makeDisplayModel(card), 
					ScreenLocation.Center, 
					ScreenLocation.Deck,
					currentHand.Count));
			}
			if (midStepMovements.Count > 0) {
				obs.OnNext(midStepMovements);
			}

			foreach (var index in currentHandIndicesToCheck) {
				var card = currentHand[index];
				lastMovements.Add(new CardMovementInstruction(makeDisplayModel(card), 
					ScreenLocation.Deck, 
					handLocationFromIndex(index),
					currentHand.Count));
			}
			if (lastMovements.Count > 0) {
				obs.OnNext(lastMovements);
			}

			return null;
		});
	}

	private CardDisplayModel displayModel(Card card, TrainState state) {
		if (state.Cars
					.Select(car => car.Type)
					.Any(carType => carType == card.ModifiedByCar)) {
			card = card
				.CopyWithModifiedValues(card.CarModifications)
				.CopyWithSource(card);
		}
		
		return displayModel(card);
	}

	private CardDisplayModel displayModel(Card card) {
		return new CardDisplayModel(card, model.CanPlayCard(card));
	}

	private ScreenLocation handLocationFromIndex(int index) {
		switch (index) {
			case (0): return ScreenLocation.Hand1;
			case (1): return ScreenLocation.Hand2;
			case (2): return ScreenLocation.Hand3;
			case (3): return ScreenLocation.Hand4;
			case (4): return ScreenLocation.Hand5;
			case (5): return ScreenLocation.Hand6;
			case (6): return ScreenLocation.Hand7;
			case (7): return ScreenLocation.Hand8;
			case (8): return ScreenLocation.Hand9;
		}
		throw new ArgumentOutOfRangeException($"Received unhandled index: {index}");
	}

	public IObservable<IEnumerable<Tradition>> Traditions => model.State
		.Select(state => state.Cards.Traditions)
		.StartWith(new Tradition[0])
		.DistinctUntilChanged();

	public IObservable<IReadOnlyList<TrainCar>> Train => model.State
		.Select(state => state.Train.Cars);

	public IObservable<bool> ShowEndGameScreen => model.State
		.Select(state => state.Train.TotalPopulation <= 0);
	#endregion
	#endregion

	public SceneViewModel() : this(SceneModel.InitialSceneModel()) { }

	public SceneViewModel(ISceneModel model) {
		this.model = model;
	}
}
