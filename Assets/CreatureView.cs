using UnityEngine;
using System.Collections;
using System.Linq;

public class CreatureView : MonoBehaviour {
	public SpriteRenderer CreatureSprite;

	Instantiator Instantiator;
	Creature Creature;

	Vector3 targetPosition;
	float speed;

	public bool IsMoving;

	void Start() {
	
	}
	
	void Update() {
		if (IsMoving) {
			transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			IsMoving = Vector3.Distance(transform.position, targetPosition) > 0.001f;
			if (!IsMoving)
				transform.position = targetPosition;
		} else if (transform.position.x != Creature.Position.X || transform.position.y != Creature.Position.Y) {
			IsMoving = true;
			targetPosition = new Vector3(Creature.Position.X, Creature.Position.Y, 0);
			speed = Vector3.Distance(transform.position, targetPosition) * 5f;
		}
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
