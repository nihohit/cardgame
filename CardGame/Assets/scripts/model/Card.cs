using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Card : BaseValueClass {
  public string Name { get; }
	public Card Source { get; }
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
	public CarType RequiresCar { get; }
	public CarType ModifiedByCar { get; }
	public Dictionary<string, int> CarModifications { get; }

	protected Card(string name,
		Card source,
		int populationCost,
		int fuelChange,
		int materialsChange,
		int populationChange,
		int armyChange,
		TraditionType addTradition,
		bool exhaustible,
		int numberOfCardsToChooseToExhaust,
		int numberOfCardsToChooseToReplace,
		bool defaultChoice,
		CarType carToRemove,
		TrainCar carToAdd,
		bool locationLimited,
		CarType requiresCar,
		CarType modifiedByCar,
		Dictionary<string, int> carModifications) {
		AssertUtils.AreEqual(ModifiedByCar == CarType.None, 
			CarModifications == null);
		Name = name;
		Source = source;
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
		RequiresCar = requiresCar;
		ModifiedByCar = modifiedByCar;
		CarModifications = carModifications;
	}

	public static Card MakeCard(
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
		bool locationLimited = false,
		CarType requiresCar = CarType.None,
		CarType modifiedByCar = CarType.None,
		Dictionary<string, int> carModifications = null) {
		return new Card(name,
			null,
			populationCost,
			fuelChange,
			materialsChange,
			populationChange,
			armyChange,
			addTradition,
			exhaustible,
			numberOfCardsToChooseToExhaust,
			numberOfCardsToChooseToReplace,
			defaultChoice,
			carToRemove,
			carToAdd,
			locationLimited,
			requiresCar,
			modifiedByCar,
			carModifications);
	}

	public Card MakeExhaustibleCopy() {
		return this.CopyWithSetValue("Exhaustible", true);
	}
}
