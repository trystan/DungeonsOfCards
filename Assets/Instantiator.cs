using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Instantiator : MonoBehaviour {
	public GameObject CreatureViewPrefab;

	public CreatureView Add(Creature creature) {
		var view = Instantiate(CreatureViewPrefab);
		view.GetComponent<CreatureView>().Initialize(creature, this);
		return view.GetComponent<CreatureView>();
	}

	public void Remove(CreatureView view) {
		GameObject.Destroy(view.gameObject);
	}
}
