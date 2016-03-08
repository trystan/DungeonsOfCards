using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class CardForSaleView : MonoBehaviour {
	public Text CardTitle;
	public Text CardCost;

	Card Card;
	Creature Seller;
	Creature Buyer;
	MerchantPanelController Panel;

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show(Card card, Creature seller, Creature buyer, MerchantPanelController panel) {
		gameObject.SetActive(true);
		Card = card;
		Buyer = buyer;
		Seller = seller;
		Panel = panel;

		CardTitle.text = card.Name;
		CardCost.text = card.GoldCost + " gold";
	}

	public void Buy() {
		var goldCardCount = Buyer.DrawStack.Count(c => c.Name == "Gold")
			+ Buyer.HandStack.Count(c => c.Name == "Gold")
			+ Buyer.AttackStack.Count(c => c.Name == "Gold")
			+ Buyer.DefenseStack.Count(c => c.Name == "Gold")
			+ Buyer.DiscardStack.Count(c => c.Name == "Gold");

		if (goldCardCount >= Card.GoldCost) {
			(Seller.Ai as MerchantAi).CardsForSale.Remove(Card);
			Buyer.DiscardStack.Add(Card);
			Buyer.ShuffleEverythingIntoDrawStack();
			var goldCardsSpent = Buyer.DrawStack.Where(c => c.Name == "Gold").Take(Card.GoldCost).ToList();
			goldCardsSpent.ForEach(c => Buyer.DrawStack.Remove(c));
			Buyer.ShuffleEverythingIntoDrawStack();
			Hide();
			Panel.Refresh();
		}
	}
}
