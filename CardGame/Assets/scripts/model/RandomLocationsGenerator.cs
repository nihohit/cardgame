using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RandomLocationsGenerator :ILocationsGenerator {
	public IEnumerable<Location> Locations() {
		yield return initialLocation();

		while (true) {
			yield return randomLocation();
		}
	}
	
	private Location initialLocation() {
		return makeLocation("Home Station",
			new Dictionary<LocationContent, double> {
				{LocationContent.FuelStorage, 0.25},
				{LocationContent.Woods, 0.25},
				{LocationContent.SpareMaterials, 0.25},
				{LocationContent.LivingPeople, 0.25}
			});
	}

	private Location randomLocation() {
		Func<Location>[] locationSources = {
			village,
			abandonedFactory,
			abandonedVillage,
			emptyWoods,
			trainWorkshop,
			mountains
		};

		return locationSources[Randomizer.Next(0, locationSources.Length)]();
	}

	private Location village() {
		return makeLocation("Village",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.1},
				{LocationContent.LivingPeople, 0.7},
				{LocationContent.WildAnimals, 0.05},
				{trainCarComponent(), 0.05},
				{LocationContent.LivingQuartersCarComponents, 0.1},
			});
	}
	
	private Location abandonedVillage() {
		return makeLocation("Abandoned Village",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.15},
				{LocationContent.LivingPeople, 0.05},
				{LocationContent.WildAnimals, 0.1},
				{LocationContent.FuelStorage, 0.2},
				{LocationContent.SpareMaterials, 0.2},
				{LocationContent.MinableMaterials, 0.1},
				{LocationContent.WorkhouseCarComponents, 0.1},
				{LocationContent.LivingQuartersCarComponents, 0.1},
			});
	}

	private Location emptyWoods() {
		return makeLocation("Woods",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.5},
				{LocationContent.LivingPeople, 0.15},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.SpareMaterials, 0.05},
				{LocationContent.MinableMaterials, 0.05},
				{trainCarComponent(), 0.1},
			});
	}

	private Location trainWorkshop() {
		var componentsDictionary = trainCarComponentsDictionary(6, 0.11);
		var extraStuffDictionary = new Dictionary<LocationContent, double> {
			{LocationContent.LivingPeople, 0.11},
			{LocationContent.FuelStorage, 0.12},
			{LocationContent.SpareMaterials, 0.11},
		};
		return makeLocation("Train Workshop",
			componentsDictionary.CombineWith(extraStuffDictionary));
	}

	private Location abandonedFactory() {
		return makeLocation("Abandoned Factory",
			new Dictionary<LocationContent, double> {
			{LocationContent.LivingPeople, 0.1},
			{LocationContent.FuelStorage, 0.3},
			{LocationContent.SpareMaterials, 0.3},
			{LocationContent.WorkhouseCarComponents, 0.1},
			{LocationContent.RefineryCarComponents, 0.1},
			{LocationContent.EngineCarComponents, 0.1}
		});
  }

	private Location mountains() {
		return makeLocation("Mountains region",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.1},
				{LocationContent.LivingPeople, 0.15},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.SpareMaterials, 0.05},
				{LocationContent.FuelStorage, 0.05},
				{LocationContent.MinableMaterials, 0.3},
				{trainCarComponent(), 0.2},
			});
	}

	private Location makeLocation(string name, 
		Dictionary<LocationContent, double> dict, 
		int contentCount = 3) {
		return new Location(name, 
			Randomizer.ChooseWeightedValues(dict)
				.Take(contentCount));
	}
	
	private Dictionary<LocationContent, double> trainCarComponentsDictionary(
		int count, 
		double chance) {
		return trainCarComponents(count)
			.ToDictionary(component => component, _ => chance);
	}
	
	private IEnumerable<LocationContent> trainCarComponents(int count) {
		return new[] {
				LocationContent.ArmoryCarComponents,
				LocationContent.CannonCarComponents,
				LocationContent.EngineCarComponents,
				LocationContent.GeneralCarComponents,
				LocationContent.RefineryCarComponents,
				LocationContent.WorkhouseCarComponents,
				LocationContent.LivingQuartersCarComponents
			}.Shuffle()
			.Take(count);
	}
	
	private LocationContent trainCarComponent() {
		return trainCarComponents(1).First();
	}
}