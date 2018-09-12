using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardsStates {
  private static Card[] initialDeck = new [] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

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
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck);
    state = state.ShuffleCurrentDeck();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

  [Test]
  public void DrawCardsToHand() {
		Randomizer.SetTestableRandom(4);
    var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck()
      .DrawCardsToHand(2);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar") }, state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

	[Test]
	public void MultipleDrawCardsToHand() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.DrawCardsToHand(1);

		CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar") }, state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
	}

	[Test]
  public void DiscardCardsFromHand() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck()
      .DrawCardsToHand(2)
      .DiscardCardFromHand(initialDeck[1]);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("foo") }, state.Hand);
    CollectionAssert.AreEqual(new [] { cardWithName("bar") }, state.DiscardPile);
  }

  [Test]
  public void DiscardHand() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck()
      .DrawCardsToHand(2)
      .DiscardHand();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar") }, state.DiscardPile);
	}

  [Test]
  public void ShuffleDiscardToDeck() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck()
      .DrawCardsToHand(2)
      .DiscardHand()
      .ShuffleDiscardToDeck();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro"), cardWithName("foo"), cardWithName("bar") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

	[Test]
	public void AddCardsToDiscard() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
			.ShuffleCurrentDeck()
			.AddCardsToDiscard(new[] { cardWithName("Hey") });

		CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new [] { cardWithName("Hey") }, state.DiscardPile);
	}

	[Test]
	public void PlayExhaustibleCard() {
		var playedCard = new Card("card",
			addDeck: DeckType.Test,
			exhaustible: true
		);
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { playedCard }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(playedCard);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { cardWithName("test") }, state.DiscardPile);
	}

	[Test]
	public void PlayNonExhaustibleCard() {
		var playedCard = new Card("card",
			addDeck: DeckType.Test
		);
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { playedCard }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(playedCard);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { playedCard, cardWithName("test") }, state.DiscardPile);
	}

	[Test]
	public void RemoveCardFromCorrectIndex() {
		var addedCard = initialDeck[1].ShallowClone();
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { addedCard }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(5)
			.PlayCard(addedCard);

		CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.Hand);
		CollectionAssert.AreEqual(new[] { addedCard }, state.DiscardPile);

		state = CardsState.NewState(initialDeck.Concat(new[] { addedCard }))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(5)
			.PlayCard(addedCard);

		CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.Hand);
		CollectionAssert.AreEqual(new[] { addedCard }, state.DiscardPile);
	}
}
