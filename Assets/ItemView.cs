using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class ItemView : MonoBehaviour {
	public SpriteRenderer ItemSprite;
	public Text NameLabel;

	Instantiator Instantiator;
	Item Item;
	Creature Player;

	void Update() {
		if (!Item.Exists)
			Instantiator.Remove(this);
		else if (!Player.Exists || Player.Position.DistanceTo(Item.Position) < 3) {
			NameLabel.gameObject.SetActive(true);
			NameLabel.transform.position = Camera.main.WorldToScreenPoint(new Vector3(Item.Position.X, Item.Position.Y, 0)) + new Vector3(0,8,0);
		} else {
			NameLabel.gameObject.SetActive(false);
		}
	}

	public void Initialize(Item item, Creature player, Instantiator instantiator) {
		Item = item;
		Instantiator = instantiator;
		Player = player;

		NameLabel.gameObject.SetActive(false);
		NameLabel.text = item.Card != null ? item.Card.Name : item.Pack.Name;

		var parts = item.SpriteName.Split(':');
		var sprites = Resources.LoadAll<Sprite>(parts[0]);
		ItemSprite.sprite = sprites.Single(s => s.name == parts[1]);

		transform.position = new Vector3(item.Position.X, item.Position.Y, 0);
	}
}
