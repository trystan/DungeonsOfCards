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

	void Update () {
		DrawLabel.text = "Draw [" + Player.DrawStack.Count + "]";
		AttackLabel.text = "Attack [" + Player.AttackStack.Count + "/" + Player.MaximumAttackCards + "]";
		DefenseLabel.text = "Defense [" + Player.DefenseStack.Count + "/" + Player.MaximumDefenseCards + "]";
		HandLabel.text = "Hand [" + Player.HandStack.Count + "/" + Player.MaximumHandCards + "]";
		DiscardLabel.text = "Discard [" + Player.DiscardStack.Count + "]";
	}

	public void Show(Creature creature) {
		Player = creature;
	}
}
