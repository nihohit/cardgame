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
	public int Army { get; }
	public int AddGold { get; }
	public int AddIndustry { get; }
	public int AddPopulation { get; }
	public int AddArmy { get; }

	public EmpireState(int gold, int industry, int population, int army) :
	  this(gold, industry, population, army, 0, 0, 0, 0) {}

	private EmpireState(int gold, int industry, int population, int army,
	  int addGold, int addIndustry, int addPopulation, int addArmy) {
	  Gold = gold;
	  Industry = industry;
	  Population = population;
		Army = army;
	  AddGold = addGold;
	  AddIndustry = addIndustry;
	  AddPopulation = addPopulation;
		AddArmy = addArmy;
	}

	public EmpireState NextTurnState() {
		return new EmpireState(
			Gold + AddGold, 
			Industry + AddIndustry, 
			Population + AddPopulation,
			Army + AddArmy);
	}

	public EmpireState ChangeGold(int goldChange) {
		return new EmpireState(Gold + goldChange, Industry, Population, Army, 
			AddGold, AddIndustry, AddPopulation, AddArmy);
	}

	public EmpireState ChangeIndustry(int changeIndustry) {
		return new EmpireState(Gold, Industry + changeIndustry, Population, Army,
			AddGold, AddIndustry, AddPopulation, AddArmy);
	}

	public EmpireState ChangePopulation(int populationChange) {
		return new EmpireState(Gold, Industry, Population + populationChange, Army,
			AddGold, AddIndustry, AddPopulation, AddArmy);
	}

	public EmpireState ChangeArmy(int armyChange) {
		return new EmpireState(Gold, Industry, Population, Army + armyChange,
			AddGold, AddIndustry, AddPopulation, AddArmy);
	}

	public bool CanPlayCard(Card card) {
		return Gold >= card.GoldCost &&
			Industry >= card.IndustryCost &&
			Population >= card.PopulationCost &&
			Army >= card.ArmyCost;
	}

	public EmpireState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		return new EmpireState(
			Gold - card.GoldCost,
			Industry - card.IndustryCost,
			Population - card.PopulationCost,
			Army - card.ArmyCost,
			card.GoldGain + AddGold,
			card.IndustryGain + AddIndustry,
			card.PopulationGain + AddPopulation,
			card.ArmyGain + AddArmy);
	}

	public override string ToString() {
		return $"Gold: {Gold}, next turn:{Gold + AddGold}\n" +
		  $"Industry: {Industry}, next turn:{Industry + AddIndustry}\n" +
		  $"Population: {Population}, next turn:{Population + AddPopulation}\n" +
		  $"Army: {Army}, next turn:{Army + AddArmy}\n";
	}
}
