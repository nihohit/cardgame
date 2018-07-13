using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour {
  private TextMesh text;
	public SceneManager Manager { get; set; }

	private Card _model;
	public Card CardModel { get {
			return _model;
		} set {
			_model = value;
			text.text = value.Name;
		}
	}

	// Use this for initialization
	void Awake() {
    text = GetComponentInChildren<TextMesh>();
	}

	private void OnMouseDown() {
		Manager.CardWasClicked(this);
	}
}
