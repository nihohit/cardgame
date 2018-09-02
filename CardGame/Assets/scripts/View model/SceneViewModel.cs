﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ISceneViewModel {
	#region inputs
	void setDoneButtonClicked(IObservable<Unit> observable);
	void setSelectedCardObservation(IObservable<Card> observable);
	void setDeckWasClicked(IObservable<Unit> observable);
	#endregion

	#region outputs
	IObservable<string> StateDescription { get; }
	IObservable<int> DeckCount { get; }
	IObservable<int> DiscardPileCount { get; }
	IObservable<bool> DisplayDoneButton { get; }
	IObservable<IEnumerable<Card>> CardsInMultiDisplay { get; }
	IObservable<string> TextForMultiDisplay { get; }
	IObservable<string> TextForDoneButton { get; }
	IObservable<Unit> HideMultiDisplay { get; }
	IObservable<CardMovementInstruction> CardMovementInstructions { get; }
	#endregion
}

public class SceneViewModel : ISceneViewModel {
	private ISceneModel model;

	#region ISceneViewModel
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

	public IObservable<string> StateDescription => model.State
		.Select(state => state.Empire.ToString());

	public IObservable<int> DeckCount => model.State
		.Select(state => state.Cards.CurrentDeck.Count());

	public IObservable<int> DiscardPileCount => model.State
		.Select(state => state.Cards.DiscardPile.Count());

	public IObservable<IEnumerable<Card>> Hand => model.State
		.Select(state => state.Cards.Hand);

	public IObservable<bool> DisplayDoneButton => model.State
		.Select(state => state.Mode != CardHandlingMode.Event);

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
		.Select(state => state.Mode == CardHandlingMode.Regular ? "End Turn" : "Done");

	public IObservable<Unit> HideMultiDisplay => model.State
		.Select(state => state.Mode)
		.DistinctUntilChanged()
		.Where(mode => mode == CardHandlingMode.Regular)
		.Select(_ => Unit.Default);

	public IObservable<CardMovementInstruction> CardMovementInstructions => model.State
		.Select(state => state.Cards)
		.StartWith(CardsState.NewState(new Card[0]))
		.DistinctUntilChanged()
		.Pairwise()
		.SelectMany(pair => movementsFromStateDifferences(pair));

	private IObservable<CardMovementInstruction> movementsFromStateDifferences(Pair<CardsState> pair) {
		return Observable.Create<CardMovementInstruction>(obs => {
			var previousHand = pair.Previous.Hand.ToList();
			var currentHand = pair.Current.Hand.ToList();
			var discardedCards = new List<Card>();
			var cardsMovedFromDiscardToDeck = new List<Card>();
			var currentHandIndicesToCheck = Enumerable.Range(0, currentHand.Count).ToList();
			for(int i = 0; i < previousHand.Count; i++) {
				var card = previousHand[i];
				var currentIndex = currentHand.FindIndex(currentCard => currentCard == card);
				if (currentIndex == -1) {
					obs.OnNext(new CardMovementInstruction(card, handLocationFromIndex(i), ScreenLocation.DiscardPile));
					discardedCards.Add(card);
					continue;
				}
				currentHandIndicesToCheck.Remove(currentIndex);
				if (currentIndex == i) {
					continue;
				}

				obs.OnNext(new CardMovementInstruction(card, handLocationFromIndex(i), handLocationFromIndex(currentIndex)));
			}

			foreach (var index in currentHandIndicesToCheck) {
				var card = currentHand[index];
				obs.OnNext(new CardMovementInstruction(card, ScreenLocation.Deck, handLocationFromIndex(index)));
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
				obs.OnNext(new CardMovementInstruction(card, ScreenLocation.Center, ScreenLocation.DiscardPile));
			}

			var previousDeck = pair.Previous.CurrentDeck.ToList();
			foreach (var card in pair.Current.CurrentDeck) {
				if (previousDeck.FindIndex(previousCard => previousCard == card) != -1) {
					continue;
				}
				if (cardsMovedFromDiscardToDeck.FindIndex(discardedCard => discardedCard == card) != -1) {
					obs.OnNext(new CardMovementInstruction(card, ScreenLocation.DiscardPile, ScreenLocation.Deck));
					continue;
				}
				obs.OnNext(new CardMovementInstruction(card, ScreenLocation.Center, ScreenLocation.Deck));
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
	#endregion

	public SceneViewModel() : this(new SceneModel()) { }

	public SceneViewModel(ISceneModel model) {
		this.model = model;
	}
}
