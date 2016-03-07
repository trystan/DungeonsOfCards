using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public enum CardType {
	Attack, Defense, Normal
}

public enum CardSpecialEffect {
	None, Discard1FromEachPile, Heal1Health, Lose1Health, Draw3, Vampire1, Evade,
	Draw5Attack, Draw5Defense, IncreaseDefenseSize, IncreaseAttackSize,
	SpawnSkeleton, AddCardToOther, AddCardToSelf
}

public class Card {
	public string Name;
	public CardType CardType;

	public int CombatBonus;
	public bool DoesBlockOtherCard;
	public bool DoesStopCombat;
	public Func<Card> ExtraCard;

	public CardSpecialEffect OnInHand = CardSpecialEffect.None;
	public CardSpecialEffect OnDraw = CardSpecialEffect.None;
	public CardSpecialEffect OnDie = CardSpecialEffect.None;
	public CardSpecialEffect OnUse = CardSpecialEffect.None;

	public void DoAction(Game game, Creature user, Creature other, CardSpecialEffect effect) {
		switch (effect) {
		case CardSpecialEffect.None:
			break;
		case CardSpecialEffect.Evade:
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
			break;
		case CardSpecialEffect.Vampire1: 
			other.TakeDamage(1);
			if (user.CurrentHealth < user.MaximumHealth)
				user.CurrentHealth++;
			break;
		case CardSpecialEffect.AddCardToSelf: {
			var newCard = ExtraCard();
			user.DrawStack.Add(newCard);
			if (user == game.Player)
				game.NewCards.Add(newCard);
			break; 
		}
		case CardSpecialEffect.AddCardToOther: {
			var newCard = ExtraCard();
			other.DrawStack.Add(newCard);
			if (other == game.Player)
				game.NewCards.Add(newCard);
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
					user.DiscardStack.Add(c);
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
					user.DiscardStack.Add(c);
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
			user.TakeDamage(1);
			break;
		case CardSpecialEffect.Discard1FromEachPile:
			if (user.HandStack.Any()) {
				var i = UnityEngine.Random.Range(0, user.HandStack.Count);
				var card = user.HandStack[i];
				user.HandStack.RemoveAt(i);
				card.UndoAction(game, user, card.OnInHand);
				user.DiscardStack.Add(card);
			}
			if (user.AttackStack.Any()) {
				var i = UnityEngine.Random.Range(0, user.AttackStack.Count);
				var card = user.AttackStack[i];
				user.AttackStack.RemoveAt(i);
				user.DiscardStack.Add(card);
			}
			if (user.DefenseStack.Any()) {
				var i = UnityEngine.Random.Range(0, user.DefenseStack.Count);
				var card = user.DefenseStack[i];
				user.DefenseStack.RemoveAt(i);
				user.DiscardStack.Add(card);
			}
			break;
		case CardSpecialEffect.IncreaseAttackSize:
			user.MaximumAttackCards++;
			break;
		case CardSpecialEffect.IncreaseDefenseSize:
			user.MaximumDefenseCards++;
			break;
		case CardSpecialEffect.SpawnSkeleton:
			game.Creatures.Add(game.Catalog.Skeleton(user.Position.X, user.Position.Y));
			break;
		default:
			throw new NotImplementedException(Name + " " + effect);
		}
	}

	public void UndoAction(Game game, Creature user, CardSpecialEffect effect) {
		switch (effect) {
		case CardSpecialEffect.IncreaseAttackSize:
			user.MaximumAttackCards--;
			break;
		case CardSpecialEffect.IncreaseDefenseSize:
			user.MaximumDefenseCards--;
			break;
		}
	}
}
