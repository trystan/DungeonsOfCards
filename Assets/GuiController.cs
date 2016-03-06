using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class GuiController : MonoBehaviour {
	public Text DrawLabel;
	public Text HandLabel;
	public Text AttackLabel;
	public Text DefenseLabel;
	public Text DiscardLabel;

	Creature Player;

	void Start () {
	
	}

	void Update () {
		DrawLabel.text = "Draw [" + Player.DrawStack.Count + "]";
		AttackLabel.text = "Attack [" + Player.AttackStack.Count + "]";
		DefenseLabel.text = "Defense [" + Player.DefenseStack.Count + "]";
		HandLabel.text = "Hand [" + Player.HandStack.Count + "]";
		DiscardLabel.text = "Discard [" + Player.DiscardStack.Count + "]";
	}

	public void Show(Creature creature) {
		Player = creature;
	}
}
