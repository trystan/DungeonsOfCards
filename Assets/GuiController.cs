using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class GuiController : MonoBehaviour {
	public Text DrawLabel;
	public Text HandLabel;
	public Text AttackLabel;
	public Text DefenseLabel;
	public Text DiscardLabel;
	public Image FadeToBlack;
	public Text FadeText;

	public GameObject AlivePanel;
	public GameObject DeadPanel;

	bool fadeOut = false;
	bool fadeIn = false;
	float fadeCounter = 1f;
	Action fadeCallback;
	Game game;

	void Update () {
		if (game.Player != null) {
			DrawLabel.text = "Draw (" + game.Player.DrawPile.Count + ")";
			AttackLabel.text = "Attack " + game.Player.AttackValue + " + (" + game.Player.AttackPile.Count + "/" + game.Player.MaximumAttackCards + ")";
			DefenseLabel.text = "Defense " + game.Player.DefenseValue + " + (" + game.Player.DefensePile.Count + "/" + game.Player.MaximumDefenseCards + ")";
			HandLabel.text = "Hand (" + game.Player.HandPile.Count + "/" + game.Player.MaximumHandCards + ")";
			DiscardLabel.text = "Discard (" + game.Player.DiscardPile.Count + ")";
		}

		if (game.Player != null && !game.Player.Exists) {
			AlivePanel.SetActive(false);
			DeadPanel.SetActive(true);
		}

		if (fadeOut) {
			fadeCounter -= Time.deltaTime;
			if (fadeCounter <= 0) {
				fadeOut = false;
				fadeIn = true;
				fadeCounter = 1f;
				fadeCallback();
				FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, 1);
				FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, 1);
			} else {
				FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, (1 - fadeCounter) * 2);
				FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, (1 - fadeCounter) * 2);
			}
		} else if (fadeIn) {
			fadeCounter -= Time.deltaTime;
			if (fadeCounter <= 0) {
				fadeIn = false;
				FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, 0);
				FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, 0);
				FadeToBlack.gameObject.SetActive(false);
			} else {
				FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, fadeCounter * 2);
				FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, fadeCounter * 2);
			}
		}
	}

	public void Show(Game game) {
		this.game = game;
		AlivePanel.SetActive(true);
		DeadPanel.SetActive(false);
	}

	public void FadeIn(string text, Action callback) {
		FadeToBlack.gameObject.SetActive(true);
		FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, 1);
		FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, 1);
		FadeText.text = text;
		fadeCallback = callback;
		fadeOut = false;
		fadeIn = true;
		fadeCounter = 1f;
		callback();
	}

	public void FadeOutAndIn(string text, Action callback) {
		FadeToBlack.gameObject.SetActive(true);
		FadeToBlack.color = new Color(FadeToBlack.color.r, FadeToBlack.color.g, FadeToBlack.color.b, 0);
		FadeText.color = new Color(FadeText.color.r, FadeText.color.g, FadeText.color.b, 0);
		FadeText.text = text;
		fadeCallback = callback;
		fadeOut = true;
		fadeIn = false;
		fadeCounter = 1f;
	}
}
