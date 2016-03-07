﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public GuiController guiController;
	public TileMesh FloorTileMesh;
	public TileMesh WallTileMesh;

	Game game;
	List<CreatureView> CreatureViews = new List<CreatureView>();
	List<CardView> CardViews = new List<CardView>();

	void Start() {
		game = new Game(20, 20) {
			Catalog = new Catalog(),
		};

		new LevelBuilder().Build(game);

		FloorTileMesh.ShowLevel(new FloorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			CreatureViews.Add(Instantiator.Add(c));

		foreach (var c in game.Player.DrawStack)
			CardViews.Add(Instantiator.Add(game, c, game.Player));
		
		Camera.main.GetComponent<CameraController>().Follow(CreatureViews[0].gameObject);

		guiController.Show(game.Player);
	}

	void Update() {
		foreach (var c in game.NewCards)
			CardViews.Add(Instantiator.Add(game, c, game.Player));
		game.NewCards.Clear();

		game.Effects.ForEach(e => e.Delay -= Time.deltaTime);
		foreach (var e in game.Effects.Where(e => e.Delay < 0).ToList()) {
			e.Callback(game);
			game.Effects.Remove(e);
		}

		foreach (var popup in game.Popups)
			Instantiator.Add(popup);
		game.Popups.Clear();

		if (CreatureViews.Any(v => v.IsMoving))
			return;

		if (game.Player.Exists) {
			var mx = 0;
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad6))
				mx = 1;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.Keypad4))
				mx = -1;

			var my = 0;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad8))
				my = 1;
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Keypad2))
				my = -1;
			
			var wait = Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Keypad5);

			if (mx != 0 || my != 0 || wait) {
				game.Player.MoveBy(game, mx, my);
				game.TakeTurn();
			}
		} else {
			game.TakeTurn();
		}
	}
}
