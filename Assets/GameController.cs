using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public GuiController guiController;
	public TileMesh FloorTileMesh;
	public TileMesh WallTileMesh;

	Game game;
	List<CreatureView> CreatureViews = new List<CreatureView>();
	List<CardView> CardViews = new List<CardView>();

	void Start() {
		game = new Game(20, 20) {
			Catalog = new Catalog(),
		};

		new LevelBuilder().Build(game);

		FloorTileMesh.ShowLevel(new FloorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			CreatureViews.Add(Instantiator.Add(c));

		foreach (var c in game.Player.DrawStack)
			CardViews.Add(Instantiator.Add(c, game.Player));
		
		Camera.main.GetComponent<CameraController>().Follow(CreatureViews[0].gameObject);

		guiController.Show(game.Player);
	}

	void Update() {
		if (CreatureViews.Any(v => v.IsMoving))
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
	public List<Card> TestCards() {
		return Util.Shuffle(new List<Card>() {
			new Card() { Name = "Attack +1", CardType = CardType.Attack },
			new Card() { Name = "Attack +2", CardType = CardType.Attack },
			new Card() { Name = "Attack +2", CardType = CardType.Attack },
			new Card() { Name = "Attack +3", CardType = CardType.Attack },
			new Card() { Name = "Defense +1", CardType = CardType.Defense },
			new Card() { Name = "Defense +2", CardType = CardType.Defense },
			new Card() { Name = "Defense +2", CardType = CardType.Defense },
			new Card() { Name = "Defense +3", CardType = CardType.Defense },
			new Card() { Name = "Gold", CardType = CardType.Normal },
			new Card() { Name = "Gold", CardType = CardType.Normal },
			new Card() { Name = "Gold", CardType = CardType.Normal },
			new Card() { Name = "Arrow", CardType = CardType.Normal },
			new Card() { Name = "Arrow", CardType = CardType.Normal },
			new Card() { Name = "Heal", CardType = CardType.Normal },
			new Card() { Name = "Poisoned", CardType = CardType.Normal },
			new Card() { Name = "Poisoned", CardType = CardType.Normal },
		});
	}

	public Creature Player(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new PlayerAi(),
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
			AttackValue = 2,
			MaximumAttackCards = 2,
			DefenseValue = 2,
			MaximumDefenseCards = 2,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
			DrawStack = TestCards(),
		};
	}

	public Creature Skeleton(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new CreatureAi(),
			SpriteName = "DawnLike/Characters/Undead0:skeleton",
			AttackValue = 1,
			MaximumAttackCards = 1,
			DefenseValue = 1,
			MaximumDefenseCards = 1,
			MaximumHealth = 5,
			CurrentHealth = 5,
			MaximumHandCards = 3,
			DrawStack = TestCards(),
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

public enum CardType {
	Attack, Defense, Normal
}

public class Card {
	public string Name;
	public CardType CardType;
}

public class Creature {
	public bool Exists = true;
	public Point Position;
	public string SpriteName;
	public AI Ai;

	public int AttackValue;
	public int MaximumAttackCards;
	public int DefenseValue;
	public int MaximumDefenseCards;
	public int MaximumHealth;
	public int CurrentHealth;
	public int MaximumHandCards;

	public List<Card> DrawStack = new List<Card>();
	public List<Card> AttackStack = new List<Card>();
	public List<Card> DefenseStack = new List<Card>();
	public List<Card> HandStack = new List<Card>();
	public List<Card> DiscardStack = new List<Card>();

	public void MoveBy(Game game, int mx, int my) {
		var next = Position + new Point(mx, my);

		if (game.GetTile(Position.X + mx, Position.Y + my).BlocksMovement) {
			EndTurn();
			return;
		}
		
		var other = game.GetCreature(next);

		if (other != null && other != this)
			Attack(other);
		else
			Position += new Point(mx, my);

		EndTurn();
	}

	void EndTurn() {
		if (!DrawStack.Any()) {
			if (!DiscardStack.Any()) {
				DiscardStack.AddRange(AttackStack);
				AttackStack.Clear();
				DiscardStack.AddRange(DefenseStack);
				DefenseStack.Clear();
				DiscardStack.AddRange(HandStack);
				HandStack.Clear();
			}
			DrawStack.AddRange(Util.Shuffle(DiscardStack));
			DiscardStack.Clear();
		}

		var pulledCard = DrawStack.Last();
		DrawStack.Remove(pulledCard);

		if (pulledCard.CardType == CardType.Attack) {
			AttackStack.Add(pulledCard);
			while (AttackStack.Count > MaximumAttackCards) {
				var toDiscard = AttackStack[0];
				AttackStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		} else if (pulledCard.CardType == CardType.Defense) {
			DefenseStack.Add(pulledCard);
			while (DefenseStack.Count > MaximumDefenseCards) {
				var toDiscard = DefenseStack[0];
				DefenseStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		} else {
			HandStack.Add(pulledCard);
			while (HandStack.Count > MaximumHandCards) {
				var toDiscard = HandStack[0];
				HandStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		}
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

public static class Util {
	public static List<T> Shuffle<T>(List<T> things) {
		var newList = new List<T>();
		var list = things.Select(x => x).ToList();
		while (list.Any()) {
			var i = UnityEngine.Random.Range(0, list.Count);
			newList.Add(list[i]);
			list.RemoveAt(i);
		}
		return newList;
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
		foreach (var c in Creatures) {
			if (c.Exists)
				c.TakeTurn(this);
		}

		Creatures.RemoveAll(c => !c.Exists);
	}
}
