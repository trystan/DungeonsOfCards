using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public interface AI {
	void TakeTurn(Game game, Creature creature);
}

public class PlayerAi : AI {
	public void TakeTurn(Game game, Creature creature) {
	}
}

public class ComputerAi : AI {
	public void TakeTurn(Game game, Creature creature) {
		var cardsToPlay = new Dictionary<Card, int>();

		foreach (var c in creature.HandStack) {
			switch (c.OnUse) {
			case CardSpecialEffect.Draw3: {
					var chance = creature.MaximumHandCards - creature.HandStack.Count
						+ creature.MaximumAttackCards - creature.AttackStack.Count
						+ creature.MaximumDefenseCards - creature.DefenseStack.Count;
					if (chance > 0)
						cardsToPlay.Add(c, chance);
					break;
				}
			case CardSpecialEffect.Draw5Attack: {
					var chance = (creature.MaximumAttackCards - creature.AttackStack.Count) * 2;
					if (chance > 0)
						cardsToPlay.Add(c, chance);
					break;
				}
			case CardSpecialEffect.Draw5Defense: {
					var chance = (creature.MaximumDefenseCards - creature.DefenseStack.Count) * 2;
					if (chance > 0)
						cardsToPlay.Add(c, chance);
					break;
				}
			case CardSpecialEffect.Heal1Health: {
					var chance = (creature.MaximumHealth - creature.CurrentHealth) * 2;
					if (chance > 0)
						cardsToPlay.Add(c, chance);
					break;
				}
			}
		}

		for (var x = -1; x < 2; x++) {
			for (var y = -1; y < 2; y++) {
				if (x != 0 && y != 0)
					continue;

				if (x == 0 && y == 0) {
					var strength = 3 - GetDanger(game, creature, new Point(0,0));
					if (strength > 0)
						cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, strength);
				}

				var neighbor = creature.Position + new Point(x,y);

				var other = game.GetCreature(neighbor);
				if (other != null) {
					if (other.TeamName != creature.TeamName) {
						var strength = 5 + (creature.AttackValue + creature.AttackStack.Count) * 2 - other.DefenseValue - other.DefenseStack.Count;
						if (strength > 0)
							cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, strength);
					}
				} else if (!game.GetTile(neighbor.X, neighbor.Y).BlocksMovement) {
					var strength = 5 - GetDanger(game, creature, neighbor);
					if (strength > 0)
						cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, strength);
				}
			}
		}

		var averageWeight = cardsToPlay.Values.Sum() / cardsToPlay.Count;
		var weightedCardsToPlay = new Dictionary<Card, int>();
		foreach (var kv in cardsToPlay.Where(p => p.Value > averageWeight))
			weightedCardsToPlay[kv.Key] = kv.Value * kv.Value;

		var chosenCard = Util.WeightedChoice(cardsToPlay);
		if (chosenCard.Name.StartsWith("AI_MOVE")) {
			var xy = chosenCard.Name.Split(' ')[1];
			var x = int.Parse(xy.Split('x')[0]);
			var y = int.Parse(xy.Split('x')[1]);
			creature.MoveBy(game, x, y);
		} else
			creature.UseCard(game, chosenCard);
	}

	int GetDanger(Game game, Creature creature, Point point) {
		var n = game.GetCreature(point + new Point( 0, 1));
		var s = game.GetCreature(point + new Point( 0,-1));
		var w = game.GetCreature(point + new Point(-1, 0));
		var e = game.GetCreature(point + new Point( 1, 0));
		return (n != null && n.TeamName != creature.TeamName ? 1 : 0)
			+ (s != null && s.TeamName != creature.TeamName ? 1 : 0)
			+ (w != null && w.TeamName != creature.TeamName ? 1 : 0)
			+ (e != null && e.TeamName != creature.TeamName ? 1 : 0);
	}
}