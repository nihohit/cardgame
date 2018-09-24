// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Tradition :BaseValueClass {
	public string Name { get; }
	public string CardToEnhance { get; }
	public string PropertyToEnhance { get; }
	public int IncreaseInValue { get; }
	public DeckType DeckToAdd { get; }

	public Tradition(string name,
		string cardToEnhance = null,
		string propertyToEnhance = null,
		int increaseInValue = 0,
		DeckType deckToAdd = DeckType.None) {
		Name = name;
		CardToEnhance = cardToEnhance;
		PropertyToEnhance = propertyToEnhance;
		IncreaseInValue = increaseInValue;
		DeckToAdd = deckToAdd;
	}
}
