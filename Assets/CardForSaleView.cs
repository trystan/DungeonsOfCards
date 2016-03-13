using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Linq;

public class CardForSaleView : MonoBehaviour {
	public Text CardTitle;
	public Text CardCost;
	public Image Image;

	Card Card;
	Creature Seller;
	Creature Buyer;
	MerchantPanelController Panel;

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show(Game game, Card card, Creature seller, Creature buyer, MerchantPanelController panel) {
		gameObject.SetActive(true);
		Card = card;
		Buyer = buyer;
		Seller = seller;
		Panel = panel;

		CardTitle.text = card.Name;
		CardCost.text = card.GoldCost + " gold";

		if (card.SpriteName == null) {
			Image.gameObject.SetActive(false);
		} else {
			Image.gameObject.SetActive(true);
			var parts = card.SpriteName.Split(':');
			var sprites = Resources.LoadAll<Sprite>(parts[0]);
			Image.sprite = sprites.Single(s => s.name == parts[1]);
		}
	}

	public void Buy() {
		var goldCardCount = Buyer.DrawPile.Count(c => c.Name == "Gold")
			+ Buyer.HandPile.Count(c => c.Name == "Gold")
			+ Buyer.AttackPile.Count(c => c.Name == "Gold")
			+ Buyer.DefensePile.Count(c => c.Name == "Gold")
			+ Buyer.DiscardPile.Count(c => c.Name == "Gold");

		if (goldCardCount >= Card.GoldCost) {
			(Seller.Ai as MerchantAi).CardsForSale.Remove(Card);
			Buyer.DiscardPile.Add(Card);
			Buyer.ShuffleEverythingIntoDrawStack();
			var goldCardsSpent = Buyer.DrawPile.Where(c => c.Name == "Gold").Take(Card.GoldCost).ToList();
			goldCardsSpent.ForEach(c => {
				Buyer.DrawPile.Remove(c);
				c.Exists = false;
			});
			Buyer.ShuffleEverythingIntoDrawStack();
			Hide();
			Globals.MessageBus.Send(new Messages.CardAdded(Card));
			Panel.Refresh();
		}
	}
}
