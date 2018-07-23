using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {
	public GameObject CardPrefab;
	public GameObject stateDescription;

	private System.Random random = new System.Random();
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

	// Use this for initialization
	void Start() {
		state = new EmpireState(0, 0, 0);
		cardPool = new CardScriptPool(CardPrefab, 10);
		deck = GameObject.Find("Deck").GetComponent<DeckScript>();
		deck.Manager = this;
		discardPile = GameObject.Find("Discard Pile").GetComponent<DeckScript>();
		discardPile.Manager = this;
		cards = CardsState.NewState(CardsCollection.Cards()).ShuffleCurrentDeck(random);
	}

	public void DeckWasClicked(DeckScript clickedDeck) {
		if (clickedDeck == deck) {
			passCardsToDiscard();
		} else {
			passCardsToDeck();
		}
	}

	private void passCardsToDeck() {
		var sizeOfDiscard = cards.DiscardPile.Count();
		if (sizeOfDiscard == 0) {
			return;
		}

		cards = cards.ShuffleDiscardToDeck(random);
	}

	private void passCardsToDiscard() {
		if (cards.CurrentDeck.Count() == 0) {
			return;
		}

		cards = cards.DrawCardsToHand(1);
	}

	public void CardWasClicked(CardScript card) {
		if (!state.CanPlayCard(card.CardModel)) {
			Debug.Log($"Can't play {card.CardModel.Name}");
			return;
		}

		state = state.PlayCard(card.CardModel);
		cards = cards.DiscardCardFromHand(card.CardModel);
	}
}
