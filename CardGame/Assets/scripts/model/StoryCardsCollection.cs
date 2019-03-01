// Copyright (c) 2019 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StoryCardsCollection {
	private static Dictionary<string, Card> stories = new[] {
		Card.MakeCard("storyTest"),

		// Cave people story
		Card.MakeCard("Cave People",
			exhaustible: true,
			locationLimited: true,
			containedEvent: new EventCard {
				Description = "Your scouts have found some people living in caves in this area. They are friendly, and invite you over to see how they live. A few of them seem willing to join you, and others offer you to investigate deeper into the caves.",
				Options = new[] {
					Card.MakeCard("Raid the cave",
						materialsChange: 3,
						armyChange: -1,
						populationCost: 1),
					Card.MakeCard("Ask them to join",
						populationCost: 1,
						populationChange: 1),
					Card.MakeCard("Delve Deeper",
						populationCost: 1,
						storyCardsToAddToTopOfDeck: "CavePeople2".Yield()),
					Card.MakeCard("Exit the caves")
				}
			}),
		Card.MakeCard("Delve Deeper",
			identifier: "CavePeople2",
			locationLimited: true,
			containedEvent: new EventCard {
				Description = "Going deeper into the caves, you find a vast cave network. Some of these caves have been used in the past and seem to still contain equipment and supplies. Others seem like they were never entered before. Some tunnels lead even deeper underground.",
				Options = new[] {
					Card.MakeCard("Check the used caves",
						storyCardsToAddToHand: new [] { "CavePeople2_Materials", "CavePeople2_Fuel", "CavePeople2_Army"}.Shuffle().Take(1),
						populationCost: 1),
					// TODO: Add something for the virgin caves
					Card.MakeCard("Delve Even Deeper",
						populationCost: 1,
						storyCardsToAddToTopOfDeck: "CavePeople3".Yield()),
					Card.MakeCard("Exit the caves")
				}
			}),
		Card.MakeCard("Found Materials",
			identifier: "CavePeople2_Materials",
			materialsChange: 3,
			exhaustible:true,
			locationLimited: true),
		Card.MakeCard("Found Fuel",
			identifier: "CavePeople2_Fuel",
			fuelChange: 2,
			exhaustible:true,
			locationLimited: true),
		Card.MakeCard("Found Weapons",
			identifier: "CavePeople2_Army",
			armyChange: 2,
			exhaustible:true,
			locationLimited: true),
		Card.MakeCard("Delve Even Deeper",
			identifier: "CavePeople3",
			containedEvent: new EventCard {
				Name = "Delve Even Deeper",
				Description = "There seems to be absolutely nothing in the depths.",
				Options = Card.MakeCard("Exit the caves").Yield()
			}),

			// Train builders
			Card.MakeCard("Train Builders",
				containedEvent: new EventCard {
					Description = "Some people in the settlement are trying to build a train just like yours, to escape. They have plans for  a train, and have amassed some of the materials. They ask for your help in building the train, and offer to share their schematics if you'll help.",
					Options = new[] {
						Card.MakeCard("Ignore them"),
						Card.MakeCard("Help them",
							materialsChange: -3,
							populationCost: 1,
							storyCardsToAddToTopOfDeck:new []{
								"TrainBuildersSchematics",
								"TrainBuildersFueling"
							}),
						Card.MakeCard("Raid them",
							armyChange: -2,
							materialsChange: 2,
							fuelChange: 2,
							populationCost: 1,
							modifiedByCar: CarType.Cannon,
							carModifications: new Dictionary<string, int> {
									{ "ArmyChange", 1 },
									{ "MaterialsChange", -1 }
								})
					}
				}),
			Card.MakeCard("Improve Engine",
				identifier: "TrainBuildersSchematics",
				carToRemove: CarType.Engine,
				carToAdd: new TrainCar(-1, CarType.Engine),
				materialsChange: -2,
				populationCost: 1, 
				exhaustible:true),
			// TODO - should have an additional effect.
			Card.MakeCard("Fuel the Train",
				identifier: "TrainBuildersFueling",
				fuelChange: -2,
				populationCost: 1,
				exhaustible:true,
				locationLimited: true),

			// Feral Kids
			Card.MakeCard("Kids on Tracks", 
				identifier:"Kids",
				containedEvent: new EventCard {
					Description = "The train is stopped by a rather strange thing: several teenagers armed with wooden spears are standing on the tracks, blocking them with their bodies.",
					Options = new [] {
						"KidsRunOver",
						"KidsPrisoners",
						"Kids2"
					}.Select(GetCard),
					Blocking = true
				}),

			Card.MakeCard("Run them over", identifier: "KidsRunOver"),
			Card.MakeCard("Take them prisoners",
				identifier: "KidsPrisoners",
				armyChange: -1,
				populationChange: 1),
			Card.MakeCard("Stop and talk to them",
				identifier: "Kids2",
				containedEvent: new EventCard {
					Name = "Talk to the Kids",
					Description = "You leave the train to talk to the grubby teens and understand what they want. Apparently, they think that they can rob the train. They aim their spears at you and try to act menacing.",
					Options = new [] {
						"KidsBribe",
						"KidsAmbush",
						"KidsCooperate"
					}.Select(GetCard),
					Blocking = true
				}),
			Card.MakeCard("Give them something",
				identifier: "KidsBribe",
				materialsChange: -1),
			Card.MakeCard("Force them to show their home",
				identifier:"KidsAmbush",
				armyChange: -1,
				containedEvent: new EventCard {
					Name = "Ambush!",
					Description = "When you threatened them, the kids took your scouts to their base, or so they claimed. Your scouts underestimated the kids, and fell into an ambush.",
					Options = new[] {
						Card.MakeCard("Extract who you can",
							populationChange: -1,
							armyChange: -2,
							modifiedByCar:CarType.Cannon,
							carModifications: new Dictionary<string, int>{
								{ "ArmyChange", 1 }
							}),
						Card.MakeCard("Defend yourselves",
							armyChange: -4,
							modifiedByCar:CarType.Cannon,
							carModifications: new Dictionary<string, int>{
								{ "ArmyChange", 1 }
							}),
						Card.MakeCard("Abandon your scouts",
							populationChange: -2,
							defaultChoice:true),
					}
				}),
			Card.MakeCard("Cooperate",
				identifier:"KidsCooperate",
				materialsChange: -3,
				containedEvent: new EventCard {
					Name = "Cooperate with the Kids",
					Description = "You indulge the kids with the bounty you have. They take everything you offer and run back to their hideout, inviting you over as an afterthought. You find a cave filled wit lost kids of all ages, and they're willing to help you out.",
					Options = new[] {
						Card.MakeCard("Ask for Fuel",
							fuelChange: 2),
						Card.MakeCard("Ask for Volunteers", populationChange: 1),
						Card.MakeCard("Ask for Weapons", armyChange: 2)
					}
				}),
			
			// Examine train wreck

			// Bandit Base
		}.ToDictionary(keySelector: card => card.Identifier);

	public static Card GetCard(string identifier) => stories.Get(identifier);
}
