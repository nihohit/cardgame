// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmpireState {
	public int Gold { get; private set; }
	public int Industry { get; private set; }
	public int Population { get; private set; }

	public EmpireState(int gold, int industry, int population) {
		Gold = gold;
		Industry = industry;
		Population = population;
	}

	public bool CanPlayCard(Card card) {
		return Gold >= card.GoldCost &&
			Industry >= card.IndustryCost &&
			Population >= card.PopulationCost;
	}

	public EmpireState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		return new EmpireState(
			Gold + card.GoldGain - card.GoldCost,
			Industry + card.IndustryGain - card.IndustryCost,
			Population + card.PopulationGain - card.PopulationCost);
	}

	public override string ToString() {
		return $"Gold: {Gold}\nIndustry: {Industry}\nPopulation: {Population}";
	}
}
