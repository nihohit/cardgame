// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
		new Card("Build Village",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			exhaustible:true
		),
		new Card("Farming",
			populationCost: 1,
			materialsChange: 2
		),
		new Card("Arm Militia",
			populationCost: 1,
			materialsChange: -1,
			armyChange: 1
		),
		new Card("Market Day",
			populationCost: 1,
			materialsChange: -1,
			fuelChange: 2
		),
		new Card("Public Discussion",
			populationCost: 1,
			fuelChange: -2,
			numberOfCardsToChooseToReplace: 1
		),
		new Card("Ostracize",
			populationCost: 1,
			fuelChange: -2,
			numberOfCardsToChooseToExhaust: 1
		),
		new Card("Buy Slaves",
			populationCost: 2,
			fuelChange: -10
		),
		new Card("Fishing",
			populationCost: 1,
			materialsChange: 1,
			fuelChange: 1
		),
		new Card("Hire Mercenaries",
			populationCost: 1,
			fuelChange: -1,
			armyChange: 1
		),
		new Card("Mine",
			populationCost: 1,
			materialsChange: 3
		)
	}.ToDictionary(card => card.Name, card => card));


	public static IEnumerable<Card> Cards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Manual Labour", 2},
			{"Convert To Fuel", 1},
			{"Explore", 1},
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
