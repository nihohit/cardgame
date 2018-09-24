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
				new Card("Scare them",
					industryCost: 2
				),
				new Card("Fight them",
					armyCost: 1
				),
				new Card("They attack",
					populationCost: 1
				)
			}
		}
	}.ToDictionary(card => card.Name, card => card));

	public static IEnumerable<EventCard> Cards() {
		return events.objectForDictionary(new Dictionary<string, int> {
			{ "Raiders", 2},
			{ "Wild animals", 2},
		});	
	}

	public static IEnumerable<EventCard> CardsForDeck(DeckType deckType) {
		switch (deckType) {
			case DeckType.Village: 
				return villageDeck();
			case DeckType.VillageCenter: 
				return villageCenterDeck();
			case DeckType.Test:
				return testDeck();
			default:
				return new EventCard[0];
		}
	}

	private static IEnumerable<EventCard> villageDeck() {
		return events.objectForDictionary(new Dictionary<string, int>{
		});
	}

	private static IEnumerable<EventCard> villageCenterDeck() {
		return events.objectForDictionary(new Dictionary<string, int>{
		});
	}

	private static IEnumerable<EventCard> testDeck() {
		return events.objectForDictionary(new Dictionary<string, int>{
		});
	}
}
