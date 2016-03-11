using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Game {
	public List<Creature> Creatures = new List<Creature>();
	public List<Item> Items = new List<Item>();
	public List<DelayedEffect> Effects = new List<DelayedEffect>();
	public List<TextPopup> Popups = new List<TextPopup>();
	public List<Card> NewCards = new List<Card>();
	public List<Item> NewItems = new List<Item>();
	public List<Creature> NewCreatures = new List<Creature>();
	public Catalog Catalog;
	public Creature Player;

	public int CurrentLevel = 1;
	public Creature CurrentMerchant;

	public bool FloorsUpdated;
	public bool WallsUpdated;
	public bool ObjectsUpdated;
	public int ReadyToLoadNextLevel;
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

	void Clear() {
		Creatures.Where(c => c != Player).ToList().ForEach(c => c.Exists = false);
		Items.ForEach(i => i.Exists = false);
		Effects.Clear();
		Popups.Clear();
		NewCards.Clear();
		NewItems.Clear();
		for (var x = 0; x < Width; x++) {
			for (var y = 0; y < Height; y++) {
				tiles[x,y] = Tile.Wall;
			}
		}
	}

	public Creature GetCreature(Point p) {
		return Creatures.FirstOrDefault(c => c.Exists && c.Position == p);
	}

	public Item GetItem(Point p) {
		return Items.FirstOrDefault(i => i.Exists && i.Position == p);
	}

	public Tile GetTile(int x, int y) {
		if (x >= 0 && y >= 0 && x < Width && y < Height)
			return tiles[x,y];
		else
			return Tile.OutOfBounds;
	}

	public void SetTile(int x, int y, Tile tile) {
		if (x >= 0 && y >= 0 && x < Width && y < Height) {
			tiles[x,y] = tile;
			ObjectsUpdated = true;
		}
	}

	public void TakeTurn() {
		foreach (var c in Creatures.ToList()) {
			if (c.Exists)
				c.TakeTurn(this);
		}

		Creatures.RemoveAll(c => !c.Exists);
	}

	public void ExitLevelDownStairs(Creature creature) {
		creature.StairsDownCounter++;
		if (creature == Player) {
			ReadyToLoadNextLevel = 1;
		} else {
			creature.Exists = false;
		}
	}

	public void ExitLevelUpStairs(Creature creature) {
		creature.StairsUpCounter++;
		if (creature == Player) {
			ReadyToLoadNextLevel = -1;
		} else {
			creature.Exists = false;
		}
	}

	public void PreviousLevel() {
		CurrentLevel--;
		ReadyToLoadNextLevel = 0;
		Clear();
		new LevelBuilder().Build(this, true);
		NewItems.AddRange(Items);
		NewCreatures.AddRange(Creatures);
	}

	public void NextLevel() {
		CurrentLevel++;
		ReadyToLoadNextLevel = 0;
		Clear();
		new LevelBuilder().Build(this, false);
		Player.DeepestFloor = Mathf.Max(Player.DeepestFloor, CurrentLevel);
		NewItems.AddRange(Items);
		NewCreatures.AddRange(Creatures);
	}
}
