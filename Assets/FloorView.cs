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
