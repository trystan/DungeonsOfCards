using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Tile {
	public static Tile OutOfBounds = new Tile { FloorIndex = 8, BlocksMovement = true, IsFloor = false };

	private static int FloorCenter(int x, int y) {
		return x * 7 + 1 + y * 63 + 21;
	}
	public static Tile Floor1 = new Tile { FloorIndex = FloorCenter( 0, 2) };
	public static Tile Floor2 = new Tile { FloorIndex = FloorCenter( 0, 3) };
	public static Tile Floor3 = new Tile { FloorIndex = FloorCenter( 1, 7) };
	public static Tile Floor4 = new Tile { FloorIndex = FloorCenter( 1, 8) };
	public static Tile Floor5 = new Tile { FloorIndex = FloorCenter( 0, 8) };
	public static Tile Floor6 = new Tile { FloorIndex = FloorCenter( 1, 3) };
	public static Tile Wall = new Tile { FloorIndex = 0, BlocksMovement = true, IsFloor = false, IsWall = true };
	public static Tile DoorClosed = new Tile { FloorIndex = FloorCenter(1,8), MiddleIndex = 0, IsFloor = true, IsDoor = true, BlocksMovement = true };
	public static Tile DoorOpen   = new Tile { FloorIndex = FloorCenter(1,8), MiddleIndex = 6, IsFloor = true, IsDoor = true };

	public static Tile StairsDown = new Tile { FloorIndex = FloorCenter(0,1), IsFloor = true };
	public static Tile StairsUp = new Tile { FloorIndex = FloorCenter(0,1), IsFloor = true };

	public int FloorIndex = -1;
	public int MiddleIndex = -1;
	public bool BlocksMovement;
	public bool IsFloor = true;
	public bool IsDoor = false;
	public bool IsWall = false;
}
