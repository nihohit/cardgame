using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class Card : BaseValueClass {
  public string Name { get; }
	public string Identifier { get; }
	public Card Source { get; }
	public int PopulationCost { get; }
	public int FuelChange { get; }
	public int MaterialsChange { get; }
	public int PopulationChange { get; }
	public int ArmyChange { get; }
	public TraditionType AddTradition { get; }
	public bool Exhaustible { get; }
	public int NumberOfCardsToChooseToExhaust { get; }
	public int NumberOfCardsToDraw { get; }
	public int NumberOfCardsToChooseToDiscard { get; }
	public Func<Card, bool> CardDrawingFilter { get; }
	public bool DefaultChoice { get; }
	public TrainCar CarToAdd { get; }
	public IEnumerable<CarType> CarOptionsToAdd { get; }
	public CarType CarToRemove { get; }
	public bool LocationLimited { get; }
	public CarType RequiresCar { get; }
	public CarType ModifiedByCar { get; }
	public Dictionary<string, int> CarModifications { get; }
	public string CustomDescription { get; }
	public IEnumerable<string> StoryCardsToAddToHand { get; }
	public IEnumerable<string> StoryCardsToAddToDiscard { get; }
	public IEnumerable<string> StoryCardsToAddToTopOfDeck { get; }
	public IEnumerable<string> StoryCardsToRemove { get; }
	public EventCard ContainedEvent { get; }

	protected Card(string name,
		string identifier,
		Card source,
		int populationCost,
		int fuelChange,
		int materialsChange,
		int populationChange,
		int armyChange,
		TraditionType addTradition,
		bool exhaustible,
		int numberOfCardsToChooseToExhaust,
		int numberOfCardsToDraw,
		int numberOfCardsToChooseToDiscard,
		Func<Card, bool> cardDrawingFilter,
		bool defaultChoice,
		CarType carToRemove,
		TrainCar carToAdd,
		IEnumerable<CarType> carOptionsToAdd,
		bool locationLimited,
		CarType requiresCar,
		CarType modifiedByCar,
		Dictionary<string, int> carModifications,
		string customDescription,
		IEnumerable<string> storyCardsToAddToHand,
		IEnumerable<string> storyCardsToAddToDiscard,
		IEnumerable<string> storyCardsToAddToTopOfDeck,
		IEnumerable<string> storyCardsToRemove,
		EventCard containedEvent) {
		AssertUtils.AreEqual(ModifiedByCar == CarType.None, 
			CarModifications == null);
		Name = name;
		Identifier = identifier;
		Source = source;
		PopulationCost = populationCost;
		FuelChange = fuelChange;
		MaterialsChange = materialsChange;
		PopulationChange = populationChange;
		ArmyChange = armyChange;
		AddTradition = addTradition;
		Exhaustible = exhaustible;
		NumberOfCardsToChooseToExhaust = numberOfCardsToChooseToExhaust;
		NumberOfCardsToDraw = numberOfCardsToDraw;
		NumberOfCardsToChooseToDiscard = numberOfCardsToChooseToDiscard;
		CardDrawingFilter = cardDrawingFilter;
		DefaultChoice = defaultChoice;
		CarToAdd = carToAdd;
		CarOptionsToAdd = carOptionsToAdd;
		CarToRemove = carToRemove;
		LocationLimited = locationLimited;
		RequiresCar = requiresCar;
		ModifiedByCar = modifiedByCar;
		CarModifications = carModifications;
		CustomDescription = customDescription;
		StoryCardsToAddToHand = storyCardsToAddToHand ?? Enumerable.Empty<string>();
		StoryCardsToAddToDiscard = storyCardsToAddToDiscard ?? Enumerable.Empty<string>();
		StoryCardsToAddToTopOfDeck = storyCardsToAddToTopOfDeck ?? Enumerable.Empty<string>();
		StoryCardsToRemove = storyCardsToRemove ?? Enumerable.Empty<string>();
		ContainedEvent = containedEvent;
	}

	public static Card MakeCard(
		string name,
		string identifier = null,
		int populationCost = 0,
		int fuelChange = 0, 
		int materialsChange = 0, 
		int populationChange = 0,
		int armyChange = 0,
		TraditionType addTradition = TraditionType.None,
		bool exhaustible = false, 
		int numberOfCardsToChooseToExhaust = 0,
		int numberOfCardsToChooseToDraw = 0,
		int numberOfCardsToChooseToDiscard = 0,
		Func<Card, bool> cardDrawingFilter = null,
		bool defaultChoice = false,
		CarType carToRemove = CarType.None,
		TrainCar carToAdd = null,
		IEnumerable<CarType> carOptionsToAdd = null,
		bool locationLimited = false,
		CarType requiresCar = CarType.None,
		CarType modifiedByCar = CarType.None,
		Dictionary<string, int> carModifications = null,
		string customDescription = null,
		IEnumerable<string> storyCardsToAddToHand = null,
		IEnumerable<string> storyCardsToAddToDiscard = null,
		IEnumerable<string> storyCardsToAddToTopOfDeck = null,
		IEnumerable<string> storyCardsToRemove = null,
		EventCard containedEvent = null) {
		return new Card(name,
			identifier ?? name,
			null,
			populationCost,
			fuelChange,
			materialsChange,
			populationChange,
			armyChange,
			addTradition,
			exhaustible,
			numberOfCardsToChooseToExhaust,
			numberOfCardsToChooseToDraw,
			numberOfCardsToChooseToDiscard,
			cardDrawingFilter,
			defaultChoice,
			carToRemove,
			carToAdd,
			carOptionsToAdd,
			locationLimited,
			requiresCar,
			modifiedByCar,
			carModifications, 
			customDescription, 
			storyCardsToAddToHand,
			storyCardsToAddToDiscard,
			storyCardsToAddToTopOfDeck,
			storyCardsToRemove,
			containedEvent);
	}

	public Card MakeExhaustibleCopy() {
		return this.CopyWithSetValue("Exhaustible", true);
	}

	public Card MakeLocationLimitedCopy() {
		return this.CopyWithSetValue("LocationLimited", true);
	}

	public Card Original() {
		var original = this;
		while (original.Source != null) {
			original = original.Source;
		}
		return original;
	}

	public Card CopyWithSource(Card source) {
		return this.CopyWithSetValue("Source", source);
	}
}

public static class CardExtensions {
	public static IEnumerable<Card> RemoveLocationCards(this IEnumerable<Card> cards) {
		return cards.Where(card => !card.LocationLimited);
	}

	public static IEnumerable<Card> RemoveSingleCardIdentity(this IEnumerable<Card> cards, 
		Card card) {
		var original = card.Original();
		return cards
			.RemoveFirstWhere(checkedCard => original == checkedCard.Original());
	}

	public static IEnumerable<Card> RemoveSingleCarEquality(this IEnumerable<Card> cards,
	Card card) {
		var original = card.Original();
		return cards
			.RemoveFirstWhere(checkedCard => original.Equals(checkedCard.Original()));
	}
}
