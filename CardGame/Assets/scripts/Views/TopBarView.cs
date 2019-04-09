using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TopBarView : MonoBehaviour {
	private TextMeshProUGUI populationValue;
	private TextMeshProUGUI goldValue;
	private TextMeshProUGUI armyValue;
	private TextMeshProUGUI knowledgeValue;
	private Button doneButton;

	// Use this for initialization
	void Awake () {
		doneButton = transform.Find("DoneButton").GetComponent<Button>();
		populationValue = transform.Find("PopulationMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		goldValue = transform.Find("GoldMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		armyValue = transform.Find("ArmyMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		knowledgeValue = transform.Find("KnowledgeMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
	}

	public void DisplayDoneButton(bool display) {
		doneButton.gameObject.SetActive(display);
	}

	public System.IObservable<Unit> DoneButtonClicked() {
		return doneButton.onClick.AsObservable();
	}

	public void SetDoneButtonText(string text) {
		doneButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	public void SetPopulationValue(string value) {
		populationValue.text = value;
	}

	public void SetGoldValue(string value) {
		goldValue.text = value;
	}

	public void SetArmyValue(string value) {
		armyValue.text = value;
	}

	public void SetKnowledgeValue(string value) {
		knowledgeValue.text = value;
	}
}
