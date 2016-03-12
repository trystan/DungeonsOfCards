using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class CreatureSelectPanelController : MonoBehaviour {
	public Text Name;
	public Text Category;
	public Text Stats;
	public Text Cards;
	public Image Image;

	Creature Creature;

	public void Show(Creature creature, Pack pack1, Pack pack2) {
		Creature = creature;

		Name.text = creature.Name;
		Category.text = creature.TeamName;
		Stats.text = "Hitpoints:\t\t" + creature.MaximumHealth
			+ "\nAttack:\t\t\t" + creature.AttackValue + " + " + creature.MaximumAttackCards + " cards"
			+ "\nDefense:\t\t" + creature.DefenseValue + " + " + creature.MaximumDefenseCards + " cards"
			+ "\nHand:\t\t\t" + creature.MaximumHandCards + " cards";
		
		var parts = creature.SpriteName.Split(':');
		var sprites = Resources.LoadAll<Sprite>(parts[0]);
		Image.sprite = sprites.Single(s => s.name == parts[1]);

		var standardCards = new List<string>();
		standardCards.AddRange(pack1.Cards.Select(c => c.Name));
		standardCards.AddRange(pack2.Cards.Select(c => c.Name));

		Cards.text = "Cards: " + pack1.Name + " pack, " + pack2.Name + " pack";
		foreach (var cards in creature.DrawPile.Where(c => !standardCards.Contains(c.Name)).GroupBy(c => c.Name)) {
			var card = cards.First();
			var count = creature.DrawPile.Count(c => c.Name == card.Name);
			if (count == 1)
				Cards.text += ", " + card.Name;
			else
				Cards.text += ", " + card.Name + " (x" + count + ")";
		}
	}

	public void Clicked() {
		Creature.Ai = new PlayerAi();
		Globals.nextPlayer = Creature;
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}
}
