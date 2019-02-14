using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

public class MultiCardDisplayScript : MonoBehaviour {
	public string Title { get { return titleBox.text; } }
	public string Description { get { return descriptionBox.text; } }
	public GameObject sprite;
	private TextMeshPro titleBox;
	private TextMeshPro descriptionBox;
	private readonly List<CardScript> cardScripts = new List<CardScript>();
	private CardScriptPool cardPool;

	private const string kMultiCardLayerName = "MultiCardDisplay";

	private void Awake() {
		titleBox = transform.Find("Title").GetComponent<TextMeshPro>();
		descriptionBox = transform.Find("Description").GetComponent<TextMeshPro>();
		foreach (var text in GetComponentsInChildren<TextMeshPro>()) {
			text.sortingLayerID = SortingLayer.NameToID(kMultiCardLayerName);
		}
		foreach (var internalRenderer in GetComponentsInChildren<Renderer>()) {
			internalRenderer.sortingLayerID = SortingLayer.NameToID(kMultiCardLayerName);
		}

		sprite.transform.localScale = new Vector3(Screen.width, Screen.height, 1);
	}

	public void InitialSetup(CardScriptPool cardScriptPool) {
		cardPool = cardScriptPool;
	}

	public IObservable<Card> setup( 
		string title,
		string description,
		IEnumerable<CardDisplayModel> cards) {
		gameObject.SetActive(true);
		titleBox.text = title;
		descriptionBox.text = description;
		releaseCards();
		return setupCardScripts(cards);
	}

	private IObservable<Card> setupCardScripts(IEnumerable<CardDisplayModel> cards) {
		AssertUtils.IsEmpty(cardScripts, "cardScripts");
		cardScripts.AddRange(cards.Select(card => cardPool.CardForModel(card)));
		foreach (var cardScript in cardScripts) {
			setupCardScriptLayers(cardScript, kMultiCardLayerName);
		}
		adjustCardsLocations();
		return cardScripts.Select(cardScript => cardScript.ClickObservation()).Merge();
	}

	private void adjustCardsLocations() {
		Camera cam = Camera.main;
		float camWindowHeight = 2f * cam.orthographicSize;
		float camWindowWidth = camWindowHeight * cam.aspect;
		var halfCamWidth = camWindowWidth / 2;
		var cardSize = cardScripts[0].GetComponent<BoxCollider2D>().size;
		var widthPerCard = camWindowWidth / cardScripts.Count;
		AssertUtils.Greater(widthPerCard, cardSize.x);

		for (int i = 0; i < cardScripts.Count; i++) {
			var cardScript = cardScripts[i];
			cardScript.transform.position = new Vector3(-halfCamWidth + (widthPerCard / 2) + (i * widthPerCard), -camWindowHeight / 3, 0);
		}
	}

	public void FinishWork() {
		releaseCards();
		gameObject.SetActive(false);
	}

	private void releaseCards() {
		foreach (var cardScript in cardScripts) {
			setupCardScriptLayers(cardScript, "Default");
			cardPool.ReleaseCard(cardScript);
		}
		cardScripts.Clear();
	}

	private void setupCardScriptLayers(CardScript script, string layerName) {
		foreach (var text in script.GetComponentsInChildren<TextMeshPro>()) {
			text.sortingLayerID = SortingLayer.NameToID(layerName);
		}
		foreach (var internalRenderer in script.GetComponentsInChildren<Renderer>()) {
			internalRenderer.sortingLayerID = SortingLayer.NameToID(layerName);
		}
		script.gameObject.layer = LayerMask.NameToLayer(layerName);
	}
}
