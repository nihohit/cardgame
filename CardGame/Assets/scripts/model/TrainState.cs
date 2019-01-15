// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TrainCar : BaseValueClass {
	public int FuelConsumption { get; }
	public CarType Type { get; }

	public TrainCar(
		int fuelConsumption,
		CarType type) {
		FuelConsumption = fuelConsumption;
		Type = type;
	}
}

public class TrainState : BaseValueClass {
	public int Fuel { get; }
	public int Materials { get; }
	public int Army { get; }
	public int TotalPopulation { get; }
	public int AvailablePopulation { get; }
	public int LivingSpace { get; }
	public int FuelConsumption { get; }
	public IReadOnlyList<Card> PlayedCards { get; }
	public IReadOnlyList<TrainCar> Cars { get; }
	public Location CurrentLocation { get; }
	public Location NextLocation { get; }

	public static TrainState InitialState(
		int fuel,
		int materials,
		int population,
		int army,
		Location initialLocation,
		Location nextLocation) {
		var cars = new List<TrainCar> {
			new TrainCar(0, CarType.Engine),
			new TrainCar(1, CarType.General),
			new TrainCar(1, CarType.General)
		};
		return new TrainState(
			fuel,
			materials,
			population,
			population,
			army,
			new List<Card>(),
			cars,
			initialLocation,
			nextLocation);
	}

	private TrainState(
		int fuel,
		int materials,
		int totalPopulation,
		int availablePopulation,
		int army,
		IReadOnlyList<Card> playedCards,
		IReadOnlyList<TrainCar> cars,
		Location currentLocation,
		Location nextLocation) {
		AssertUtils.Positive(fuel, "fuel");
		AssertUtils.Positive(materials, "materials");
		AssertUtils.Positive(availablePopulation, "availablePopulation");
		AssertUtils.StrictlyPositive(cars.Count, "Cars count");
		AssertUtils.EqualOrGreater(totalPopulation, availablePopulation);
		Fuel = fuel;
		Materials = materials;
		TotalPopulation = totalPopulation;
		AvailablePopulation = availablePopulation;
		Army = army;
		PlayedCards = playedCards;
		Cars = cars;
		CurrentLocation = currentLocation;
		NextLocation = nextLocation;
		FuelConsumption = Cars.Sum(car => car.FuelConsumption);
		LivingSpace = livingSpaceForCars(cars);

		AssertUtils.EqualOrGreater(LivingSpace, TotalPopulation);
	}

	private int livingSpaceForCars(IEnumerable<TrainCar> cars) {
		return cars.Sum(car => spacePerCar(car.Type));
	}

	private int spacePerCar(CarType car) {
		switch (car) {
			case CarType.LivingQuarters:
				return 3;
			case CarType.General:
				return 2;
			case CarType.Engine:
				return 0;
			case CarType.Workhouse:
			case CarType.Armory:
			case CarType.Refinery:
			case CarType.Cannon:
				return 1;
			default:
				AssertUtils.UnreachableCode("Illegal type: " + car);
				return Int32.MinValue;
		}
	}

	private const int kUnsetValue = -1;

	private TrainState newState(
		int fuel = kUnsetValue,
		int materials = kUnsetValue,
		int totalPopulation = kUnsetValue,
		int availablePopulation = kUnsetValue,
		int army = kUnsetValue,
		IReadOnlyList<Card> playedCards = null,
		IReadOnlyList<TrainCar> cars = null,
		Location currentLocation = null,
		Location nextLocation = null) {
		return new TrainState(
			fuel == kUnsetValue ? Fuel : fuel,
			materials == kUnsetValue ? Materials : materials,
			totalPopulation == kUnsetValue ? TotalPopulation : totalPopulation,
			availablePopulation == kUnsetValue ? AvailablePopulation : availablePopulation,
			army == kUnsetValue ? Army : army,
			playedCards ?? PlayedCards,
			cars ?? Cars,
			currentLocation ?? CurrentLocation,
			nextLocation ?? NextLocation);
	}

	public TrainState NextTurnState() {
		var remainingMaterials = Materials - MaterialsConsumption();
		var remainingPopulation = remainingMaterials < 0 ? TotalPopulation - 1 : TotalPopulation;
		return newState(
			materials: Math.Max(remainingMaterials, 0),
			totalPopulation: remainingPopulation,
			availablePopulation: remainingPopulation
		);
	}

	public int MaterialsConsumption() {
		return TotalPopulation;
	}

	public bool CanDrive() {
		return FuelConsumption <= Fuel;
	}

	public TrainState Drive(Location nextLocation) {
		return newState(
			fuel: Fuel - FuelConsumption,
			currentLocation: NextLocation,
			nextLocation: nextLocation);
	}

	public TrainState ChangeFuel(int fuelChange) {
		return newState(fuel: Fuel + fuelChange);
	}

	public TrainState ChangeMaterials(int changeMaterials) {
		return newState(materials: Materials + changeMaterials);
	}

	public TrainState ChangePopulation(int populationChange) {
		return newState(totalPopulation: TotalPopulation + populationChange); ;
	}
	
	public TrainState ChangeAvailablePopulation(int populationChange) {
		return newState(availablePopulation: AvailablePopulation + populationChange);
	}

	public TrainState ChangeArmy(int armyChange) {
		return newState(army: Army + armyChange);
	}

	public bool CanPlayCard(Card card) {
		return (
			Fuel >= -card.FuelChange &&
			Materials >= -card.MaterialsChange &&
			AvailablePopulation >= card.PopulationCost &&
			LivingSpace >= card.PopulationChange + TotalPopulation &&
			Army >= -card.ArmyChange &&
			(card.CarToRemove == CarType.None || Cars.Any(car => car.Type.Equals(card.CarToRemove)))) ||
			card.DefaultChoice;
	}

	public TrainState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		var playedCards = new List<Card>(PlayedCards) {
			card
		};

		bool actionNeeded = true;
		var cars = Cars;
		if (card.CarToRemove == CarType.None && card.CarToAdd != null) {
			cars = cars.Append(card.CarToAdd).ToList();
		} else if (card.CarToRemove != CarType.None && card.CarToAdd != null) {
			cars = cars.Select(car => {
				if (actionNeeded && car.Type == card.CarToRemove) {
					actionNeeded = false;
					return card.CarToAdd;
				}

				return car;
			}).ToList();
		} else if (card.CarToRemove != CarType.None && card.CarToAdd == null) {
			cars = cars.RemoveFirstWhere(car => car.Type == card.CarToRemove).ToList();
		}

		var newTotalPopulation = Math.Max(TotalPopulation + card.PopulationChange, 0);
		newTotalPopulation = Math.Min(newTotalPopulation, livingSpaceForCars(cars));
		var newAvailablePopulation = Math.Max(AvailablePopulation - card.PopulationCost, 0);
		newAvailablePopulation = Math.Min(newAvailablePopulation, newTotalPopulation);

		return newState(
			fuel: Math.Max(Fuel + card.FuelChange, 0),
			materials: Math.Max(Materials + card.MaterialsChange, 0),
			totalPopulation: newTotalPopulation,
			availablePopulation: newAvailablePopulation,
			army: Math.Max(Army + card.ArmyChange, 0),
			playedCards: playedCards,
			cars: cars);
	}
}
