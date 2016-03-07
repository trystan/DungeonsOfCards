using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class TextPopup {
	public float TTL = 1f;
	public string Text;
	public Vector3 WorldPosition;
	public Vector3 Offset = new Vector3(0,0.5f,0);

	public TextPopup(string text, Point worldPosition, Vector3 offset) {
		Text = text;
		WorldPosition = new Vector3(worldPosition.X, worldPosition.Y, 0);
		Offset = offset;
	}
}
