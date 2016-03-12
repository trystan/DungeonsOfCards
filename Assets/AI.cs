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

public class MerchantAi : AI {
	public string Name;
	public List<Card> CardsForSale;

	public void TakeTurn(Game game, Creature creature) {
	}
}

public class ComputerAi : AI {
	Point lastMovedDirection;

	public void TakeTurn(Game game, Creature creature) {
		var cardsToPlay = new Dictionary<Card, int>();

		foreach (var c in creature.HandStack) {
			switch (c.OnUse) {
			case CardSpecialEffect.None:
				break;
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
			default:
				cardsToPlay.Add(c, UnityEngine.Random.Range(1, 5));
				break;
			}
		}

		for (var x = -1; x < 2; x++) {
			for (var y = -1; y < 2; y++) {
				if (x != 0 && y != 0)
					continue;

				var neighbor = creature.Position + new Point(x,y);
				var cardName = "AI_MOVE " + x + "x" + y;

				var strength = 0;
				if (new Point(x,y) == lastMovedDirection)
					strength = 2;
				else if (new Point(x,y) == lastMovedDirection * -1)
					strength = 0;
				else
					strength = 1;

				var fear = Mathf.RoundToInt(5 * (1f - (1f * creature.CurrentHealth / creature.MaximumHealth)));

				if (x == 0 && y == 0) {
					strength -= GetDanger(game, creature, new Point(0,0)) + fear;
					if (strength > 0)
						cardsToPlay.Add(new Card() { Name = cardName }, strength);
				}


				var other = game.GetCreature(neighbor);
				if (other != null) {
					if (other.TeamName != creature.TeamName && other.TeamName != "Merchant") {
						strength = GetAttackability(creature, other) - fear;
						if (strength > 0)
							cardsToPlay.Add(new Card() { Name = cardName }, strength);
					}
				} else if (!game.GetTile(neighbor.X, neighbor.Y).BlocksMovement) {
					strength -= GetDanger(game, creature, neighbor);
					if (strength > 0)
						cardsToPlay.Add(new Card() { Name = cardName }, strength);
				}
			}
		}

		if (cardsToPlay.Count == 0) {
			creature.MoveBy(game, 0, 0);
		} else {
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
				if (x != 0 && y != 0)
					lastMovedDirection = new Point(x,y);
			} else
				creature.UseCard(game, chosenCard);
		}
	}

	int GetDanger(Game game, Creature creature, Point point) {
		var total = 0;
		foreach (var p in new Point[] { new Point( -1, 1), new Point(-1, 0), new Point(-1,-1),
				new Point(  0, 1), new Point( 0, 0), new Point( 0,-1),
				new Point(  1, 1), new Point( 1, 0), new Point( 1,-1),
				new Point( -2, 0), new Point( 2, 0), new Point( 0,-2), new Point( 0, 2),
				new Point( -3, 0), new Point( 3, 0), new Point( 0,-3), new Point( 0, 3)})
			total += GetImmediateDanger(game, creature, p);
		return total;
	}

	int GetImmediateDanger(Game game, Creature creature, Point point) {
		var h = game.GetCreature(point);
		return (h == null ? 0 : GetDanger(creature, h));
	}

	int GetDanger(Creature self, Creature other) {
		if (self.TeamName == other.TeamName || other.TeamName == "Merchant")
			return 0;
		else {
			return (((other.AttackValue + other.MaximumAttackCards / 2) - (self.DefenseValue + self.DefenseStack.Count)) * 2
				- GetAttackability(self, other)) * 2;
		}
	}

	int GetAttackability(Creature self, Creature other) {
		if (self.TeamName == other.TeamName || other.TeamName == "Merchant")
			return 0;
		else
			return ((self.AttackValue + self.AttackStack.Count) - (other.DefenseValue + other.MaximumDefenseCards / 2)) * 2;
	}
}