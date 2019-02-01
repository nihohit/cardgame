// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum ScreenLocation { Center, Deck, DiscardPile, Hand1, Hand2, Hand3, Hand4, Hand5, Hand6, Hand7, Hand8, Hand9}

public class CardMovementInstruction: BaseValueClass {
	public ScreenLocation From { get; }
	public ScreenLocation To { get; }
	public CardDisplayModel Card { get; }
	public int NumberOfCardsInHand { get; }

	public CardMovementInstruction (CardDisplayModel card, 
		ScreenLocation from, 
		ScreenLocation to,
		int numberOfCardsInHand) {
		Card = card;
		To = to;
		From = from;
		NumberOfCardsInHand = numberOfCardsInHand;
	}
}
