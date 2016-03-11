
namespace Messages {
	public struct NextLevel {
		public Game Game;

		public NextLevel(Game game) { 
			Game = game;
		}
	}

	public struct CardAdded {
		public Card Card;

		public CardAdded(Card card) { 
			Card = card;
		}
	}

	public struct CreatureAdded {
		public Creature Creature;

		public CreatureAdded(Creature creature) { 
			Creature = creature;
		}
	}

	public struct ItemAdded {
		public Item Item;

		public ItemAdded(Item item) { 
			Item = item;
		}
	}

	public struct AddPopup {
		public TextPopup Popup;

		public AddPopup(TextPopup popup) {
			Popup = popup;
		}
	}
}
