using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RandomLocationsGenerator :ILocationsGenerator {
	private bool cavesDone;
	private bool trainDone;
	private bool kidsDone;

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
		var story = trainDone ? null : "Train Builders";
		trainDone = true;
		return makeLocation("Village",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.1},
				{LocationContent.LivingPeople, 0.6},
				{LocationContent.WildAnimals, 0.05},
				{LocationContent.OldHouses, 0.07},
				{LocationContent.Workhouse, 0.07},
				{LocationContent.FuelRefinery, 0.05},
				{LocationContent.ArmyBase, 0.06}
			}, 
			storyEvent: story);
	}
	
	private Location abandonedVillage() {
		return makeLocation("Abandoned Village",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.15},
				{LocationContent.LivingPeople, 0.05},
				{LocationContent.WildAnimals, 0.1},
				{LocationContent.FuelStorage, 0.15},
				{LocationContent.Storehouse, 0.15},
				{LocationContent.Mine, 0.1},
				{LocationContent.Workhouse, 0.1},
				{LocationContent.OldHouses, 0.1},
				{LocationContent.ArmyBase, 0.1}
			});
	}

	private Location emptyWoods() {
		var story = kidsDone ? null : "Kids";
		kidsDone = true;
		return makeLocation("Woods",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.45},
				{LocationContent.LivingPeople, 0.1},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.Storehouse, 0.05},
				{LocationContent.Mine, 0.05},
				{LocationContent.TrainWreck, 0.07},
				{LocationContent.FuelStorage, 0.07},
				{LocationContent.ArmyBase, 0.06}
			},
			storyEvent: story);
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
		var story = cavesDone ? null : "Cave People";
		cavesDone = true;
		return makeLocation("Mountains region",
			new Dictionary<LocationContent, double> {
				{LocationContent.Woods, 0.1},
				{LocationContent.LivingPeople, 0.15},
				{LocationContent.WildAnimals, 0.15},
				{LocationContent.Storehouse, 0.05},
				{LocationContent.FuelStorage, 0.05},
				{LocationContent.Mine, 0.2},
				{LocationContent.Howitizer, 0.1},
				{LocationContent.FuelRefinery, 0.1},
				{LocationContent.ArmyBase, 0.1}
			},
			storyEvent: story);
	}

	private Location makeLocation(string name, 
		Dictionary<LocationContent, double> dict, 
		int contentCount = 3,
		string storyEvent = null) {
		return new Location(name, 
			Randomizer.ChooseWeightedValues(dict)
				.Take(contentCount),
			storyEvent: storyEvent);
	}
}