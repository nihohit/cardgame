using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Card : BaseValueClass {
  public string Name { get; }
	public int PopulationCost { get; }
	public int FuelChange { get; }
	public int MaterialsChange { get; }
	public int PopulationChange { get; }
	public int ArmyChange { get; }
	public TraditionType AddTradition { get; }
	public bool Exhaustible { get; }
	public int NumberOfCardsToChooseToExhaust { get; }
	public int NumberOfCardsToChooseToReplace { get; }
	public bool DefaultChoice { get; }
	public TrainCar CarToAdd { get; }
	public CarType CarToRemove { get; }
	public bool LocationLimited { get; }

	public Card(
		string name,
		int populationCost = 0,
		int fuelChange = 0, 
		int materialsChange = 0, 
		int populationChange = 0,
		int armyChange = 0,
		TraditionType addTradition = TraditionType.None,
		bool exhaustible = false, 
		int numberOfCardsToChooseToExhaust = 0,
		int numberOfCardsToChooseToReplace = 0,
		bool defaultChoice = false,
		CarType carToRemove = CarType.None,
		TrainCar carToAdd = null,
		bool locationLimited = false) {
		Name = name;
		PopulationCost = populationCost;
		FuelChange = fuelChange;
		MaterialsChange = materialsChange;
		PopulationChange = populationChange;
		ArmyChange = armyChange;
		AddTradition = addTradition;
		Exhaustible = exhaustible;
		NumberOfCardsToChooseToExhaust = numberOfCardsToChooseToExhaust;
		NumberOfCardsToChooseToReplace = numberOfCardsToChooseToReplace;
		DefaultChoice = defaultChoice;
		CarToAdd = carToAdd;
		CarToRemove = carToRemove;
		LocationLimited = locationLimited;
	}

	public Card MakeExhaustibleCopy() {
		return new Card(
			Name,
			PopulationCost,
			FuelChange,
			MaterialsChange,
			PopulationChange,
			ArmyChange,
			AddTradition,
			true,
			NumberOfCardsToChooseToExhaust,
			NumberOfCardsToChooseToReplace,
			DefaultChoice,
			CarToRemove,
			CarToAdd,
			LocationLimited);
	}
}
