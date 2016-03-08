using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Game {
	public List<Creature> Creatures = new List<Creature>();
	public List<DelayedEffect> Effects = new List<DelayedEffect>();
	public List<TextPopup> Popups = new List<TextPopup>();
	public List<Card> NewCards = new List<Card>();
	public Catalog Catalog;
	public Creature Player;

	public bool Updated;
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
		if (x >= 0 && y >= 0 && x < Width && y < Height) {
			tiles[x,y] = tile;
			Updated = true;
		}
	}

	public void TakeTurn() {
		foreach (var c in Creatures.ToList()) {
			if (c.Exists)
				c.TakeTurn(this);
		}

		Creatures.RemoveAll(c => !c.Exists);
	}
}
