using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Tile {
	public static Tile OutOfBounds = new Tile { Index = 8, BlocksMovement = true };

	public static Tile Floor = new Tile { Index = 0 };
	public static Tile Wall = new Tile { Index = 1, BlocksMovement = true };

	public int Index;
	public bool BlocksMovement;
}
