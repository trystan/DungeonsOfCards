using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class MerchantPanelController : MonoBehaviour {
	public Text Title;
	public Text Exposition;
	public Transform CardsPanel;
	public GameObject CardForSalePrefab;

	Game Game;
	MerchantAi Merchant;
	Creature Seller;
	Creature Buyer;

	void Start() {
		Hide();
	}

	void Update() {
		if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return))
			Hide();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show(Game game, Creature seller, Creature buyer) {
		gameObject.SetActive(true);

		Merchant = seller.Ai as MerchantAi;
		Seller = seller;
		Buyer = buyer;

		Refresh();

		foreach (Transform child in CardsPanel)
			GameObject.Destroy(child.gameObject);

		foreach (var c in Merchant.CardsForSale) {
			var view = Instantiate(CardForSalePrefab);
			view.GetComponent<CardForSaleView>().Show(game, c, Seller, Buyer, this);
			view.transform.SetParent(CardsPanel);
		}
	}

	public void Refresh() {
		Title.text = Merchant.Name;

		var goldCardCount = Buyer.DrawStack.Count(c => c.Name == "Gold")
			+ Buyer.HandStack.Count(c => c.Name == "Gold")
			+ Buyer.AttackStack.Count(c => c.Name == "Gold")
			+ Buyer.DefenseStack.Count(c => c.Name == "Gold")
			+ Buyer.DiscardStack.Count(c => c.Name == "Gold");

		if (goldCardCount == 1)
			Exposition.text = "You have " + goldCardCount + " gold card.";
		else
			Exposition.text = "You have " + goldCardCount + " gold cards.";
	}
}
