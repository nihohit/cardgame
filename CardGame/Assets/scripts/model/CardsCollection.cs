// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class LocationBasedCards {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		new Card("Build Regular Car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.General),
			exhaustible:true
		),
		new Card("Convert Car to Refinery",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Refinery),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Convert Car to Workhouse",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Workhouse),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Convert Car to Cannon",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Cannon),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Convert Car to Armory",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Armory),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Convert Car to Living Quarters",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.LivingQuarters),
			carToRemove: CarType.General,
			exhaustible:true
		)
	}.ToDictionary(card => card.Name, card => card));
}

public static class CardsCollection {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		new Card("test"),
		new Card("Manual Labour",
			populationCost: 1,
			materialsChange: 1
		),
		new Card("Convert To Fuel",
			populationCost: 1,
			materialsChange: -1,
			fuelChange: 1
		),
		new Card("Arm Militia",
			populationCost: 1,
			materialsChange: -1,
			armyChange: 1
		),
		new Card("Refine Materials",
			populationCost: 1,
			fuelChange: -1,
			materialsChange: 3
		),
		new Card("Hire Mercenaries",
			populationCost: 1,
			fuelChange: -1,
			armyChange: 1
		),
	}.ToDictionary(card => card.Name, card => card));


	public static IEnumerable<Card> Cards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Manual Labour", 2},
			{"Convert To Fuel", 1},
			{"Hire Mercenaries", 1}
		});	
	}

	public static IEnumerable<Card> CardsForTrainCar(CarType car) {
		switch (car) {
			case CarType.Cannon: 
				return villageDeck();
			case CarType.Engine: 
				return villageCenterDeck();
			case CarType.General: 
				return fishingVillageDeck();
			case CarType.Workhouse: 
				return exploreDeck();
			case CarType.Refinery: 
				return mineDeck();
			case CarType.Test:
				return testDeck();
			default:
				return new Card[0];
		}
	}

	private static IEnumerable<Card> villageDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Temple", 1},
			{"Village center", 1},
			{"Town", 1},
			{"Farming", 3},
			{"Arm Militia", 1}
		});
	}

	private static IEnumerable<Card> fishingVillageDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Temple", 1},
			{"Village center", 1},
			{"Port Town", 1},
			{"Fishing", 3},
			{"Arm Militia", 1}
		});
	}

	private static IEnumerable<Card> villageCenterDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Market Day", 2},
			{"Public Discussion", 1},
			{"Buy Slaves", 1},
			{"Ostracize", 1},
		});
	}

	private static IEnumerable<Card> exploreDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Village", 1},
			{"Fishing Village", 1},
			{"Build Mine", 1},
		}).Shuffle().Take(1);
	}

	private static IEnumerable<Card> mineDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Mine", 1}
		});
	}

	private static IEnumerable<Card> testDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"test", 1}
		});
	}
}
