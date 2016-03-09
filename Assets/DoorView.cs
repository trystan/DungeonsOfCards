using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class DoorView : ITileMeshSource {
	public int Width { get { return game.Width; } }
	public int Height { get { return game.Height; } }
	public bool HasChangedSinceLastRender { 
		get { return game.ObjectsUpdated; }
		set { game.ObjectsUpdated = value; }
	}

	public int GetTileIndex(int x, int y) {
		var t = game.GetTile(x, y);
		return t.IsDoor ? t.MiddleIndex : 38;
	}

	Game game;
	public DoorView(Game game) {
		this.game = game;
	}
}
