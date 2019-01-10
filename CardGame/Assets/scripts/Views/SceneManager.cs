using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
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
	private MultiCardDisplayScript multiCardDisplay;
	private TopBarView topBarView;
	private readonly CardScript[] currentHand = new CardScript[9];
	private TextMeshProUGUI textBox;

	private CardScriptPool cardPool;
	private ISceneViewModel viewModel;
	private List<List<Action>> animationOrders = new List<List<Action>>();
	private readonly object cardAnimationsLock = new object();
	private int currentCardAnimationsInProgress;
	private TraditionScript[] traditionScripts;
	private TrainViewScript trainView;

	void Start() {
		setPrivateGameObjects();
		setupViewModel();
	}

	private void setPrivateGameObjects() {
		topBarView = GameObject.FindObjectOfType<TopBarView>();
		cardPool = new CardScriptPool(CardPrefab, 40);
		deck = GameObject.Find("Deck").GetComponent<DeckScript>();
		discardPile = GameObject.Find("Discard Pile").GetComponent<DeckScript>();
		multiCardDisplay = Resources.FindObjectsOfTypeAll<MultiCardDisplayScript>()[0];
		multiCardDisplay.InitialSetup(cardPool);
		traditionScripts = FindObjectsOfType<TraditionScript>()
			.OrderBy(traditionScript => traditionScript.transform.position.x)
			.ToArray();
		trainView = FindObjectOfType<TrainViewScript>();
		textBox = GameObject.Find("MainText").GetComponent<TextMeshProUGUI>();
	}

	private void setupViewModel() {
		viewModel = new SceneViewModel();
		setViewModelInputs();
		osberveViewModelOutputs();
	}

	private void setViewModelInputs() {
		viewModel.setDeckWasClicked(deck.OnMouseDownAsObservable());
		viewModel.setDoneButtonClicked(topBarView.DoneButtonClicked());
		viewModel.setDriveButtonClicked(topBarView.DriveButtonClicked());
		viewModel.setStayButtonClicked(topBarView.StayButtonClicked());
	}

	private void osberveViewModelOutputs() {
		viewModel.MainTextContent.Subscribe(description => {
			textBox.text = description;
		});
		viewModel.PopulationValue.Subscribe(topBarView.SetPopulationValue);
		viewModel.FuelValue.Subscribe(topBarView.SetFuelValue);
		viewModel.MaterialsValue.Subscribe(topBarView.SetMaterialsValue);
		viewModel.ArmyValue.Subscribe(topBarView.SetArmyValue);
		viewModel.DeckCount.Subscribe(count => {
			deck.SetCardNumber(count);
		});
		viewModel.DiscardPileCount.Subscribe(count => {
			discardPile.SetCardNumber(count);
		});
		viewModel.DisplayDoneButton.Subscribe(topBarView.DisplayDoneButton);
		viewModel.DisplayDriveButton.Subscribe(topBarView.DisplayDriveButton);
		viewModel.DisplayStayButton.Subscribe(topBarView.DisplayStayButton);
		viewModel.CardMovementInstructions.Subscribe(moveCards);
		viewModel.TextForDoneButton.Subscribe(topBarView.SetDoneButtonText);
		viewModel.HideMultiDisplay.Subscribe(_ => multiCardDisplay.FinishWork());
		Observable.Zip(viewModel.CardsInMultiDisplay, viewModel.TextForMultiDisplay, toCardsTextPair)
			.Subscribe(setMultiCardDisplayCardSelectionObservation);
		viewModel.Traditions
			.DistinctUntilChanged()
			.Subscribe(updateTraditions);
		viewModel.Train.Subscribe(train => trainView.setTrain(train));
	}

	private void moveCards(IEnumerable<CardMovementInstruction> instructions) {
		lock(cardAnimationsLock) {
			animationOrders.Add(instructions.Select(animateInstruction).ToList());
			if (animationOrders.Count == 1) {
				startNextOrdersRound(animationOrders[0]);
			}
		}
	}

	private Action animateInstruction(CardMovementInstruction instruction) {
		return () => {
			CardScript cardScript;
			if (locationInHand(instruction.From)) {
				var fromIndex = handIndexForLocation(instruction.From);
				cardScript = currentHand[fromIndex];
				currentHand[fromIndex] = null;
			} else {
				cardScript = cardPool.CardForModel(instruction.Card);
				cardScript.transform.position = positionForLocation(instruction.From);
			}

			StartCoroutine(cardScript.gameObject.MoveOverSpeed(
			 positionForLocation(instruction.To),
			 30,
			 () => {
				 if (!locationInHand(instruction.To)) {
					 cardPool.ReleaseCard(cardScript);
				 } else {
					 if (!locationInHand(instruction.From)) {
						 viewModel.setSelectedCardObservation(cardScript.ClickObservation());
					 }
					 currentHand[handIndexForLocation(instruction.To)] = cardScript;
				 }
				 markCardAnimationEnded();
			 }));
		};
	}

	private void markCardAnimationEnded() {
		lock (cardAnimationsLock) {
			--currentCardAnimationsInProgress;
			if (currentCardAnimationsInProgress > 0) {
				return;
			}
			animationOrders.RemoveAt(0);
			var nextOrders = animationOrders.FirstOrDefault();
			if (nextOrders != null) {
				startNextOrdersRound(nextOrders);
			}
		}
	}

	private void startNextOrdersRound(List<Action> nextOrders) {
		lock(cardAnimationsLock) {
			if (nextOrders.Count == 0) {
				markCardAnimationEnded();
				return;
			}
			currentCardAnimationsInProgress = nextOrders.Count;
			StartCoroutine(animateNextRound(nextOrders));
		}
	}

	private IEnumerator animateNextRound(List<Action> nextOrders) {
		foreach (var action in nextOrders) {
			action();
			yield return new WaitForSeconds(0.03f);
		}
	}

	private bool locationInHand(ScreenLocation location) {
		return location != ScreenLocation.Center &&
			location != ScreenLocation.Deck &&
			location != ScreenLocation.DiscardPile;
	}

	private Vector3 positionForLocation(ScreenLocation location) {
		switch(location) {
			case ScreenLocation.Center:
				return Vector3.zero;
			case ScreenLocation.Deck:
				return deck.transform.position;
			case ScreenLocation.DiscardPile:
				return discardPile.transform.position;
			case ScreenLocation.Hand1:
			case ScreenLocation.Hand2:
			case ScreenLocation.Hand3:
			case ScreenLocation.Hand4:
			case ScreenLocation.Hand5:
			case ScreenLocation.Hand6:
			case ScreenLocation.Hand7:
			case ScreenLocation.Hand8:
			case ScreenLocation.Hand9:
				return handLocationForIndex(handIndexForLocation(location));
			default:
				throw new ArgumentException($"Received {location} as position");
		}
	}

	private int handIndexForLocation(ScreenLocation location) {
		switch (location) {
			case ScreenLocation.Hand1: return 0;
			case ScreenLocation.Hand2: return 1;
			case ScreenLocation.Hand3: return 2;
			case ScreenLocation.Hand4: return 3;
			case ScreenLocation.Hand5: return 4;
			case ScreenLocation.Hand6: return 5;
			case ScreenLocation.Hand7: return 6;
			case ScreenLocation.Hand8: return 7;
			case ScreenLocation.Hand9: return 8;
			default:
				throw new ArgumentException($"Received {location} as hand location");
		}
	}

	private Vector3 handLocationForIndex(int index) {
		Vector3 deckRight = deck.transform.position + Vector3.right * deck.GetComponent<BoxCollider2D>().size.x / 2 * deck.transform.localScale.x;
		var size = CardPrefab.GetComponent<BoxCollider2D>().size * CardPrefab.transform.localScale.x;
		return deckRight + new Vector3((size.x * index) + (size.x / 2), 0, 0);
	}

	private KeyValuePair<IEnumerable<CardDisplayModel>, string> toCardsTextPair(IEnumerable<CardDisplayModel> cards, string text) {
		return new KeyValuePair<IEnumerable<CardDisplayModel>, string>(cards, text);
	}

	private void setMultiCardDisplayCardSelectionObservation(KeyValuePair<IEnumerable<CardDisplayModel>, string> pair) {
		viewModel.setSelectedCardObservation(multiCardDisplay.setup(pair.Key, pair.Value));
	}

	private void updateTraditions(IEnumerable<Tradition> traditions) {
		var traditionsAsList = traditions.ToList();
		AssertUtils.EqualOrLesser(traditionsAsList.Count, traditionScripts.Length);
		for (int i = 0; i < traditionsAsList.Count; i++) {
			traditionScripts[i].setTradition(traditionsAsList[i]);
		}
		for (int i = traditionsAsList.Count; i < traditionScripts.Length; i++) {
			traditionScripts[i].setTradition(null);
		}
	}
}
