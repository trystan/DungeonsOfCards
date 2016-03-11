using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public GuiController guiController;
	public TileMesh FloorTileMesh;
	public TileMesh DoorTileMesh;
	public TileMesh WallTileMesh;

	public MerchantPanelController merchantPanel;

	Game game;
	List<CreatureView> CreatureViews = new List<CreatureView>();
	List<CardView> CardViews = new List<CardView>();
	List<ItemView> ItemViews = new List<ItemView>();

	CreatureView playerView;

	bool ready = false;

	void Start() {
		guiController.FadeIn("Level 1", NewGame);
	}

	void NewGame() {
		game = new Game(25, 25) {
			Catalog = new Catalog(),
		};
		game.Player = Globals.nextPlayer;
		game.Creatures.Add(game.Player);

		new LevelBuilder().Build(game);

		FloorTileMesh.ShowLevel(new FloorView(game));
		DoorTileMesh.ShowLevel(new DoorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			CreatureViews.Add(Instantiator.Add(c));

		foreach (var item in game.Items)
			ItemViews.Add(Instantiator.Add(game, item));

		foreach (var c in game.Player.DrawStack)
			CardViews.Add(Instantiator.Add(game, c, game.Player));

		playerView = CreatureViews.Single(v => v.Creature == game.Player);
		Camera.main.GetComponent<CameraController>().Follow(playerView.gameObject);

		guiController.Show(game.Player);
		ready = true;
	}

	void Update() {
		if (ready && game.ReadyToLoadNextLevel) {
			ready = false;
			guiController.FadeOutAndIn("Level " + (game.CurrentLevel + 1), () => { 
				game.NextLevel();
				Camera.main.GetComponent<CameraController>().Follow(playerView.gameObject);
				ready = true;
			});
		}

		if (!ready)
			return;
		
		foreach (var c in game.NewCards)
			CardViews.Add(Instantiator.Add(game, c, game.Player));
		game.NewCards.Clear();

		foreach (var item in game.NewItems)
			ItemViews.Add(Instantiator.Add(game, item));
		game.NewItems.Clear();

		foreach (var c in game.NewCreatures)
			CreatureViews.Add(Instantiator.Add(c));
		game.NewCreatures.Clear();

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
			if (merchantPanel.gameObject.activeInHierarchy) {
				// do nothing
			} else if (game.CurrentMerchant != null) {
				merchantPanel.Show(game, game.CurrentMerchant, game.Player);
				game.CurrentMerchant = null;
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
				}
			}
		} else {
			merchantPanel.Hide();
			game.TakeTurn();

			if (Input.GetKey(KeyCode.KeypadEnter) || Input.GetKey(KeyCode.Return))
				UnityEngine.SceneManagement.SceneManager.LoadScene(1);
		}
	}
}
