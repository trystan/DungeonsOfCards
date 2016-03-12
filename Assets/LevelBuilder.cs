using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class LevelBuilder {
	float doorPercentage;
	int roomAttempts;
	int extraConnectionAttempts;
	int enemyCount;
	int commonLootCount;
	int rareLootCount;

	Tile defaultFloor;
	List<Point> corridors = new List<Point>();

	Point stairsUpPosition;
	Point stairsDownPosition;

	Tile RandomFloorTile() {
		return Util.Shuffle(new List<Tile>() { Tile.Floor1, Tile.Floor2, Tile.Floor3, Tile.Floor4, Tile.Floor5, Tile.Floor6 })[0];
	}

	public void Build(Game game, bool atDownStairs) {
		doorPercentage = UnityEngine.Random.value;
		roomAttempts = UnityEngine.Random.Range(20, 40);
		extraConnectionAttempts = UnityEngine.Random.Range(20, 40);
		enemyCount = UnityEngine.Random.Range(8, 14) + game.CurrentLevel;
		commonLootCount = UnityEngine.Random.Range(8, 14) + game.CurrentLevel;
		rareLootCount = UnityEngine.Random.Range(0, 2);

		defaultFloor = RandomFloorTile();

		var roomWidth = UnityEngine.Random.Range(3,5);
		var roomHeight = UnityEngine.Random.Range(3,5);

		game.FloorsUpdated = true;
		game.WallsUpdated = true;
		game.ObjectsUpdated = true;

		var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();
		var rx = (game.Width - roomWidth) / 2;
		var ry = (game.Height - roomHeight) / 2;
		for (var x = 0; x < roomWidth; x++) {
			for (var y = 0; y < roomHeight; y++) {
				game.SetTile(rx + x, ry + y, floor);
			}
		}

		for (var i = 0; i < roomAttempts; i++)
			AddRoom(game, UnityEngine.Random.Range(5,7), UnityEngine.Random.Range(5,7));

		AddSouthConnections(game);
		AddEastConnections(game);
		AddExtraDoors(game);

		CleanupDoors(game);

		AddStairsDownAndStairsUpPosition(game);

		AddCommonLoot(game);
		AddRareLoot(game);

		if (game.Player == null)
			AddPlayer(game);
		else 
			RepositionPlayer(game, atDownStairs);
		
		AddAlly(game);
		AddEnemies(game);
	}

	void AddExtraDoors(Game game) {
		var doorCount = Mathf.RoundToInt(doorPercentage * 5);
		for (var i = 0; i < doorCount; i++) {
			if (corridors.Any()) {
				var p = Util.Shuffle(corridors)[0];
				corridors.Remove(p);
				game.SetTile(p.X, p.Y, Tile.DoorClosed);
			}
		}
	}

	void AddStairsDownAndStairsUpPosition(Game game) {
		var x = -1;
		var y = -1;

		var downPositions = new List<Point>();
		var upPositions = new List<Point>();

		while (downPositions.Count < 5) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
			if (game.GetTile(x,y).IsWall
					|| game.GetTile(x-1,y+1).IsWall 
					|| game.GetTile(x-1,y+0).IsWall 
					|| game.GetTile(x-1,y-1).IsWall 
					|| game.GetTile(x-0,y+1).IsWall 
					|| game.GetTile(x-0,y-1).IsWall 
					|| game.GetTile(x+1,y+1).IsWall 
					|| game.GetTile(x+1,y+0).IsWall 
					|| game.GetTile(x+1,y-1).IsWall 
					|| game.GetCreature(new Point(x,y)) != null 
					|| game.GetItem(new Point (x,y)) != null)
				continue;
			downPositions.Add(new Point(x,y));
		}

		while (upPositions.Count < 5) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
			if (game.GetTile(x,y).IsWall
					|| game.GetTile(x-1,y+1).IsWall
					|| game.GetTile(x-1,y+0).IsWall
					|| game.GetTile(x-1,y-1).IsWall
					|| game.GetTile(x-0,y+1).IsWall
					|| game.GetTile(x-0,y-1).IsWall
					|| game.GetTile(x+1,y+1).IsWall
					|| game.GetTile(x+1,y+0).IsWall
					|| game.GetTile(x+1,y-1).IsWall
					|| game.GetCreature(new Point(x,y)) != null 
					|| game.GetItem(new Point (x,y)) != null)
				continue;
			upPositions.Add(new Point(x,y));
		}

		int longestDistance = 0;
		for (var i = 0; i < upPositions.Count; i++) {
			for (var j = 0; j < downPositions.Count; j++) {
				var dist = upPositions[i].DistanceTo(downPositions[j]);
				if (dist <= longestDistance)
					continue;

				longestDistance = dist;
				stairsUpPosition = upPositions[i];
				stairsDownPosition = downPositions[j];
			}
		}

		game.SetTile(stairsUpPosition.X, stairsUpPosition.Y ,Tile.StairsUp);
		game.SetTile(stairsDownPosition.X, stairsDownPosition.Y ,Tile.StairsDown);
	}

	void CleanupDoors(Game game) {
		for (var x = 0; x < game.Width; x++) {
			for (var y = 0; y < game.Height; y++) {
				if (!game.GetTile(x,y).IsDoor)
					continue;
				
				var openSpaces = 0;
				if (!game.GetTile(x-1,y).IsWall) openSpaces++;
				if (!game.GetTile(x+1,y).IsWall) openSpaces++;
				if (!game.GetTile(x,y-1).IsWall) openSpaces++;
				if (!game.GetTile(x,y+1).IsWall) openSpaces++;

				if (openSpaces > 2) {
					var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();
					game.SetTile(x, y, floor);
				}
			}
		}
	}

	void RepositionPlayer(Game game, bool atDownStairs) {
		game.Player.Position = atDownStairs ? (stairsDownPosition + new Point(0,1)) : (stairsUpPosition - new Point(0,1));
	}

	void AddPlayer(Game game) {
		var p = stairsUpPosition - new Point(0,1);
		var player = game.Catalog.Player(p.X, p.Y);
		game.Player = player;
		game.Creatures.Add(player);
	}

	void AddEnemies(Game game) {
		for (var i = 0; i < enemyCount; i++) {
			var x = -1;
			var y = -1;

			while (game.GetTile(x,y).BlocksMovement 
					|| game.GetTile(x,y) == Tile.StairsDown
					|| game.GetCreature(new Point(x,y)) != null 
					|| game.GetItem(new Point (x,y)) != null) {
				x = UnityEngine.Random.Range(0, game.Width);
				y = UnityEngine.Random.Range(0, game.Height);
			}

			var enemy = game.Catalog.Enemy(x,y);
			if (enemy.TeamName == game.Player.TeamName)
				enemy = game.Catalog.Enemy(x,y);
			
			game.Creatures.Add(enemy);
		}
	}

	void AddAlly(Game game) {
		var x = -1;
		var y = -1;

		while (game.GetTile(x,y).IsWall
				|| game.GetTile(x-1,y+1).IsWall 
				|| game.GetTile(x-1,y+0).IsWall 
				|| game.GetTile(x-1,y-1).IsWall 
				|| game.GetTile(x-0,y+1).IsWall 
				|| game.GetTile(x-0,y-1).IsWall 
				|| game.GetTile(x+1,y+1).IsWall 
				|| game.GetTile(x+1,y+0).IsWall 
				|| game.GetTile(x+1,y-1).IsWall 
				|| game.GetTile(x,y) == Tile.StairsDown
				|| game.GetCreature(new Point(x,y)) != null 
				|| game.GetItem(new Point (x,y)) != null) {
			x = UnityEngine.Random.Range(0, game.Width);
			y = UnityEngine.Random.Range(0, game.Height);
		}

		game.Creatures.Add(game.Catalog.Merchant(x,y));
	}

	void AddCommonLoot(Game game) {
		for (var i = 0; i < commonLootCount; i++) {
			var x = UnityEngine.Random.Range(0, game.Width);
			var y = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(x,y).BlocksMovement || game.GetTile(x,y) == Tile.StairsDown)
				continue;

			var card = Util.Shuffle(new List<Card>() {
				game.Catalog.Card("Gold"),
				game.Catalog.Card("Gold"),
				game.Catalog.Card("Gold"),
				game.Catalog.Card("Attack +1"),
				game.Catalog.Card("Defense +1"),
			})[0];

			game.Items.Add(game.Catalog.CardItem(x, y, card));
		}
	}

	void AddAmulet(Game game, int number) {
		var amulet = game.Catalog.AmuletCard(number);

		while (true) {
			var x = UnityEngine.Random.Range(0, game.Width);
			var y = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(x,y).BlocksMovement || game.GetTile(x,y) == Tile.StairsDown)
				continue;
			
			game.Items.Add(game.Catalog.CardItem(x, y, amulet));
			break;
		}
	}

	void AddRareLoot(Game game) {
		if (game.CurrentLevel == 5)
			AddAmulet(game, 0);
		else if (game.CurrentLevel == 7)
			AddAmulet(game, 1);
		else if (game.CurrentLevel == 9)
			AddAmulet(game, 2);

		for (var i = 0; i < rareLootCount; i++) {
			var x = UnityEngine.Random.Range(0, game.Width);
			var y = UnityEngine.Random.Range(0, game.Height);

			if (game.GetTile(x,y).BlocksMovement || game.GetTile(x,y) == Tile.StairsDown)
				continue;

			var pack = Util.Shuffle(new List<Pack>() {
				game.Catalog.GoldPack(),
				game.Catalog.BasicPack(),
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
		for (var i = 0; i < extraConnectionAttempts; i++) {
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
				var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();
				for (var l = 0; l < length; l++) {
					game.SetTile(cx,cy+l,floor);
					corridors.Add(new Point(cx, cy+l));
				}
			}
		}
	}

	void AddEastConnections(Game game) {
		for (var i = 0; i < extraConnectionAttempts; i++) {
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
				var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();
				for (var l = 0; l < length; l++) {
					game.SetTile(cx+l,cy,floor);
					corridors.Add(new Point(cx+l, cy));
				}
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
		var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();

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
		var floor = UnityEngine.Random.value < 0.9f ? defaultFloor : RandomFloorTile();
		var thisFloor = UnityEngine.Random.value < doorPercentage ? Tile.DoorClosed : floor;
		game.SetTile(candidate.X, candidate.Y, thisFloor);
	}
}
