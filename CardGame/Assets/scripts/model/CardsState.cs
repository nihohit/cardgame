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

	private CardsState(IReadOnlyList<Card> persistentDeck,
		IReadOnlyList<Card> currentDeck,
		IReadOnlyList<Card> discardPile,
		IReadOnlyList<Card> hand,
		IReadOnlyList<Tradition> traditions) {
    PersistentDeck = persistentDeck;
    CurrentDeck = currentDeck;
    DiscardPile = discardPile;
    Hand = hand;
		Traditions = traditions;
	}

	private CardsState makeState(IReadOnlyList<Card> persistentDeck = null,
		IReadOnlyList<Card> currentDeck = null,
		IReadOnlyList<Card> discardPile = null,
		IReadOnlyList<Card> hand = null,
		IReadOnlyList<Tradition> traditions = null) {
		return new CardsState(persistentDeck ?? PersistentDeck,
			currentDeck ?? CurrentDeck,
			discardPile ?? DiscardPile,
			hand ?? Hand,
			traditions ?? Traditions);
	}

  public static CardsState NewState(IEnumerable<Card> deck) {
    return new CardsState(deck.ToList(), new Card[0], new Card[0], new Card[0], new Tradition[0]);
  }

  public CardsState ShuffleCurrentDeck() {
    var deckAsList = PersistentDeck.ToList();
    return makeState(currentDeck: deckAsList.Shuffle().ToList());
  }

  public CardsState DiscardCardFromHand(Card card) {
    return ExhaustCardFromHand(card).addCardsToDiscard(new []{card});
  }

  public CardsState ExhaustCardFromHand(Card card) {
    var clearedHand = Hand.RemoveSingleCardIdentity(card).ToList();
    return makeState(hand: clearedHand);
  }

  public CardsState DiscardHand() {
    var discardList = DiscardPile.ToList();
    discardList.AddRange(Hand);
    return makeState(discardPile: discardList , hand: new Card[0]);
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
    return makeState(currentDeck: currentDeckList,hand: newHand);
  }

  public CardsState ShuffleDiscardToDeck() {
    var deckAsList = CurrentDeck.ToList();
    deckAsList.AddRange(DiscardPile.Shuffle());
    return makeState(currentDeck: deckAsList, discardPile: new Card[0]);
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
		return makeState(
			currentDeck: currentDeckAsList,
			discardPile: discardAsList,
			hand: handAsList);
	}

	public CardsState AddTradition(Tradition tradition) {
		var traditions = Traditions.ToList();
		traditions.Add(tradition);
		var modifyCardFunction = modifyCardWithTradition(tradition, false);
		return new CardsState(
			PersistentDeck.Select(modifyCardFunction).ToList(),
			CurrentDeck.Select(modifyCardFunction).ToList(),
			DiscardPile.Select(modifyCardFunction).ToList(),
			Hand.Select(modifyCardFunction).ToList(),
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
			PersistentDeck.Select(modifyCardFunction).ToList(),
			CurrentDeck.Select(modifyCardFunction).ToList(),
			DiscardPile.Select(modifyCardFunction).ToList(),
			Hand.Select(modifyCardFunction).ToList(),
			traditions);
	}

	public CardsState LeaveLocation() {
		return makeState(
			currentDeck: CurrentDeck.RemoveLocationCards().ToList(),
			discardPile: DiscardPile.RemoveLocationCards().ToList(),
			hand: Hand.RemoveLocationCards().ToList());
	}

	public CardsState EnterLocation(Location location) {
		var newCards = location.Content.SelectMany(content => LocationBasedCards.CardsForContent(content))
			.Shuffle();
		return makeState(currentDeck: CurrentDeck.Interleave(newCards).ToList());
	}
}
