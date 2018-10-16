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
					goldCost: 1
				),
				new Card("Fight them",
					armyCost: 2
				),
				new Card("They attack",
					populationCost: 1
				)
			}
		},
		new EventCard {
			Name = "Wild animals",
			Options = new Card[] {
				new Card("Build traps",
					industryCost: 2
				),
				new Card("Fight them",
					armyCost: 1
				),
				new Card("They attack",
					populationCost: 1
				)
			}
		},
		new EventCard {
			Name = "Minor plague",
			Options = new Card[] {
				new Card("Buy medicine",
					goldCost: 2
				),
				new Card("Quarantine the sick",
					armyCost: 1,
					populationCost: 1
				),
				new Card("Let the sick die",
					populationCost: 2,
					defaultChoice: true
				)
			}
		},
		new EventCard {
			Name = "Famine",
			Options = new Card[] {
				new Card("Buy food",
					goldCost: 2
				),
				new Card("Raid Neighbours",
					armyCost: 1
				),
				new Card("Lose people and productivity",
					populationCost: 1,
					industryCost: 1,
					defaultChoice: true
				)
			}
		}
	}.ToDictionary(card => card.Name, card => card));

	public static EventCard EventCardForState(EmpireState state) {
		if (state.Population < 5) {
			return smallEmpireCard();
		} else if (state.Population < 10) {
			return mediumEmpireCard();
		} else {
			return largeEmpireCard();
		}
	}

	private static EventCard smallEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 3},
			{ "Wild animals", 3},
			{ "Famine", 3},
			{ "Minor plague", 1},
		}).Shuffle().First();	
	}

	private static EventCard mediumEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 1},
			{ "Wild animals", 1},
		}).Shuffle().First();
	}

	private static EventCard largeEmpireCard() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 1},
			{ "Wild animals", 1},
		}).Shuffle().First();
	}
}
