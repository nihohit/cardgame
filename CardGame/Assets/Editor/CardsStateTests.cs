using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class CardsStates {
	private readonly static Card[] empty = new Card[0];

  private readonly static IReadOnlyList<Card> initialDeck = new [] {
		cardWithName("foo"),
		cardWithName("bar"),
		cardWithName("baz"),
		cardWithName("bro")
	};

  private static Card cardWithName(string name) {
    return Card.MakeCard(name);
  }

	[TearDown]
	public void TearDown() {
		Randomizer.SetRandom(new System.Random());
	}

  [Test]
	public void CreateNewState() {
    var state = CardsState.NewState(initialDeck);

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(empty, state.CurrentDeck);
    CollectionAssert.AreEqual(empty, state.Hand);
    CollectionAssert.AreEqual(empty, state.DiscardPile);
		CollectionAssert.AreEqual(new Tradition[0], state.Traditions);
  }

  [Test]
  public void ShuffleCardsToDeck() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck);
    state = state.ShuffleCurrentDeck();

    CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
    CollectionAssert.AreEqual(new [] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
    CollectionAssert.AreEqual(empty, state.Hand);
    CollectionAssert.AreEqual(empty, state.DiscardPile);
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
    CollectionAssert.AreEqual(empty, state.DiscardPile);
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
		CollectionAssert.AreEqual(empty, state.DiscardPile);
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
		CollectionAssert.AreEqual(empty, state.Hand);
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
    CollectionAssert.AreEqual(empty, state.Hand);
    CollectionAssert.AreEqual(empty, state.DiscardPile);
  }

	[Test]
	public void AddCardsToDiscard() {
		Randomizer.SetTestableRandom(4);
		var state = CardsState.NewState(initialDeck)
			.ShuffleCurrentDeck()
			.AddCardsToDiscard(new[] { cardWithName("Hey") });

		CollectionAssert.AreEqual(initialDeck, state.PersistentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new [] { cardWithName("Hey") }, state.DiscardPile);
	}

	[Test]
	public void PlayExhaustibleCard() {
		var playedCard = Card.MakeCard("card",
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
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new[] { cardWithName("trainTest") }, state.DiscardPile);
	}

	[Test]
	public void PlayExhaustibleModifiedCard() {
		var source = Card.MakeCard("card",
			addTradition: TraditionType.Test,
			carToAdd: new TrainCar(0, CarType.Test),
			exhaustible: false
		);
		var modified = source
			.CopyWithSetValue("Exhaustible", true)
			.CopyWithSource(source);
		modified = modified.CopyWithSource(modified);
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { source }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(modified);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new[] { cardWithName("trainTest") }, state.DiscardPile);
	}

	[Test]
	public void PlayNonExhaustibleCard() {
		var playedCard = Card.MakeCard("card",
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
		);
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { playedCard }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(playedCard);

		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new[] { playedCard, cardWithName("trainTest") }, state.DiscardPile);
	}

	[Test]
	public void RemoveCardFromCorrectIndex() {
		var addedCard = initialDeck[1].ShallowClone<Card>();
		Randomizer.SetTestableRandom(5);
		var state = CardsState.NewState(new[] { addedCard }.Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(5)
			.PlayCard(addedCard);

		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.Hand);
		CollectionAssert.AreEqual(new[] { addedCard }, state.DiscardPile);

		state = CardsState.NewState(initialDeck.Concat(new[] { addedCard }))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(5)
			.PlayCard(addedCard);

		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") }, state.Hand);
		CollectionAssert.AreEqual(new[] { addedCard }, state.DiscardPile);
	}

	[Test]
	public void AddTradition() {
		var tradition = new Tradition("foo");
		var state = CardsState.NewState(empty)
			.AddTradition(tradition);

		CollectionAssert.AreEqual(new[] { tradition }, state.Traditions);
	}

	[Test]
	public void RemoveTradition() {
		var tradition = new Tradition("foo");
		var state = CardsState.NewState(empty)
			.AddTradition(tradition)
			.RemoveTradition(tradition);

		CollectionAssert.AreEqual(new Tradition[0], state.Traditions);
	}

	[Test]
	public void ModifyCardInDeckWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			Card.MakeCard("bar"),
			Card.MakeCard("foo")
		}).ShuffleCurrentDeck()
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance:"FuelChange", increaseInValue:2));

		CollectionAssert.AreEqual(new[] {
			Card.MakeCard("bar", fuelChange: 2),
			Card.MakeCard("foo")
		}, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(empty, state.DiscardPile);
	}

	[Test]
	public void ModifyCardInHandWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			Card.MakeCard("bar"),
			Card.MakeCard("foo")
		}).ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] { Card.MakeCard("foo"), }, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] { Card.MakeCard("bar", fuelChange: 2), }, state.Hand);
		CollectionAssert.AreEqual(empty, state.DiscardPile);
	}

	[Test]
	public void ModifyCardInDiscardPileWithTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			Card.MakeCard("bar"),
			Card.MakeCard("foo")
		}).ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.DiscardHand()
			.AddTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] { Card.MakeCard("foo"), }, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new[] { Card.MakeCard("bar", fuelChange: 2), }, state.DiscardPile);
	}

	[Test]
	public void ModifyAddedCardsWithTradition() {
		Randomizer.SetTestableRandom(1);
		var card = Card.MakeCard("card",
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
		);
		var tradition = new Tradition("foo", "trainTest", "FuelChange", 1);
		Randomizer.SetTestableRandom(1);
		var state = CardsState.NewState(card.Yield())
			.AddTradition(tradition)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(new[] { card, Card.MakeCard("trainTest", fuelChange: 1), }, state.DiscardPile);
	}

	[Test]
	public void ShouldApplyTraditionOnlyOnce() {
		Randomizer.SetTestableRandom(1);
		var card = Card.MakeCard("card",
			carToAdd: new TrainCar(0, CarType.Test),
			addTradition: TraditionType.Test
		);
		var tradition = new Tradition("foo", "trainTest", "FuelChange", 1);
		Randomizer.SetTestableRandom(1);
		var state = CardsState.NewState(card.Yield())
			.AddTradition(tradition)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card)
			.ShuffleDiscardToDeck()
			.DrawCardsToHand(2);
		state = state.PlayCard(state.Hand.ToList()[1]);

		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(card.Yield(), state.Hand);
		CollectionAssert.AreEqual(new[] { Card.MakeCard("trainTest", fuelChange: 1), }, state.DiscardPile);
	}

	[Test]
	public void ModifyCardsWithRemovedTradition() {
		Randomizer.SetTestableRandom(2);
		var state = CardsState.NewState(new[] {
			Card.MakeCard("foo"),
			Card.MakeCard("bar")
		}).ShuffleCurrentDeck()
			.RemoveTradition(new Tradition("hi", cardToEnhance: "bar", propertyToEnhance: "FuelChange", increaseInValue: 2));

		CollectionAssert.AreEqual(new[] {
			Card.MakeCard("foo"),
			Card.MakeCard("bar", fuelChange: -2)
		}, state.CurrentDeck);
		CollectionAssert.AreEqual(empty, state.Hand);
		CollectionAssert.AreEqual(empty, state.DiscardPile);
	}
	
	[Test]
	public void ShouldRemoveCardsAccordingToRemovedCar() {
		Randomizer.SetTestableRandom(3);
		var card = Card.MakeCard("card",
			carToRemove: CarType.Test
		);
		var initialCards = new[] {
			TrainCarsCardCollection.CardsForTrainCar(CarType.Test),
			new[] {
				Card.MakeCard("foo"),
				card
			}
		}.SelectMany(collection => collection);

		var state = CardsState.NewState(initialCards)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(3)
			.PlayCard(card);
			
		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] {Card.MakeCard("foo")}, state.Hand);
		CollectionAssert.AreEqual(card.Yield(), state.DiscardPile);
	}
	
	[Test]
	public void ShouldRemoveCardsFromDiscardThenDeckThenHand() {
		Randomizer.SetTestableRandom(4);
		var card = Card.MakeCard("card",
			carToRemove: CarType.Test
		);
		var initialCards = new[] {
			TrainCarsCardCollection.CardsForTrainCar(CarType.Test),
			card.Yield(),
			TrainCarsCardCollection.CardsForTrainCar(CarType.Test),
			TrainCarsCardCollection.CardsForTrainCar(CarType.Test),
		}.SelectMany(collection => collection);

		var state = CardsState.NewState(initialCards)
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.DiscardHand();
		
		var testCard = Card.MakeCard("trainTest");
		CollectionAssert.AreEqual(new[] {card, testCard, testCard }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[] {}, state.Hand);
		CollectionAssert.AreEqual(new[] { testCard }, state.DiscardPile);
		
		state = state
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(new[] { testCard, testCard }, state.CurrentDeck);
		CollectionAssert.AreEqual(new Card[] {}, state.Hand);
		CollectionAssert.AreEqual(card.Yield(), state.DiscardPile);

		state = state
			.DrawCardsToHand(1)
			.DiscardHand()
			.ShuffleDiscardToDeck()
			.DrawCardsToHand(2);
			
		CollectionAssert.AreEqual(new[] {testCard}, state.CurrentDeck);
		CollectionAssert.AreEqual(new[] {testCard, card}, state.Hand);
		CollectionAssert.AreEqual(empty, state.DiscardPile);
		
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

	[Test]
	public void ShouldDrawCardsAccordingToCard() {
		Randomizer.SetTestableRandom(5);
		var card = Card.MakeCard("hi",
			numberOfCardsToChooseToDraw: 2);

		var state = CardsState.NewState(card.Yield().Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(initialDeck.Skip(2), state.CurrentDeck);
		CollectionAssert.AreEqual(initialDeck.Take(2), state.Hand);
		CollectionAssert.AreEqual(card.Yield(), state.DiscardPile);
	}

	[Test]
	public void ShouldDrawCardsUpToDeckLimit() {
		Randomizer.SetTestableRandom(5);
		var card = Card.MakeCard("hi",
			numberOfCardsToChooseToDraw: 5);

		var state = CardsState.NewState(card.Yield().Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(2)
			.DiscardCardFromHand(initialDeck[0])
			.PlayCard(card);

		CollectionAssert.AreEqual(empty, state.CurrentDeck);
		CollectionAssert.AreEqual(initialDeck.Skip(1).Take(3), state.Hand);
		CollectionAssert.AreEqual(new[] { initialDeck[0], card }, state.DiscardPile);
	}

	[Test]
	public void ShouldDrawCardsAccordingToFilter() {
		Randomizer.SetTestableRandom(5);
		var card = Card.MakeCard("hi",
			numberOfCardsToChooseToDraw: 1,
			cardDrawingFilter: checkedCard => checkedCard.Name.Equals("baz"));

		var state = CardsState.NewState(card.Yield().Concat(initialDeck))
			.ShuffleCurrentDeck()
			.DrawCardsToHand(1)
			.PlayCard(card);

		CollectionAssert.AreEqual(
			new Card[] { initialDeck[0], initialDeck[1], initialDeck[3] }, 
			state.CurrentDeck);
		CollectionAssert.AreEqual(initialDeck[2].Yield(), state.Hand);
		CollectionAssert.AreEqual(card.Yield(), state.DiscardPile);
	}
}
