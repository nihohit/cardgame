using System.Collections;
using System.Collections.Generic;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

public class TopBarView : MonoBehaviour {
	private TextMeshProUGUI populationValue;
	private TextMeshProUGUI fuelValue;
	private TextMeshProUGUI materialsValue;
	private TextMeshProUGUI armyValue;
	private Button doneButton;
	private Button driveButton;
	private Button stayButton;

	// Use this for initialization
	void Awake () {
		doneButton = transform.Find("DoneButton").GetComponent<Button>();
		driveButton = transform.Find("DriveButton").GetComponent<Button>();
		stayButton = transform.Find("StayButton").GetComponent<Button>();
		populationValue = transform.Find("PopulationMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		fuelValue = transform.Find("FuelMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		materialsValue = transform.Find("MaterialsMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
		armyValue = transform.Find("ArmyMarker")
			.Find("Value").GetComponent<TextMeshProUGUI>();
	}

	public void DisplayStayButton(bool display) {
		stayButton.gameObject.SetActive(display);
	}

	public void DisplayDriveButton(bool display) {
		driveButton.gameObject.SetActive(display);
	}

	public void DisplayDoneButton(bool display) {
		doneButton.gameObject.SetActive(display);
	}

	public System.IObservable<Unit> DoneButtonClicked() {
		return doneButton.onClick.AsObservable();
	}

	public System.IObservable<Unit> DriveButtonClicked() {
		return driveButton.onClick.AsObservable();
	}

	public System.IObservable<Unit> StayButtonClicked() {
		return stayButton.onClick.AsObservable();
	}

	public void SetDoneButtonText(string text) {
		doneButton.GetComponentInChildren<TextMeshProUGUI>().text = text;
	}

	public void SetPopulationValue(string value) {
		populationValue.text = value;
	}

	public void SetFuelValue(string value) {
		fuelValue.text = value;
	}

	public void SetMaterialsValue(string value) {
		materialsValue.text = value;
	}

	public void SetArmyValue(string value) {
		armyValue.text = value;
	}
}
