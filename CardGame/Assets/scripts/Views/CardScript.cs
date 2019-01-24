using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UniRx.Triggers;
using UniRx;
using System.Linq;

public class CardScript : MonoBehaviour {
	private bool needsSetup = true;
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
	private SpriteRenderer spriteRenderer;

	private Subject<Unit> modelSetSubject = new Subject<Unit>();
	private CardDisplayModel _model;
	public CardDisplayModel CardModel { get {
			return _model;
		} set {
			if (needsSetup) {
				InitialCardSetup();
			}
			_model = value;
			var card = value.Card;
			nameField.text = card.Name;
			fuelCost.SetValue(card.FuelChange);
			populationCost.SetDoubleValue(-card.PopulationCost, card.PopulationChange);
			materialsCost.SetValue(card.MaterialsChange);
			armyCost.SetValue(card.ArmyChange);
			modelSetSubject.OnNext(Unit.Default);
			setTraits(card);
			spriteRenderer.color = value.Playable ? Color.white : Color.gray;

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

		if (card.RequiresCar != CarType.None) {
			yield return $"Requires {carName(card.RequiresCar)}";
		}

		if (card.ModifiedByCar != CarType.None) {
			yield return $"Improved with {carName(card.ModifiedByCar)}";
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
				return "Housing";
		}

		AssertUtils.UnreachableCode($"unknown type {carType}");
		return "";
	}

	private void InitialCardSetup() {
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
		spriteRenderer = transform.Find("Sprite").GetComponent<SpriteRenderer>();
	}

	public IObservable<Card> ClickObservation() {
		return this.OnMouseDownAsObservable()
			.Select(_ => CardModel.Card)
			.TakeUntil(modelSetSubject);
	}
}
