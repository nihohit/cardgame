using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TraditionScript : MonoBehaviour {
	private TextMeshPro text;
	private Tradition tradition;

	private void Awake() {
		text = GetComponentInChildren<TextMeshPro>();
	}

	public void setTradition(Tradition tradition) {
		this.tradition = tradition;
		if (tradition == null) {
			gameObject.SetActive(false);
			return;
		}
		gameObject.SetActive(true);
		text.gameObject.SetActive(false);
		text.text = textForTradition();
	}

	private void OnMouseEnter() {
		text.gameObject.SetActive(true);
	}

	private string textForTradition() {
		return $"Increase {tradition.CardToEnhance}'s {tradition.PropertyToEnhance} by {tradition.IncreaseInValue}";
	}

	private void OnMouseExit() {
		text.gameObject.SetActive(false);
	}
}
