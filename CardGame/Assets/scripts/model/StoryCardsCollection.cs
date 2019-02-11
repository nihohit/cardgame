// Copyright (c) 2019 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class StoryCardsCollection {
	private static Dictionary<string, Card> stories = new[] {
			Card.MakeCard("storyTest")
		}.ToDictionary(keySelector: card => card.Identifier);

  public static Card GetCard(string identifier) => stories.Get(identifier);
}
