using System;
using System.Collections.Generic;
using System.Linq;

public static class LocationBasedCards {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		new Card("Build Basic Car",
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
		new Card("Build Refinery Car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Refinery),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Build Workhouse car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Workhouse),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Build Cannon Car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Cannon),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Build Armory Car",
			populationCost: 1,
			populationChange: 1,
			materialsChange: -1,
			carToAdd: new TrainCar(1, CarType.Armory),
			carToRemove: CarType.General,
			exhaustible:true
		),
		new Card("Build Housing Car",
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
			fuelChange: 2
		),
		new Card("Easy Foraging",
			materialsChange: 1
		),
		new Card("Abandoned Weapons",
			armyChange: 1
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
		new Card("Drain Fuel Tank",
			populationCost:1,
			fuelChange:2
		),
		new Card("Work for Materials", 
			populationCost:1,
			materialsChange:2
		),
		new Card("Work for Fuel", 
			populationCost:1,
			fuelChange:1
		),
		new Card("Buy weapons",
			materialsChange:-1,
			armyChange: 1
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
		new Card("Make Weapons",
			fuelChange:-1,
			materialsChange:-1,
			armyChange:2
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
			default:
				throw new ArgumentOutOfRangeException(nameof(content), content, null);
		}
	}

	private static IEnumerable<Card> mineCards() {
		return cards.objectForDictionary(new Dictionary<string, int> {
			{"Mine Materials", 2},
			{"Mine with Explosives", 2},
			{"Abandoned Weapons", 1}
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
			{"Easy Foraging", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> armoryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Armory Car", 1},
			{"Make Weapons", 1},
		});
	}

	private static IEnumerable<Card> trainWreckCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Engine", 1},
			{"Build Basic Car", 2},
			{"Drain Fuel Tank", 1},
			{"Collect Materials", 1},
			{"Abandoned Weapons", 1}
		}).Shuffle().Take(3);
	}

	private static IEnumerable<Card> howitizerCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Cannon Car", 1},
			{"Abandoned Weapons", 1},
			{"Drain Fuel Tank", 1}
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> fuelRefineryCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Refinery Car", 1},
			{"Drain Fuel Tank", 1}
		});
	}

	private static IEnumerable<Card> workhouseCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Workhouse Car", 1},
			{"Make Weapons", 1},
			{"Work for Materials", 1},
			{"Work for Fuel", 1},
		}).Shuffle().Take(2);
	}

	private static IEnumerable<Card> oldHousesCards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Housing Car", 1},
			{"Abandoned Weapons", 1},
			{"Collect Materials", 1 }
		}).Shuffle().Take(2);
	}
}