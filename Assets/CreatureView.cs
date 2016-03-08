using UnityEngine;
using System.Collections;
using System.Linq;
using UnityEngine.UI;

public class CreatureView : MonoBehaviour {
	public SpriteRenderer CreatureSprite;
	public Transform HealthBackground;
	public Transform HealthBar;
	public Text NameLabel;

	Instantiator Instantiator;
	Creature Creature;

	Vector3 targetPosition;
	float speed;

	public bool IsMoving;

	void Start() {
	
	}
	
	void Update() {
		if (Creature.Exists) {
			if (Creature.CurrentHealth == Creature.MaximumHealth) {
				HealthBackground.gameObject.SetActive(false);
				HealthBar.gameObject.SetActive(false);
			} else {
				HealthBackground.gameObject.SetActive(true);
				HealthBar.gameObject.SetActive(true);
				HealthBackground.localScale = new Vector3(12,1,1);
				var percent = Creature.CurrentHealth * 1f / Creature.MaximumHealth;
				HealthBar.localScale = new Vector3(percent * 12,1,1);
				HealthBar.localPosition = new Vector3((1-percent) * -0.375f, 0.5f, 0);
			}

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

			NameLabel.transform.position = Camera.main.WorldToScreenPoint(transform.position) - new Vector3(0,8,0);
		} else
			Instantiator.Remove(this);
	}

	public void Initialize(Creature creature, Instantiator instantiator) {
		Creature = creature;
		Instantiator = instantiator;

		NameLabel.text = creature.TeamName;
		NameLabel.enabled = creature.TeamName == "Merchant";

		var parts = creature.SpriteName.Split(':');
		var sprites = Resources.LoadAll<Sprite>(parts[0]);
		CreatureSprite.sprite = sprites.Single(s => s.name == parts[1]);

		transform.position = new Vector3(creature.Position.X, creature.Position.Y, 0);
	}
}
