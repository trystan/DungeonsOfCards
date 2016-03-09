using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelBuilder {
	
	public void Build(Game game) {
		var roomWidth = UnityEngine.Random.Range(3,5);
		var roomHeight = UnityEngine.Random.Range(3,5);

		game.FloorsUpdated = true;
		game.WallsUpdated = true;
		game.ObjectsUpdated = true;

		var rx = (game.Width - roomWidth) / 2;
		var ry = (game.Height - roomHeight) / 2;
		var floor = Tile.RandomFloor();
		for (var x = 0; x < roomWidth; x++) {
			for (var y = 0; y < roomHeight; y++) {
				game.SetTile(rx + x, ry + y, floor);
			}
		}

		for (var i = 0; i < 30; i++)
			AddRoom(game, UnityEngine.Random.Range(5,7), UnityEngine.Random.Range(5,7));

		AddSouthConnections(game);
		AddEastConnections(game);

		CleanupDoors(game);

		AddStairsDown(game);

		AddCommonLoot(game);
		AddRareLoot(game);

		if (game.Player == null)
			AddPlayer(game);
		else 
			RepositionPlayer(game);
		
		AddAlly(game);
		AddEnemies(game);

		if (game.Player == null)
			game.Player = game.Creatures[0];
	}

	void AddStairsDown(Game game) {
		var x = -1;
		var y = -1;

		while (game.GetTile(x,y).BlocksMovement
			|| game.GetTile(x-1,y+1).BlocksMovement 
			|| game.GetTile(x-1,y+0).BlocksMovement 
			|| game.GetTile(x-1,y-1).BlocksMovement 
			|| game.GetTile(x-0,y+1).BlocksMovement 
			|| game.GetTile(x-0,y-1).BlocksMovement 
			|| game.GetTile(x+1,y+1).BlocksMovement 
			|| game.GetTile(x+1,y+0).BlocksMovement 
			|| game.GetTile(x+1,y-1).BlocksMovement 
			|| game.GetCreature(new Point(x,y)) != null 
			|| game.GetItem(new Point (x,y)) != null) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
		}

		game.SetTile(x,y,Tile.StairsDown);
	}

	void CleanupDoors(Game game) {
		for (var x = 0; x < game.Width; x++) {
			for (var y = 0; y < game.Height; y++) {
				if (!game.GetTile(x,y).IsDoor)
					continue;
				
				var openSpaces = 0;
				if (!game.GetTile(x-1,y).BlocksMovement) openSpaces++;
				if (!game.GetTile(x+1,y).BlocksMovement) openSpaces++;
				if (!game.GetTile(x,y-1).BlocksMovement) openSpaces++;
				if (!game.GetTile(x,y+1).BlocksMovement) openSpaces++;

				if (openSpaces > 2)
					game.SetTile(x, y, Tile.Floor2);
			}
		}
	}
	void RepositionPlayer(Game game) {
		var x = -1;
		var y = -1;

		while (game.GetTile(x,y).BlocksMovement
			|| game.GetTile(x,y) == Tile.StairsDown
			|| game.GetCreature(new Point(x,y)) != null 
			|| game.GetItem(new Point (x,y)) != null) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
		}

		game.Player.Position = new Point(x,y);
	}

	void AddPlayer(Game game) {
		var x = -1;
		var y = -1;

		while (game.GetTile(x,y).BlocksMovement 
				|| game.GetTile(x,y) == Tile.StairsDown
				|| game.GetCreature(new Point(x,y)) != null 
				|| game.GetItem(new Point (x,y)) != null) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
		}

		game.Creatures.Add(game.Catalog.Player(x,y));
	}

	void AddEnemies(Game game) {
		for (var i = 0; i < 8; i++) {
			var x = -1;
			var y = -1;

			while (game.GetTile(x,y).BlocksMovement 
					|| game.GetTile(x,y) == Tile.StairsDown
					|| game.GetCreature(new Point(x,y)) != null 
					|| game.GetItem(new Point (x,y)) != null) {
				x = UnityEngine.Random.Range(0, game.Width);
				y = UnityEngine.Random.Range(0, game.Height);
			}

			game.Creatures.Add(game.Catalog.Enemy(x,y));
		}
	}

	void AddAlly(Game game) {
		var x = -1;
		var y = -1;

		while (game.GetTile(x,y).BlocksMovement
				|| game.GetTile(x-1,y+1).BlocksMovement 
				|| game.GetTile(x-1,y+0).BlocksMovement 
				|| game.GetTile(x-1,y-1).BlocksMovement 
				|| game.GetTile(x-0,y+1).BlocksMovement 
				|| game.GetTile(x-0,y-1).BlocksMovement 
				|| game.GetTile(x+1,y+1).BlocksMovement 
				|| game.GetTile(x+1,y+0).BlocksMovement 
				|| game.GetTile(x+1,y-1).BlocksMovement 
				|| game.GetTile(x,y) == Tile.StairsDown
				|| game.GetCreature(new Point(x,y)) != null 
				|| game.GetItem(new Point (x,y)) != null) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
		}

		game.Creatures.Add(game.Catalog.Merchant(x,y));
	}

	void AddCommonLoot(Game game) {
		for (var i = 0; i < 20; i++) {
			var x = UnityEngine.Random.Range(0, game.Width);
			var y = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(x,y).BlocksMovement || game.GetTile(x,y) == Tile.StairsDown)
				continue;

			var card = Util.Shuffle(new List<Card>() {
				new Card() { Name = "Gold", CardType = CardType.Normal },
				new Card() { Name = "Gold", CardType = CardType.Normal },
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Defense +1", CardType = CardType.Normal, CombatBonus = 1 },
			})[0];

			game.Items.Add(game.Catalog.CardItem(x, y, card));
		}
	}

	void AddRareLoot(Game game) {
		for (var i = 0; i < 1; i++) {
			var x = UnityEngine.Random.Range(0, game.Width);
			var y = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(x,y).BlocksMovement || game.GetTile(x,y) == Tile.StairsDown)
				continue;

			var pack = Util.Shuffle(new List<Pack>() {
				game.Catalog.AdventurerPack(),
				game.Catalog.AttackPack(),
				game.Catalog.DefensePack(),
				game.Catalog.PriestPack(),
				game.Catalog.RoguePack(),
				game.Catalog.WizardPack(),
			})[0];
			game.Items.Add(game.Catalog.PackItem(x, y, pack));
		}
	}

	void AddSouthConnections(Game game) {
		var floor = Tile.RandomFloor();
		for (var i = 0; i < 20; i++) {
			var cx = UnityEngine.Random.Range(0, game.Width);
			var cy = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(cx,cy).BlocksMovement)
				continue;

			while (game.GetTile(cx,cy).IsFloor)
				cy++;

			var length = 0;
			while (game.GetTile(cx,cy+length) == Tile.Wall)
				length++;

			if (game.GetTile(cx,cy+length).IsFloor && length > 1 && length < 8) {
				for (var l = 0; l < length; l++)
					game.SetTile(cx,cy+l,floor);
			}
		}
	}

	void AddEastConnections(Game game) {
		var floor = Tile.RandomFloor();
		for (var i = 0; i < 20; i++) {
			var cx = UnityEngine.Random.Range(0, game.Width);
			var cy = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(cx,cy).BlocksMovement)
				continue;

			while (game.GetTile(cx,cy).IsFloor)
				cx++;

			var length = 0;
			while (game.GetTile(cx+length,cy) == Tile.Wall)
				length++;

			if (game.GetTile(cx+length,cy).IsFloor && length > 1 && length < 8) {
				for (var l = 0; l < length; l++)
					game.SetTile(cx+l,cy,floor);
			}
		}
	}

	void AddRoom(Game game, int roomWidth, int roomHeight) {
		var candidates = new List<Point>();
		for (var x = 0; x < game.Width; x++) {
			for (var y = 0; y < game.Height; y++) {
				var ok = false;
				for (var xo = 1; xo < roomWidth-1; xo++) {
					if (game.GetTile(x + xo, y - 1).IsFloor)
						ok = true;
				}
				for (var xo = 1; xo < roomWidth-1; xo++) {
					if (game.GetTile(x + xo, y + roomWidth).IsFloor)
						ok = true;
				}
				for (var yo = 1; yo < roomHeight-1; yo++) {
					if (game.GetTile(x - 1, y + yo).IsFloor)
						ok = true;
				}
				for (var yo = 1; yo < roomHeight-1; yo++) {
					if (game.GetTile(x + roomWidth, y + yo).IsFloor)
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
		if (candidates.Any())
			AddRoom(game, roomWidth, roomHeight, candidates[UnityEngine.Random.Range(0, candidates.Count)]);
	}

	void AddRoom(Game game, int roomWidth, int roomHeight, Point position) {
		var nDoorCandidates = new List<Point>();
		var sDoorCandidates = new List<Point>();
		var wDoorCandidates = new List<Point>();
		var eDoorCandidates = new List<Point>();

		var floor = Tile.RandomFloor();
		for (var xo = 0; xo < roomWidth; xo++) {
			for (var yo = 0; yo < roomHeight; yo++) {
				if (xo == 0 && yo == 0 || xo == 0 && yo == roomHeight - 1 
					|| xo == roomWidth - 1 && yo == 0 || xo == roomWidth - 1 && yo == roomHeight - 1)
					continue;

				if (xo == 0 || yo == 0 || xo == roomWidth - 1 || yo == roomHeight - 1) {
					game.SetTile(position.X + xo, position.Y + yo, Tile.Wall);

					if (yo == 0 && game.GetTile(position.X + xo, position.Y + yo - 1).IsFloor)
						sDoorCandidates.Add(new Point(position.X + xo, position.Y + yo));
					if (yo == roomHeight-1 && game.GetTile(position.X + xo, position.Y + yo + 1).IsFloor)
						sDoorCandidates.Add(new Point(position.X + xo, position.Y + yo));
					if (xo == 0 && game.GetTile(position.X + xo - 1, position.Y + yo).IsFloor)
						wDoorCandidates.Add(new Point(position.X + xo, position.Y + yo));
					if (xo == roomWidth-1 && game.GetTile(position.X + xo + 1, position.Y + yo).IsFloor)
						eDoorCandidates.Add(new Point(position.X + xo, position.Y + yo));
				} else {
					game.SetTile(position.X + xo, position.Y + yo, floor);
				}
			}
		}

		if (nDoorCandidates.Any()) {
			AddDoor(game, nDoorCandidates[UnityEngine.Random.Range(0, nDoorCandidates.Count)]);
			if (UnityEngine.Random.value < 0.25f)
				AddDoor(game, nDoorCandidates[UnityEngine.Random.Range(0, nDoorCandidates.Count)]);
		}

		if (sDoorCandidates.Any()) {
			AddDoor(game, sDoorCandidates[UnityEngine.Random.Range(0, sDoorCandidates.Count)]);
			if (UnityEngine.Random.value < 0.25f)
				AddDoor(game, sDoorCandidates[UnityEngine.Random.Range(0, sDoorCandidates.Count)]);
		}

		if (wDoorCandidates.Any()) {
			AddDoor(game, wDoorCandidates[UnityEngine.Random.Range(0, wDoorCandidates.Count)]);
			if (UnityEngine.Random.value < 0.25f)
				AddDoor(game, wDoorCandidates[UnityEngine.Random.Range(0, wDoorCandidates.Count)]);
		}

		if (eDoorCandidates.Any()) {
			AddDoor(game, eDoorCandidates[UnityEngine.Random.Range(0, eDoorCandidates.Count)]);
			if (UnityEngine.Random.value < 0.25f)
				AddDoor(game, eDoorCandidates[UnityEngine.Random.Range(0, eDoorCandidates.Count)]);
		}
	}

	void AddDoor(Game game, Point candidate) {
		var floor = UnityEngine.Random.value < 0.33f ? Tile.DoorClosed : Tile.Floor2;
		game.SetTile(candidate.X, candidate.Y, floor);
	}
}
