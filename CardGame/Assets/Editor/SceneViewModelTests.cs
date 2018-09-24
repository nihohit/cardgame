using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System;
using UniRx;
using System.Collections.Generic;

public class SceneViewModelTests {
	private readonly static Card[] initialDeck = new[] { cardWithName("foo"), cardWithName("bar"), cardWithName("baz"), cardWithName("bro") };

	private static Card cardWithName(string name) {
		return new Card(name);
	}

	private class FakeModel : ISceneModel {
		public Subject<SceneState> StateSubject { get; private set; } = new Subject<SceneState>();
		public Card PlayedCard { get; private set; }
		public bool TryDrawCardCalled { get; private set; }
		public bool UserFinishedModeCalled { get; private set; }

		public IObservable<SceneState> State => StateSubject;

		public bool PlayCard(Card card) {
			PlayedCard = card;
			return true;
		}

		public void TryDrawCard() {
			TryDrawCardCalled = true;
		}

		public void UserFinishedMode() {
			UserFinishedModeCalled = true;
		}
	}

	private FakeModel fakeModel;
	private SceneViewModel viewModel;

	[SetUp]
	public void SetupTests() {
		fakeModel = new FakeModel();
		viewModel = new SceneViewModel(fakeModel);
	}

	private SceneState scene(CardsState cards = null, 
		EmpireState empire = null, 
		CardHandlingMode mode = CardHandlingMode.Regular,
		EventCard currentEvent = null) {
		return new SceneState(cards, empire, mode, currentEvent);
	}

	#region imports
	[Test]
  public void PassDoneButtonClicksToModel() {
		var subject = new Subject<Unit>();
		viewModel.setDoneButtonClicked(subject);

		Assert.IsFalse(fakeModel.UserFinishedModeCalled);

		subject.OnNext(Unit.Default);

		Assert.IsTrue(fakeModel.UserFinishedModeCalled);
	}

	[Test]
	public void PassDeckClicksToModelAsAttemptsToDrawCards() {
		var subject = new Subject<Unit>();
		viewModel.setDeckWasClicked(subject);

		Assert.IsFalse(fakeModel.TryDrawCardCalled);

		subject.OnNext(Unit.Default);

		Assert.IsTrue(fakeModel.TryDrawCardCalled);
	}

	[Test]
	public void PassSelectedCardModel() {
		var subject = new Subject<Card>();
		var card = new Card("");
		viewModel.setSelectedCardObservation(subject);

		Assert.IsNull(fakeModel.PlayedCard);

		subject.OnNext(card);

		Assert.AreSame(card, fakeModel.PlayedCard);
	}
	#endregion

	#region outputs

	[Test]
	public void PassStateDescription() {
		var description = viewModel.StateDescription.ToReactiveProperty();

		var state = new EmpireState(1, 2, 3, 4);
		fakeModel.StateSubject.OnNext(scene(empire: state));

		Assert.AreEqual(state.ToString(), description.Value);
	}

	[Test]
	public void PassDeckCount() {
		var count = viewModel.DeckCount.ToReactiveProperty();

		var cards = CardsState.NewState(initialDeck);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(0, count.Value);

		cards = cards.ShuffleCurrentDeck();
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(4, count.Value);

		cards = cards.DrawCardsToHand(1);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(3, count.Value);
	}

	[Test]
	public void PassDiscardPileCount() {
		var count = viewModel.DiscardPileCount.ToReactiveProperty();

		var cards = CardsState.NewState(initialDeck);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(0, count.Value);

		cards = cards.ShuffleCurrentDeck().DrawCardsToHand(2);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(0, count.Value);

		cards = cards.DiscardHand();
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		Assert.AreEqual(2, count.Value);
	}

	[Test]
	public void PassCardMovementsToAndFromHand() {
		Randomizer.SetTestableRandom(4);
		var sentInstructions = new List<IEnumerable<CardMovementInstruction>>();
		viewModel.CardMovementInstructions.Subscribe(sentInstructions.Add);

		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck();
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		sentInstructions.Clear();
		cards = cards.DrawCardsToHand(2);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> { new [] {
			new CardMovementInstruction(initialDeck[0], ScreenLocation.Deck, ScreenLocation.Hand1),
			new CardMovementInstruction(initialDeck[1], ScreenLocation.Deck, ScreenLocation.Hand2)
		} }, sentInstructions);

		sentInstructions.Clear();
		cards = cards.DiscardHand().DrawCardsToHand(2);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> {
			new [] {
				new CardMovementInstruction(initialDeck[0], ScreenLocation.Hand1, ScreenLocation.DiscardPile),
				new CardMovementInstruction(initialDeck[1], ScreenLocation.Hand2, ScreenLocation.DiscardPile),
			}, 
			new [] {
				new CardMovementInstruction(initialDeck[2], ScreenLocation.Deck, ScreenLocation.Hand1),
				new CardMovementInstruction(initialDeck[3], ScreenLocation.Deck, ScreenLocation.Hand2)
			}
		}, sentInstructions);

		sentInstructions.Clear();
		cards = cards.PlayCard(initialDeck[2]);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> { new [] {
			new CardMovementInstruction(initialDeck[2], ScreenLocation.Hand1, ScreenLocation.DiscardPile),
			new CardMovementInstruction(initialDeck[3], ScreenLocation.Hand2, ScreenLocation.Hand1)
		}}, sentInstructions);
	}

	[Test]
	public void PassCardMovementsToDeck() {
		Randomizer.SetTestableRandom(4);
		var sentInstructions = new List<IEnumerable<CardMovementInstruction>>();
		viewModel.CardMovementInstructions.Subscribe(sentInstructions.Add);

		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck();
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> { new [] {
			new CardMovementInstruction(initialDeck[0], ScreenLocation.Center, ScreenLocation.Deck),
			new CardMovementInstruction(initialDeck[1], ScreenLocation.Center, ScreenLocation.Deck),
			new CardMovementInstruction(initialDeck[2], ScreenLocation.Center, ScreenLocation.Deck),
			new CardMovementInstruction(initialDeck[3], ScreenLocation.Center, ScreenLocation.Deck)
		} }, sentInstructions);

		
		cards = cards.DrawCardsToHand(2).DiscardHand();
		fakeModel.StateSubject.OnNext(scene(cards: cards));
		sentInstructions.Clear();
		cards = cards.ShuffleDiscardToDeck();
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> { new[] {
			new CardMovementInstruction(initialDeck[0], ScreenLocation.DiscardPile, ScreenLocation.Deck),
			new CardMovementInstruction(initialDeck[1], ScreenLocation.DiscardPile, ScreenLocation.Deck)
		}}, sentInstructions);
	}

	[Test]
	public void PassCardMovementsToDiscard() {
		Randomizer.SetTestableRandom(4);
		var sentInstructions = new List<IEnumerable<CardMovementInstruction>>();
		viewModel.CardMovementInstructions.Subscribe(sentInstructions.Add);
		
		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck();
		fakeModel.StateSubject.OnNext(scene(cards: cards));
		sentInstructions.Clear();
		cards = cards.AddCardsToDiscard(initialDeck);
		fakeModel.StateSubject.OnNext(scene(cards: cards));

		CollectionAssert.AreEquivalent(new List<IEnumerable<CardMovementInstruction>> { new[] {
			new CardMovementInstruction(initialDeck[0], ScreenLocation.Center, ScreenLocation.DiscardPile),
			new CardMovementInstruction(initialDeck[1], ScreenLocation.Center, ScreenLocation.DiscardPile),
			new CardMovementInstruction(initialDeck[2], ScreenLocation.Center, ScreenLocation.DiscardPile),
			new CardMovementInstruction(initialDeck[3], ScreenLocation.Center, ScreenLocation.DiscardPile)
		}}, sentInstructions);
	}

	[Test]
	public void PassDisplayDoneButtonAccordingToMode() {
		var shouldDisplay = viewModel.DisplayDoneButton.ToReactiveProperty();

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Exhaust));

		Assert.IsTrue(shouldDisplay.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Replace));

		Assert.IsTrue(shouldDisplay.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Regular));

		Assert.IsTrue(shouldDisplay.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Event));

		Assert.IsFalse(shouldDisplay.Value);
	}

	[Test]
	public void PassCardsInMultiDisplay() {
		var displayedCards = viewModel.CardsInMultiDisplay.ToReactiveProperty();

		var cards = CardsState.NewState(initialDeck).ShuffleCurrentDeck().DrawCardsToHand(4);
		var eventOptions = new Card[] { cardWithName("hi"), cardWithName("bye") };
		var eventCard = new EventCard {
			Name = "test",
			Options = eventOptions
		};
		fakeModel.StateSubject.OnNext(scene(cards: cards, 
			mode: CardHandlingMode.Exhaust, 
			currentEvent: eventCard));

		CollectionAssert.AreEquivalent(initialDeck, displayedCards.Value);

		fakeModel.StateSubject.OnNext(scene(cards: cards,
			mode: CardHandlingMode.Event,
			currentEvent: eventCard));

		CollectionAssert.AreEquivalent(eventOptions, displayedCards.Value);

		fakeModel.StateSubject.OnNext(scene(cards: cards,
			mode: CardHandlingMode.Replace,
			currentEvent: eventCard));

		CollectionAssert.AreEquivalent(initialDeck, displayedCards.Value);
	}

	[Test]
	public void PassMultiDisplayTextAccordingToState() {
		var description = viewModel.TextForMultiDisplay.ToReactiveProperty();

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Exhaust));

		Assert.AreEqual("Choose cards to exhaust", description.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Replace));

		Assert.AreEqual("Choose cards to replace", description.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Event, currentEvent: new EventCard {
			Name = "test"
		}));

		Assert.AreEqual(description.Value, "test");
	}

	[Test]
	public void PassTextForDoneButton() {
		var description = viewModel.TextForDoneButton.ToReactiveProperty();

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Exhaust));

		Assert.AreEqual("Done", description.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Regular));

		Assert.AreEqual("End Turn", description.Value);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Replace));

		Assert.AreEqual("Done", description.Value);
	}

	[Test]
	public void PassHideMultiDisplay() {
		int numberOfCounts = 0;
		viewModel.HideMultiDisplay.Subscribe(_ => {
			++numberOfCounts;
		});

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Exhaust));

		Assert.AreEqual(0, numberOfCounts);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Replace));

		Assert.AreEqual(0, numberOfCounts);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Regular));

		Assert.AreEqual(1, numberOfCounts);

		fakeModel.StateSubject.OnNext(scene(mode: CardHandlingMode.Event));

		Assert.AreEqual(1, numberOfCounts);
	}
	#endregion
}
