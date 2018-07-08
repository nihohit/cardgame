using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardScript : MonoBehaviour {
  private TextMesh text;

	// Use this for initialization
	void Awake() {
    text = GetComponentInChildren<TextMesh>();
	}

  void SetName(string name) {
    text.text = name;
  }
}
