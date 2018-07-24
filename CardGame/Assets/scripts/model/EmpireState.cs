// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmpireState {
	public int Gold { get; }
	public int Industry { get; }
	public int Population { get; }
	public int AddGold { get; }
	public int AddIndustry { get; }
	public int AddPopulation { get; }

	public EmpireState(int gold, int industry, int population) :
	  this(gold, industry, population, 0, 0, 0) {}

	private EmpireState(int gold, int industry, int population, 
	  int addGold, int addIndustry, int addPopulation) {
	  Gold = gold;
	  Industry = industry;
	  Population = population;
	  AddGold = addGold;
	  AddIndustry = addIndustry;
	  AddPopulation = addPopulation;
	}

	public EmpireState NextTurnState() {
		return new EmpireState(Gold + AddGold, Industry + AddIndustry, Population + AddPopulation);
	}

	public bool CanPlayCard(Card card) {
		return Gold >= card.GoldCost &&
			Industry >= card.IndustryCost &&
			Population >= card.PopulationCost;
	}

	public EmpireState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		return new EmpireState(
			Gold - card.GoldCost,
			Industry - card.IndustryCost,
			Population - card.PopulationCost,
			card.GoldGain + AddGold,
			card.IndustryGain + AddIndustry,
			card.PopulationGain + AddPopulation);
	}

	public override string ToString() {
		return $"Gold: {Gold}, next turn:{Gold + AddGold}\n" +
		  $"Industry: {Industry}, next turn:{Industry + AddIndustry}\n" +
		  $"Population: {Population}, next turn:{Population + AddPopulation}\n";
	}
}
