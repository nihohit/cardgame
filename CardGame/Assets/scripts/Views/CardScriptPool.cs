// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CardScriptPool {
	private List<CardScript> cardPool;

	public CardScriptPool(GameObject cardPrefab, int sizeOfPool) {
		cardPool = Enumerable.Range(0, sizeOfPool)
			.Select(_ => GameObject.Instantiate<GameObject>(cardPrefab).GetComponent<CardScript>())
			.ToList();
		foreach(var card in cardPool) {
			card.gameObject.SetActive(false);
		}
	}

	public CardScript CardForModel(CardDisplayModel cardModel) {
		var cardScript = cardPool[0];
		cardScript.gameObject.SetActive(true);
		cardPool.Remove(cardScript);
		cardScript.CardModel = cardModel;
		return cardScript;
	}

	public void ReleaseCard(CardScript card) {
		card.gameObject.SetActive(false);
		cardPool.Add(card);
	}
}
