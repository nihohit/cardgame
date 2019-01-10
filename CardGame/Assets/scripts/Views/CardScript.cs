using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UniRx.Triggers;
using UniRx;
using System.Text;
using System.Linq;

public class CardScript : MonoBehaviour {
  private TextMeshPro nameField;
	private TextMeshPro traitField;
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
			nameField.text = value.Name;
			fuelCost.SetValue(value.FuelChange);
			populationCost.SetDoubleValue(-value.PopulationCost, value.PopulationChange);
			materialsCost.SetValue(value.MaterialsChange);
			armyCost.SetValue(value.ArmyChange);
			modelSetSubject.OnNext(Unit.Default);
			setTraits(value);

			fuelGain.SetValue(0);
			materialsGain.SetValue(0);
			armyGain.SetValue(0);
			populationGain.SetValue(0);
		}
	}

	private void setTraits(Card card) {
		traitField.text = getTraits(card).ToJoinedString("\n");
	}

	private IEnumerable<string> getTraits(Card card) {
		if (card.AddTradition != TraditionType.None) {
			yield return $"Add tradition: {card.AddTradition}";
		}

		if (card.NumberOfCardsToChooseToExhaust > 0) {
			yield return $"Remove {card.NumberOfCardsToChooseToExhaust} cards";
		}

		if (card.NumberOfCardsToChooseToReplace > 0) {
			yield return $"Replace {card.NumberOfCardsToChooseToReplace} cards";
		}

		if (card.CarToAdd != null && card.CarToAdd.Type != CarType.None) {
			if (card.CarToRemove != CarType.None) {
				yield return $"Replace {carName(card.CarToRemove)} with {carName(card.CarToAdd.Type)}";
			} else {
				yield return $"Add {carName(card.CarToAdd.Type)}";
			}
		} else if (card.CarToRemove != CarType.None) {
			yield return $"Remove {carName(card.CarToRemove)}";
		}

		if (card.DefaultChoice) {
			yield return "Default choice";
		}

		if (card.Exhaustible) {
			yield return "Single use";
		}
	}

	private string carName(CarType carType) {
		switch (carType) {
			case CarType.Engine:
				return "Engine";
			case CarType.General:
				return "Basic";
			case CarType.Workhouse:
				return "Workhouse";
			case CarType.Armory:
				return "Armory";
			case CarType.Refinery:
				return "Refinery";
			case CarType.Cannon:
				return "Cannon";
			case CarType.LivingQuarters:
				return "LivingQuarters";
		}

		AssertUtils.UnreachableCode($"unknown type {carType}");
		return "";
	}

	private void Awake() {
		nameField = transform.Find("Name").GetComponent<TextMeshPro>();
		traitField = transform.Find("Traits").GetComponent<TextMeshPro>();
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
}
