using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardScript : MonoBehaviour {
  private TextMeshPro text;
	public SceneManager Manager { get; set; }

	private Card _model;
	public Card CardModel { get {
			return _model;
		} set {
			_model = value;
			text.text = value.ToString();
		}
	}

	// Use this for initialization
	void Awake() {
    text = GetComponentInChildren<TextMeshPro>();
	}

	private void OnMouseDown() {
		Manager.CardWasClicked(this);
	}
}
