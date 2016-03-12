using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

public class ChooseController : MonoBehaviour {
	public CreatureSelectPanelController ChoicePanel;

	List<Creature> Creatures = new List<Creature>();
	List<Pack> Pack1s = new List<Pack>();
	List<Pack> Pack2s = new List<Pack>();

	int index;

	void Start() {
		var catalog = new Catalog();
		AddChoice(catalog.RogueLizard(0,0));
		AddChoice(catalog.AttackLizard(0,0));
		AddChoice(catalog.DefenseLizard(0,0));
		AddChoice(catalog.PriestLizard(0,0));

		AddChoice(catalog.Skeleton(0,0));
		AddChoice(catalog.Zombie(0,0));
		AddChoice(catalog.Vampire(0,0));
		AddChoice(catalog.Ghost(0,0));

		AddChoice(catalog.TreePerson(0,0));
		AddChoice(catalog.LeafPerson(0,0));
		AddChoice(catalog.ShroomPerson(0,0));
		AddChoice(catalog.MossMan(0,0));

		index = UnityEngine.Random.Range(0, Creatures.Count);
		Show();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.H) || Input.GetKeyDown(KeyCode.Keypad4))
			Previous();
		if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.L) || Input.GetKeyDown(KeyCode.Keypad6))
			Next();
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			SelectCurrentCreature();
	}

	void AddChoice(Creature creature) {
		Creatures.Add(creature);
		Pack1s.Add(creature.Packs[0]);
		Pack2s.Add(creature.Packs[1]);
	}

	public void Previous() {
		index--;
		if (index < 0)
			index += Creatures.Count;
		Show();
	}

	public void Next() {
		index++;
		if (index >= Creatures.Count)
			index -= Creatures.Count;
		Show();
	}

	public void SelectCurrentCreature() {
		Globals.nextPlayer = Creatures[index];
		Globals.nextPlayer.Ai = new PlayerAi();
		UnityEngine.SceneManagement.SceneManager.LoadScene(2);
	}

	void Show() {
		var creature = Creatures[index];
		var pack1 = Pack1s[index];
		var pack2 = Pack2s[index];

		ChoicePanel.Show(creature, pack1, pack2);
	}
}
