// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class TraditionsCollection {
	private static BaseCollection<Tradition> traditions = new BaseCollection<Tradition>(new Tradition[] {
		new Tradition("Test"),
		new Tradition("Agricultural Tradition",
			cardToEnhance:"Farming",
			propertyToEnhance:"IndustryGain",
			increaseInValue:1),
		new Tradition("Sea-going Tradition",
			cardToEnhance:"Fishing",
			propertyToEnhance:"IndustryGain",
			increaseInValue:1),
		new Tradition("Agricultural Tithes",
			cardToEnhance:"Farming",
			propertyToEnhance:"GoldGain",
			increaseInValue:1),
		new Tradition("Sea-going Tithes",
			cardToEnhance:"Fishing",
			propertyToEnhance:"GoldGain",
			increaseInValue:1),
		new Tradition("Underground Spirits",
			cardToEnhance:"Mine",
			propertyToEnhance:"IndustryGain",
			increaseInValue:1),
		new Tradition("Breeding",
			cardToEnhance:"Build Village",
			propertyToEnhance:"PopulationGain",
			increaseInValue:1),
		new Tradition("Sea Breeding",
			cardToEnhance:"Fishing Village",
			propertyToEnhance:"PopulationGain",
			increaseInValue:1),
		new Tradition("Mutual beneficiality",
			cardToEnhance:"Barter",
			propertyToEnhance:"GoldGain",
			increaseInValue:1),
		new Tradition("Free Trading",
			cardToEnhance:"Barter",
			propertyToEnhance:"IndustryCost",
			increaseInValue:-1),
	}.ToDictionary(tradition => tradition.Name, tradition => tradition));

	public static IEnumerable<Tradition> TraditionsForDeck(DeckType deckType) {
		switch (deckType) {
			case DeckType.BaseTraditions:
				return baseTraditionsDeck();
			case DeckType.Test:
				return testDeck();
			default:
				return new Tradition[0];
		}
	}

	private static IEnumerable<Tradition> baseTraditionsDeck() {
		return traditions.objectForDictionary(new Dictionary<string, int> {
			{"Agricultural Tradition", 1},
			{"Sea-going Tradition", 1},
			{"Agricultural Tithes", 1},
			{"Sea-going Tithes", 1},
		}).Shuffle().Take(1);
	}

	private static IEnumerable<Tradition> testDeck () {
		return traditions.objectForDictionary(new Dictionary<string, int> {
			{"Test", 1}
		});
	}
}
