// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public enum CardHandlingMode { Regular, Event, Replace, Exhaust }

public class SceneState {
	public CardsState Cards { get; }
	public EmpireState Empire { get; }
	public CardHandlingMode Mode { get; }
	public EventCard CurrentEvent { get; }

	public SceneState(CardsState cards, 
		EmpireState empire, 
		CardHandlingMode mode,
		EventCard currentEvent) {
		Cards = cards;
		Empire = empire;
		Mode = mode;
		CurrentEvent = currentEvent;
	}
}

public interface ISceneModel {
	IObservable<SceneState> State { get; }

	bool PlayCard(Card card);
	void UserFinishedMode();
	void TryDrawCard();
}

public class SceneModel : ISceneModel {
	private EventCard nextEvent;
	private CardsState cards;
	private EmpireState empireState;
	private CardHandlingMode mode = CardHandlingMode.Regular;
	private readonly List<Card> playedCards = new List<Card>();
	private int cardsToHandle;
	private ReplaySubject<SceneState> stateSubject = new ReplaySubject<SceneState>(1);

	public SceneModel() : this(
		CardsState.NewState(CardsCollection.Cards())
			.ShuffleCurrentDeck()
			.DrawCardsToHand(Constants.MAX_CARDS_IN_HAND),
		new EmpireState(1, 1, 2, 0, new List<Card>()), 
		CardHandlingMode.Regular) {}

	public SceneModel(CardsState cards, EmpireState empireState, 
		CardHandlingMode mode) {
		nextEvent = EventCardsCollections.EventCardForState(empireState);
		this.cards = cards;
		this.empireState = empireState;
		this.mode = mode;
		EventUtils.LogStartTurnEvent(eventCardName(), empireState.ToString(), cards.Hand);
		sendCompletedState();
	}

	#region ISceneModel
	public IObservable<SceneState> State => stateSubject;

	public bool PlayCard(Card card) {
		bool succeeded = false;
		switch (mode) {
			case CardHandlingMode.Regular:
				succeeded = playRegularCard(card);
				break;
			case CardHandlingMode.Event:
				succeeded = playEventCard(card);
				break;
			case CardHandlingMode.Replace:
				succeeded = replaceCard(card);
				break;
			case CardHandlingMode.Exhaust:
				succeeded = exhaustCard(card);
				break;
		}
		if (succeeded) {
			sendCompletedState();
		}
		return succeeded;
	}

	private bool playRegularCard(Card card) {
		if (!empireState.CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return false;
		}
		empireState = empireState.PlayCard(card);
		cards = cards.PlayCard(card);
		playedCards.Add(card);
		switchModeAccordingToCard(card);
		return true;
	}

	private void switchModeAccordingToCard(Card card) {
		if (card.NumberOfCardsToChooseToExhaust > 0) {
			mode = CardHandlingMode.Exhaust;
			cardsToHandle = card.NumberOfCardsToChooseToExhaust;
		} else if (card.NumberOfCardsToChooseToReplace > 0) {
			mode = CardHandlingMode.Replace;
			cardsToHandle = card.NumberOfCardsToChooseToReplace;
		}
	}

	private bool playEventCard(Card card) {
		if (!empireState.CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return false;
		}
		empireState = empireState.PlayCard(card);
		empireState = empireState.NextTurnState();
		EventUtils.LogEventCardPlayed(card, eventCardName(), empireState.ToString(), cards.Hand);
		mode = CardHandlingMode.Regular;
		startTurn();
		return true;
	}

	private bool replaceCard(Card card) {
		cards = cards
			.DiscardCardFromHand(card)
			.DrawCardsToHand(1);
		changeModeIfNeeded();
		return true;
	}

	private bool exhaustCard(Card card) {
		cards = cards.ExhaustCardFromHand(card);
		changeModeIfNeeded();
		return true;
	}

	private void changeModeIfNeeded() {
		cardsToHandle--;
		if (cardsToHandle != 0) {
			return;
		}
		mode = CardHandlingMode.Regular;
	}

	private void startTurn() {
		drawNewHand();
		empireState = empireState.NextTurnState();
		EventUtils.LogStartTurnEvent(eventCardName(), empireState.ToString(), cards.Hand);
		playedCards.Clear();
	}

	private void drawNewHand() {
		var remainingCards = Math.Max(Constants.MAX_CARDS_IN_HAND - cards.CurrentDeck.Count(), 0);
		cards = cards.DrawCardsToHand(Constants.MAX_CARDS_IN_HAND - remainingCards);
		if (remainingCards > 0) {
			cards = cards.ShuffleDiscardToDeck()
				.DrawCardsToHand(remainingCards);
		}
	}

	public void UserFinishedMode() {
		switch (mode) {
			case CardHandlingMode.Exhaust:
			case CardHandlingMode.Replace:
				mode = CardHandlingMode.Regular;
				break;
			case CardHandlingMode.Regular:
				endTurn();
				break;
			default:
				AssertUtils.UnreachableCode($"Illegal mode: {mode}");
				break;
		}
		sendCompletedState();
	}

	private void endTurn() { 
		empireState = empireState.NextTurnState();
		EventUtils.LogEndTurnEvent(playedCards, eventCardName(), empireState.ToString(), cards.Hand);
		mode = CardHandlingMode.Event;
		nextEvent = EventCardsCollections.EventCardForState(empireState);
		cards = cards.DiscardHand();
	}

	public void TryDrawCard() {
		if (!canDrawCard()) {
			return;
		}

		cards = cards.DrawCardsToHand(1);
		empireState = empireState.ChangeGold(-1);
		sendCompletedState();
	}

	private bool canDrawCard() {
		return empireState.Gold > 0 &&
			cards.Hand.Count() < Constants.MAX_CARDS_IN_HAND &&
			cards.CurrentDeck.Count() > 0;
	}

	#endregion

	private void sendCompletedState() {
		stateSubject.OnNext(new SceneState(cards, empireState, mode, nextEvent));
	}

	private string eventCardName() {
		return nextEvent.Name;
	}
}
