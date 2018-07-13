using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class CardsStates {
  private static List<Card> initialDeck = new List<Card>() { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

  private static System.Random newRandom() {
    return new System.Random(1);
  }

  private static Card cardWithName(string name) {
    return new Card(name);
  }


  [Test]
	public void CreateNewState() {
    var state = CardsState.NewState(initialDeck);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

  [Test]
  public void ShuffleCardsToDeck() {
    var random = newRandom();
    var state = CardsState.NewState(initialDeck);
    state = state.ShuffleCurrentDeck(random);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("baz"), cardWithName("bro"), cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

  [Test]
  public void DrawCardsToHand() {
    var random = newRandom();
    var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck(random)
      .DrawCardsToHand(2);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("baz"), cardWithName("bro") }, state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

	[Test]
	public void MultipleDrawCardsToHand() {
		var random = newRandom();
		var state = CardsState.NewState(initialDeck)
			.ShuffleCurrentDeck(random)
			.DrawCardsToHand(1)
			.DrawCardsToHand(1);

		CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new List<Card>() { cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new List<Card>() { cardWithName("baz"), cardWithName("bro") }, state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
	}

	[Test]
  public void DiscardCardsFromHand() {
    var random = newRandom();
    var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck(random)
      .DrawCardsToHand(2)
      .DiscardCardFromHand(cardWithName("bro"));

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("baz") }, state.Hand);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("bro") }, state.DiscardPile);
  }

  [Test]
  public void DiscardHand() {
    var random = newRandom();
    var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck(random)
      .DrawCardsToHand(2)
      .DiscardHand();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new List<Card>() { }, state.Hand);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("baz"), cardWithName("bro") }, state.DiscardPile);
  }

  [Test]
  public void shuffleDiscardToDeck() {
    var random = newRandom();
    var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck(random)
      .DrawCardsToHand(2)
      .DiscardHand()
      .ShuffleDiscardToDeck(random);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new List<Card>() { cardWithName("foo"), cardWithName("bar"), cardWithName("bro"), cardWithName("baz") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new List<Card>() { }, state.Hand);
    CollectionAssert.AreEqual(new List<Card>() { }, state.DiscardPile);
  }
}
