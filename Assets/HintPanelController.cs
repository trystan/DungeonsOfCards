using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HintPanelController : MonoBehaviour {
	public Text label;

	Game Game;
	int startingTurnNumber;

	void Start() {
		Hide();
	}

	void Update() {
		if (startingTurnNumber != Game.TurnNumber || Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Escape))
			Hide();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show(Game game, string hint) {
		gameObject.SetActive(true);
		Game = game;
		startingTurnNumber = game.TurnNumber;
		label.text = hint;
	}
}
