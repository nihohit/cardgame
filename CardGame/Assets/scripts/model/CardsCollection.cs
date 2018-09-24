// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DeckType { None, Test, Village,FishingVillage, PortTown, BaseTraditions, VillageCenter, Town, Explore, Mine }

public static class CardsCollection {
	private static BaseCollection<Card> cards = new BaseCollection<Card>(new Card[] {
		new Card("test"),
		new Card("Manual Labour",
			populationGain: 1,
			populationCost: 1,
			industryGain: 1
		),
		new Card("Barter",
			populationGain: 1,
			populationCost: 1,
			industryCost: 1,
			goldGain: 1
		),
		new Card("Build Village",
			populationGain: 2,
			populationCost: 1,
			industryCost: 1,
			addDeck: DeckType.Village,
			exhaustible:true
		),
		new Card("Temple",
			populationGain: 1,
			populationCost: 1,
			industryCost: 4,
			exhaustible: true,
			addDeck: DeckType.BaseTraditions
		),
		new Card("Village center",
			populationGain: 1,
			populationCost: 1,
			industryCost: 4,
			exhaustible: true,
			addDeck: DeckType.VillageCenter
		),
		new Card("Town",
			populationGain: 3,
			populationCost: 2,
			industryCost: 10,
			exhaustible: true,
			addDeck: DeckType.Town
		),
		new Card("Farming",
			populationGain: 1,
			populationCost: 1,
			industryGain: 2
		),
		new Card("Arm Militia",
			populationGain: 1,
			populationCost: 1,
			industryCost: 1,
			armyGain: 1
		),
		new Card("Market Day",
			populationGain: 1,
			populationCost: 1,
			industryCost: 1,
			goldGain: 2
		),
		new Card("Public Discussion",
			populationGain: 1,
			populationCost: 1,
			goldCost: 2,
			numberOfCardsToChooseToReplace: 1
		),
		new Card("Ostracize",
			populationGain: 1,
			populationCost: 1,
			goldCost: 2,
			numberOfCardsToChooseToExhaust: 1
		),
		new Card("Buy Slaves",
			populationGain: 2,
			populationCost: 1,
			goldCost: 10
		),
		new Card("Fishing Village",
			populationGain: 2,
			populationCost: 1,
			industryCost: 2,
			addDeck: DeckType.FishingVillage,
			exhaustible:true
		),
		new Card("Fishing",
			populationGain: 1,
			populationCost: 1,
			industryGain: 1,
			goldGain: 1
		),
		new Card("Port Town",
			populationGain: 2,
			populationCost: 1,
			industryCost: 10,
			addDeck: DeckType.PortTown,
			exhaustible:true
		),
		new Card("Explore",
			populationGain: 1,
			populationCost: 1,
			addDeck: DeckType.Explore
		),
		new Card("Hire Mercenaries",
			populationGain: 1,
			populationCost: 1,
			goldCost: 1,
			armyGain: 1
		),
		new Card("Build Mine",
			populationGain: 1,
			populationCost: 1,
			industryCost: 1,
			addDeck: DeckType.Mine,
			exhaustible:true
		),
		new Card("Mine",
			populationGain: 1,
			populationCost: 1,
			industryGain: 3
		)
	}.ToDictionary(card => card.Name, card => card));


	public static IEnumerable<Card> Cards() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Manual Labour", 2},
			{"Barter", 1},
			{"Explore", 1},
			{"Hire Mercenaries", 1}
		});	
	}

	public static IEnumerable<Card> CardsForDeck(DeckType deckType) {
		switch (deckType) {
			case DeckType.Village: 
				return villageDeck();
			case DeckType.VillageCenter: 
				return villageCenterDeck();
			case DeckType.FishingVillage:
				return fishingVillageDeck();
			case DeckType.Explore:
				return exploreDeck();
			case DeckType.Mine:
				return mineDeck();
			case DeckType.Test:
				return testDeck();
			default:
				return new Card[0];
		}
	}

	private static IEnumerable<Card> villageDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Temple", 1},
			{"Village center", 1},
			{"Town", 1},
			{"Farming", 3},
			{"Arm Militia", 1}
		});
	}

	private static IEnumerable<Card> fishingVillageDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Temple", 1},
			{"Village center", 1},
			{"Port Town", 1},
			{"Fishing", 3},
			{"Arm Militia", 1}
		});
	}

	private static IEnumerable<Card> villageCenterDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Market Day", 2},
			{"Public Discussion", 1},
			{"Buy Slaves", 1},
			{"Ostracize", 1},
		});
	}

	private static IEnumerable<Card> exploreDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Build Village", 1},
			{"Fishing Village", 1},
			{"Build Mine", 1},
		}).Shuffle().Take(1);
	}

	private static IEnumerable<Card> mineDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"Mine", 1}
		});
	}

	private static IEnumerable<Card> testDeck() {
		return cards.objectForDictionary(new Dictionary<string, int>{
			{"test", 1}
		});
	}
}
