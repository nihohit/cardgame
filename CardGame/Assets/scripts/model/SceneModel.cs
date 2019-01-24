﻿// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public class SceneState {
	public CardsState Cards { get; }
	public TrainState Train { get; }
	public CardHandlingMode Mode { get; }
	public EventCard CurrentEvent { get; }

	public SceneState(CardsState cards, 
		TrainState empire, 
		CardHandlingMode mode,
		EventCard currentEvent) {
		Cards = cards;
		Train = empire;
		Mode = mode;
		CurrentEvent = currentEvent;
	}
}

public interface ISceneModel {
	IObservable<SceneState> State { get; }
	IObservable<bool> PopulationDied { get; }

	bool CanPlayCard(Card card);
	bool PlayCard(Card card);
	void UserFinishedMode();
	void UserChoseToDrive();
	void TryDrawCard();
}

public class SceneModel : ISceneModel {
	private EventCard nextEvent;
	private CardsState cards;
	private TrainState trainState;
	private CardHandlingMode mode = CardHandlingMode.Regular;
	private readonly List<Card> playedCards = new List<Card>();
	private int cardsToHandle;
	private readonly ReplaySubject<SceneState> stateSubject = new ReplaySubject<SceneState>(1);
	private readonly IEnumerator<Location> locations;
	private Subject<bool> populationDiedSubject = new Subject<bool>();

	public static SceneModel InitialSceneModel() {
		var locationsEnumerator = new RandomLocationsGenerator().Locations().GetEnumerator();
		locationsEnumerator.MoveNext();
		var initialLocation = locationsEnumerator.Current;
		locationsEnumerator.MoveNext();
		var trainState = TrainState.InitialState(4, 6, 3, 1,
					initialLocation, 
			locationsEnumerator.Current);
		var initialCards = TrainCarsCardCollection.BaseCards(trainState.Cars.Select(car => car.Type));
		var cardState = CardsState.NewState(initialCards)
			.ShuffleCurrentDeck()
			.EnterLocation(trainState.CurrentLocation)
			.DrawCardsToHand(Constants.MAX_CARDS_IN_HAND);
		return new SceneModel(cardState, trainState, CardHandlingMode.Regular, locationsEnumerator);
	}

	public SceneModel(CardsState cards, TrainState trainState, 
		CardHandlingMode mode, IEnumerator<Location> locations) {
		this.locations = locations;
		nextEvent = EventCardsCollections.EventCardForState(trainState);
		this.cards = cards;
		this.trainState = trainState;
		this.mode = mode;
		EventUtils.LogStartTurnEvent(eventCardName(), trainState.ToString(), cards.Hand);
		sendCompletedState();
	}

	#region ISceneModel
	public IObservable<SceneState> State => stateSubject;

	public bool CanPlayCard(Card card) {
		return trainState.CanPlayCard(card);
	}

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
		if (!CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return false;
		}
		trainState = trainState.PlayCard(card);
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
		if (!CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return false;
		}
		trainState = trainState.PlayCard(card);
		EventUtils.LogEventCardPlayed(card, eventCardName(), trainState.ToString(), cards.Hand);
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
		var lastPopulationCount = trainState.TotalPopulation;
		trainState = trainState.NextTurnState();
		populationDiedSubject.OnNext(lastPopulationCount > trainState.TotalPopulation);
		EventUtils.LogStartTurnEvent(eventCardName(), trainState.ToString(), cards.Hand);
		playedCards.Clear();
	}

	private void drawNewHand() {
		var remainingCards = Math.Max(Constants.MAX_CARDS_IN_HAND - cards.CurrentDeck.Count(), 0);
		cards = cards
			.DiscardHand()
			.DrawCardsToHand(Constants.MAX_CARDS_IN_HAND - remainingCards);
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

	public void UserChoseToDrive() {
		AssertUtils.AssertConditionMet(trainState.CanDrive(), "Cannot drive");
		locations.MoveNext();
		var nextLocation = locations.Current;
		trainState = trainState.Drive(nextLocation);
		cards = cards
			.LeaveLocation()
			.EnterLocation(trainState.CurrentLocation);
		endTurn();
		sendCompletedState();
	}

	private void endTurn() { 
		EventUtils.LogEndTurnEvent(playedCards, eventCardName(), trainState.ToString(), cards.Hand);
		mode = CardHandlingMode.Event;
		nextEvent = EventCardsCollections.EventCardForState(trainState);
		cards = cards.DiscardHand();
	}

	public void TryDrawCard() {
		if (!canDrawCard()) {
			return;
		}

		drawNewHand();
		trainState = trainState.ChangeAvailablePopulation(-1);
		sendCompletedState();
	}

	private bool canDrawCard() {
		return trainState.AvailablePopulation > 0;
	}

	public IObservable<bool> PopulationDied {
		get { return populationDiedSubject.StartWith(false); }
	}

	#endregion

	private void sendCompletedState() {
		stateSubject.OnNext(new SceneState(cards, trainState, mode, nextEvent));
	}

	private string eventCardName() {
		return nextEvent.Name;
	}
}
