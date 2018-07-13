using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckScript : MonoBehaviour {
  private TextMesh text;
	public SceneManager Manager { get; set; }

  // Use this for initialization
  void Awake () {
    text = GetComponentInChildren<TextMesh>();
  }
	
  public void SetCardNumber(int cardNumber) {
    text.text = cardNumber.ToString();
  }

	private void OnMouseDown() {
		Manager.DeckWasClicked(this);
	}
}
