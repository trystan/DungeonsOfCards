using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public static class Util {
	public static List<T> Shuffle<T>(IEnumerable<T> things) {
		var newList = new List<T>();
		var list = things.Select(x => x).ToList();
		while (list.Any()) {
			var i = UnityEngine.Random.Range(0, list.Count);
			newList.Add(list[i]);
			list.RemoveAt(i);
		}
		return newList;
	}

	public static T WeightedChoice<T>(Dictionary<T, int> choices) {
		var i = UnityEngine.Random.Range(0, choices.Values.Sum());
		foreach (var kv in choices) {
			if (i < kv.Value)
				return kv.Key;
			i -= kv.Value;
		}
		Debug.Log("WeightedChoice miss " + i + " / " + choices.Values.Sum());
		return choices.First().Key;
	}
}
