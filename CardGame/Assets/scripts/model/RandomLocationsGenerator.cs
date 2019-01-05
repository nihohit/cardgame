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
				{LocationContent.Storehouse, 0.25},
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
				{LocationContent.LivingPeople, 0.6},
				{LocationContent.WildAnimals, 0.05},
				{LocationContent.OldHouses, 0.1},
				{LocationContent.Workhouse, 0.1},
				{LocationContent.FuelRefinery, 0.05},
			});
	}
	
	private Location abandonedVillage() {
		return makeLocation("Abandoned Village",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.15},
				{LocationContent.LivingPeople, 0.05},
				{LocationContent.WildAnimals, 0.1},
				{LocationContent.FuelStorage, 0.2},
				{LocationContent.Storehouse, 0.2},
				{LocationContent.Mine, 0.1},
				{LocationContent.Workhouse, 0.1},
				{LocationContent.OldHouses, 0.1},
			});
	}

	private Location emptyWoods() {
		return makeLocation("Woods",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.45},
				{LocationContent.LivingPeople, 0.1},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.Storehouse, 0.05},
				{LocationContent.Mine, 0.05},
				{LocationContent.TrainWreck, 0.1},
				{LocationContent.FuelStorage, 0.1},
			});
	}

	private Location trainWorkshop() {
		return makeLocation("Industrial Zone",
			new Dictionary<LocationContent, double> {
				{LocationContent.LivingPeople, 0.051},
				{LocationContent.FuelStorage, 0.1},
				{LocationContent.Storehouse, 0.2},
				{LocationContent.TrainWreck, 0.05},
				{LocationContent.OldHouses, 0.1},
				{LocationContent.FuelRefinery, 0.2},
				{LocationContent.Workhouse, 0.2},
				{LocationContent.Mine, 0.1},
		});
	}

	private Location abandonedFactory() {
		return makeLocation("Abandoned Factory",
			new Dictionary<LocationContent, double> {
				{LocationContent.LivingPeople, 0.1},
				{LocationContent.FuelStorage, 0.2},
				{LocationContent.Storehouse, 0.3},
				{LocationContent.Workhouse, 0.1},
				{LocationContent.FuelRefinery, 0.1},
				{LocationContent.TrainWreck, 0.1},
				{LocationContent.Howitizer, 0.1}
		});
  }

	private Location mountains() {
		return makeLocation("Mountains region",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.1},
				{LocationContent.LivingPeople, 0.15},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.Storehouse, 0.05},
				{LocationContent.FuelStorage, 0.05},
				{LocationContent.Mine, 0.3},
				{LocationContent.Howitizer, 0.1},
				{LocationContent.FuelRefinery, 0.1}
			});
	}

	private Location makeLocation(string name, 
		Dictionary<LocationContent, double> dict, 
		int contentCount = 3) {
		return new Location(name, 
			Randomizer.ChooseWeightedValues(dict)
				.Take(contentCount));
	}
}