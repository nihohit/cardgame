// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventCardsCollections {
	private static Dictionary<string, EventCard> events = new EventCard[] {
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
	}.ToDictionary(card => card.Name, card => card);

	public static IEnumerable<EventCard> Cards() {
		return cardsForDictionary(new Dictionary<string, int> {
			{ "Raiders", 2},
			{ "Wild animals", 2},
		});	
	}

	private static IEnumerable<EventCard> cardsForDictionary(Dictionary<string, int> cardDictionary) {
		return cardDictionary.SelectMany(pair => createCopies(pair.Key, pair.Value));	
	}

	private static IEnumerable<EventCard> createCopies(string eventName, int copies) {
		var card = events.Get(eventName, "created events");
		for(int i = 0; i <  copies; i++) {
			yield return card.ShallowClone();
		}
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
		return cardsForDictionary(new Dictionary<string, int>{
		});
	}

	private static IEnumerable<EventCard> villageCenterDeck() {
		return cardsForDictionary(new Dictionary<string, int>{
		});
	}

	private static IEnumerable<EventCard> testDeck() {
		return cardsForDictionary(new Dictionary<string, int>{
		});
	}
}
