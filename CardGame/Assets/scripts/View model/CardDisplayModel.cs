// Copyright (c) 2019 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardDisplayModel : BaseValueClass {
	public Card Card { get; }
	public bool Playable { get; }

	public CardDisplayModel(Card card, bool playable) {
		Card = card;
		Playable = playable;
	}
}
