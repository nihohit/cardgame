using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CardScript : MonoBehaviour {
  private TextMeshPro text;
	private CardValueScript goldCost;
	private CardValueScript populationCost;
	private CardValueScript industryCost;
	private CardValueScript armyCost;
	private CardValueScript goldGain;
	private CardValueScript industryGain;
	private CardValueScript populationGain;
	private CardValueScript armyGain;
	public SceneManager Manager { get; set; }

	private Card _model;
	public Card CardModel { get {
			return _model;
		} set {
			_model = value;
			text.text = value.ToString();
			goldCost.SetValue(value.GoldCost);
			populationCost.SetValue(value.PopulationCost);
			industryCost.SetValue(value.IndustryCost);
			armyCost.SetValue(value.ArmyCost);
			goldGain.SetValue(value.GoldGain);
			industryGain.SetValue(value.IndustryGain);
			populationGain.SetValue(value.PopulationGain - value.PopulationCost);
			armyGain.SetValue(value.ArmyGain);
		}
	}

	// Use this for initialization
	void Awake() {
    text = GetComponentInChildren<TextMeshPro>();
		goldCost = transform.Find("GoldCost").GetComponent< CardValueScript>();
		populationCost = transform.Find("PopulationCost").GetComponent<CardValueScript>();
		industryCost = transform.Find("IndustryCost").GetComponent<CardValueScript>();
		armyCost = transform.Find("ArmyCost").GetComponent<CardValueScript>();
		goldGain = transform.Find("GoldGain").GetComponent<CardValueScript>();
		industryGain = transform.Find("IndustryGain").GetComponent<CardValueScript>();
		populationGain = transform.Find("PopulationGain").GetComponent<CardValueScript>();
		armyGain = transform.Find("ArmyGain").GetComponent<CardValueScript>();
	}

	private void OnMouseDown() {
		Manager.CardWasClicked(this);
	}
}
