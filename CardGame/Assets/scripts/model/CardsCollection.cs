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
				Name = "Farm",
				PopulationGain = 1,
				IndustryGain = 1,
			},
			new Card{
				Name = "Fishing Villages",
				PopulationGain = 3
			},
			new Card{
				Name = "Hunting Outposts",
				PopulationGain = 3
			},
			new Card{
				Name = "Migrate across sea",
				PopulationCost = 2
			},
			new Card{
				Name = "Migrate across plains",
				PopulationCost = 1
			},
			new Card{
				Name = "City",
				PopulationGain = 1,
				IndustryGain = 1,
				GoldGain = 1
			},
			new Card{
				Name = "Build Cities"
			},
			new Card{
				Name = "Build Cities"
			},
			new Card{
				Name = "Build Cities"
			}
		};
	}
}
