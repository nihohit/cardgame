using System;
using System.Collections.Generic;
using System.Linq;

public static class LocationBasedCards {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		Card.MakeCard("Build Basic Car",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.General),
			exhaustible:true
		),
		Card.MakeCard("Build Engine",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(0, CarType.Engine),
			exhaustible:true
		),
		Card.MakeCard("Upgrade to Refinery",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Refinery),
			carToRemove: CarType.General,
			exhaustible:true
		),
		Card.MakeCard("Upgrade to Workhouse",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Workhouse),
			carToRemove: CarType.General,
			exhaustible:true
		),
		Card.MakeCard("Upgrade to Cannon",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Cannon),
			carToRemove: CarType.General,
			exhaustible:true
		),
		Card.MakeCard("Upgrade to Armory",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Armory),
			carToRemove: CarType.General,
			exhaustible:true
		),
		Card.MakeCard("Upgrade to Housing",
			populationCost: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.LivingQuarters),
			carToRemove: CarType.General,
			exhaustible:true
		),
		Card.MakeCard("Build Refinery",
			populationCost: 1,
			materialsChange: -2,
			carToAdd: new TrainCar(1, CarType.Refinery),
			exhaustible:true
		),
		Card.MakeCard("Build Workhouse",
			populationCost: 1,
			materialsChange: -2,
			carToAdd: new TrainCar(1, CarType.Workhouse),
			exhaustible:true
		),
		Card.MakeCard("Build Cannon",
			populationCost: 1,
			materialsChange: -2,
			carToAdd: new TrainCar(1, CarType.Cannon),
			exhaustible:true
		),
		Card.MakeCard("Build Armory",
			populationCost: 1,
			materialsChange: -2,
			carToAdd: new TrainCar(1, CarType.Armory),
			exhaustible:true
		),
		Card.MakeCard("Build Housing",
			populationCost: 1,
			materialsChange: -2,
			carToAdd: new TrainCar(1, CarType.LivingQuarters),
			exhaustible:true
		),
		Card.MakeCard("Cut Trees",
			populationCost: 1,
			materialsChange: 2
		),
		Card.MakeCard("Make Charcoal",
			populationCost: 1,
			fuelChange: 2,
			modifiedByCar: CarType.Refinery,
			carModifications: new Dictionary<string, int> {
				{"FuelChange",1}
			}
		),
		Card.MakeCard("Easy Foraging",
			materialsChange: 1
		),
		Card.MakeCard("Abandoned Weapons",
			armyChange: 1
		),
		Card.MakeCard("Mine Materials", 
			populationCost:1,
			materialsChange:2,
			modifiedByCar: CarType.Workhouse,
			carModifications: new Dictionary<string, int> {
				{"MaterialsChange",1}
			}
		), 
		Card.MakeCard("Mine with Explosives",
			populationCost:1,
			materialsChange:4,
			fuelChange:-1,
			modifiedByCar: CarType.Cannon,
			carModifications: new Dictionary<string, int> {
				{"MaterialsChange",2}
			}
		),  
		Card.MakeCard("Collect Materials", 
			populationCost:1,
			materialsChange:3
		),  
		Card.MakeCard("Drain Fuel Storage", 
			populationCost:1,
			fuelChange:3
		),
		Card.MakeCard("Drain Fuel Tank",
			populationCost:1,
			fuelChange:2
		),
		Card.MakeCard("Work for Materials", 
			populationCost:1,
			materialsChange:2,
			modifiedByCar: CarType.Workhouse,
			carModifications: new Dictionary<string, int> {
				{"MaterialsChange",1}
			}
		),
		Card.MakeCard("Work for Fuel", 
			populationCost:1,
			fuelChange:1,
			modifiedByCar: CarType.Workhouse,
			carModifications: new Dictionary<string, int> {
				{"FuelChange",1}
			}
		),
		Card.MakeCard("Buy weapons",
			materialsChange:-1,
			armyChange: 1
		),
		Card.MakeCard("Hire Mercenaries",
			fuelChange:-2,
			armyChange: 3
		),
		Card.MakeCard("Trade for Materials", 
			materialsChange:2,
			fuelChange:-1
		),
		Card.MakeCard("Trade for Fuel",
			fuelChange:1,
			materialsChange:-1
		),
		Card.MakeCard("Make Weapons",
			fuelChange:-1,
			materialsChange:-1,
			armyChange:2,
			modifiedByCar: CarType.Armory,
			carModifications: new Dictionary<string, int> {
				{"fuelChange",1}
			}
		),
		Card.MakeCard("Small scale Hunt", 
			populationCost:1,
			materialsChange:2
		),
		Card.MakeCard("Large scale Hunt", 
			populationCost:1,
			armyChange:-1,
			materialsChange:5
		),
		Card.MakeCard("Fuel from Feces", 
			populationCost:1,
			fuelChange:1,
			modifiedByCar: CarType.Refinery,
			carModifications: new Dictionary<string, int> {
				{"FuelChange",1}
			}
		),
		Card.MakeCard("Find Survivors",
			populationCost:1,
			populationChange:1,
			materialsChange:-1
		),
		Card.MakeCard("Recruit the Locals",
			populationChange:1,
			materialsChange:-2,
			modifiedByCar: CarType.LivingQuarters,
			carModifications: new Dictionary<string, int> {
				{"MaterialsChange",1}
			}
		),
	}
		.Select(card => card.MakeExhaustibleCopy())
		.Select(card => card.MakeLocationLimitedCopy())
		.ToDictionary(card => card.Name, card => card));

	public static IEnumerable<Card> CardsForContent(LocationContent content) {
		return internalCardsForContent(content);
	}

	private static IEnumerable<Card> internalCardsForContent(LocationContent content) { 
		switch (content) {
			case LocationContent.Armory:
				return armoryCards();
			case LocationContent.TrainWreck:
				return trainWreckCards();
			case LocationContent.Howitizer:
				return howitizerCards();
			case LocationContent.FuelRefinery:
				return fuelRefineryCards();
			case LocationContent.Workhouse:
				return workhouseCards();
			case LocationContent.OldHouses:
				return oldHousesCards();
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
			case LocationContent.Storehouse:
				return storehouseCards();
			case LocationContent.Mine:
				return mineCards();
			case LocationContent.ArmyBase:
				return armyBaseCards();
			default:
				throw new ArgumentOutOfRangeException(nameof(content), content, null);
		}
	}

	private static IEnumerable<Card> mineCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Mine Materials", 2},
			{"Mine with Explosives", 2},
			{"Abandoned Weapons", 1},
			{"Find Survivors", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> storehouseCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Collect Materials", 2},
			{"Abandoned Weapons", 1}
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> fuelStorageCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Drain Fuel Storage", 2},
			{"Abandoned Weapons", 1}
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> livingPopulationCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Work for Materials", 1},
			{"Work for Fuel", 1},
			{"Trade for Materials", 1},
			{"Trade for Fuel", 1},
			{"Recruit the Locals", 1},
			{"Hire Mercenaries", 1}
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> wildAnimalsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Small scale Hunt", 4},
			{"Large scale Hunt", 2},
			{"Fuel from Feces", 2},
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> woodsCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Cut Trees", 2},
			{"Make Charcoal", 2},
			{"Easy Foraging", 1},
			{"Find Survivors", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> armoryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Armory", 1},
			{"Upgrade to Armory", 1},
			{"Make Weapons", 1},
		});
	}

	private static IEnumerable<Card> trainWreckCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Basic Car", 2},
			{"Drain Fuel Tank", 1},
			{"Collect Materials", 1},
			{"Abandoned Weapons", 1},
			{"Find Survivors", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> howitizerCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Cannon", 1},
			{"Upgrade to Cannon", 1},
			{"Abandoned Weapons", 1},
			{"Drain Fuel Tank", 1}
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> fuelRefineryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Refinery", 1},
			{"Upgrade to Refinery", 1},
			{"Drain Fuel Tank", 1}
		});
	}

	private static IEnumerable<Card> workhouseCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Workhouse", 1},
			{"Upgrade to Workhouse", 1},
			{"Make Weapons", 1},
			{"Work for Materials", 1},
			{"Work for Fuel", 1},
			{"Find Survivors", 1},
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> armyBaseCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Command Center", 1},
			{"Build Cannon", 1},
			{"Make Weapons", 1},
			{"Abandoned Weapons", 1},
			{"Drain Fuel Tank", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> oldHousesCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Housing", 1},
			{"Upgrade to Housing", 1},
			{"Abandoned Weapons", 1},
			{"Collect Materials", 1 },
			{"Find Survivors", 1}
		}).Shuffle().Take(2);
	}
}