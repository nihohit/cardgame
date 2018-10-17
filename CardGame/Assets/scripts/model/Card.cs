using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Card : BaseValueClass {
  public string Name { get; }
	public int GoldCost { get; }
	public int IndustryCost { get; }
	public int PopulationCost { get; }
	public int ArmyCost { get; }
	public int GoldGain { get; }
	public int IndustryGain { get; }
	public int PopulationGain { get; }
	public int ArmyGain { get; }
	public DeckType AddDeck { get; }
	public bool Exhaustible { get; }
	public int NumberOfCardsToChooseToExhaust { get; }
	public int NumberOfCardsToChooseToReplace { get; }
	public bool DefaultChoice { get; }

	public Card(string name, 
		int goldCost = 0, 
		int industryCost = 0, 
		int populationCost = 0,
		int armyCost = 0,
		int goldGain = 0,
		int industryGain = 0,
		int populationGain = 0,
		int armyGain = 0,
		DeckType addDeck = DeckType.None,
		bool exhaustible = false, 
		int numberOfCardsToChooseToExhaust = 0,
		int numberOfCardsToChooseToReplace = 0,
		bool defaultChoice = false) {
		Name = name;
		GoldCost = goldCost;
		IndustryCost = industryCost;
		PopulationCost = populationCost;
		ArmyCost = armyCost;
		GoldGain = goldGain;
		IndustryGain = industryGain;
		PopulationGain = populationGain;
		ArmyGain = armyGain;
		AddDeck = addDeck;
		Exhaustible = exhaustible;
		NumberOfCardsToChooseToExhaust = numberOfCardsToChooseToExhaust;
		NumberOfCardsToChooseToReplace = numberOfCardsToChooseToReplace;
		DefaultChoice = defaultChoice;
	}
}
