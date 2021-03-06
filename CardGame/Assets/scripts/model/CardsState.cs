// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsState : BaseValueClass {
  public IReadOnlyList<Card> PersistentDeck { get; }
  public IReadOnlyList<Card> CurrentDeck { get; }
  public IReadOnlyList<Card> DiscardPile { get; }
  public IReadOnlyList<Card> Hand { get; }
	public IReadOnlyList<Tradition> Traditions { get; }

	private CardsState(IEnumerable<Card> presistentDeck, 
    IEnumerable<Card> currentDeck = null,
    IEnumerable<Card> discardPile = null,
    IEnumerable<Card> hand = null,
		IEnumerable<Tradition> traditions = null) {
    PersistentDeck = presistentDeck.ToList();
    CurrentDeck = (currentDeck ?? Enumerable.Empty<Card>()).ToList();
    DiscardPile = (discardPile ?? Enumerable.Empty<Card>()).ToList();
    Hand = (hand ?? Enumerable.Empty<Card>()).ToList();
		Traditions = (traditions ?? Enumerable.Empty<Tradition>()).ToList();
	}

  public static CardsState NewState(IEnumerable<Card> deck) {
    return new CardsState(deck);
  }

  public CardsState ShuffleCurrentDeck() {
    var deckAsList = PersistentDeck.ToList();
    return new CardsState(PersistentDeck, 
			currentDeck: deckAsList.Shuffle(),
			traditions: Traditions);
  }

  public CardsState DiscardCardFromHand(Card card) {
    return ExhaustCardFromHand(card).addCardsToDiscard(new []{card});
  }

  public CardsState ExhaustCardFromHand(Card card) {
    var handList = Hand.ToList();
    handList.RemoveIdentical(card);
    return new CardsState(PersistentDeck, CurrentDeck, DiscardPile, handList, Traditions);
  }

  public CardsState DiscardHand() {
    var discardList = DiscardPile.ToList();
    discardList.AddRange(Hand);
    return new CardsState(PersistentDeck, CurrentDeck, discardList , null, Traditions);
  }

  public CardsState DrawCardsToHand(int numberOfCardsToDraw) {
    var currentDeckList = CurrentDeck.ToList();
    Debug.AssertFormat(currentDeckList.Count >= numberOfCardsToDraw, 
		                   "Tried to draw {0} cards from {1} unused cards", 
		                   numberOfCardsToDraw, currentDeckList.Count);
    var firstCards = currentDeckList.Take(numberOfCardsToDraw);
    var newHand = Hand.ToList();
    newHand.AddRange(firstCards);
    currentDeckList.RemoveRange(0, numberOfCardsToDraw);
    return new CardsState(PersistentDeck, currentDeckList, DiscardPile, newHand, Traditions);
  }

  public CardsState ShuffleDiscardToDeck() {
    var discardAsList = DiscardPile.ToList();
    var deckAsList = CurrentDeck.ToList();
    deckAsList.AddRange(discardAsList);
    return new CardsState(PersistentDeck, deckAsList.Shuffle(), null, Hand, Traditions);
  }

  public CardsState AddCardsToDiscard(IEnumerable<Card> cards) {
		return addCardsToDiscard(modifyCardsWithTraditions(cards));
  }

	private CardsState addCardsToDiscard(IEnumerable<Card> cards) {
		var discardAsList = DiscardPile.ToList();
		discardAsList.AddRange(cards);
		return new CardsState(PersistentDeck, CurrentDeck, discardAsList, Hand, Traditions);
	}

	private IEnumerable<Card> modifyCardsWithTraditions(IEnumerable<Card> cards) {
		foreach (var card in cards) {
			var result = card;
			foreach (var tradition in Traditions) {
				result = modifyCardWithTradition(tradition, false)(result);
			}
			yield return result;
		}
	}

	public CardsState PlayCard(Card card) {
    var state = card.Exhaustible ? ExhaustCardFromHand(card) : DiscardCardFromHand(card);
    return state
			.AddCardsToDiscard(CardsCollection.CardsForDeck(card.AddDeck))
			.addTraditions(TraditionsCollection.TraditionsForDeck(card.AddDeck));
  }

	private CardsState addTraditions(IEnumerable<Tradition> traditions) {
		var cardState = this;
		foreach (var tradition in traditions) {
			cardState = cardState.AddTradition(tradition);
		}
		return cardState;
	}

	public CardsState AddTradition(Tradition tradition) {
		var traditions = Traditions.ToList();
		traditions.Add(tradition);
		var modifyCardFunction = modifyCardWithTradition(tradition, false);
		return new CardsState(
			PersistentDeck.Select(modifyCardFunction),
			CurrentDeck.Select(modifyCardFunction),
			DiscardPile.Select(modifyCardFunction),
			Hand.Select(modifyCardFunction),
			traditions);
	}

	private Func<Card, Card> modifyCardWithTradition(Tradition tradition, bool invertValue) {
		if (tradition.CardToEnhance == null) {
			return card => card;
		}
		var propertyToEnhance = typeof(Card).GetProperty(tradition.PropertyToEnhance);
		return card => {
			if (!card.Name.Equals(tradition.CardToEnhance)) {
				return card;
			}
			var valueAdjustment = invertValue ? -tradition.IncreaseInValue : tradition.IncreaseInValue;
			var finalValue = (int)propertyToEnhance.GetValue(card) + valueAdjustment;
			return card.SetValue(tradition.PropertyToEnhance, finalValue);
		};
	}

	public CardsState RemoveTradition(Tradition tradition) {
		var traditions = Traditions.ToList();
		traditions.Remove(tradition);
		var modifyCardFunction = modifyCardWithTradition(tradition, true);
		return new CardsState(
			PersistentDeck.Select(modifyCardFunction),
			CurrentDeck.Select(modifyCardFunction),
			DiscardPile.Select(modifyCardFunction),
			Hand.Select(modifyCardFunction),
			traditions);
	}
}
