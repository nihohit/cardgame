﻿// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

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
	public Location CurrentLocation { get; }

	public static TrainState InitialState(
		int fuel,
		int materials,
		int population,
		int army,
		Location initialLocation) {
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
			initialLocation);
	}

	private TrainState(
		int fuel,
		int materials,
		int totalPopulation,
		int availablePopulation,
		int army,
		IReadOnlyList<Card> playedCards,
		IReadOnlyList<TrainCar> cars,
		Location currentLocation) {
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
		CurrentLocation = currentLocation;
	}

	public TrainState NextTurnState() {
		return new TrainState(
			Fuel,
			Materials,
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards,
			Cars,
			CurrentLocation);
	}

	public int fuelConsumption() {
		return Cars.Sum(car => car.FuelConsumption);
	}

	public bool CanDrive() {
		return fuelConsumption() < Fuel;
	}

	public TrainState Drive(Location toLocation) {
		return new TrainState(Fuel - fuelConsumption(),
			Materials,
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards,
			Cars,
			toLocation);
	}

	public TrainState ChangeFuel(int fuelChange) {
		return new TrainState(
			Fuel + fuelChange,
			Materials,
			TotalPopulation,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars,
			CurrentLocation);
	}

	public TrainState ChangeMaterials(int changeMaterials) {
		return new TrainState(Fuel,
			Materials + changeMaterials,
			TotalPopulation,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars, 
			CurrentLocation);
	}

	public TrainState ChangePopulation(int populationChange) {
		return new TrainState(Fuel,
			Materials,
			TotalPopulation + populationChange,
			AvailablePopulation,
			Army,
			PlayedCards,
			Cars,
			CurrentLocation); ;
	}
	
	public TrainState ChangeAvailablePopulation(int populationChange) {
		return new TrainState(Fuel,
			Materials,
			TotalPopulation,
			AvailablePopulation + populationChange,
			Army,
			PlayedCards,
			Cars,
			CurrentLocation); ;
	}

	public TrainState ChangeArmy(int armyChange) {
		return new TrainState(
			Fuel,
			Materials,
			TotalPopulation,
			AvailablePopulation,
			Army + armyChange,
			PlayedCards,
			Cars,
			CurrentLocation);
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

		return new TrainState(
			Math.Max(Fuel + card.FuelChange, 0),
			Math.Max(Materials + card.MaterialsChange, 0),
			Math.Max(TotalPopulation + card.PopulationChange, 0),
			Math.Max(AvailablePopulation - card.PopulationCost, 0),
			Math.Max(Army + card.ArmyChange, 0),
			playedCards,
			cars,
			CurrentLocation);
	}
}
