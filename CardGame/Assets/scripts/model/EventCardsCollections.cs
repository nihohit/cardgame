// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventCardsCollections {
	private static BaseCollection<EventCard> events = new BaseCollection<EventCard>(new EventCard[] {
		new EventCard {
			Name = "Raiders",
			Description = "Small buggies appear over carefully conceled positions, and on them you see several well armed men. They shout over a loudspeaker \"Your fuel or your life\".",
			Options = new Card[] {
				Card.MakeCard("Pay them off",
					fuelChange: -1
				),
				Card.MakeCard("Fight them",
					armyChange: -2
				),
				Card.MakeCard("They attack",
					populationChange: -1
				),
				Card.MakeCard("Blow Them Away",
					requiresCar:CarType.Cannon,
					materialsChange: -1
				)
			}
		},
		new EventCard {
			Name = "Wild Animals",
			Description = "During the latest stop several wolves have been seen around the train. your scouts are afraid that they will be attacked.",
			Options = new Card[] {
				Card.MakeCard("Build traps",
					materialsChange: -2
				),
				Card.MakeCard("Fight them",
					armyChange: -1
				),
				Card.MakeCard("They attack",
					populationChange: -1
				),
				Card.MakeCard("Blow Them Away",
					requiresCar:CarType.Cannon,
					materialsChange: -1
				)
			}
		},
		new EventCard {
			Name = "Minor Plague",
			Description = "Some disease is making the rounds through the train, and will kill scores if not properly contained.",
			Options = new Card[] {
				Card.MakeCard("Buy medicine",
					fuelChange: -2
				),
				Card.MakeCard("Quarantine the sick",
					armyChange: -1,
					populationChange: -1
				),
				Card.MakeCard("Let the sick die",
					populationChange: -2,
					defaultChoice: true
				),
				Card.MakeCard("Build impromptu infirmary",
					requiresCar:CarType.LivingQuarters,
					materialsChange:-2
				)
			}
		},
		new EventCard {
			Name = "Famine",
			Options = new Card[] {
				Card.MakeCard("Buy food",
					fuelChange: -2
				),
				Card.MakeCard("Raid Neighbours",
					armyChange: -2,
					modifiedByCar:CarType.Cannon,
					carModifications:new Dictionary<string, int>{
						{ "ArmyChange", 1 }
					}
				),
				Card.MakeCard("Lose people and productivity",
					populationChange: -1,
					populationCost: -1,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "plague",
			Options = new Card[] {
				Card.MakeCard("Buy medicine",
					fuelChange: -4
				),
				Card.MakeCard("Quarantine the sick",
					armyChange: -2,
					populationChange: -1
				),
				Card.MakeCard("Let the sick die",
					populationChange: -3,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Natural Disaster",
			Options = new Card[] {
				Card.MakeCard("Rebuild",
					materialsChange: -5
				),
				Card.MakeCard("Build temporary shelters",
					materialsChange: -2,
					populationChange: -1
				),
				Card.MakeCard("Let the population handle it",
					populationChange: -2,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Animal Attacks",
			Options = new Card[] {
				Card.MakeCard("Hire Mercenaries",
					fuelChange: -6
				),
				Card.MakeCard("Defend the citizens",
					armyChange: -3
				),
				Card.MakeCard("They attack",
					populationCost: 2,
					armyChange: -2
				)
			}
		},
		new EventCard {
			Name = "Trading Caravan",
			Description = "You meet an armed trading caravan. When they hear of your journey, they offer a trade at their best rates.",
			Options = new Card[] {
				Card.MakeCard("Pillage Them",
					armyChange: -3,
					materialsChange: 3,
					fuelChange: 3,
					modifiedByCar: CarType.Cannon,
					carModifications: new Dictionary<string, int> {
						{"ArmyChange", 2}
					}
				),
				Card.MakeCard("Trade Materials",
					materialsChange: -2,
					fuelChange: 2
				),
				Card.MakeCard("Trade Fuel",
					materialsChange: 2,
					fuelChange: -2
				),
				Card.MakeCard("Leave")
			}
		},
		new EventCard {
			Name = "Convoy under Attack",
			Description = "You see a lightly-armed trading caravan attacked by raiders.",
			Options = new Card[] {
				Card.MakeCard("Defend Them",
					armyChange: -2,
					materialsChange: 3,
					modifiedByCar: CarType.Cannon,
					carModifications: new Dictionary<string, int> {
						{"ArmyChange", 1}
					}
				),
				Card.MakeCard("Accept Refugees",
					populationChange: 1,
					armyChange: -1
				),
				Card.MakeCard("Ignore")
			}
		},
		new EventCard {
			Name = "Obstacle on Tracks",
			Description = "An avalanche of trees and rocks is blocking the tracks. You cannot continue without disposing of them in some way.",
			Options = new Card[] {
				Card.MakeCard("Ram it through",
					fuelChange: -2
				),
				Card.MakeCard("Stop and clear it",
					populationCost: 1,
					modifiedByCar: CarType.Workhouse,
					carModifications: new Dictionary<string, int> {
						{"MaterialsChange", 1}
					}
				),
				Card.MakeCard("Blow it Away",
					requiresCar:CarType.Cannon,
					materialsChange: -1
				)
			}
		},
		new EventCard {
			Name = "Mutiny!",
			Description = "Some people on the train have had it with your leadership, and are up at arms! How will you restore control of the train?",
			Options = new Card[] {
				Card.MakeCard("Stop it by Force",
					armyChange: -1
				),
				Card.MakeCard("Bribe the population",
					materialsChange: -2,
					modifiedByCar: CarType.LivingQuarters,
					carModifications: new Dictionary<string, int> {
						{"MaterialsChange", 1}
					}
				),
				Card.MakeCard("Organize a counter-militia",
					materialsChange: -3,
					armyChange: 1,
					modifiedByCar: CarType.Armory,
					carModifications: new Dictionary<string, int> {
						{"MaterialsChange", 1}
					}
				),
				Card.MakeCard("Enforce Martial Law",
					populationCost: -2,
					armyChange: 1
				),
				Card.MakeCard("Civil War",
					populationChange: -2,
					defaultChoice: true
				)
			}
		},
	}.ToDictionary(keySelector: card => card.Name));

	public static EventCard EventCardForState(TrainState state) {
		if (state.TotalPopulation < 5) {
			return smallTrainCard();
		} else if (state.TotalPopulation < 10) {
			return mediumTrainCard();
		} else {
			return largeTrainCard();
		}
	}

	private static EventCard smallTrainCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 2},
			{ "Wild Animals", 2},
			{ "Minor Plague", 1},
			{ "Trading Caravan", 1},
			{ "Convoy under Attack", 1},
			//{ "Obstacle on Tracks", 1},
			{ "Mutiny!", 1},
		}).Shuffle().First();	
	}

	private static EventCard mediumTrainCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "plague", 2},
			{ "Wild Animals", 1},
			{ "Famine", 1},
			{ "Natural Disaster", 1},
		}).Shuffle().First();
	}

	private static EventCard largeTrainCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Famine", 1},
			{ "Natural Disaster", 1},
			{ "Animal Attacks", 1},
		}).Shuffle().First();
	}
}
