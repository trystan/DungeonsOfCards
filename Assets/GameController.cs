using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public TileMesh FloorTileMesh;
	public TileMesh WallTileMesh;

	Game game;
	List<CreatureView> Views = new List<CreatureView>();

	void Start() {
		game = new Game(20, 20) {
			Catalog = new Catalog(),
		};

		new LevelBuilder().Build(game);

		FloorTileMesh.ShowLevel(new FloorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			Views.Add(Instantiator.Add(c));

		Camera.main.GetComponent<CameraController>().Follow(Views[0].gameObject);
	}

	void Update() {
		if (Views.Any(v => v.IsMoving))
			return;
		
		var mx = 0;
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad6))
			mx = 1;
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.Keypad4))
			mx = -1;

		var my = 0;
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad8))
			my = 1;
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Keypad2))
			my = -1;
		
		var wait = Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Keypad5);

		if (mx != 0 || my != 0 || wait) {
			game.Player.MoveBy(game, mx, my);
			game.TakeTurn();
		}
	}
}

public class Catalog {
	public Creature Player(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new PlayerAi(),
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
			AttackValue = 2,
			DefenseValue = 2,
			MaximumHealth = 10,
			CurrentHealth = 10,
		};
	}

	public Creature Skeleton(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new CreatureAi(),
			SpriteName = "DawnLike/Characters/Undead0:skeleton",
			AttackValue = 1,
			DefenseValue = 1,
			MaximumHealth = 3,
			CurrentHealth = 3,
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

	public static Point operator +(Point a, Point b) {
		return new Point(a.X + b.X, a.Y + b.Y);
	}

	public static bool operator ==(Point a, Point b) {
		return a.Equals(b);
	}

	public static bool operator !=(Point a, Point b) {
		return !a.Equals(b);
	}

	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Point))
			return false;
		Point other = (Point)obj;
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode() {
		unchecked {
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}

public interface AI {
	void TakeTurn(Game game, Creature creature);
}

public class PlayerAi : AI {
	public void TakeTurn(Game game, Creature creature) {
	}
}

public class CreatureAi : AI {
	public void TakeTurn(Game game, Creature creature) {
		creature.MoveBy(game, UnityEngine.Random.Range(-1,2), UnityEngine.Random.Range(-1,2));
	}
}

public class Creature {
	public bool Exists = true;
	public Point Position;
	public string SpriteName;
	public AI Ai;

	public int AttackValue;
	public int DefenseValue;
	public int MaximumHealth;
	public int CurrentHealth;

	public void MoveBy(Game game, int mx, int my) {
		var next = Position + new Point(mx, my);

		if (game.GetTile(Position.X + mx, Position.Y + my).BlocksMovement)
			return;
		
		var other = game.GetCreature(next);

		if (other != null && other != this)
			Attack(other);
		else
			Position += new Point(mx, my);
	}

	public void TakeTurn(Game game) {
		Ai.TakeTurn(game, this);
	}

	public void Attack(Creature other) {
		var damage = Mathf.Max(1, AttackValue - other.DefenseValue);
		other.TakeDamage(damage);
	}

	public void TakeDamage(int amount) {
		CurrentHealth -= amount;
		if (CurrentHealth <= 0)
			Die();
	}

	public void Die() {
		Exists = false;
	}
}

public class Tile {
	public static Tile OutOfBounds = new Tile { Index = 8, BlocksMovement = true };

	public static Tile Floor = new Tile { Index = 0 };
	public static Tile Wall = new Tile { Index = 1, BlocksMovement = true };

	public int Index;
	public bool BlocksMovement;
}

public class Game {
	public List<Creature> Creatures = new List<Creature>();
	public Catalog Catalog;
	public Creature Player;

	public int Width;
	public int Height;
	Tile[,] tiles;

	public Game(int width, int height) {
		Width = width;
		Height = height;
		tiles = new Tile[width,height];

		for (var x = 0; x < width; x++)
		for (var y = 0; y < height; y++) {
			tiles[x,y] = Tile.Wall;
		}
	}

	public Creature GetCreature(Point p) {
		return Creatures.FirstOrDefault(c => c.Exists && c.Position == p);
	}

	public Tile GetTile(int x, int y) {
		if (x >= 0 && y >= 0 && x < Width && y < Height)
			return tiles[x,y];
		else
			return Tile.OutOfBounds;
	}

	public void SetTile(int x, int y, Tile tile) {
		if (x >= 0 && y >= 0 && x < Width && y < Height)
			tiles[x,y] = tile;
	}

	public void TakeTurn() {
		foreach (var c in Creatures)
			c.TakeTurn(this);

		Creatures.RemoveAll(c => !c.Exists);
	}
}
