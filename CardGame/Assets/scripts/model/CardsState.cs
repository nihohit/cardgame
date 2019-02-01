﻿// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

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

	private CardsState(IEnumerable<Card> persistentDeck, 
    IEnumerable<Card> currentDeck = null,
    IEnumerable<Card> discardPile = null,
    IEnumerable<Card> hand = null,
		IEnumerable<Tradition> traditions = null) {
    PersistentDeck = persistentDeck.ToList();
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
    var clearedHand = Hand.RemoveSingleCardIdentity(card);
    return new CardsState(PersistentDeck, CurrentDeck, DiscardPile, clearedHand, Traditions);
  }

  public CardsState DiscardHand() {
    var discardList = DiscardPile.ToList();
    discardList.AddRange(Hand);
    return new CardsState(PersistentDeck, CurrentDeck, discardList , null, Traditions);
  }

	public CardsState DrawCardsToHand(int numberOfCardsToDraw) {
		return drawCardsToHand(numberOfCardsToDraw, null);
	}

	private CardsState drawCardsToHand(int numberOfCardsToDraw, Func<Card, bool> cardDrawingFilter) {
		if (numberOfCardsToDraw == 0) {
			return this;
		}

    var currentDeckList = CurrentDeck.ToList();
    var firstCards = currentDeckList
			.Where(cardDrawingFilter ?? (_ => true))
			.Take(numberOfCardsToDraw)
			.ToList();
    var newHand = Hand.ToList();
    newHand.AddRange(firstCards);
		firstCards.ForEach(card => {
			currentDeckList.Remove(card);
		});
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
		return cards.Select(card => {
			return Traditions.Aggregate(card, 
					(aggreagteCard, tradition) => modifyCardWithTradition(tradition, false)(aggreagteCard)
			);
		});
	}

	public CardsState PlayCard(Card card) {
    var state = card.Exhaustible ? ExhaustCardFromHand(card) : DiscardCardFromHand(card);
    return state
			.AddCardsToDiscard(card.CarToAdd == null ? new Card[0] : TrainCarsCardCollection.CardsForTrainCar(card.CarToAdd.Type))
	    .removeCards(TrainCarsCardCollection.CardsForTrainCar(card.CarToRemove))
			.drawCardsToHand(card.NumberOfCardsToDraw, card.CardDrawingFilter)
			.addTraditions(TraditionsCollection.TraditionsForDeck(card.AddTradition));
  }

	private CardsState addTraditions(IEnumerable<Tradition> traditions) {
		var cardState = this;
		foreach (var tradition in traditions) {
			cardState = cardState.AddTradition(tradition);
		}
		return cardState;
	}

	private CardsState removeCards(IEnumerable<Card> cards) {
		var discardAsList = DiscardPile.ToList();
		var currentDeckAsList = CurrentDeck.ToList();
		var handAsList = Hand.ToList();
		foreach (var card in modifyCardsWithTraditions(cards)) {
			if (discardAsList.Remove(card)) {
				continue;
			}

			if (currentDeckAsList.Remove(card)) {
				continue;
			}

			handAsList = handAsList.RemoveSingleCarEquality(card).ToList();
		}
		return new CardsState(PersistentDeck,
			currentDeckAsList,
			discardAsList,
			handAsList,
			Traditions);
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
		return card => {
			if (!card.Name.Equals(tradition.CardToEnhance)) {
				return card;
			}
			var valueAdjustment = invertValue ? -tradition.IncreaseInValue : tradition.IncreaseInValue;
			return card.CopyWithModifiedValue(tradition.PropertyToEnhance, valueAdjustment);
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

	public CardsState LeaveLocation() {
		return new CardsState(PersistentDeck,
			CurrentDeck.RemoveLocationCards(),
			DiscardPile.RemoveLocationCards(),
			Hand.RemoveLocationCards(),
			Traditions);
	}

	public CardsState EnterLocation(Location location) {
		var newCards = location.Content.SelectMany(content => LocationBasedCards.CardsForContent(content)).ToList();
		return new CardsState(PersistentDeck,
			CurrentDeck.Concat(newCards).Shuffle(),
			DiscardPile,
			Hand,
			Traditions);
	}
}
