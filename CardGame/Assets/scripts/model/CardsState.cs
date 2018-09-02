// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsState : BaseValueClass {
  public IEnumerable<Card> PersistentDeck { get; }

  public IEnumerable<Card> CurrentDeck { get; }

  public IEnumerable<Card> DiscardPile { get; }

  public IEnumerable<Card> Hand { get; }

  private CardsState(IEnumerable<Card> presistentDeck, 
    IEnumerable<Card> currentDeck = null,
    IEnumerable<Card> discardPile = null,
    IEnumerable<Card> hand = null) {
    PersistentDeck = presistentDeck.ToList();
    CurrentDeck = currentDeck ?? new List<Card>();
    DiscardPile = discardPile ?? new List<Card>();
    Hand = hand ?? new List<Card>();
  }

  public static CardsState NewState(IEnumerable<Card> deck) {
    return new CardsState(deck);
  }

  public CardsState ShuffleCurrentDeck() {
    var deckAsList = PersistentDeck.ToList();
    return new CardsState(PersistentDeck, deckAsList.Shuffle());
  }

  public CardsState DiscardCardFromHand(Card card) {
    return ExhaustCardFromHand(card).AddCardsToDiscard(new []{card});
  }

  public CardsState ExhaustCardFromHand(Card card) {
    var handList = Hand.ToList();
    Debug.AssertFormat(handList.Contains(card), "{0} is not present in {1}", 
		                   card, handList.ToJoinedString(", "));
    handList.Remove(card);
    return new CardsState(PersistentDeck, CurrentDeck, DiscardPile, handList);
  }

  public CardsState DiscardHand() {
    var discardList = DiscardPile.ToList();
    discardList.AddRange(Hand);
    return new CardsState(PersistentDeck, CurrentDeck, discardList , null);
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
    return new CardsState(PersistentDeck, currentDeckList, DiscardPile, newHand);
  }

  public CardsState ShuffleDiscardToDeck() {
    var discardAsList = DiscardPile.ToList();
    var deckAsList = CurrentDeck.ToList();
    deckAsList.AddRange(discardAsList);
    return new CardsState(PersistentDeck, deckAsList.Shuffle(), null, Hand);
  }

  public CardsState AddCardsToDiscard(IEnumerable<Card> cards) {
    var discardAsList = DiscardPile.ToList();
    discardAsList.AddRange(cards);
    return new CardsState(PersistentDeck, CurrentDeck, discardAsList, Hand);
  }

  public CardsState PlayCard(Card card) {
    var state = card.Exhaustible ? ExhaustCardFromHand(card) : DiscardCardFromHand(card);
    return state.AddCardsToDiscard(CardsCollection.CardsForDeck(card.AddDeck));
  }
}
