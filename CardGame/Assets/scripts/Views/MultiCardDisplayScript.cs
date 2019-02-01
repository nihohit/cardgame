using UnityEngine;
using System.Collections;
using TMPro;
using System.Collections.Generic;
using System.Linq;
using System;
using UniRx;

public class MultiCardDisplayScript : MonoBehaviour {
	public string Description { get { return textMesh.text; } }
	private TextMeshPro textMesh;
	private readonly List<CardScript> cardScripts = new List<CardScript>();
	private CardScriptPool cardPool;

	private void Awake() {
		textMesh = GetComponentInChildren<TextMeshPro>();
		foreach (var text in GetComponentsInChildren<TextMeshPro>()) {
			text.sortingLayerID = SortingLayer.NameToID("MultiCardDisplay");
		}
		foreach (var internalRenderer in GetComponentsInChildren<Renderer>()) {
			internalRenderer.sortingLayerID = SortingLayer.NameToID("MultiCardDisplay");
		}

	}

	public void InitialSetup(CardScriptPool cardScriptPool) {
		cardPool = cardScriptPool;
	}

	public IObservable<Card> setup(IEnumerable<CardDisplayModel> cards, string description) {
		gameObject.SetActive(true);
		textMesh.text = description;
		releaseCards();
		return setupCardScripts(cards);
	}

	private IObservable<Card> setupCardScripts(IEnumerable<CardDisplayModel> cards) {
		AssertUtils.IsEmpty(cardScripts, "cardScripts");
		cardScripts.AddRange(cards.Select(card => cardPool.CardForModel(card)));
		foreach (var cardScript in cardScripts) {
			setupCardScriptLayers(cardScript, "MultiCardDisplay");
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
	}
}
