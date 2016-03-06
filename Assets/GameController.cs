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

		var roomWidth = UnityEngine.Random.Range(3,5);
		var roomHeight = UnityEngine.Random.Range(3,5);
		var rx = (game.Width - roomWidth) / 2;
		var ry = (game.Height - roomHeight) / 2;
		for (var x = 0; x < roomWidth; x++)
		for (var y = 0; y < roomHeight; y++) {
			game.SetTile(rx + x, ry + y, Tile.Floor);
		}

		for (var i = 0; i < 30; i++) {
			roomWidth = UnityEngine.Random.Range(5,7);
			roomHeight = UnityEngine.Random.Range(5,7);

			var candidates = new List<Point>();
			for (var x = 0; x < game.Width; x++) {
				for (var y = 0; y < game.Height; y++) {
					var ok = false;
					for (var xo = 1; xo < roomWidth-1; xo++) {
						if (game.GetTile(x + xo, y - 1) == Tile.Floor)
							ok = true;
					}
					for (var xo = 1; xo < roomWidth-1; xo++) {
						if (game.GetTile(x + xo, y + roomWidth) == Tile.Floor)
							ok = true;
					}
					for (var yo = 1; yo < roomHeight-1; yo++) {
						if (game.GetTile(x - 1, y + yo) == Tile.Floor)
							ok = true;
					}
					for (var yo = 1; yo < roomHeight-1; yo++) {
						if (game.GetTile(x + roomWidth, y + yo) == Tile.Floor)
							ok = true;
					}

					if (!ok)
						continue;
					
					for (var xo = 0; xo < roomWidth; xo++) {
						for (var yo = 0; yo < roomHeight; yo++) {
							if (game.GetTile(x + xo, y + yo) != Tile.Wall)
								ok = false;
						}
					}
					if (ok)
						candidates.Add(new Point(x, y));
				}
			}
			if (candidates.Any()) {
				var candidate = candidates[UnityEngine.Random.Range(0, candidates.Count)];
				var nDoorCandidates = new List<Point>();
				var sDoorCandidates = new List<Point>();
				var wDoorCandidates = new List<Point>();
				var eDoorCandidates = new List<Point>();

				for (var xo = 0; xo < roomWidth; xo++) {
					for (var yo = 0; yo < roomHeight; yo++) {
						if (xo == 0 && yo == 0 || xo == 0 && yo == roomHeight - 1 
								|| xo == roomWidth - 1 && yo == 0 || xo == roomWidth - 1 && yo == roomHeight - 1)
							continue;
						
						if (xo == 0 || yo == 0 || xo == roomWidth - 1 || yo == roomHeight - 1) {
							game.SetTile(candidate.X + xo, candidate.Y + yo, Tile.Wall);

							if (yo == 0 && game.GetTile(candidate.X + xo, candidate.Y + yo - 1) == Tile.Floor)
								sDoorCandidates.Add(new Point(candidate.X + xo, candidate.Y + yo));
							if (yo == roomHeight-1 && game.GetTile(candidate.X + xo, candidate.Y + yo + 1) == Tile.Floor)
								sDoorCandidates.Add(new Point(candidate.X + xo, candidate.Y + yo));
							if (xo == 0 && game.GetTile(candidate.X + xo - 1, candidate.Y + yo) == Tile.Floor)
								wDoorCandidates.Add(new Point(candidate.X + xo, candidate.Y + yo));
							if (xo == roomWidth-1 && game.GetTile(candidate.X + xo + 1, candidate.Y + yo) == Tile.Floor)
								eDoorCandidates.Add(new Point(candidate.X + xo, candidate.Y + yo));
						} else {
							game.SetTile(candidate.X + xo, candidate.Y + yo, Tile.Floor);
						}
					}
				}

				if (nDoorCandidates.Any()) {
					var doorCandidate = nDoorCandidates[UnityEngine.Random.Range(0, nDoorCandidates.Count)];
					game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					if (UnityEngine.Random.value < 0.25f) {
						doorCandidate = nDoorCandidates[UnityEngine.Random.Range(0, nDoorCandidates.Count)];
						game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					}
				}

				if (sDoorCandidates.Any()) {
					var doorCandidate = sDoorCandidates[UnityEngine.Random.Range(0, sDoorCandidates.Count)];
					game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					if (UnityEngine.Random.value < 0.25f) {
						doorCandidate = sDoorCandidates[UnityEngine.Random.Range(0, sDoorCandidates.Count)];
						game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					}
				}

				if (wDoorCandidates.Any()) {
					var doorCandidate = wDoorCandidates[UnityEngine.Random.Range(0, wDoorCandidates.Count)];
					game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					if (UnityEngine.Random.value < 0.25f) {
						doorCandidate = wDoorCandidates[UnityEngine.Random.Range(0, wDoorCandidates.Count)];
						game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					}
				}

				if (eDoorCandidates.Any()) {
					var doorCandidate = eDoorCandidates[UnityEngine.Random.Range(0, eDoorCandidates.Count)];
					game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					if (UnityEngine.Random.value < 0.25f) {
						doorCandidate = eDoorCandidates[UnityEngine.Random.Range(0, eDoorCandidates.Count)];
						game.SetTile(doorCandidate.X, doorCandidate.Y, Tile.Floor);
					}
				}
			}
		}

		for (var i = 0; i < 20; i++) {
			var cx = UnityEngine.Random.Range(0, game.Width);
			var cy = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(cx,cy) != Tile.Floor)
				continue;

			while (game.GetTile(cx,cy) == Tile.Floor)
				cy++;

			var length = 0;
			while (game.GetTile(cx,cy+length) == Tile.Wall)
				length++;

			if (game.GetTile(cx,cy+length) == Tile.Floor && length > 1 && length < 8) {
				for (var l = 0; l < length; l++)
					game.SetTile(cx,cy+l,Tile.Floor);
			}
		}

		for (var i = 0; i < 20; i++) {
			var cx = UnityEngine.Random.Range(0, game.Width);
			var cy = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(cx,cy) != Tile.Floor)
				continue;

			while (game.GetTile(cx,cy) == Tile.Floor)
				cx++;

			var length = 0;
			while (game.GetTile(cx+length,cy) == Tile.Wall)
				length++;

			if (game.GetTile(cx+length,cy) == Tile.Floor && length > 1 && length < 8) {
				for (var l = 0; l < length; l++)
					game.SetTile(cx+l,cy,Tile.Floor);
			}
		}
		
		game.Creatures.Add(game.Catalog.Player(1,2));
		game.Creatures.Add(game.Catalog.Player(6,4));
		game.Creatures.Add(game.Catalog.Player(3,8));
		game.Creatures.Add(game.Catalog.Player(4,3));
		game.Player = game.Creatures[0];

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

public class FloorView : ITileMeshSource {
	public int Width { get { return game.Width; } }
	public int Height { get { return game.Height; } }
	public bool HasChangedSinceLastRender { get; set; }

	int rowWidth = 21;
	int columnWidth = 1;
	int center = 85;

	int at(int xdiff, int ydiff) {
		return center + ydiff * rowWidth + xdiff * columnWidth;
	}

	public int GetTileIndex(int x, int y) {
		if (game.GetTile(x, y) == Tile.Floor) {
			var n = game.GetTile(x, y+1) == Tile.Floor;
			var s = game.GetTile(x, y-1) == Tile.Floor;
			var w = game.GetTile(x-1, y) == Tile.Floor;
			var e = game.GetTile(x+1, y) == Tile.Floor;

			if (n && s && w && e)
				return at( 0,  0);

			if (!n && s && w && e)
				return at( 0, -1);
			if (n && !s && w && e)
				return at( 0,  1);
			if (n && s && !w && e)
				return at(-1,  0);
			if (n && s && w && !e)
				return at( 1,  0);

			if (!n && s && w && !e)
				return at( 1, -1);
			if (n && !s && w && !e)
				return at( 1,  1);
			if (!n && s && !w && e)
				return at(-1, -1);
			if (n && !s && !w && e)
				return at(-1,  1);
			
			if (n && s && !w && !e)
				return at( 2,  0);
			if (n && !s && !w && !e)
				return at( 2,  1);
			if (!n && s && !w && !e)
				return at( 2, -1);

			if (!n && !s && w && e)
				return at( 4,  0);
			if (!n && !s && !w && e)
				return at( 3,  0);
			if (!n && !s && w && !e)
				return at( 5,  0);
			
			return at( 4, -1);
		} else {
			return 9;
		}
	}

	Game game;
	public FloorView(Game game) {
		this.game = game;
		HasChangedSinceLastRender = true;
	}
}

public class WallView : ITileMeshSource {
	public int Width { get { return game.Width; } }
	public int Height { get { return game.Height; } }
	public bool HasChangedSinceLastRender { get; set; }

	int rowWidth = 21;
	int columnWidth = 1;
	int center = 85;

	int at(int xdiff, int ydiff) {
		return center + ydiff * rowWidth + xdiff * columnWidth;
	}

	public int GetTileIndex(int x, int y) {
		return game.GetTile(x, y) == Tile.Wall ? 9 : 8;
	}

	Game game;
	public WallView(Game game) {
		this.game = game;
		HasChangedSinceLastRender = true;
	}
}