using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ChooseController : MonoBehaviour {
	public RectTransform Container;
	public GameObject ChoicePanel;

	void Start() {
		var catalog = new Catalog();
		AddChoice(catalog.RogueLizard(0,0), catalog.GenericPack(), catalog.RoguePack());
		AddChoice(catalog.AttackLizard(0,0), catalog.GenericPack(), catalog.AttackPack());
		AddChoice(catalog.DefenseLizard(0,0), catalog.GenericPack(), catalog.DefensePack());
		AddChoice(catalog.PriestLizard(0,0), catalog.GenericPack(), catalog.PriestPack());

		AddChoice(catalog.Skeleton(0,0), catalog.UndeadPack(), catalog.SkeletonPack());
		AddChoice(catalog.Zombie(0,0), catalog.UndeadPack(), catalog.ZombiePack());
		AddChoice(catalog.Vampire(0,0), catalog.UndeadPack(), catalog.VampirePack());
		AddChoice(catalog.Ghost(0,0), catalog.UndeadPack(), catalog.GhostPack());

		AddChoice(catalog.TreePerson(0,0), catalog.FloraPack(), catalog.TreePack());
		AddChoice(catalog.LeafPerson(0,0), catalog.FloraPack(), catalog.FloraPack());
		AddChoice(catalog.ShroomPerson(0,0), catalog.FloraPack(), catalog.FungusPack());
		AddChoice(catalog.MossMan(0,0), catalog.FloraPack(), catalog.MossPack());
	}

	void AddChoice(Creature creature, Pack pack1, Pack pack2) {
		var view = Instantiate(ChoicePanel);
		view.transform.SetParent(Container);
		view.GetComponent<CreatureSelectPanelController>().Show(creature, pack1, pack2);
	}
}
