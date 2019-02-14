using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

public class DeckScript : MonoBehaviour {
  private TextMesh text;
	private InputHandler touchHandler;

	// Use this for initialization
	void Awake () {
		touchHandler = FindObjectOfType<InputHandler>();
		text = GetComponentInChildren<TextMesh>();
  }
	
  public void SetCardNumber(int cardNumber) {
    text.text = cardNumber.ToString();
  }

	public IObservable<Unit> OnTouchDownAsObservable() {
		return touchHandler.TouchedGameObject
			.Where(obj => obj == gameObject)
			.Select(_ => Unit.Default);
	}
}
