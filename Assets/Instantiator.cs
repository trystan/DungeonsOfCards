using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Instantiator : MonoBehaviour {
	public Canvas Canvas;

	public GameObject CreatureViewPrefab;
	public GameObject CardViewPrefab;
	public GameObject TextViewPrefab;
	public GameObject ItemViewPrefab;

	public CreatureView Add(Creature creature) {
		var view = Instantiate(CreatureViewPrefab);
		view.GetComponent<CreatureView>().Initialize(creature, this);
		return view.GetComponent<CreatureView>();
	}

	public void Remove(CreatureView view) {
		GameObject.Destroy(view.gameObject);
	}

	public ItemView Add(Game game, Item item) {
		var view = Instantiate(ItemViewPrefab);
		view.GetComponent<ItemView>().Initialize(item, game.Player, this);
		return view.GetComponent<ItemView>();
	}

	public void Remove(ItemView view) {
		GameObject.Destroy(view.gameObject);
	}

	public CardView Add(Game game, Card card, Creature holder) {
		var view = Instantiate(CardViewPrefab);
		view.GetComponent<CardView>().Initialize(game, card, holder, this);
		view.transform.SetParent(Canvas.transform);
		return view.GetComponent<CardView>();
	}

	public void Remove(CardView view) {
		GameObject.Destroy(view.gameObject);
	}

	public TextPopupView Add(TextPopup popup) {
		var view = Instantiate(TextViewPrefab);
		view.GetComponent<TextPopupView>().Initialize(popup, this);
		view.transform.SetParent(Canvas.transform);
		return view.GetComponent<TextPopupView>();
	}

	public void Remove(TextPopupView view) {
		GameObject.Destroy(view.gameObject);
	}
}
