using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Creature {
	public bool Exists = true;
	public Point Position;
	public string SpriteName;
	public string Name;
	public string TeamName;
	public AI Ai;

	public int AttackValue;
	public int MaximumAttackCards;
	public int DefenseValue;
	public int MaximumDefenseCards;
	public int MaximumHealth;
	public int CurrentHealth;
	public int MaximumHandCards;

	public int DeepestFloor = 1;
	public int StairsUpCounter;
	public int StairsDownCounter;

	public Pack[] Packs;

	public List<Card> DrawPile = new List<Card>();
	public List<Card> AttackPile = new List<Card>();
	public List<Card> DefensePile = new List<Card>();
	public List<Card> HandPile = new List<Card>();
	public List<Card> DiscardPile = new List<Card>();

	public void MoveBy(Game game, int mx, int my) {
		var next = Position + new Point(mx, my);

		var tile = game.GetTile(next.X, next.Y);
		var other = game.GetCreature(next);

		if (other != null && other != this) {
			if (other.TeamName == "Merchant" && this == game.Player)
				Globals.MessageBus.Send(new Messages.TalkToMerchant(this, other));
			else if (other.TeamName == TeamName && this == game.Player) {
				other.Position = this.Position;
				this.Position = next;
			} else
				Attack(game, other);
		} else if (tile == Tile.DoorClosed) {
			game.SetTile(next.X, next.Y, Tile.DoorOpen);
		} else if (tile == Tile.StairsDown) {
			Position = next;
			game.ExitLevelDownStairs(this);
		} else if (tile == Tile.StairsUp) {
			if (game.CurrentLevel > 0) {
				Position = next;
				game.ExitLevelUpStairs(this);
			}
		} else if (tile.BlocksMovement) {
			
		} else {
			Position = next;
			var item = game.GetItem(Position);
			if (item != null) {
				item.Exists = false;
				if (item.Card != null) {
					Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(item.Card.Name, item.Position, Vector3.zero)));
					if (this == game.Player)
						Globals.MessageBus.Send(new Messages.CardAdded(item.Card));
					DrawPile.Add(item.Card);
				} else if (item.Pack != null) {
					Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(item.Pack.Name, item.Position, Vector3.zero)));
					var allCards = Util.Shuffle(item.Pack.Cards);
					for (var i = 0; i < allCards.Count; i++) {
						var index = i;
						game.Effects.Add(new DelayedEffect() {
							Delay = 0.05f * index,
							Callback = (g) => {
								if (this == game.Player)
									Globals.MessageBus.Send(new Messages.CardAdded(allCards[index]));
								this.DrawPile.Add(allCards[index]);
							}
						});
					}
				}
			}
			Draw1Card(game);
		}

		EndTurn(game);
	}

	public void UseCard(Game game, Card card) {
		HandPile.Remove(card);
		card.DoAction(game, this, null, card.OnUse);
		Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(card.Name, Position, new Vector3(0,4,0))));
		DiscardPile.Add(card);
	}

	public void ShuffleEverythingIntoDrawStack() {
		DiscardPile.AddRange(AttackPile);
		AttackPile.Clear();
		DiscardPile.AddRange(DefensePile);
		DefensePile.Clear();
		DiscardPile.AddRange(HandPile);
		HandPile.Clear();
		DiscardPile.AddRange(DrawPile);
		DrawPile.Clear();
		DrawPile.AddRange(Util.Shuffle(DiscardPile));
		DiscardPile.Clear();
	}

	void ReshuffleIntoDrawStack() {
		if (!DiscardPile.Any()) {
			ShuffleEverythingIntoDrawStack();
		} else {
			DrawPile.AddRange(Util.Shuffle(DiscardPile));
			DiscardPile.Clear();
		}
	}

	public Card GetTopDrawCard() {
		if (!DrawPile.Any())
			ReshuffleIntoDrawStack();
		
		var pulledCard = DrawPile.Last();
		DrawPile.Remove(pulledCard);
		return pulledCard;
	}

	public void Draw1Card(Game game) {
		KeepCard(game, GetTopDrawCard());
	}

	public void KeepCard(Game game, Card pulledCard) {
		pulledCard.DoAction(game, this, null, pulledCard.OnDraw);

		if (pulledCard.CardType == CardType.Attack) {
			AttackPile.Add(pulledCard);
			while (AttackPile.Count > MaximumAttackCards) {
				var toDiscard = AttackPile[0];
				AttackPile.RemoveAt(0);
				DiscardPile.Add(toDiscard);
			}
		} else if (pulledCard.CardType == CardType.Defense) {
			DefensePile.Add(pulledCard);
			while (DefensePile.Count > MaximumDefenseCards) {
				var toDiscard = DefensePile[0];
				DefensePile.RemoveAt(0);
				DiscardPile.Add(toDiscard);
			}
		} else {
			HandPile.Add(pulledCard);
			pulledCard.DoAction(game, this, null, pulledCard.OnInHand);
			while (HandPile.Count > MaximumHandCards) {
				var toDiscard = HandPile[0];
				HandPile.RemoveAt(0);
				toDiscard.UndoAction(game, this, toDiscard.OnInHand);
				DiscardPile.Add(toDiscard);
			}
		}
	}

	void EndTurn(Game game) {
		while (AttackPile.Count > MaximumAttackCards) {
			var toDiscard = AttackPile[0];
			AttackPile.RemoveAt(0);
			DiscardPile.Add(toDiscard);
		}
		while (DefensePile.Count > MaximumDefenseCards) {
			var toDiscard = DefensePile[0];
			DefensePile.RemoveAt(0);
			DiscardPile.Add(toDiscard);
		}
		while (HandPile.Count > MaximumHandCards) {
			var toDiscard = HandPile[0];
			HandPile.RemoveAt(0);
			toDiscard.UndoAction(game, this, toDiscard.OnInHand);
			DiscardPile.Add(toDiscard);
		}
	}

	public void TakeTurn(Game game) {
		Ai.TakeTurn(game, this);
	}

	public void Attack(Game game, Creature other) {
		new Combat(game, this, other).Resolve();
	}

	public void TakeDamage(Game game, int amount) {
		CurrentHealth -= amount;
		if (CurrentHealth <= 0)
			Die(game);
	}

	public void Die(Game game) {
		Exists = false;
		foreach (var amulet in AllCards().Where(c => c.Name.StartsWith("Amulet of "))) {
			var item = game.Catalog.CardItem(Position.X, Position.Y, amulet);
			game.Items.Add(item);
			Globals.MessageBus.Send(new Messages.ItemAdded(item));
		}
	}

	public List<Card> AllCards() {
		var all = new List<Card>();
		all.AddRange(DrawPile);
		all.AddRange(AttackPile);
		all.AddRange(DefensePile);
		all.AddRange(HandPile);
		all.AddRange(DiscardPile);
		return all;
	}
}
