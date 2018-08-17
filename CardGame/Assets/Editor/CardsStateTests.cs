using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardsStates {
  private static Card[] initialDeck = new [] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

	private class TestRandom : System.Random {
		private int counter = 4;
		public TestRandom(int i) : base(0) {
			counter = i;
		}

		public override int Next(int i) {
			if (counter == 1) {
				counter = 4;
			}
			return --counter;
		}
	}

  private static void setNewRandom(int count) {
		Randomizer.SetRandom(new TestRandom(count));
  }

  private static Card cardWithName(string name) {
    return new Card {
			Name = name
		};
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
    var state = CardsState.NewState(initialDeck);
    state = state.ShuffleCurrentDeck();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
  }

  [Test]
  public void DrawCardsToHand() {
    setNewRandom(4);
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
		setNewRandom(4);
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
		setNewRandom(4);
		var state = CardsState.NewState(initialDeck)
      .ShuffleCurrentDeck()
      .DrawCardsToHand(2)
      .DiscardCardFromHand(cardWithName("bar"));

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new [] { cardWithName("foo") }, state.Hand);
    CollectionAssert.AreEqual(new [] { cardWithName("bar") }, state.DiscardPile);
  }

  [Test]
  public void DiscardHand() {
		setNewRandom(4);
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
		setNewRandom(4);
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
		setNewRandom(4);
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
		var playedCard = new Card {
			Name = "card",
			AddDeck = DeckType.Test,
			Exhaustible = true
		};
		setNewRandom(5);
		var state = CardsState.NewState(new[] { playedCard }.Union(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(playedCard);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { cardWithName("test") }, state.DiscardPile);
	}

	[Test]
	public void PlayNonExhaustibleCard() {
		var playedCard = new Card {
			Name = "card",
			AddDeck = DeckType.Test
		};
		setNewRandom(5);
		var state = CardsState.NewState(new[] { playedCard }.Union(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(playedCard);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { playedCard, cardWithName("test") }, state.DiscardPile);
	}
}
