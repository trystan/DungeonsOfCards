using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class CreatureSelectPanelController : MonoBehaviour {
	public Text Name;
	public Text Category;
	public Text Stats;
	public Text Pack1;
	public Text Pack2;
	public Image Image;

	Creature Creature;

	public void Show(Creature creature, Pack pack1, Pack pack2) {
		Creature = creature;

		Name.text = creature.Name;
		Category.text = creature.TeamName;
		Stats.text = "Hitpoints:\t\t" + creature.MaximumHealth
			+ "\nAttack:\t\t\t" + creature.AttackValue + " + " + creature.MaximumAttackCards + " cards"
			+ "\nDefense:\t\t" + creature.DefenseValue + " + " + creature.MaximumDefenseCards + " cards"
			+ "\nHand:\t\t\t\t" + creature.MaximumHandCards + " cards";
		
		var parts = creature.SpriteName.Split(':');
		var sprites = Resources.LoadAll<Sprite>(parts[0]);
		Image.sprite = sprites.Single(s => s.name == parts[1]);

		Pack1.text = pack1.Name + " pack:\n" + string.Join("\n", pack1.Cards.Select(c => c.Name).Distinct().OrderBy(x => x).ToArray());
		Pack2.text = pack2.Name + " pack:\n" + string.Join("\n", pack2.Cards.Select(c => c.Name).Distinct().OrderBy(x => x).ToArray());
	}

	public void Clicked() {
		Creature.Ai = new PlayerAi();
		Globals.nextPlayer = Creature;
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}
}
