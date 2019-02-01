﻿// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;

public static class TrainCarsCardCollection {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		Card.MakeCard("test"),
		Card.MakeCard("Manual Labour",
			populationCost: 1,
			materialsChange: 1
		),
		Card.MakeCard("Make Fuel",
			populationCost: 1,
			materialsChange: -1,
			fuelChange: 1
		),
		Card.MakeCard("Arm Militia",
			populationCost: 1,
			materialsChange: -1,
			armyChange: 1
		),
		Card.MakeCard("Conduct Training",
			populationCost: 1,
			materialsChange: -2,
			armyChange: 3
		),
		Card.MakeCard("Refine Materials",
			populationCost: 1,
			fuelChange: -1,
			materialsChange: 3
		),
		Card.MakeCard("Refine Fuel",
			populationCost: 1,
			fuelChange: 3,
			materialsChange: -2
		),
		Card.MakeCard("Reevaluate Options",
			numberOfCardsToChooseToDraw: 3,
			numberOfCardsToChooseToDiscard: 2,
			materialsChange: -1
		),
		Card.MakeCard("Gather Workers",
			numberOfCardsToChooseToDraw: 2,
			cardDrawingFilter: card => !card.LocationLimited,
			materialsChange: -1,
			customDescription: "only train cards"
		),
		Card.MakeCard("Scout Ahead",
			numberOfCardsToChooseToDraw: 2,
			cardDrawingFilter: card => card.LocationLimited,
			fuelChange: -1,
			customDescription: "only location cards"
		),
	}.ToDictionary(card => card.Name, card => card));


	public static IEnumerable<Card> BaseCards(IEnumerable<CarType> initialCars) {
		return initialCars
			.SelectMany(carType => CardsForTrainCar(carType))
			.Concat(cards.objectForDictionary(new Dictionary<string, int>{
				{"Arm Militia", 1 }
			}));	
	}

	public static IEnumerable<Card> CardsForTrainCar(CarType car) {
		switch (car) {
			case CarType.Cannon:
				return cannonCards();
			case CarType.Engine:
				return engineCards();
			case CarType.General:
			case CarType.LivingQuarters:
				return generalCards();
			case CarType.Workhouse:
				return workhouseCards();
			case CarType.Refinery:
				return refineryCards();
			case CarType.Armory:
				return armoryCards();
			case CarType.CommandCenter:
				return commandCenterCards();
			case CarType.Test:
				return testDeck();
			case CarType.None:
				return Enumerable.Empty<Card>();
			default:
				throw new ArgumentOutOfRangeException(nameof(car), car, null);
		}
	}

	private static IEnumerable<Card> armoryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Conduct Training", 1},
		});
	}

	private static IEnumerable<Card> refineryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Refine Fuel", 1},
		});
	}

	private static IEnumerable<Card> workhouseCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Refine Materials", 1},
		});
	}

	private static IEnumerable<Card> generalCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Manual Labour", 1},
		});
	}

	private static IEnumerable<Card> engineCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Make Fuel", 1},
		});	
	}

	private static IEnumerable<Card> cannonCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
		});
	}

	private static IEnumerable<Card> commandCenterCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{ "Reevaluate Options", 1 },
			{ "Scout Ahead", 1 },
			{ "Gather Workers", 1 }
		});
	}

	private static IEnumerable<Card> testDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"test", 1}
		});
	}
}
