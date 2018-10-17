// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EmpireState : BaseValueClass {
	public int Gold { get; }
	public int Industry { get; }
	public int TotalPopulation { get; }
	public int AvailablePopulation { get; }
	public int Army { get; }
	public IReadOnlyList<Card> PlayedCards { get; }

	public static EmpireState InitialState(int gold, int industry, int population, int army) {
		return new EmpireState(gold, industry, population, population, army, new List<Card>());
	}

	private EmpireState(int gold, int industry, int totalPopulation, int availablePopulation, int army, IReadOnlyList<Card> playedCards) {
	  Gold = gold;
	  Industry = industry;
	  TotalPopulation = totalPopulation;
		AvailablePopulation = availablePopulation;
		Army = army;
		PlayedCards = playedCards;
	}

	public EmpireState NextTurnState() {
		return new EmpireState(
			Gold, 
			Industry, 
			TotalPopulation,
			TotalPopulation,
			Army,
			PlayedCards);
	}

	public EmpireState ChangeGold(int goldChange) {
		return new EmpireState(Gold + goldChange, Industry, TotalPopulation, AvailablePopulation, Army, PlayedCards);
	}

	public EmpireState ChangeIndustry(int changeIndustry) {
		return new EmpireState(Gold, Industry + changeIndustry, TotalPopulation, AvailablePopulation, Army, PlayedCards);
	}

	public EmpireState ChangePopulation(int populationChange) {
		return new EmpireState(Gold, Industry, TotalPopulation + populationChange, AvailablePopulation, Army, PlayedCards);
	}

	public EmpireState ChangeArmy(int armyChange) {
		return new EmpireState(Gold, Industry, TotalPopulation, AvailablePopulation, Army + armyChange, PlayedCards);
	}

	public bool CanPlayCard(Card card) {
		return (Gold >= -card.GoldChange &&
			Industry >= -card.IndustryChange &&
			AvailablePopulation >= card.PopulationCost &&
			Army >= -card.ArmyChange) ||
			card.DefaultChoice;
	}

	public EmpireState PlayCard(Card card) {
		Debug.Assert(CanPlayCard(card));

		var playedCards = new List<Card>(PlayedCards) {
			card
		};

		return new EmpireState(
			Math.Max(Gold + card.GoldChange, 0),
			Math.Max(Industry + card.IndustryChange, 0),
			Math.Max(TotalPopulation + card.PopulationChange, 0),
			Math.Max(AvailablePopulation - card.PopulationCost, 0),
			Math.Max(Army + card.ArmyChange, 0),
			playedCards);
	}
}
