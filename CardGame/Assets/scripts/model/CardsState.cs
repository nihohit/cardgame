// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardsState {
  public IEnumerable<Card> PersistentDeck { get; }

  public IEnumerable<Card> CurrentDeck { get; }

  public IEnumerable<Card> DiscardPile { get; }

  public IEnumerable<Card> Hand { get; }

  private void shuffle<T>(IList<T> list, System.Random random) {
    int n = list.Count;
    while (n > 1) {
      n--;
      int k = random.Next(n + 1);
      T value = list[k];
      list[k] = list[n];
      list[n] = value;
    }
  }

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

  public CardsState ShuffleCurrentDeck(System.Random random) {
    var deckAsList = PersistentDeck.ToList();
    shuffle(deckAsList, random);
    return new CardsState(PersistentDeck, deckAsList);
  }

  public CardsState DiscardCardFromHand(Card card) {
    return ExhaustCardFromHand(card).AddCardsToDiscard(new []{card});
  }

  public CardsState ExhaustCardFromHand(Card card) {
    var handList = Hand.ToList();
    Debug.AssertFormat(handList.Contains(card), "{0} is not present in {1}", card, string.Join(", ", handList.Select(cardInHand => cardInHand.ToString()).ToArray()));
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
    Debug.AssertFormat(currentDeckList.Count >= numberOfCardsToDraw, "Tried to draw {0} cards from {1} unused cards", numberOfCardsToDraw, currentDeckList.Count);
    var firstCards = currentDeckList.Take(numberOfCardsToDraw);
    var newHand = Hand.ToList();
    newHand.AddRange(firstCards);
    currentDeckList.RemoveRange(0, numberOfCardsToDraw);
    return new CardsState(PersistentDeck, currentDeckList, DiscardPile, newHand);
  }

  public CardsState ShuffleDiscardToDeck(System.Random random) {
    var discardAsList = DiscardPile.ToList();
    var deckAsList = CurrentDeck.ToList();
    deckAsList.AddRange(discardAsList);
    shuffle(deckAsList, random);
    return new CardsState(PersistentDeck, deckAsList, null, Hand);
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
