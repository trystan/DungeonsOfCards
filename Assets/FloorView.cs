using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class FloorView : ITileMeshSource {
	public int Width { get { return game.Width; } }
	public int Height { get { return game.Height; } }
	public bool HasChangedSinceLastRender { get; set; }

	int rowWidth = 21;
	int columnWidth = 1;

	int at(int floorIndex, int xdiff, int ydiff) {
		return floorIndex + ydiff * rowWidth + xdiff * columnWidth;
	}

	public int GetTileIndex(int x, int y) {
		var tile = game.GetTile(x, y);

		if (tile.IsFloor) {
			var n = game.GetTile(x, y+1).FloorIndex == tile.FloorIndex;
			var s = game.GetTile(x, y-1).FloorIndex == tile.FloorIndex;
			var w = game.GetTile(x-1, y).FloorIndex == tile.FloorIndex;
			var e = game.GetTile(x+1, y).FloorIndex == tile.FloorIndex;

			if (n && s && w && e)
				return at(tile.FloorIndex, 0,  0);

			if (!n && s && w && e)
				return at(tile.FloorIndex, 0, -1);
			if (n && !s && w && e)
				return at(tile.FloorIndex, 0,  1);
			if (n && s && !w && e)
				return at(tile.FloorIndex,-1,  0);
			if (n && s && w && !e)
				return at(tile.FloorIndex, 1,  0);

			if (!n && s && w && !e)
				return at(tile.FloorIndex, 1, -1);
			if (n && !s && w && !e)
				return at(tile.FloorIndex, 1,  1);
			if (!n && s && !w && e)
				return at(tile.FloorIndex,-1, -1);
			if (n && !s && !w && e)
				return at(tile.FloorIndex,-1,  1);
			
			if (n && s && !w && !e)
				return at(tile.FloorIndex, 2,  0);
			if (n && !s && !w && !e)
				return at(tile.FloorIndex, 2,  1);
			if (!n && s && !w && !e)
				return at(tile.FloorIndex, 2, -1);

			if (!n && !s && w && e)
				return at(tile.FloorIndex, 4,  0);
			if (!n && !s && !w && e)
				return at(tile.FloorIndex, 3,  0);
			if (!n && !s && w && !e)
				return at(tile.FloorIndex, 5,  0);
			
			return at(tile.FloorIndex, 4, -1);
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
