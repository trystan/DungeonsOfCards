using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Tile {
	public static Tile OutOfBounds = new Tile { FloorIndex = 8, BlocksMovement = true, IsFloor = false };

	public static Tile Floor1 = new Tile { FloorIndex = 21 *  3 + 22 };
	public static Tile Floor2 = new Tile { FloorIndex = 21 *  6 + 22 };
	public static Tile Floor3 = new Tile { FloorIndex = 21 *  9 + 22 };
	public static Tile Floor4 = new Tile { FloorIndex = 21 *  6 + 22 };
	public static Tile Wall = new Tile { FloorIndex = 0, BlocksMovement = true, IsFloor = false };
	public static Tile DoorClosed = new Tile { FloorIndex = Floor2.FloorIndex, MiddleIndex = 0, IsFloor = true, IsDoor = true, BlocksMovement = true };
	public static Tile DoorOpen   = new Tile { FloorIndex = Floor2.FloorIndex, MiddleIndex = 6, IsFloor = true, IsDoor = true };

	public static Tile RandomFloor() {
		return Floor2; // Util.Shuffle(new List<Tile>() { Floor1, Floor2, Floor3, Floor4 })[0];
	}

	public int FloorIndex = -1;
	public int MiddleIndex = -1;
	public bool BlocksMovement;
	public bool IsFloor = true;
	public bool IsDoor = false;
}
