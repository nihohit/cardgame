using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

public interface ISceneViewModel {
	#region inputs
	void setDoneButtonClicked(IObservable<Unit> observable);
	void setSelectedCardObservation(IObservable<Card> observable);
	void setDeckWasClicked(IObservable<Unit> observable);
	#endregion

	#region outputs
	IObservable<string> StateDescription { get; }
	IObservable<int> DeckCount { get; }
	IObservable<int> DiscardPileCount { get; }
	IObservable<IEnumerable<Card>> Hand { get; }
	IObservable<bool> DisplayDoneButton { get; }
	IObservable<IEnumerable<Card>> CardsInMultiDisplay { get; }
	IObservable<string> TextForMultiDisplay { get; }
	IObservable<string> TextForDoneButton { get; }
	IObservable<Unit> HideMultiDisplay { get; }
	#endregion
}

public class SceneViewModel : ISceneViewModel {
	private ISceneModel model;

	#region ISceneViewModel
	public void setDoneButtonClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.UserFinishedMode());
	}

	public void setSelectedCardObservation(IObservable<Card> observable) {
		observable.Subscribe(cardWasClicked);
	}

	private void cardWasClicked(Card card) {
		model.PlayCard(card);
	}

	public void setDeckWasClicked(IObservable<Unit> observable) {
		observable.Subscribe(_ => model.TryDrawCard());
	}

	public IObservable<string> StateDescription => model.State
		.Select(state => state.Empire.ToString());

	public IObservable<int> DeckCount => model.State
		.Select(state => state.Cards.CurrentDeck.Count());

	public IObservable<int> DiscardPileCount => model.State
		.Select(state => state.Cards.DiscardPile.Count());

	public IObservable<IEnumerable<Card>> Hand => model.State
		.Select(state => state.Cards.Hand);

	public IObservable<bool> DisplayDoneButton => model.State
		.Select(state => state.Mode != CardHandlingMode.Event);

	public IObservable<IEnumerable<Card>> CardsInMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(cardsInMultiCardDisplay);

	private IEnumerable<Card> cardsInMultiCardDisplay(SceneState state) {
		if (state.Mode == CardHandlingMode.Event) {
			return state.CurrentEvent.Options;
		}
		return state.Cards.Hand;
	}

	public IObservable<string> TextForMultiDisplay => model.State
		.Where(state => state.Mode != CardHandlingMode.Regular)
		.Select(stateMultiCardDescription);

	private string stateMultiCardDescription(SceneState state) {
		switch (state.Mode) {
			case CardHandlingMode.Exhaust:
				return "Choose cards to exhaust";
			case CardHandlingMode.Replace:
				return "Choose cards to replace";
			case CardHandlingMode.Event:
				return state.CurrentEvent.Name;
			default:
				AssertUtils.UnreachableCode();
				break;
		}
		return null;
	}

	public IObservable<string> TextForDoneButton => model.State
		.Select(state => state.Mode == CardHandlingMode.Regular ? "End Turn" : "Done");

	public IObservable<Unit> HideMultiDisplay => model.State
		.Select(state => state.Mode)
		.DistinctUntilChanged()
		.Where(mode => mode == CardHandlingMode.Regular)
		.Select(_ => Unit.Default);
	#endregion

	public SceneViewModel() : this(new SceneModel()) { }

	public SceneViewModel(ISceneModel model) {
		this.model = model;
	}
}
