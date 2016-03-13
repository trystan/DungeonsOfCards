using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum CardType {
	Attack, Defense, Normal
}

public enum CardSpecialEffect {
	None, Discard1FromEachPile, Heal1Health, Lose1Health, Draw3, Evade, Blink,
	Draw5Attack, Draw5Defense, IncreaseDefenseSize, IncreaseAttackSize, IncreaseHandSize,
	SpawnSkeleton, AddCardToOther, AddCardToSelf, Pray, HealTeam, TurnUndead, DamageClosest,
	VampireBite, GhostForm, IncreaseAllStats,
	IncreaseAllSizes, ChargeNearest, ReduceAllSizes, DestroyBadCard,
	IncreaseAttackValue3, IncreaseDefenseValue3,
}

public class Card {
	public bool Exists = true;
	public string Name;
	public string SpriteName;
	public string Description;
	public string FlavorText;
	public int GoldCost = 2;
	public CardType CardType;
	public Vector3 WorldPointOrigin;

	public int CombatBonus;
	public bool DoesBlockOtherCard;
	public bool DoesStopCombat;
	public string StrongVs;
	public Func<Card> ExtraCard;

	public CardSpecialEffect OnInHand = CardSpecialEffect.None;
	public CardSpecialEffect OnDraw = CardSpecialEffect.None;
	public CardSpecialEffect OnDie = CardSpecialEffect.None;
	public CardSpecialEffect OnUse = CardSpecialEffect.None;

	void DoBlinkAction(Game game, Creature user) {
		var goodCandidates = new List<Point>();
		var okCandidates = new List<Point>();
		for (var x = -3; x < 4; x++) {
			for (var y = -3; y < 4; y++) {
				var xy = new Point(user.Position.X + x, user.Position.Y + y);
				var tile = game.GetTile(xy.X, xy.Y);
				if (tile.BlocksMovement
					|| tile == Tile.StairsUp
					|| tile == Tile.StairsDown
					|| game.GetCreature(xy) != null
					|| game.GetItem(xy) != null)
					continue;

				okCandidates.Add(xy);

				if (game.GetCreature(xy + new Point(-1, 0)) == null
					&& game.GetCreature(xy + new Point( 1, 0)) == null
					&& game.GetCreature(xy + new Point( 0,-1)) == null
					&& game.GetCreature(xy + new Point( 0, 1)) == null)
					goodCandidates.Add(xy);
			}
		}
		if (goodCandidates.Any()) {
			var target = Util.Shuffle(goodCandidates)[0];
			user.Position = target;
		} else if (okCandidates.Any()) {
			var target = Util.Shuffle(okCandidates)[0];
			user.Position = target;
		}
	}

	void DoEvadeAction(Game game, Creature user) {
		var candidates = new List<Point>();
		foreach (var p in new Point[] { new Point(-1,0), new Point(1,0), new Point(0,1), new Point(0,-1) }) {
			var there = user.Position + p;
			if (game.GetTile(there.X, there.Y).BlocksMovement)
				continue;
			if (game.GetCreature(there) != null)
				continue;
			candidates.Add(p);
		}

		if (candidates.Any()) {
			var candidate = Util.Shuffle(candidates)[0];
			user.MoveBy(game, candidate.X, candidate.Y);
		}
	}

	public void DoAction(Game game, Creature user, Creature other, CardSpecialEffect effect) {
		switch (effect) {
		case CardSpecialEffect.None:
			break;
		case CardSpecialEffect.Blink:
			DoBlinkAction(game, user);
			break;
		case CardSpecialEffect.DestroyBadCard:
			user.ShuffleEverythingIntoDrawStack();
			var bads = user.DrawPile.Where(c => c.Name == "Leafs" || c.Name == "Idle" || c.Name == "Disease" || c.Name == "Poison" || c.Name == "Cursed").ToList();
			if (bads.Any()) {
				var bad = Util.Shuffle(bads)[0];
				bad.Exists = false;
				user.DrawPile.Remove(bad);
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Purge " + bad.Name, user.Position, new Vector3(0,14,0))));
			}
			user.ShuffleEverythingIntoDrawStack();
			break;
		case CardSpecialEffect.Pray:
			var actions = new Dictionary<string, int>();
			actions.Add("heal", (user.MaximumHealth - user.CurrentHealth) * 2);
			actions.Add("destroy leafs", user.GetCardCount("Leafs"));
			actions.Add("destroy disease", user.GetCardCount("Disease"));
			actions.Add("destroy poison", user.GetCardCount("Poison"));
			actions.Add("destroy cursed", user.GetCardCount("Cursed") * 4);
			if (3 - user.GetCardCount("Gold") > 0)
				actions.Add("gift gold", 3 - user.GetCardCount("Gold"));
			
			actions.Add("gift priest card", 2);

			var action = Util.WeightedChoice(actions);
			switch (action) {
			case "heal": 
				user.CurrentHealth = Mathf.Min(user.CurrentHealth + 4, user.MaximumHealth);
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, heal", user.Position, new Vector3(0,14,0))));
				break;
			case "destroy leafs":
				user.ShuffleEverythingIntoDrawStack();
				foreach (var card in user.DrawPile.Where(c => c.Name == "Leafs").Take(3).ToArray()) {
					user.DrawPile.Remove(card);
					card.Exists = false;
				}
				user.ShuffleEverythingIntoDrawStack();
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, destroy leafs", user.Position, new Vector3(0,14,0))));
				break;
			case "destroy disease":
				user.ShuffleEverythingIntoDrawStack();
				foreach (var card in user.DrawPile.Where(c => c.Name == "Disease").Take(3).ToArray()) {
					user.DrawPile.Remove(card);
					card.Exists = false;
				}
				user.ShuffleEverythingIntoDrawStack();
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, destroy disease", user.Position, new Vector3(0,14,0))));
				break;
			case "destroy poison":
				user.ShuffleEverythingIntoDrawStack();
				foreach (var card in user.DrawPile.Where(c => c.Name == "Poison").Take(3).ToArray()) {
					user.DrawPile.Remove(card);
					card.Exists = false;
				}
				user.ShuffleEverythingIntoDrawStack();
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, destroy poison", user.Position, new Vector3(0,14,0))));
				break;
			case "destroy cursed":
				user.ShuffleEverythingIntoDrawStack();
				foreach (var card in user.DrawPile.Where(c => c.Name == "Cursed").Take(3).ToArray()) {
					user.DrawPile.Remove(card);
					card.Exists = false;
				}
				user.ShuffleEverythingIntoDrawStack();
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, destroy curses", user.Position, new Vector3(0,14,0))));
				break;
			case "gift gold":
				foreach (var gold in new Card[] { game.Catalog.Card("Gold"), game.Catalog.Card("Gold"), game.Catalog.Card("Gold") }) {
					gold.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
					user.DrawPile.Add(gold);
					if (user == game.Player)
						Globals.MessageBus.Send(new Messages.CardAdded(gold));
				}
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, receive gold", user.Position, new Vector3(0,14,0))));
				break;
			case "gift priest card":
				var priestCard = Util.Shuffle(game.Catalog.PriestPack().Cards)[0];
				priestCard.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
				user.DrawPile.Add(priestCard);
				if (user == game.Player)
					Globals.MessageBus.Send(new Messages.CardAdded(priestCard));
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Pray, receive " + priestCard.Name.ToLower(), user.Position, new Vector3(0,14,0))));
				break;
			default:
				Debug.Log("Unknown answer to your prayers: " + action);
				break;
			}
			break;
		case CardSpecialEffect.DamageClosest:
			var closest = game.Creatures
				.Where(c => c != user && user.Position.DistanceTo(c.Position) < 5)
				.OrderBy(c => user.Position.DistanceTo(c.Position))
				.FirstOrDefault();
			if (closest != null) {
				var damage = 2;
				if (closest.TeamName == StrongVs)
					damage *= 2;
				closest.TakeDamage(game, damage);
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(Name, closest.Position, new Vector3(0,14,0))));

				if (ExtraCard != null) {
					var addCard = ExtraCard();
					addCard.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
					closest.DrawPile.Add(addCard);
					if (closest == game.Player)
						Globals.MessageBus.Send(new Messages.CardAdded(addCard));
				}
			}
			break;
		case CardSpecialEffect.HealTeam:
			foreach (var c in game.Creatures.Where(c => c.Exists 
								&& c.TeamName == user.TeamName
								&& c.Position.DistanceTo(user.Position) < 5)) {
				if (c.CurrentHealth < c.MaximumHealth) {
					c.CurrentHealth++;
					Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Heal", c.Position, new Vector3(0,14,0))));
				}
			}
			break;
		case CardSpecialEffect.TurnUndead:
			foreach (var c in game.Creatures.Where(c => c.Exists 
								&& c.TeamName == "Undead"
								&& c.Position.DistanceTo(user.Position) < 5)) {
				c.TakeDamage(game, 5);
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup("Turn undead", c.Position, new Vector3(0,14,0))));
			}
			break;
		case CardSpecialEffect.Evade:
			DoEvadeAction(game, user);
			break;
		case CardSpecialEffect.AddCardToSelf: {
			var newCard = ExtraCard();
			newCard.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
			user.DrawPile.Add(newCard);
			if (user == game.Player)
				Globals.MessageBus.Send(new Messages.CardAdded(newCard));
			break; 
		}
		case CardSpecialEffect.AddCardToOther: {
			if (other == null) {
				for (var x = -1; x < 2; x++) {
					for (var y = -1; y < 2; y++) {
						if (x == 0 && y == 0)
							continue;
						
						var position = user.Position + new Point(x,y);

						Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(Name, position,Vector2.zero)));

						var enemy = game.GetCreature(position);
						if (enemy == null)
							continue;
						
						var newCard = ExtraCard();
						newCard.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
						enemy.DrawPile.Add(newCard);
						if (enemy == game.Player)
							Globals.MessageBus.Send(new Messages.CardAdded(newCard));
					}
				}
			} else {
				Globals.MessageBus.Send(new Messages.AddPopup(new TextPopup(Name, other.Position, Vector2.zero)));

				var newCard = ExtraCard();
				newCard.WorldPointOrigin = new Vector3(user.Position.X, user.Position.Y, 0);
				other.DrawPile.Add(newCard);
				if (other == game.Player)
					Globals.MessageBus.Send(new Messages.CardAdded(newCard));
			}
			break;
		}
		case CardSpecialEffect.Draw3:
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = g => user.Draw1Card(game) });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = g => user.Draw1Card(game) });
			game.Effects.Add(new DelayedEffect() { Delay = 0.3f, Callback = g => user.Draw1Card(game) });
			break;
		case CardSpecialEffect.Draw5Attack:
			Action<Game> drawAttack = g => {
				var c = user.GetTopDrawCard();
				if (c.CardType == CardType.Attack)
					user.KeepCard(game, c);
				else
					user.DiscardPile.Add(c);
			};
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.3f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.4f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.5f, Callback = drawAttack });
			break;
		case CardSpecialEffect.Draw5Defense:
			Action<Game> drawDefense = g => {
				var c = user.GetTopDrawCard();
				if (c.CardType == CardType.Defense)
					user.KeepCard(game, c);
				else
					user.DiscardPile.Add(c);
			};
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.3f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.4f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.5f, Callback = drawDefense });
			break;
		case CardSpecialEffect.Heal1Health:
			if (user.CurrentHealth < user.MaximumHealth)
				user.CurrentHealth++;
			break;
		case CardSpecialEffect.Lose1Health:
			user.TakeDamage(game, 1);
			break;
		case CardSpecialEffect.Discard1FromEachPile:
			if (user.HandPile.Any()) {
				var i = UnityEngine.Random.Range(0, user.HandPile.Count);
				var card = user.HandPile[i];
				user.HandPile.RemoveAt(i);
				card.UndoAction(game, user, card.OnInHand);
				user.DiscardPile.Add(card);
			}
			if (user.AttackPile.Any()) {
				var i = UnityEngine.Random.Range(0, user.AttackPile.Count);
				var card = user.AttackPile[i];
				user.AttackPile.RemoveAt(i);
				user.DiscardPile.Add(card);
			}
			if (user.DefensePile.Any()) {
				var i = UnityEngine.Random.Range(0, user.DefensePile.Count);
				var card = user.DefensePile[i];
				user.DefensePile.RemoveAt(i);
				user.DiscardPile.Add(card);
			}
			break;

		case CardSpecialEffect.IncreaseAttackValue3:
			user.AttackValue += 3;
			break;
		case CardSpecialEffect.IncreaseDefenseValue3:
			user.DefenseValue += 3;
			break;
		case CardSpecialEffect.ReduceAllSizes:
			user.MaximumAttackCards -= 2;
			user.MaximumDefenseCards -= 2;
			user.MaximumHandCards -= 2;
			break;
		case CardSpecialEffect.IncreaseAllSizes:
			user.MaximumAttackCards += 2;
			user.MaximumDefenseCards += 2;
			user.MaximumHandCards += 2;
			break;
		case CardSpecialEffect.IncreaseAllStats:
			user.AttackValue++;
			user.DefenseValue++;
			user.MaximumHealth++;
			user.CurrentHealth++;
			break;
		case CardSpecialEffect.IncreaseAttackSize:
			user.MaximumAttackCards++;
			break;
		case CardSpecialEffect.IncreaseDefenseSize:
			user.MaximumDefenseCards++;
			break;
		case CardSpecialEffect.IncreaseHandSize:
			user.MaximumDefenseCards += 2;
			break;
		case CardSpecialEffect.SpawnSkeleton:
			var creature = game.Catalog.Skeleton(user.Position.X, user.Position.Y);
			game.Creatures.Add(creature);
			Globals.MessageBus.Send(new Messages.CreatureAdded(creature));
			if (user == game.Player && !user.Exists)
				Globals.MessageBus.Send(new Messages.PlayerChangedToCreature(creature));
			break;

		case CardSpecialEffect.ChargeNearest:
			var distance = 5;
			Creature n = null;
			for (var offset = 1; offset < distance && n == null; offset++) {
				if (game.GetTile(user.Position + new Point(0, offset)).BlocksMovement)
					break;
				n = game.GetCreature(user.Position + new Point(0, offset));
			}
			Creature s = null;
			for (var offset = 1; offset < distance && s == null; offset++) {
				if (game.GetTile(user.Position + new Point(0, -offset)).BlocksMovement)
					break;
				s = game.GetCreature(user.Position + new Point(0, -offset));
			}
			Creature w = null;
			for (var offset = 1; offset < distance && w == null; offset++) {
				if (game.GetTile(user.Position + new Point(-offset, 0)).BlocksMovement)
					break;
				w = game.GetCreature(user.Position + new Point(-offset, 0));
			}
			Creature e = null;
			for (var offset = 1; offset < distance && e == null; offset++) {
				if (game.GetTile(user.Position + new Point(offset, 0)).BlocksMovement)
					break;
				e = game.GetCreature(user.Position + new Point(offset, 0));
			}

			var others = new List<Creature>();
			if (n != null && n.TeamName != user.TeamName) others.Add(n);
			if (s != null && s.TeamName != user.TeamName) others.Add(s);
			if (w != null && w.TeamName != user.TeamName) others.Add(w);
			if (e != null && e.TeamName != user.TeamName) others.Add(e);

			if (others.Any()) {
				var enemy = Util.Shuffle(others)[0];
				enemy.TakeDamage(game, 4);
				if (enemy.Exists) {
					var next = enemy.Position;
					if (enemy == n) next += new Point( 0,-1);
					if (enemy == s) next += new Point( 0, 1);
					if (enemy == w) next += new Point( 1, 0);
					if (enemy == e) next += new Point(-1, 0);
					user.Position = next;
				} else {
					user.Position = enemy.Position;
				}
			}
			break;

		case CardSpecialEffect.GhostForm:
			DoEvadeAction(game, user);
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = g => user.Draw1Card(game) });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = g => user.Draw1Card(game) });
			break;
		case CardSpecialEffect.VampireBite: 
			other.TakeDamage(game, 3);
			user.CurrentHealth = Mathf.Min(user.CurrentHealth + 3, user.MaximumHealth);
			DoBlinkAction(game, user);
			break;
		default:
			throw new NotImplementedException(Name + " " + effect);
		}
	}

	public void UndoAction(Game game, Creature user, CardSpecialEffect effect) {
		switch (effect) {
		case CardSpecialEffect.ReduceAllSizes:
			user.MaximumAttackCards += 2;
			user.MaximumDefenseCards += 2;
			user.MaximumHandCards += 2;
			break;
		case CardSpecialEffect.IncreaseAllSizes:
			user.MaximumAttackCards -= 2;
			user.MaximumDefenseCards -= 2;
			user.MaximumHandCards -= 2;
			break;
		case CardSpecialEffect.IncreaseAllStats:
			user.AttackValue--;
			user.DefenseValue--;
			user.MaximumHealth--;
			user.TakeDamage(game, 1);
			break;
		case CardSpecialEffect.IncreaseAttackSize:
			user.MaximumAttackCards--;
			break;
		case CardSpecialEffect.IncreaseDefenseSize:
			user.MaximumDefenseCards--;
			break;
		case CardSpecialEffect.IncreaseHandSize:
			user.MaximumDefenseCards -= 2;
			break;
		case CardSpecialEffect.IncreaseAttackValue3:
			user.AttackValue -= 3;
			break;
		case CardSpecialEffect.IncreaseDefenseValue3:
			user.DefenseValue -= 3;
			break;
		}
	}
}
