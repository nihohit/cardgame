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

	[TearDown]
	public void TearDown() {
		Randomizer.SetRandom(new System.Random());
	}

  [Test]
	public void CreateNewState() {
    var state = CardsState.NewState(initialDeck);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
    CollectionAssert.AreEqual(new Card[0], state.Hand);
    CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
		CollectionAssert.AreEqual(new Tradition[0], state.Traditions);
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
			addTradition: TraditionType.Test,
			carToAdd: new TrainCar(0, CarType.Test),
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
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
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
		var addedCard = initialDeck[1].ShallowClone<Card>();
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

	[Test]
	public void AddTradition() {
		var tradition = new Tradition("foo");
		var state = CardsState.NewState(new Card[0])
			.AddTradition(tradition);

		CollectionAssert.AreEqual(new[] { tradition }, state.Traditions);
	}

	[Test]
	public void RemoveTradition() {
		var tradition = new Tradition("foo");
		var state = CardsState.NewState(new Card[0])
			.AddTradition(tradition)
			.RemoveTradition(tradition);

		CollectionAssert.AreEqual(new Tradition[0], state.Traditions);
	}

	[Test]
	public void ModifyCardInDeckWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			new Card("bar"),
			new Card("foo")
		}).ShuffleCurrentDeck()
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance:"FuelChange", increaseInValue:2));

		CollectionAssert.AreEqual(new[] {
			new Card("bar", fuelChange: 2),
			new Card("foo")
		}, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
	}

	[Test]
	public void ModifyCardInHandWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			new Card("bar"),
			new Card("foo")
		}).ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] { new Card("foo"), }, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { new Card("bar", fuelChange: 2), }, state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
	}

	[Test]
	public void ModifyCardInDiscardPileWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			new Card("bar"),
			new Card("foo")
		}).ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.DiscardHand()
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] { new Card("foo"), }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { new Card("bar", fuelChange: 2), }, state.DiscardPile);
	}

	[Test]
	public void ModifyAddedCardsWithTradition() {
		Randomizer.SetTestableRandom(1);
		var card = new Card("card",
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
		);
		var tradition = new Tradition("foo", "test", "FuelChange", 1);
		Randomizer.SetTestableRandom(1);
		var state = CardsState.NewState(new[] { card })
			.AddTradition(tradition)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new[] { card, new Card("test", fuelChange: 1), }, state.DiscardPile);
	}

	[Test]
	public void ShouldApplyTraditionOnlyOnce() {
		Randomizer.SetTestableRandom(1);
		var card = new Card("card",
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
		);
		var tradition = new Tradition("foo", "test", "FuelChange", 1);
		Randomizer.SetTestableRandom(1);
		var state = CardsState.NewState(new[] { card })
			.AddTradition(tradition)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card)
			.ShuffleDiscardToDeck()
			.DrawCardsToHand(2);
		state = state.PlayCard(state.Hand.ToList()[1]);

		CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { card }, state.Hand);
		CollectionAssert.AreEqual(new[] { new Card("test", fuelChange: 1), }, state.DiscardPile);
	}

	[Test]
	public void ModifyCardsWithRemovedTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			new Card("foo"),
			new Card("bar")
		}).ShuffleCurrentDeck()
			.RemoveTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] {
			new Card("foo"),
			new Card("bar", fuelChange: -2)
		}, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[0], state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
	}
	
	[Test]
	public void ShouldRemoveCardsAccordingToRemovedCar() {
		Randomizer.SetTestableRandom(3);
		var card = new Card("card",
			carToRemove: CarType.Test
		);
		var initialCards = new[] {
			CardsCollection.CardsForTrainCar(CarType.Test),
			new[] {
				new Card("foo"),
				card
			}
		}.SelectMany(collection => collection);

		var state = CardsState.NewState(initialCards)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(3)
			.PlayCard(card);
			
		CollectionAssert.AreEqual(new Card[0], state.CurrentDeck);
		CollectionAssert.AreEqual(new[] {new Card("foo")}, state.Hand);
		CollectionAssert.AreEqual(new[] { card }, state.DiscardPile);
	}
	
	[Test]
	public void ShouldRemoveCardsFromDiscardThenDeckThenHand() {
		Randomizer.SetTestableRandom(4);
		var card = new Card("card",
			carToRemove: CarType.Test
		);
		var initialCards = new[] {
			CardsCollection.CardsForTrainCar(CarType.Test),
			new[] {
				card
			},
			CardsCollection.CardsForTrainCar(CarType.Test),
			CardsCollection.CardsForTrainCar(CarType.Test),
		}.SelectMany(collection => collection);

		var state = CardsState.NewState(initialCards)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.DiscardHand();
		
		var testCard = new Card("test");
		CollectionAssert.AreEqual(new[] {card, testCard, testCard }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[] {}, state.Hand);
		CollectionAssert.AreEqual(new[] { testCard }, state.DiscardPile);
		
		state = state
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(new[] { testCard, testCard }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[] {}, state.Hand);
		CollectionAssert.AreEqual(new[] { card }, state.DiscardPile);

		state = state
			.DrawCardsToHand(1)
			.DiscardHand()
			.ShuffleDiscardToDeck()
			.DrawCardsToHand(2);
			
		CollectionAssert.AreEqual(new[] {testCard}, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] {testCard, card}, state.Hand);
		CollectionAssert.AreEqual(new Card[0], state.DiscardPile);
		
		state = state
			.PlayCard(card);
			
		CollectionAssert.AreEqual(new Card[] {}, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] {testCard}, state.Hand);
		CollectionAssert.AreEqual(new[] {card}, state.DiscardPile);
		
		state = state
			.ShuffleDiscardToDeck()
			.DrawCardsToHand(1)
			.PlayCard(card);
			
		CollectionAssert.AreEqual(new Card[] {}, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[] {}, state.Hand);
		CollectionAssert.AreEqual(new[] {card}, state.DiscardPile);
	}
}
