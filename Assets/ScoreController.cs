using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class ScoreController : MonoBehaviour {
	public Text DeepestFloorLabel;
	public Text StairsUpLabel;
	public Text StairsDownLabel;
	public Text GoldLabel;
	public Text AliveLabel;
	public Text FirstAmuletLabel;
	public Text SecondAmuletLabel;
	public Text ThirdAmuletLabel;
	public Text TotalScoreLabel;
	public Image PlayerImage;

	void Start() {
		var player = Globals.nextPlayer;
		var totalScore = 0;

		totalScore += player.DeepestFloor * 10;
		DeepestFloorLabel.text 	= "Deepest floor\t\t\t" + player.DeepestFloor + "\tx\t10\t= " + player.DeepestFloor * 10;
		totalScore += player.StairsUpCounter * -2;
		StairsUpLabel.text 		= "Stairs up\t\t\t\t" + player.StairsUpCounter + "\tx\t-2\t= " + player.StairsUpCounter * -2;
		totalScore += player.StairsDownCounter * -2;
		StairsDownLabel.text 	= "Stairs down\t\t\t" + player.StairsDownCounter + "\tx\t-2\t= " + player.StairsDownCounter * -2;

		var goldCardCount = player.DrawStack.Count(c => c.Name == "Gold")
			+ player.HandStack.Count(c => c.Name == "Gold")
			+ player.AttackStack.Count(c => c.Name == "Gold")
			+ player.DefenseStack.Count(c => c.Name == "Gold")
			+ player.DiscardStack.Count(c => c.Name == "Gold");

		var amuletCount = player.DrawStack.Count(c => c.Name.StartsWith("Amulet of "))
			+ player.HandStack.Count(c => c.Name.StartsWith("Amulet of "))
			+ player.AttackStack.Count(c => c.Name.StartsWith("Amulet of "))
			+ player.DefenseStack.Count(c => c.Name.StartsWith("Amulet of "))
			+ player.DiscardStack.Count(c => c.Name.StartsWith("Amulet of "));

		totalScore += goldCardCount * 2;
		GoldLabel.text = "Gold\t\t\t\t\t\t" + goldCardCount + "\tx\t 2\t= " + goldCardCount * 2;

		if (player.TeamName == "Undead") {
			totalScore += 10;
			AliveLabel.text = "Undead\t\t\t\t\t1\tx\t10\t= 10";
		} else {
			totalScore += player.CurrentHealth > 0 ? 25 : 0;
			AliveLabel.text = "Alive\t\t\t\t\t\t" + (player.CurrentHealth > 0 ? 1 : 0) + "\tx\t25\t= " + (player.CurrentHealth > 0 ? 25 : 0);
		}

		totalScore += amuletCount > 0 ? 20 : 0;
		FirstAmuletLabel.text 	= "First amulet\t\t\t" + (amuletCount > 0 ? 1 : 0) + "\tx\t20\t= " + (amuletCount > 0 ? 20 : 0);
		totalScore += amuletCount > 1 ? 30 : 0;
		SecondAmuletLabel.text 	= "Second amulet\t\t" + (amuletCount > 1 ? 1 : 0) + "\tx\t30\t= " + (amuletCount > 1 ? 30 : 0);
		totalScore += amuletCount > 2 ? 40 : 0;
		ThirdAmuletLabel.text 	= "Third amulet\t\t\t" + (amuletCount > 2 ? 1 : 0) + "\tx\t40\t= " + (amuletCount > 2 ? 40 : 0);

		TotalScoreLabel.text 	= "Total score\t\t\t\t\t\t= " + totalScore;

		if (player.TeamName == "Undead") {
			var parts = player.SpriteName.Split(':');
			var sprites = Resources.LoadAll<Sprite>(parts[0]);
			PlayerImage.sprite = sprites.Single(s => s.name == parts[1]);
		}
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			UnityEngine.SceneManagement.SceneManager.LoadScene(1);
	}
}
