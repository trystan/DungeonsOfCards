using UnityEngine;
using System.Collections;
using System.Linq;

public class CreatureView : MonoBehaviour {
	public SpriteRenderer CreatureSprite;

	Instantiator Instantiator;
	Creature Creature;

	void Start() {
	
	}
	
	void Update() {
	
	}

	public void Initialize(Creature creature, Instantiator instantiator) {
		Creature = creature;
		Instantiator = instantiator;

		var parts = creature.SpriteName.Split(':');
		var sprites = Resources.LoadAll<Sprite>(parts[0]);
		CreatureSprite.sprite = sprites.Single(s => s.name == parts[1]);

		transform.position = new Vector3(creature.Position.X, creature.Position.Y, 0);
	}
}
