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
	public IReadOnlyList<Card> PlayedCards { get; }
	public IReadOnlyList<TrainCar> Cars { get; }

	public static TrainState InitialState(
		int fuel,
		int materials,
		int population,
		int army) {
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
			cars);
	}

	private TrainState(
		int fuel,
		int materials,
		int totalPopulation,
		int availablePopulation,
		int army,
		IReadOnlyList<Card> playedCards,
		IReadOnlyList<TrainCar> cars) {
		AssertUtils.Positive(fuel, "fuel");
		AssertUtils.Positive(materials, "materials");
		AssertUtils.Positive(availablePopulation, "availablePopulation");
		AssertUtils.StrictlyPositive(totalPopulation, "totalPopulation");
		AssertUtils.StrictlyPositive(cars.Count, "Cars count");
		Fuel = fuel;
		Materials = materials;
		TotalPopulation = totalPopulation;
		AvailablePopulation = availablePopulation;
		Army = army;
		PlayedCards = playedCards;
		Cars = cars;
	}

	public TrainState NextTurnState() {
		return new TrainState(
			Fuel,
			Materials,
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards,
			Cars);
	}

	public int fuelConsumption() {
		return Cars.Sum(car => car.FuelConsumption);
	}

	public bool CanDrive() {
		return fuelConsumption() < Fuel;
	}

	public TrainState Drive() {
		return new TrainState(Fuel - fuelConsumption(),
			Materials,
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards,
			Cars);
	}

	public TrainState ChangeFuel(int fuelChange) {
		return new TrainState(
			Fuel + fuelChange,
			Materials,
			TotalPopulation,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars);
	}

	public TrainState ChangeMaterials(int changeMaterials) {
		return new TrainState(Fuel,
			Materials + changeMaterials,
			TotalPopulation,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars);
	}

	public TrainState ChangePopulation(int populationChange) {
		return new TrainState(Fuel,
			Materials,
			TotalPopulation + populationChange,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars); ;
	}

	public TrainState ChangeArmy(int armyChange) {
		return new TrainState(
			Fuel,
			Materials,
			TotalPopulation,
			AvailablePopulation,
			Army + armyChange,
			PlayedCards,
			Cars);
	}

	public bool CanPlayCard(Card card) {
		return (
			Fuel >= -card.FuelChange &&
				Materials >= -card.MaterialsChange &&
					AvailablePopulation >= card.PopulationCost &&
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
		} else if (card.CarToRemove != 0 && card.CarToAdd != null) {
			cars = cars.Select(car => {
				if (!actionNeeded || !car.Equals(card.CarToRemove)) {
					return car;
				}

				actionNeeded = false;
				return card.CarToAdd;
			}).ToList();
		} else if (card.CarToRemove == CarType.None && card.CarToAdd != null) {
			cars = cars.Where(car => {
				if (!actionNeeded || !car.Equals(card.CarToRemove)) {
					return true;
				}

				actionNeeded = false;
				return false;
			}).ToList();
		}

		return new TrainState(
			Math.Max(Fuel + card.FuelChange, 0),
			Math.Max(Materials + card.MaterialsChange, 0),
			Math.Max(TotalPopulation + card.PopulationChange, 0),
			Math.Max(AvailablePopulation - card.PopulationCost, 0),
			Math.Max(Army + card.ArmyChange, 0),
			playedCards,
			cars);
	}
}
