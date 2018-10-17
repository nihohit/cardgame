using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class Card : BaseValueClass {
  public string Name { get; }
	public int PopulationCost { get; }
	public int GoldChange { get; }
	public int IndustryChange { get; }
	public int PopulationChange { get; }
	public int ArmyChange { get; }
	public DeckType AddDeck { get; }
	public bool Exhaustible { get; }
	public int NumberOfCardsToChooseToExhaust { get; }
	public int NumberOfCardsToChooseToReplace { get; }
	public bool DefaultChoice { get; }

	public Card(string name,
		int populationCost = 0,
		int goldChange = 0, 
		int industryChange = 0, 
		int populationChange = 0,
		int armyChange = 0,
		DeckType addDeck = DeckType.None,
		bool exhaustible = false, 
		int numberOfCardsToChooseToExhaust = 0,
		int numberOfCardsToChooseToReplace = 0,
		bool defaultChoice = false) {
		Name = name;
		PopulationCost = populationCost;
		GoldChange = goldChange;
		IndustryChange = industryChange;
		PopulationChange = populationChange;
		ArmyChange = armyChange;
		AddDeck = addDeck;
		Exhaustible = exhaustible;
		NumberOfCardsToChooseToExhaust = numberOfCardsToChooseToExhaust;
		NumberOfCardsToChooseToReplace = numberOfCardsToChooseToReplace;
		DefaultChoice = defaultChoice;
	}
}
