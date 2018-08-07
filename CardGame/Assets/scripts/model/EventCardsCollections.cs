// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class EventCardsCollections {
	public static IEnumerable<EventCard> Cards() {
		return cardsForDictionary(new Dictionary<EventCard, int> {
			{ new EventCard {
				Name = "Raiders",
        Option1 = new Card {
          Name = "Pay them off",
          GoldCost = 1
        },
        Option2 = new Card {
          Name = "Fight them",
          ArmyCost = 2
        },
        Default = new Card {
          Name = "They attack",
          PopulationCost = 1
        },
			}, 1},
		});	
	}

	private static IEnumerable<EventCard> cardsForDictionary(Dictionary<EventCard, int> cardDictionary) {
		return cardDictionary.SelectMany(pair => createCopies(pair.Key, pair.Value));	
	}

	private static IEnumerable<EventCard> createCopies(EventCard card, int copies) {
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
		return cardsForDictionary(new Dictionary<EventCard, int>{
		});
	}

	private static IEnumerable<EventCard> villageCenterDeck() {
		return cardsForDictionary(new Dictionary<EventCard, int>{
		});
	}

	private static IEnumerable<EventCard> testDeck() {
		return cardsForDictionary(new Dictionary<EventCard, int>{
		});
	}
}
