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
			Options = new Card[] {
				Card.MakeCard("Pay them off",
					fuelChange: -1
				),
				Card.MakeCard("Fight them",
					armyChange: -2
				),
				Card.MakeCard("They attack",
					populationChange: -1
				)
			}
		},
		new EventCard {
			Name = "Wild animals",
			Options = new Card[] {
				Card.MakeCard("Build traps",
					materialsChange: -2
				),
				Card.MakeCard("Fight them",
					armyChange: -1
				),
				Card.MakeCard("They attack",
					populationChange: -1
				)
			}
		},
		new EventCard {
			Name = "Minor plague",
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
					armyChange: -2
				),
				Card.MakeCard("Lose people and productivity",
					populationChange: -1,
					materialsChange: -1,
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
			Name = "Natural disaster1",
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
	}.ToDictionary(card => card.Name, card => card));

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
			{ "Raiders", 3},
			{ "Wild animals", 3},
			{ "Minor plague", 1},
		}).Shuffle().First();	
	}

	private static EventCard mediumTrainCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "plague", 2},
			{ "Wild animals", 1},
			{ "Famine", 1},
			{ "Natural disaster1", 1},
		}).Shuffle().First();
	}

	private static EventCard largeTrainCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Famine", 1},
			{ "Natural disaster1", 1},
			{ "Animal Attacks", 1},
		}).Shuffle().First();
	}
}
