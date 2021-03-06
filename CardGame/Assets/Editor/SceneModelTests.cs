using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UniRx;
using System.Linq;
using System.Collections.Generic;

public class SceneModelTests {
	private static Card[] initialDeck = new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

	private static Card cardWithName(string name) {
		return new Card(name);
	}

	[Test]
	public void ShouldHaveInitialStateReadyOnConstruction() {
		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck();
		var empire = new EmpireState(1, 2, 3, 4, new List<Card>());
		var mode = CardHandlingMode.Exhaust;
		var model = new SceneModel(cards, empire, mode);

		var state = model.State.ToReactiveProperty();

		Assert.AreEqual(cards, state.Value.Cards);
		Assert.AreEqual(empire, state.Value.Empire);
		Assert.AreEqual(mode, state.Value.Mode);
	}

	[Test]
	public void ShouldEndTurnWhenUserFinishedModeIsCalledInRegularMode() {
		var deck = new Card[] { new Card("foo",
			goldCost: 1,
			goldGain: 5
		)};
		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();
		model.PlayCard(deck[0]);

		model.UserFinishedMode();

		Assert.AreEqual(new EmpireState(5, 0, 0, 0, deck), state.Value.Empire);
	}

	[Test]
	public void ShouldReturnToRegularModeFromReplace() {
		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Replace);
		var state = model.State.ToReactiveProperty();

		model.UserFinishedMode();

		Assert.AreEqual(CardHandlingMode.Regular, state.Value.Mode);
	}

	[Test]
	public void ShouldReturnToRegularModeFromExhaust() {
		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Exhaust);
		var state = model.State.ToReactiveProperty();

		model.UserFinishedMode();

		Assert.AreEqual(CardHandlingMode.Regular, state.Value.Mode);
	}

	[Test]
	public void ShouldPlayCardFromHand() {
		var deck = new Card[] { new Card("foo",
			goldCost: 1,
			goldGain: 5
		)};
		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();

		model.PlayCard(deck[0]);

		Assert.AreEqual(0, state.Value.Empire.Gold);
		Assert.AreEqual(5, state.Value.Empire.AddGold);
		Assert.AreEqual(0, state.Value.Cards.Hand.Count());
		Assert.AreEqual(deck, state.Value.Cards.DiscardPile);
		Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
	}

	[Test]
	public void ShouldFailToPlayExpensiveCardFromHand() {
		var deck = new Card[] { new Card("foo",
			goldCost: 2,
			goldGain: 5
		)};
		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();

		model.PlayCard(deck[0]);

		Assert.AreEqual(1, state.Value.Empire.Gold);
		Assert.AreEqual(0, state.Value.Empire.AddGold);
		Assert.AreEqual(0, state.Value.Cards.DiscardPile.Count());
		Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
		Assert.AreEqual(deck, state.Value.Cards.Hand);
	}

	[Test]
	public void ShouldFPlayDefaultExpensiveCardFromHand() {
		var deck = new Card[] { new Card("foo",
			goldCost: 2,
			goldGain: 5,
		    defaultChoice: true
		)};
		var model = new SceneModel(CardsState.NewState(deck).ShuffleCurrentDeck().DrawCardsToHand(1),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();

		model.PlayCard(deck[0]);

		Assert.AreEqual(0, state.Value.Empire.Gold);
		Assert.AreEqual(5, state.Value.Empire.AddGold);
		Assert.AreEqual(1, state.Value.Cards.DiscardPile.Count());
		Assert.AreEqual(0, state.Value.Cards.CurrentDeck.Count());
		Assert.AreEqual(new Card[0], state.Value.Cards.Hand);
	}

	[Test]
  public void ShouldDrawCardAndPayOneGold() {
		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
			new EmpireState(1, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();

		model.TryDrawCard();

		Assert.AreEqual(1, state.Value.Cards.Hand.Count());
		Assert.AreEqual(new EmpireState(0, 0, 0, 0, new List<Card>()), state.Value.Empire);
	}

	[Test]
	public void ShouldNotDrawCardIfNoGoldIsAvailable() {
		var model = new SceneModel(CardsState.NewState(initialDeck).ShuffleCurrentDeck(),
			new EmpireState(0, 0, 0, 0, new List<Card>()), CardHandlingMode.Regular);
		var state = model.State.ToReactiveProperty();

		model.TryDrawCard();

		Assert.AreEqual(0, state.Value.Cards.Hand.Count());
		Assert.AreEqual(new EmpireState(0, 0, 0, 0, new List<Card>()), state.Value.Empire);
	}
}
