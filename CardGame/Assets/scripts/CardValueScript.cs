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

	public void SetDoubleValue(int available, int change) {
		var visible = available != 0 || change != 0;
		innerRenderer.enabled = visible;
		text.enabled = visible;
		text.text = $"{available}/{change}";
	}
}
