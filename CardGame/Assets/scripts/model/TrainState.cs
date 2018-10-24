// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum CarType { Engine, General, Workhouse, Cannon }

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
	public int Industry { get; }
	public int Army { get; }
	public int TotalPopulation { get; }
	public int AvailablePopulation { get; }
	public IReadOnlyList<Card> PlayedCards { get; }
	public IReadOnlyList<TrainCar> Cars { get; }

	public static TrainState InitialState(
		int fuel, 
		int industry,
		int population, 
		int army) {
		var cars = new List<TrainCar> {
			new TrainCar(0, CarType.Engine),
			new TrainCar(1, CarType.General)
		};
		return new TrainState(
			fuel, 
			industry, 
			population,
			population, 
			army, 
			new List<Card>(),
			cars);
	}

	private TrainState(
		int fuel, 
	    int industry, 
	    int totalPopulation,
	    int availablePopulation,
	    int army, 
		IReadOnlyList<Card> playedCards,
		IReadOnlyList<TrainCar> cars) {
		AssertUtils.Positive(fuel, "fuel");
		AssertUtils.Positive(industry, "industry");
		AssertUtils.Positive(availablePopulation, "availablePopulation");
		AssertUtils.StrictlyPositive(totalPopulation, "totalPopulation");
		AssertUtils.StrictlyPositive(cars.Count, "Cars count");
		Fuel = fuel;
		Industry = industry;
		TotalPopulation = totalPopulation;
		AvailablePopulation = availablePopulation;
		Army = army;
		PlayedCards = playedCards;
		Cars = cars;
	}

	public TrainState NextTurnState() {
		return new TrainState(
			Fuel, 
			Industry, 
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
		return new TrainState(
			Fuel - fuelConsumption(),
			Industry,
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards,
			Cars);
	}

	public TrainState ChangeFuel(int fuelChange) {
		return new TrainState(
			Fuel + fuelChange, 
            Industry, 
            TotalPopulation, 
            AvailablePopulation, 
            Army, 
            PlayedCards,
            Cars);
	}

	public TrainState ChangeIndustry(int changeIndustry) {
		return new TrainState(
			Fuel, 
            Industry + changeIndustry, 
            TotalPopulation, 
            AvailablePopulation,
            Army, 
            PlayedCards, 
            Cars);
	}

	public TrainState ChangePopulation(int populationChange) {
		return new TrainState(
			Fuel, 
            Industry, 
            TotalPopulation + populationChange, 
            AvailablePopulation, 
            Army,
            PlayedCards,
            Cars); ;
	}

	public TrainState ChangeArmy(int armyChange) {
		return new TrainState(
			Fuel, 
            Industry,
            TotalPopulation, 
            AvailablePopulation,
            Army + armyChange, 
            PlayedCards,
            Cars);
	}

	public bool CanPlayCard(Card card) {
		return (
			Fuel >= -card.FuelChange &&
		    Industry >= -card.IndustryChange &&
	        AvailablePopulation >= card.PopulationCost &&
			Army >= -card.ArmyChange &&
			(card.CarToRemove == null || Cars.Any(car => car.Equals(card.CarToRemove)))) ||
			card.DefaultChoice;
	}

	public TrainState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		var playedCards = new List<Card>(PlayedCards) {
			card
		};

		bool actionNeeded = true;
		var cars = Cars;
		if (card.CarToRemove == null && card.CarToAdd != null) {
			cars = cars.Append(card.CarToAdd).ToList();
		} else if (card.CarToRemove != null && card.CarToAdd != null) {
			cars = cars.Select(car => {
				if (!actionNeeded || !car.Equals(card.CarToRemove)) {
					return car;
				}

				actionNeeded = false;
				return card.CarToAdd;
			}).ToList();
		} else if (card.CarToRemove == null && card.CarToAdd != null) {
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
			Math.Max(Industry + card.IndustryChange, 0),
			Math.Max(TotalPopulation + card.PopulationChange, 0),
			Math.Max(AvailablePopulation - card.PopulationCost, 0),
			Math.Max(Army + card.ArmyChange, 0),
			playedCards,
			cars);
	}
}
