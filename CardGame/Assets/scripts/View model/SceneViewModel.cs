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
	IObservable<string> StateDescription { get; }
	IObservable<int> DeckCount { get; }
	IObservable<int> DiscardPileCount { get; }
	IObservable<bool> DisplayDoneButton { get; }
	IObservable<bool> DisplayStayButton { get; }
	IObservable<bool> DisplayDriveButton { get; }
	IObservable<IEnumerable<Card>> CardsInMultiDisplay { get; }
	IObservable<string> TextForMultiDisplay { get; }
	IObservable<string> TextForDoneButton { get; }
	IObservable<Unit> HideMultiDisplay { get; }
	IObservable<IEnumerable<CardMovementInstruction>> CardMovementInstructions { get; }
	IObservable<IEnumerable<Tradition>> Traditions { get; }
	IObservable<IReadOnlyList<TrainCar>> Train { get; }
	#endregion
}

public class SceneViewModel : ISceneViewModel {
	private ISceneModel model;

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
	
	public IObservable<string> StateDescription => model.State
		.Select(state => stateDescription(state.Train));

	private string stateDescription(TrainState state) {
		return
			$"Population: {state.AvailablePopulation}/{state.TotalPopulation}\n" + 
			$"Fuel: {state.Fuel}\n" +
			$"Materials: {state.Materials}\n" +
			$"Army: {state.Army}";
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

	public IObservable<IEnumerable<Card>> CardsInMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(cardsInMultiCardDisplay);

	// TODO: Move using CardMovementInstructions.
	private IEnumerable<Card> cardsInMultiCardDisplay(SceneState state) {
		if (state.Mode == CardHandlingMode.Event) {
			return state.CurrentEvent.Options;
		}
		return state.Cards.Hand;
	}

	public IObservable<string> TextForMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(stateMultiCardDescription);

	private string stateMultiCardDescription(SceneState state) {
		switch (state.Mode) {
			case CardHandlingMode.Exhaust:
				return "Choose cards to exhaust";
			case CardHandlingMode.Replace:
				return "Choose cards to replace";
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
		.Select(state => state.Cards)
		.StartWith(CardsState.NewState(new Card[0]))
		.DistinctUntilChanged()
		.Pairwise()
		.SelectMany(pair => movementsFromStateDifferences(pair));

	private IObservable<IEnumerable<CardMovementInstruction>> movementsFromStateDifferences(Pair<CardsState> pair) {
		return Observable.Create<IEnumerable<CardMovementInstruction>>(obs => {
			// Discarding cards from hand and in-hand movements.
			var initialMovements = new List<CardMovementInstruction>();
			// Cards added to discard and draw piles.
			var midStepMovements = new List<CardMovementInstruction>();
			// Cards added to hand.
			var lastMovements = new List<CardMovementInstruction>();

			var previousHand = pair.Previous.Hand.ToList();
			var currentHand = pair.Current.Hand.ToList();
			var discardedCards = new List<Card>();
			var cardsMovedFromDiscardToDeck = new List<Card>();
			var currentHandIndicesToCheck = Enumerable.Range(0, currentHand.Count).ToList();
			for(int i = 0; i < previousHand.Count; i++) {
				var card = previousHand[i];
				var currentIndex = currentHand.FindIndex(currentCard => currentCard == card);
				if (currentIndex == -1) {
					initialMovements.Add(new CardMovementInstruction(card, handLocationFromIndex(i), ScreenLocation.DiscardPile));
					discardedCards.Add(card);
					continue;
				}
				currentHandIndicesToCheck.Remove(currentIndex);
				if (currentIndex == i) {
					continue;
				}

				initialMovements.Add(new CardMovementInstruction(card, handLocationFromIndex(i), handLocationFromIndex(currentIndex)));
			}
			if (initialMovements.Count > 0) {
				obs.OnNext(initialMovements);
			}

			var newDiscardedCards = pair.Current.DiscardPile.ToList();
			foreach (var card in discardedCards) {
				newDiscardedCards.Remove(card);
			}
			foreach(var card in pair.Previous.DiscardPile) {
				if (!newDiscardedCards.Remove(card)) {
					cardsMovedFromDiscardToDeck.Add(card);
				}
			}
			foreach(var card in newDiscardedCards) {
				midStepMovements.Add(new CardMovementInstruction(card, ScreenLocation.Center, ScreenLocation.DiscardPile));
			}

			var previousDeck = pair.Previous.CurrentDeck.ToList();
			foreach (var card in pair.Current.CurrentDeck) {
				if (previousDeck.FindIndex(previousCard => previousCard == card) != -1) {
					continue;
				}
				if (cardsMovedFromDiscardToDeck.FindIndex(discardedCard => discardedCard == card) != -1) {
					midStepMovements.Add(new CardMovementInstruction(card, ScreenLocation.DiscardPile, ScreenLocation.Deck));
					continue;
				}
				midStepMovements.Add(new CardMovementInstruction(card, ScreenLocation.Center, ScreenLocation.Deck));
			}
			if (midStepMovements.Count > 0) {
				obs.OnNext(midStepMovements);
			}

			foreach (var index in currentHandIndicesToCheck) {
				var card = currentHand[index];
				lastMovements.Add(new CardMovementInstruction(card, ScreenLocation.Deck, handLocationFromIndex(index)));
			}
			if (lastMovements.Count > 0) {
				obs.OnNext(lastMovements);
			}

			return null;
		});
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
	#endregion
	#endregion

	public SceneViewModel() : this(new SceneModel()) { }

	public SceneViewModel(ISceneModel model) {
		this.model = model;
	}
}
