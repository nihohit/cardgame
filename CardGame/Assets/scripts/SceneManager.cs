using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {
	private enum CardHandlingMode {Regular, Event, Replace, Exhaust}

	public GameObject CardPrefab;
	public GameObject stateDescription;

	private MultiCardDisplayScript multiCardDisplay;
	private DeckScript deck;
	private DeckScript discardPile;
	private IEnumerable<CardScript> _hand = new List<CardScript>();
	private IEnumerable<CardScript> hand { get {
			return _hand;
		} set {
			_hand = value;

			Vector3 nextPosition = deck.transform.position + Vector3.right * deck.GetComponent<BoxCollider2D>().size.x / 2 * deck.transform.localScale.x;
			foreach(var card in value) {
				card.Manager = this;
				var size = card.GetComponent<BoxCollider2D>().size * card.transform.localScale.x;
				card.transform.position = nextPosition + new Vector3(size.x / 2, 0, 0);
				nextPosition += new Vector3(size.x, 0, 0);
			}
		}
	}
	private CardScriptPool cardPool;
	private CardsState _cards;
	private CardsState cards { get {
			return _cards;
		} set {
			_cards = value;
			deck.SetCardNumber(value.CurrentDeck.Count());
			discardPile.SetCardNumber(value.DiscardPile.Count());
			foreach (var card in _hand) {
				cardPool.ReleaseCard(card);
			}
			hand = value.Hand.Select(cardModel => cardPool.CardForModel(cardModel)).ToList();
		}
	}
	private EmpireState _state;
	private EmpireState state {
		get {
			return _state;
		}
		set {
			_state = value;
			stateDescription.GetComponent<Text>().text = state.ToString();
		}
	}
	private List<EventCard> events;
	private List<Card> playedCards = new List<Card>();
	private CardHandlingMode handlingMode;
	private Button doneButton;
	private int cardsToHandle;

	// Use this for initialization
	void Start() {
		state = new EmpireState(1, 1, 2, 0);
		cardPool = new CardScriptPool(CardPrefab, 15);
		deck = GameObject.Find("Deck").GetComponent<DeckScript>();
		deck.Manager = this;
		discardPile = GameObject.Find("Discard Pile").GetComponent<DeckScript>();
		discardPile.Manager = this;
		cards = CardsState.NewState(CardsCollection.Cards())
			.ShuffleCurrentDeck()
			.DrawCardsToHand(Constants.MAX_CARDS_IN_HAND);
		multiCardDisplay = Resources.FindObjectsOfTypeAll<MultiCardDisplayScript>()[0];
		multiCardDisplay.InitialSetup(this, cardPool);
		events = EventCardsCollections.Cards().ToList();
		EventUtils.LogStartTurnEvent(eventCardName(), state.ToString(), cards.Hand);
		doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
	}

	public void DeckWasClicked(DeckScript clickedDeck) {
		if (handlingMode != CardHandlingMode.Regular) {
			return;
		}

		if (clickedDeck == deck) {
			tryDrawCard();
		}
	}

	private void tryDrawCard() {
		if (!canDrawCard()) {
			return;
		}

		cards = cards.DrawCardsToHand(1);
		state = state.ChangeGold(-1);
	}

	private bool canDrawCard() {
		return state.Gold > 0 &&
			cards.Hand.Count() < Constants.MAX_CARDS_IN_HAND &&
			cards.CurrentDeck.Count() > 0;
	}

	public void CardWasClicked(CardScript card) {
		switch(handlingMode) {
			case CardHandlingMode.Regular:
				playCard(card.CardModel);
				break;
			case CardHandlingMode.Event:
				playEventCard(card.CardModel);
				break;
			case CardHandlingMode.Replace:
				replaceCard(card.CardModel);
				break;
			case CardHandlingMode.Exhaust:
				exhaustCard(card.CardModel);
				break;
		}
	}

	private void playCard(Card card) {
		if (!state.CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return;
		}
		state = state.PlayCard(card);
		cards = cards.PlayCard(card);
		playedCards.Add(card);
		switchModeIfNecessary(card);
	}

	private void playEventCard(Card card) {
		if (!state.CanPlayCard(card)) {
			Debug.Log($"Can't play {card.Name}");
			return;
		}
		state = state.PlayCard(card);
		state = state.NextTurnState();
		EventUtils.LogEventCardPlayed(card, eventCardName(), state.ToString(), cards.Hand);
		handlingMode = CardHandlingMode.Regular;
		multiCardDisplay.FinishWork();
		startTurn();
	}

	private void switchModeIfNecessary(Card card) {
		if (card.NumberOfCardsToChooseToExhaust > 0) {
			cardsToHandle = card.NumberOfCardsToChooseToExhaust;
			multiCardDisplay.setup(cards.Hand, "Choose cards to exhaust");
			doneButton.GetComponentInChildren<Text>().text = "Done";
			handlingMode = CardHandlingMode.Exhaust;
		} else if (card.NumberOfCardsToChooseToReplace > 0) {
			cardsToHandle = card.NumberOfCardsToChooseToReplace;
			multiCardDisplay.setup(cards.Hand, "Choose cards to replace");
			doneButton.GetComponentInChildren<Text>().text = "Done";
			handlingMode = CardHandlingMode.Replace;
		}
	}

	private void replaceCard(Card card) {
		cards = cards
			.DiscardCardFromHand(card)
			.DrawCardsToHand(1);
		closeOrUpdateMultiCardDisplay();
	}

	private void exhaustCard(Card card) {
		cards = cards.ExhaustCardFromHand(card);
		closeOrUpdateMultiCardDisplay();
	}

	private void closeOrUpdateMultiCardDisplay() {
		cardsToHandle--;
		if (cardsToHandle == 0) {
			doneButton.GetComponentInChildren<Text>().text = "End Turn";
			closeMultiCardDisplay();
			return;
		} 
		multiCardDisplay.setup(cards.Hand, multiCardDisplay.Description);
	}

	public void EndTurnPressed() {
		switch (handlingMode) {
			case CardHandlingMode.Event:
				break;
			case CardHandlingMode.Exhaust:
				closeMultiCardDisplay();
				break;
			case CardHandlingMode.Regular:
				endTurn();
				break;
			case CardHandlingMode.Replace:
				closeMultiCardDisplay();
				break;
		}
	}

	private void closeMultiCardDisplay() {
		handlingMode = CardHandlingMode.Regular;
		cardsToHandle = 0;
		multiCardDisplay.FinishWork();
	}

	private void endTurn() {
		state = state.NextTurnState();
		EventUtils.LogEndTurnEvent(playedCards, eventCardName(), state.ToString(), cards.Hand);
		handlingMode = CardHandlingMode.Event;
		var currentEvent = events.First();
		multiCardDisplay.setup(currentEvent.Options, currentEvent.Name);
		if (events.Count > 1)	events.RemoveAt(0);
		cards = cards.DiscardHand();
		doneButton.gameObject.SetActive(false);
	}

	private void startTurn() {
		doneButton.gameObject.SetActive(true);
		drawNewHand();
		state = state.NextTurnState();
		EventUtils.LogStartTurnEvent(eventCardName(), state.ToString(), cards.Hand);
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

	private string eventCardName() {
		return events[0].Name;
	}
}
