using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public struct Point {
	public int X;
	public int Y;

	public Point(int x, int y) {
		X = x;
		Y = y;
	}

	public int DistanceTo(Point other) {
		var dx = other.X - X;
		var dy = other.Y - Y;
		return Mathf.CeilToInt(Mathf.Sqrt(dx*dx + dy*dy));
	}

	public static Point operator +(Point a, Point b) {
		return new Point(a.X + b.X, a.Y + b.Y);
	}

	public static Point operator -(Point a, Point b) {
		return new Point(a.X - b.X, a.Y - b.Y);
	}

	public static bool operator ==(Point a, Point b) {
		return a.Equals(b);
	}

	public static bool operator !=(Point a, Point b) {
		return !a.Equals(b);
	}

	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Point))
			return false;
		Point other = (Point)obj;
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode() {
		unchecked {
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}
