// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class CardsCollection {
	public static IEnumerable<Card> Cards() {
		return new Card[] {
			new Card{
				Name = "Farming",
				PopulationGain = 2,
				PopulationCost = 1
			},
			new Card{
				Name = "Farming",
				PopulationGain = 2,
				PopulationCost = 1
			},	
			new Card{
				Name = "Gather resources",
				IndustryGain = 2,
				PopulationGain = 1,
				PopulationCost = 1
			},	
			new Card{
				Name = "Gather resources",
				IndustryGain = 2,
				PopulationGain = 1,
				PopulationCost = 1
			},
			new Card{
				Name = "Farming",
				PopulationGain = 2,
				PopulationCost = 1
			},
			new Card{
				Name = "Farming",
				PopulationGain = 2,
				PopulationCost = 1
			},	
			new Card{
				Name = "Gather resources",
				IndustryGain = 2,
				PopulationGain = 1,
				PopulationCost = 1
			},	
			new Card{
				Name = "Gather resources",
				IndustryGain = 2,
				PopulationGain = 2,
				PopulationCost = 1
			},	
			new Card{
				Name = "Sell resources",
				IndustryCost = 1,
				GoldGain = 1,
				PopulationGain = 1,
				PopulationCost = 1
			},	
			new Card{
				Name = "Sell resources",
				IndustryCost = 1,
				GoldGain = 1,
				PopulationGain = 1,
				PopulationCost = 1
			}				
		};
	}
}
