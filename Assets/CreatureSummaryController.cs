using UnityEngine;
using System.Collections;

public class CreatureSummaryController : MonoBehaviour {
	Creature Creature;

	// Update is called once per frame
	void Update () {
	
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show(Creature creature) {
		gameObject.SetActive(true);
		Creature = creature;
	}
}
