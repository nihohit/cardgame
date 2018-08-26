using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UI;

public class SceneManager : MonoBehaviour {
	public GameObject CardPrefab;
	public GameObject stateDescription;
	private DeckScript deck;
	private DeckScript discardPile;
	private Button doneButton;
	private MultiCardDisplayScript multiCardDisplay;
	private IEnumerable<CardScript> currentHand = new List<CardScript>();

	private CardScriptPool cardPool;
	private ISceneViewModel viewModel;

	void Start() {
		setPrivateGameObjects();
		setupViewModel();
	}

	private void setPrivateGameObjects() {
		cardPool = new CardScriptPool(CardPrefab, 15);
		doneButton = GameObject.Find("DoneButton").GetComponent<Button>();
		deck = GameObject.Find("Deck").GetComponent<DeckScript>();
		discardPile = GameObject.Find("Discard Pile").GetComponent<DeckScript>();
		multiCardDisplay = Resources.FindObjectsOfTypeAll<MultiCardDisplayScript>()[0];
		multiCardDisplay.InitialSetup(cardPool);
	}

	private void setupViewModel() {
		viewModel = new SceneViewModel();
		setViewModelInputs();
		osberveViewModelOutputs();
	}

	private void setViewModelInputs() {
		viewModel.setDeckWasClicked(deck.OnMouseDownAsObservable());
		viewModel.setDoneButtonClicked(doneButton.onClick.AsObservable());
	}

	private void osberveViewModelOutputs() {
		viewModel.StateDescription.Subscribe(description => {
			stateDescription.GetComponent<Text>().text = description;
		});
		viewModel.DeckCount.Subscribe(count => {
			deck.SetCardNumber(count);
		});
		viewModel.DiscardPileCount.Subscribe(count => {
			discardPile.SetCardNumber(count);
		});
		viewModel.DisplayDoneButton.Subscribe(active => {
			doneButton.gameObject.SetActive(active);
		});
		viewModel.Hand.Subscribe(setNewHand);
		viewModel.TextForDoneButton.Subscribe(setDoneButtonText);
		viewModel.HideMultiDisplay.Subscribe(_ => multiCardDisplay.FinishWork());
		Observable.Zip(viewModel.CardsInMultiDisplay, viewModel.TextForMultiDisplay, toCardsTextPair).Subscribe(setMultiCardDisplayCardSelectionObservation);
	}

	private void setNewHand(IEnumerable<Card> hand) {
		releaseCurrentHand();
		currentHand = hand.Select(cardModel => cardPool.CardForModel(cardModel)).ToList();
		setupHandCardsLocations();
		setHandCardSelectionObservation();
	}

	private void releaseCurrentHand() {
		foreach (var cardScript in currentHand) {
			cardPool.ReleaseCard(cardScript);
		}
	}

	private void setupHandCardsLocations() {
		Vector3 nextPosition = deck.transform.position + Vector3.right * deck.GetComponent<BoxCollider2D>().size.x / 2 * deck.transform.localScale.x;
		foreach (var card in currentHand) {
			var size = card.GetComponent<BoxCollider2D>().size * card.transform.localScale.x;
			card.transform.position = nextPosition + new Vector3(size.x / 2, 0, 0);
			nextPosition += new Vector3(size.x, 0, 0);
		}
	}

	private void setHandCardSelectionObservation() {
		viewModel.setSelectedCardObservation(currentHand.Select(cardScript => cardScript.ClickObservation()).Merge());
	}

	private void setDoneButtonText(string text) {
		doneButton.GetComponentInChildren<Text>().text = text;
	}

	private KeyValuePair<IEnumerable<Card>, string> toCardsTextPair(IEnumerable<Card> cards, string text) {
		return new KeyValuePair<IEnumerable<Card>, string>(cards, text);
	}

	private void setMultiCardDisplayCardSelectionObservation(KeyValuePair<IEnumerable<Card>, string> pair) {
		viewModel.setSelectedCardObservation(multiCardDisplay.setup(pair.Key, pair.Value));
	}
}
