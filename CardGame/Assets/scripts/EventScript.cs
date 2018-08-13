using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EventScript : MonoBehaviour {
	private CardScript option1;
	private CardScript option2;
	private CardScript defaultCard;
  private TextMeshPro text;

	private EventCard _event;
	public EventCard Event {get {
		return _event;
	} set {
		_event = value;
		gameObject.SetActive(value != null);
		if (value == null) {
			return;
		}
		text.text = value.Name;
		option1.CardModel = value.Option1;
		option2.CardModel = value.Option2;
		defaultCard.CardModel = value.Default;
	}}

	public void Start() {
		foreach (var text in GetComponentsInChildren<TextMeshPro>()) {
			text.sortingLayerID = SortingLayer.NameToID("event card");
		}
		foreach (var internalRenderer in GetComponentsInChildren<Renderer>()) {
			internalRenderer.sortingLayerID = SortingLayer.NameToID("event card");
		}
	}

	public void SetSceneManager(SceneManager manager) {
		option1 = transform.Find("Option1").GetComponent<CardScript>();
		option2 = transform.Find("Option2").GetComponent<CardScript>();
		defaultCard = transform.Find("Default").GetComponent<CardScript>();
		text = GetComponentInChildren<TextMeshPro>();
		option1.Manager = manager;
		option2.Manager = manager;
		defaultCard.Manager = manager;
	}
}
