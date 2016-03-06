using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;

	Game game;
	List<CreatureView> Views = new List<CreatureView>();

	void Start() {
		game = new Game() {
			Catalog = new Catalog(),
		};

		game.Creatures.Add(game.Catalog.Player(1,2));
		game.Creatures.Add(game.Catalog.Player(6,4));
		game.Creatures.Add(game.Catalog.Player(3,8));
		game.Creatures.Add(game.Catalog.Player(4,3));

		foreach (var c in game.Creatures)
			Views.Add(Instantiator.Add(c));
	}

	void Update() {
		
	}
}

public class Catalog {
	public Creature Player(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
		};
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
	public Catalog Catalog;
}