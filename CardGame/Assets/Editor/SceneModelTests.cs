//using UnityEngine;
//using UnityEngine.TestTools;
//using NUnit.Framework;
//using System.Collections;
//using UniRx;
//using System.Linq;
//using System.Collections.Generic;

//public class SceneModelTests {
//	private static Card[] initialDeck = new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

//	private static Card cardWithName(string name) {
//		return Card.MakeCard(name);
//	}

//	[Test]
//	public void ShouldHaveInitialStateReadyOnConstruction() {
//		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck();
//		var empire = new TrainState(1, 2, 3, 4, new List<Card>());
//		var mode = CardHandlingMode.Exhaust;
//		var model = new SceneModel(cards, empire, mode);

//		var state = model.State.ToReactiveProperty();

//		Assert.AreEqual(cards, state.Value.Cards);
//		Assert.AreEqual(empire, state.Value.Train);
//		Assert.AreEqual(mode, state.Value.Mode);
//	}

//	[Test]
//	public void ShouldEndTurnWhenUserFinishedModeIsCalledInRegularMode() {
//		var deck = new Card[] { Card.MakeCard("foo",
//			fuelCost: 1,
//			fuelGain: 5
//		)};
//		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//		var state = model.State.ToReactiveProperty();
//		model.PlayCard(deck[0]);

//		model.UserFinishedMode();

	//	Assert.AreEqual(new TrainState(5, 0, 0, 0, deck), state.Value.Train);
//	}

//	[Test]
//	public void ShouldReturnToRegularModeFromReplace() {
//		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Replace);
//		var state = model.State.ToReactiveProperty();

//		model.UserFinishedMode();

//		Assert.AreEqual(CardHandlingMode.Regular, state.Value.Mode);
//	}

//	[Test]
//	public void ShouldReturnToRegularModeFromExhaust() {
//		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Exhaust);
//		var state = model.State.ToReactiveProperty();

//		model.UserFinishedMode();

//		Assert.AreEqual(CardHandlingMode.Regular, state.Value.Mode);
//	}

//	[Test]
//	public void ShouldPlayCardFromHand() {
//		var deck = new Card[] { Card.MakeCard("foo",
//			fuelCost: 1,
//			fuelGain: 5
//		)};
//		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//		var state = model.State.ToReactiveProperty();

//		model.PlayCard(deck[0]);

//		Assert.AreEqual(0, state.Value.Train.Fuel);
//		Assert.AreEqual(5, state.Value.Train.AddFuel);
//		Assert.AreEqual(0, state.Value.Cards.Hand.Count());
//		Assert.AreEqual(deck, state.Value.Cards.DiscardPile);
//		Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
//	}

//	[Test]
//	public void ShouldFailToPlayExpensiveCardFromHand() {
//		var deck = new Card[] { Card.MakeCard("foo",
//			fuelCost: 2,
//			fuelGain: 5
//		)};
//		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//		var state = model.State.ToReactiveProperty();

//		model.PlayCard(deck[0]);

//		Assert.AreEqual(1, state.Value.Train.Fuel);
//		Assert.AreEqual(0, state.Value.Train.AddFuel);
//		Assert.AreEqual(0, state.Value.Cards.DiscardPile.Count());
//		Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
//		Assert.AreEqual(deck, state.Value.Cards.Hand);
//	}

//	[Test]
//public void ShouldFPlayDefaultExpensiveCardFromHand() {
//	var deck = new Card[] { Card.MakeCard("foo",
//			fuelCost: 2,
//			fuelGain: 5,
//				defaultChoice: true
//		)};
//	var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
//		new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//	var state = model.State.ToReactiveProperty();

//	model.PlayCard(deck[0]);

//	Assert.AreEqual(0, state.Value.Train.Fuel);
//	Assert.AreEqual(5, state.Value.Train.AddFuel);
//	Assert.AreEqual(1, state.Value.Cards.DiscardPile.Count());
//	Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
//	Assert.AreEqual(new Card[0], state.Value.Cards.Hand);

//	[Test]
//  public void ShouldDrawCardAndPayOneFuel() {
//		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
//			new TrainState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//		var state = model.State.ToReactiveProperty();

//		model.TryDrawCard();

//		Assert.AreEqual(1, state.Value.Cards.Hand.Count());
//		Assert.AreEqual(new TrainState(0, 0, 0, 0, new List<Card>()), state.Value.Train);
//	}

//	[Test]
//	public void ShouldNotDrawCardIfNoFuelIsAvailable() {
//		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
//			new TrainState(0, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
//		var state = model.State.ToReactiveProperty();

//		model.TryDrawCard();

//		Assert.AreEqual(0, state.Value.Cards.Hand.Count());
//		Assert.AreEqual(new TrainState(0, 0, 0, 0, new List<Card>()), state.Value.Train);
//	}
//}
