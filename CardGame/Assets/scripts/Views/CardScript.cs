﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UniRx.Triggers;
using UniRx;
using System.Text;

public class CardScript : MonoBehaviour {
  private TextMeshPro text;
	private CardValueScript fuelCost;
	private CardValueScript populationCost;
	private CardValueScript materialsCost;
	private CardValueScript armyCost;
	private CardValueScript fuelGain;
	private CardValueScript materialsGain;
	private CardValueScript populationGain;
	private CardValueScript armyGain;

	private Subject<Unit> modelSetSubject = new Subject<Unit>();
	private Card _model;
	public Card CardModel { get {
			return _model;
		} set {
			_model = value;
			text.text = cardDescription(value);
			fuelCost.SetValue(value.FuelChange);
			populationCost.SetDoubleValue(-value.PopulationCost, value.PopulationChange);
			materialsCost.SetValue(value.MaterialsChange);
			armyCost.SetValue(value.ArmyChange);
			modelSetSubject.OnNext(Unit.Default);

			fuelGain.SetValue(0);
			materialsGain.SetValue(0);
			armyGain.SetValue(0);
			populationGain.SetValue(0);
		}
	}

	private void Awake() {
    text = GetComponentInChildren<TextMeshPro>();
		fuelCost = transform.Find("FuelCost").GetComponent<CardValueScript>();
		populationCost = transform.Find("PopulationCost").GetComponent<CardValueScript>();
		materialsCost = transform.Find("MaterialsCost").GetComponent<CardValueScript>();
		armyCost = transform.Find("ArmyCost").GetComponent<CardValueScript>();
		fuelGain = transform.Find("FuelGain").GetComponent<CardValueScript>();
		materialsGain = transform.Find("MaterialsGain").GetComponent<CardValueScript>();
		populationGain = transform.Find("PopulationGain").GetComponent<CardValueScript>();
		armyGain = transform.Find("ArmyGain").GetComponent<CardValueScript>();
	}

	public IObservable<Card> ClickObservation() {
		return this.OnMouseDownAsObservable()
			.Select(_ => CardModel)
			.TakeUntil(modelSetSubject);
	}

	private string cardDescription(Card card) {
		var stringBuilder = new StringBuilder();
		stringBuilder.AppendLine(card.Name);
		if (card.AddTradition != TraditionType.None) {
			stringBuilder.AppendLine($"Add deck: {card.AddTradition}");
		}
		if (card.Exhaustible) {
			stringBuilder.AppendLine("Exhaustible");
		}
		addString(stringBuilder, card.NumberOfCardsToChooseToExhaust, "Remove cards");
		addString(stringBuilder, card.NumberOfCardsToChooseToReplace, "Replace cards");
		if (card.DefaultChoice) {
			stringBuilder.AppendLine("Default choice");
		}
		return stringBuilder.ToString();
	}

	private void addString(StringBuilder builder, int propertyValue, string propertyDescription) {
		addString(builder, propertyValue != 0 ? propertyValue.ToString() : null, propertyDescription);
	}

	private void addString(StringBuilder builder, string str, string propertyDescription) {
		if (str != null) {
			builder.AppendLine($"{propertyDescription}: {str}");
		}
	}
}