// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum DeckType { None, Village }

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
				IndustryCost = 2,
				GoldGain = 1
			}, 2},
			{new Card{
				Name = "Build Village",
				PopulationGain = 2,
				PopulationCost = 1,
				IndustryCost = 0,
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
				return cardsForDictionary(new Dictionary<Card, int>{
					{new Card{
						Name = "Granary",
						PopulationGain = 1,
						PopulationCost = 1,
						IndustryCost = 3,
						Exhaustible = true
					}, 1},
					{new Card{
						Name = "Village center",
						PopulationGain = 1,
						PopulationCost = 1,
						IndustryCost = 3,
						Exhaustible = true
					}, 1},
					{new Card{
						Name = "Town",
						PopulationGain = 3,
						PopulationCost = 2,
						IndustryCost = 10,
						Exhaustible = true
					}, 1},
					{new Card{
						Name = "Farm",
						PopulationGain = 1,
						PopulationCost = 1,
						IndustryGain = 2
					}, 3}
				});
			default:
				return new Card[0];
		}
	}
}
