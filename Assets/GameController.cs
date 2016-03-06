using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public GameObject CreaturePrefab;

	Game game;
	List<CreatureView> Views = new List<CreatureView>();

	void Start() {
		game = new Game();
		game.Creatures.Add(new Creature() {
			Position = new Point(1,1),
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
		});

		game.Creatures.Add(new Creature() {
			Position = new Point(4,2),
			SpriteName = "DawnLike/Characters/Player0:Player0_6",
		});

		foreach (var c in game.Creatures) {
			var view = Instantiate(CreaturePrefab);
			view.GetComponent<CreatureView>().Initialize(c);
			Views.Add(view.GetComponent<CreatureView>());
		}
	}

	void Update() {
		
	}
}

public struct Point {
	public int X;
	public int Y;

	public Point(int x, int y) {
		X = x;
		Y = y;
	}
}

public class Creature {
	public Point Position;
	public string SpriteName;
}

public class Game {
	public List<Creature> Creatures = new List<Creature>();
}