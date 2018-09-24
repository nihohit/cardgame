// Copyright (c) 2018 Shachar Langbeheim. All rights reserved.

using System;
using System.Collections.Generic;
using System.Linq;

public class BaseCollection<T> where T : BaseValueClass {
	public Dictionary<string, T> Items { get; }

	public BaseCollection(Dictionary<string, T> items) {
		Items = items;
	}

	public IEnumerable<T> objectForDictionary(Dictionary<string, int> itemDictionary) {
		return itemDictionary.SelectMany(pair => createCopies(pair.Key, pair.Value));
	}

	public IEnumerable<T> createCopies(string itemName, int copies) {
		var item = Items.Get(itemName);
		for (int i = 0; i < copies; i++) {
			yield return item.ShallowClone<T>();
		}
	}
}
