﻿using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public GuiController guiController;
	public TileMesh FloorTileMesh;
	public TileMesh DoorTileMesh;
	public TileMesh WallTileMesh;
	public SpriteRenderer StairsUpImage;
	public SpriteRenderer StairsDownImage;

	public SpriteRenderer Hint1Image;
	public SpriteRenderer Hint2Image;
	public SpriteRenderer Hint3Image;

	public MerchantPanelController merchantPanel;
	public HintPanelController hintPanel;

	Game game;
	List<CreatureView> CreatureViews = new List<CreatureView>();
	List<CardView> CardViews = new List<CardView>();
	List<ItemView> ItemViews = new List<ItemView>();
	List<Subscription> Subscriptions = new List<Subscription>();

	CreatureView playerView;

	bool ready = false;

	void Start() {
		guiController.FadeIn("Dungeon level 1", NewGame);

		Subscriptions.Add(Globals.MessageBus.On<Messages.NextLevel>(message => {
			ready = false;
			var level = game.CurrentLevel + game.ReadyToLoadNextLevel;
			if (level < 1) {
				guiController.FadeOutAndIn("Exiting the dungeon", () => {
					Subscriptions.ForEach(s => s.Remove());
					UnityEngine.SceneManagement.SceneManager.LoadScene(3);
				});
			} else {
				guiController.FadeOutAndIn("Dungeon level " + level, () => { 
					if (game.ReadyToLoadNextLevel > 0)
						game.NextLevel();
					else
						game.PreviousLevel();
					PositionStairs();
					Camera.main.GetComponent<CameraController>().Follow(playerView.gameObject);
					ready = true;
				});
			}
		}));

		Subscriptions.Add(Globals.MessageBus.On<Messages.CreatureAdded>(message => {
			CreatureViews.Add(Instantiator.Add(message.Creature));
		}));
			
		Subscriptions.Add(Globals.MessageBus.On<Messages.ItemAdded>(message => {
			ItemViews.Add(Instantiator.Add(game, message.Item));
		}));

		Subscriptions.Add(Globals.MessageBus.On<Messages.CardAdded>(message => {
			CardViews.Add(Instantiator.Add(game, message.Card, game.Player));
		}));

		Subscriptions.Add(Globals.MessageBus.On<Messages.AddPopup>(message => {
			Instantiator.Add(message.Popup);
		}));

		Subscriptions.Add(Globals.MessageBus.On<Messages.TalkToMerchant>(message => {
			merchantPanel.Show(game, message.Merchant, message.Buyer);
		}));

		Subscriptions.Add(Globals.MessageBus.On<Messages.PlayerChangedToCreature>(message => {
			game.Player = message.Creature;
			game.Player.Ai = new PlayerAi();
			game.Player.DrawPile.ForEach(c => Globals.MessageBus.Send(new Messages.CardAdded(c)));
			game.Player.AttackPile.ForEach(c => Globals.MessageBus.Send(new Messages.CardAdded(c)));
			game.Player.DefensePile.ForEach(c => Globals.MessageBus.Send(new Messages.CardAdded(c)));
			game.Player.HandPile.ForEach(c => Globals.MessageBus.Send(new Messages.CardAdded(c)));
			game.Player.DiscardPile.ForEach(c => Globals.MessageBus.Send(new Messages.CardAdded(c)));
		}));
	}

	void NewGame() {
		game = new Game(25, 25) {
			Catalog = new Catalog(),
		};
		if (Globals.nextPlayer != null) {
			game.Player = Globals.nextPlayer;
			game.Creatures.Add(game.Player);
		}

		new LevelBuilder().Build(game, false);
		Globals.nextPlayer = game.Player;

		FloorTileMesh.ShowLevel(new FloorView(game));
		DoorTileMesh.ShowLevel(new DoorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			CreatureViews.Add(Instantiator.Add(c));

		foreach (var item in game.Items)
			ItemViews.Add(Instantiator.Add(game, item));

		foreach (var c in game.Player.DrawPile)
			CardViews.Add(Instantiator.Add(game, c, game.Player));

		PositionStairs();

		playerView = CreatureViews.Single(v => v.Creature == game.Player);
		Camera.main.GetComponent<CameraController>().Follow(playerView.gameObject);

		guiController.Show(game);
		ready = true;
	}

	void PositionStairs() {
		Hint1Image.transform.position = new Vector3(-99,-99,0);
		Hint2Image.transform.position = new Vector3(-99,-99,0);
		Hint3Image.transform.position = new Vector3(-99,-99,0);
		StairsDownImage.transform.position = new Vector3(-99,-99,0);
		StairsUpImage.transform.position = new Vector3(-99,-99,0);
		for (var x = 0; x < game.Width; x++) {
			for (var y = 0; y < game.Height; y++) {
				var tile = game.GetTile(x,y);
				if (tile == Tile.StairsDown) {
					StairsDownImage.transform.position = new Vector3(x,y,0);
				} else if (tile == Tile.StairsUp) {
					StairsUpImage.transform.position = new Vector3(x,y,0);
				}
			}
		}

		if (game.Hints.Count > 0)
			Hint1Image.transform.position = new Vector3(game.Hints.ElementAt(0).Key.X, game.Hints.ElementAt(0).Key.Y, 0);
		if (game.Hints.Count > 1)
			Hint2Image.transform.position = new Vector3(game.Hints.ElementAt(1).Key.X, game.Hints.ElementAt(1).Key.Y, 0);
		if (game.Hints.Count > 2)
			Hint3Image.transform.position = new Vector3(game.Hints.ElementAt(2).Key.X, game.Hints.ElementAt(2).Key.Y, 0);
	}

	void Update() {
		if (!ready)
			return;
		
		game.Effects.ForEach(e => e.Delay -= Time.deltaTime);
		foreach (var e in game.Effects.Where(e => e.Delay < 0).ToList()) {
			e.Callback(game);
			game.Effects.Remove(e);
		}

		if (CreatureViews.Any(v => v.IsMoving))
			return;
		
		if (game.Player.Exists) {
			if (merchantPanel.gameObject.activeInHierarchy) {
				// do nothing
			} else {
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
					if (game.Hints.ContainsKey(game.Player.Position))
						hintPanel.Show(game, game.Hints[game.Player.Position]);
				}
			}
		} else {
			merchantPanel.Hide();
			game.TakeTurn();

			if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return)) {
				Subscriptions.ForEach(s => s.Remove());
				UnityEngine.SceneManagement.SceneManager.LoadScene(3);
			}
		}
	}
}
