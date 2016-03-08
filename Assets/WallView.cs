using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

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
	