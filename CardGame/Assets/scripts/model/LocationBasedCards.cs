using System;
using System.Collections.Generic;
using System.Linq;

public static class LocationBasedCards {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		new Card("Build Regular Car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.General),
			exhaustible:true
		),
		new Card("Build Engine",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(0, CarType.Engine),
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
		),
		new Card("Cut Trees",
			populationCost: 1,
			materialsChange: 2
		),
		new Card("Make Charcoal",
			populationCost: 1,
			fuelChange: 1
		),
		new Card("Easy Foraging",
			materialsChange: 1
		), 
		new Card("Mine Materials", 
			populationCost:1,
			materialsChange:2
		), 
		new Card("Mine with Explosives",
			populationCost:1,
			materialsChange:4,
			fuelChange:-1
		),  
		new Card("Collect Materials", 
			populationCost:1,
			materialsChange:3
		),  
		new Card("Drain Fuel Storage", 
			populationCost:1,
			fuelChange:3
		),
		new Card("Work for Materials", 
			populationCost:1,
			materialsChange:2
		),
		new Card("Work for Fuel", 
			populationCost:1,
			fuelChange:1
		),
		new Card("Hire Mercenaries",
			fuelChange:-2,
			armyChange: 3
		),
		new Card("Trade for Materials", 
			materialsChange:2,
			fuelChange:-1
		),
		new Card("Trade for Fuel",
			fuelChange:1,
			materialsChange:-1
		),
		new Card("Small scale Hunt", 
			populationCost:1,
			materialsChange:2
		),
		new Card("Large scale Hunt", 
			populationCost:1,
			armyChange:-1,
			materialsChange:5
		),
		new Card("Fuel from Feces", 
			populationCost:1,
			fuelChange:1
		),
	}.ToDictionary(card => card.Name, card => card));

	public static IEnumerable<Card> CardsForContent(LocationContent content) {
		return internalCardsForContent(content)
			.Select(card => card.MakeExhaustibleCopy());
	}

	private static IEnumerable<Card> internalCardsForContent(LocationContent content) { 
		switch (content) {
			case LocationContent.ArmoryCarComponents:
				return armoryComponentsCards();
			case LocationContent.EngineCarComponents:
				return engineComponentsCards();
			case LocationContent.CannonCarComponents:
				return cannonComponentsCards();
			case LocationContent.GeneralCarComponents:
				return generalComponentsCards();
			case LocationContent.RefineryCarComponents:
				return refineryComponentsCards();
			case LocationContent.WorkhouseCarComponents:
				return workhouseComponentsCards();
			case LocationContent.LivingQuartersCarComponents:
				return livingQuartersComponentsCards();
			case LocationContent.Test:
				return new Card[0];
			case LocationContent.Woods:
				return woodsCards();
			case LocationContent.WildAnimals:
				return wildAnimalsCards();
			case LocationContent.LivingPeople:
				return livingPopulationCards();
			case LocationContent.FuelStorage:
				return fuelStorageCards();
			case LocationContent.SpareMaterials:
				return spareMaterialsCards();
			case LocationContent.MinableMaterials:
				return minableMineralsCards();
			default:
				throw new ArgumentOutOfRangeException(nameof(content), content, null);
		}
	}

	private static IEnumerable<Card> minableMineralsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Mine Materials", 2},
			{"Mine with Explosives", 2}
		});
	}

	private static IEnumerable<Card> spareMaterialsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Collect Materials", 1}
		});
	}

	private static IEnumerable<Card> fuelStorageCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Drain Fuel Storage", 1}
		});
	}

	private static IEnumerable<Card> livingPopulationCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Work for Materials", 1},
			{"Work for Fuel", 1},
			{"Trade for Materials", 1},
			{"Trade for Fuel", 1},
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> wildAnimalsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Small scale Hunt", 4},
			{"Large scale Hunt", 2},
			{"Fuel from Feces", 1},
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> woodsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Cut Trees", 2},
			{"Make Charcoal", 2},
			{"Easy Foraging", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> armoryComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Convert Car to Armory", 1},
		});
	}

	private static IEnumerable<Card> engineComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Engine", 1},
		});
	}

	private static IEnumerable<Card> cannonComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Convert Car to Cannon", 1},
		});
	}

	private static IEnumerable<Card> generalComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Regular Car", 1},
		});
	}

	private static IEnumerable<Card> refineryComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Convert Car to Refinery", 1},
		});
	}

	private static IEnumerable<Card> workhouseComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Convert Car to Workhouse", 1},
		});
	}

	private static IEnumerable<Card> livingQuartersComponentsCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Convert Car to Living Quarters", 1},
		});
	}
}