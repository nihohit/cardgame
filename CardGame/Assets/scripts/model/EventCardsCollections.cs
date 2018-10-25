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
				new Card("Pay them off",
					fuelChange: -1
				),
				new Card("Fight them",
					armyChange: -2
				),
				new Card("They attack",
					populationChange: -1
				)
			}
		},
		new EventCard {
			Name = "Wild animals",
			Options = new Card[] {
				new Card("Build traps",
					materialsChange: -2
				),
				new Card("Fight them",
					armyChange: -1
				),
				new Card("They attack",
					populationChange: -1
				)
			}
		},
		new EventCard {
			Name = "Minor plague",
			Options = new Card[] {
				new Card("Buy medicine",
					fuelChange: -2
				),
				new Card("Quarantine the sick",
					armyChange: -1,
					populationChange: -1
				),
				new Card("Let the sick die",
					populationChange: -2,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Famine",
			Options = new Card[] {
				new Card("Buy food",
					fuelChange: -2
				),
				new Card("Raid Neighbours",
					armyChange: -2
				),
				new Card("Lose people and productivity",
					populationChange: -1,
					materialsChange: -1,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "plague",
			Options = new Card[] {
				new Card("Buy medicine",
					fuelChange: -4
				),
				new Card("Quarantine the sick",
					armyChange: -2,
					populationChange: -1
				),
				new Card("Let the sick die",
					populationChange: -3,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Natural disaster1",
			Options = new Card[] {
				new Card("Rebuild",
					materialsChange: -5
				),
				new Card("Build temporary shelters",
					materialsChange: -2,
					populationChange: -1
				),
				new Card("Let the population handle it",
					populationChange: -2,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Animal Attacks",
			Options = new Card[] {
				new Card("Hire Mercenaries",
					fuelChange: -6
				),
				new Card("Defend the citizens",
					armyChange: -3
				),
				new Card("They attack",
					populationCost: 2,
					armyChange: -2
				)
			}
		},
	}.ToDictionary(card => card.Name, card => card));

	public static EventCard EventCardForState(TrainState state) {
		if (state.TotalPopulation < 5) {
			return smallEmpireCard();
		} else if (state.TotalPopulation < 10) {
			return mediumEmpireCard();
		} else {
			return largeEmpireCard();
		}
	}

	private static EventCard smallEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 3},
			{ "Wild animals", 3},
			{ "Minor plague", 1},
		}).Shuffle().First();	
	}

	private static EventCard mediumEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "plague", 2},
			{ "Wild animals", 1},
			{ "Famine", 1},
			{ "Natural disaster1", 1},
		}).Shuffle().First();
	}

	private static EventCard largeEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Famine", 1},
			{ "Natural disaster1", 1},
			{ "Animal Attacks", 1},
		}).Shuffle().First();
	}
}
