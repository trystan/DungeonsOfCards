using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public TileMesh FloorTileMesh;

	FloorView floorView;
	Game game;
	List<CreatureView> Views = new List<CreatureView>();

	void Start() {
		game = new Game(24, 24) {
			Catalog = new Catalog(),
		};

		var rx = 5;
		var ry = 5;
		for (var x = 0; x < 5; x++)
		for (var y = 0; y < 5; y++) {
			game.SetTile(rx + x, ry + y, Tile.Floor);
		}
		
		game.Creatures.Add(game.Catalog.Player(1,2));
		game.Creatures.Add(game.Catalog.Player(6,4));
		game.Creatures.Add(game.Catalog.Player(3,8));
		game.Creatures.Add(game.Catalog.Player(4,3));
		game.Player = game.Creatures[0];


		floorView = new FloorView(game);
		FloorTileMesh.ShowLevel(floorView);

		foreach (var c in game.Creatures)
			Views.Add(Instantiator.Add(c));
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
			game.Player.MoveBy(mx, my);
		}
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

	public static Point operator +(Point a, Point b) {
		return new Point(a.X + b.X, a.Y + b.Y);
	}
}

public class Creature {
	public Point Position;
	public string SpriteName;

	public void MoveBy(int mx, int my) {
		Position += new Point(mx, my);
	}
}

public class Tile {
	public static Tile OutOfBounds = new Tile { Index = 8 };

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
}

public class FloorView : MonoBehaviour, ITileMeshSource {
	public int Width { get { return game.Width; } }
	public int Height { get { return game.Height; } }
	public bool HasChangedSinceLastRender { get; set; }

	public int GetTileIndex(int x, int y) {
		return game.GetTile(x, y) == Tile.Floor ? 85 : 9;
	}

	Game game;
	public FloorView(Game game) {
		this.game = game;
		HasChangedSinceLastRender = true;
	}
}