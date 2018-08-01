using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CardValueScript : MonoBehaviour {
	private TextMeshPro text;
	private Renderer innerRenderer;

	// Use this for initialization
	void Awake () {
		text = GetComponentInChildren<TextMeshPro>();
		innerRenderer = GetComponentInChildren<Renderer>();
	}
	
	public void SetValue(int value) {
		innerRenderer.enabled = value != 0;
		text.enabled = value != 0;
		text.text = value.ToString();
	}
}
