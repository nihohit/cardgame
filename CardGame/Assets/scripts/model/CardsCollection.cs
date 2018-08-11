// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DeckType { None, Test, Village, Temple, VillageCenter, Town, PublicDiscourse1 }

public static class CardsCollection {
	public static IEnumerable<Card> Cards() {
		return cardsForDictionary(new Dictionary<Card, int>{
			{ new Card{
				Name = "Manual Labour",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryGain = 1
			}, 4},
			{new Card{
				Name = "Barter",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryCost = 1,
				GoldGain = 1
			}, 2},
			{new Card{
				Name = "Build Village",
				PopulationGain = 2,
				PopulationCost = 1,
				IndustryCost = 1,
				AddDeck = DeckType.Village
			}, 1}
		});	
	}

	private static IEnumerable<Card> cardsForDictionary(Dictionary<Card, int> cardDictionary) {
		return cardDictionary.SelectMany(pair => createCopies(pair.Key, pair.Value));	
	}

	private static IEnumerable<Card> createCopies(Card card, int copies) {
		for(int i = 0; i <  copies; i++) {
			yield return card.ShallowClone();
		}
	}

	public static IEnumerable<Card> CardsForDeck(DeckType deckType) {
		switch (deckType) {
			case DeckType.Village: 
				return villageDeck();
			case DeckType.VillageCenter: 
				return villageCenterDeck();
			case DeckType.Test:
				return testDeck();
			default:
				return new Card[0];
		}
	}

	private static IEnumerable<Card> villageDeck() {
		return cardsForDictionary(new Dictionary<Card, int>{
			{new Card{
				Name = "Temple",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryCost = 4,
				Exhaustible = true,
				AddDeck = DeckType.Temple
			}, 1},
			{new Card{
				Name = "Village center",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryCost = 4,
				Exhaustible = true,
				AddDeck = DeckType.VillageCenter
			}, 1},
			{new Card{
				Name = "Town",
				PopulationGain = 3,
				PopulationCost = 2,
				IndustryCost = 10,
				Exhaustible = true,
				AddDeck = DeckType.Town
			}, 1},
			{new Card{
				Name = "Farming",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryGain = 2
			}, 3},
			{new Card{
				Name = "Arm Militia",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryCost = 1,
				ArmyGain = 1
			}, 1}
		});
	}

	private static IEnumerable<Card> villageCenterDeck() {
		return cardsForDictionary(new Dictionary<Card, int>{
			{new Card{
				Name = "Market Day",
				PopulationGain = 1,
				PopulationCost = 1,
				IndustryCost = 1,
				GoldGain = 2,
			}, 2},
			{new Card{
				Name = "Public Discussion",
				PopulationGain = 2,
				PopulationCost = 2,
				GoldCost = 2,
				AddDeck = DeckType.PublicDiscourse1
			}, 1},
			{new Card{
				Name = "Buy Slaves",
				PopulationGain = 2,
				PopulationCost = 1,
				GoldCost = 10
			}, 1},
		});
	}

	private static IEnumerable<Card> testDeck() {
		return cardsForDictionary(new Dictionary<Card, int>{
			{new Card{
				Name = "test"
			}, 1}
		});
	}
}
