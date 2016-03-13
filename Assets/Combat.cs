using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Combat {
	Game Game;
	Creature Attacker;
	Creature Defender;

	public Combat(Game game, Creature attacker, Creature defender) {
		Game = game;
		Attacker = attacker;
		Defender = defender;
	}

	public void Resolve() {
		var attackerAttack = Attacker.AttackValue;
		var defenderDefense = Defender.DefenseValue;

		var maxCards = Mathf.Max(Attacker.AttackPile.Count, Defender.DefensePile.Count);

		var defenderStrongVs = new List<string>();
		var attackerStrongVs = new List<string>();

		for (var i = 0; i < maxCards; i++) {
			var defenderCard = Defender.DefensePile.Count > i ? Defender.DefensePile[i] : null;
			var attackerCard = Attacker.AttackPile.Count > i ? Attacker.AttackPile[i] : null;
			var blockAttack = false;
			var endsCombat = false;

			if (defenderCard != null) {
				defenderDefense += defenderCard.CombatBonus;
				blockAttack = defenderCard.DoesBlockOtherCard;
				endsCombat = defenderCard.DoesStopCombat;

				if (defenderCard.StrongVs != null)
					defenderStrongVs.Add(defenderCard.StrongVs);
					
				defenderCard.DoAction(Game, Defender, Attacker, defenderCard.OnUse);

				var popup = new TextPopup(defenderCard.Name, Defender.Position, new Vector3(0,14 * i,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f,
					Callback = g => Globals.MessageBus.Send(new Messages.AddPopup(popup)),
				});
			}

			if (attackerCard != null && !blockAttack) {
				attackerAttack += attackerCard.CombatBonus;
				endsCombat = endsCombat || attackerCard.DoesStopCombat;

				if (attackerCard.StrongVs != null)
					attackerStrongVs.Add(attackerCard.StrongVs);

				attackerCard.DoAction(Game, Attacker, Defender, attackerCard.OnUse);

				var popup = new TextPopup(attackerCard.Name, Attacker.Position, new Vector3(0,14 * i + 7,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f + 0.05f,
					Callback = g => Globals.MessageBus.Send(new Messages.AddPopup(popup)),
				});
			}

			if (endsCombat)
				break;
		}

		if (attackerStrongVs.Contains(Defender.TeamName))
			attackerAttack *= 2;
		
		if (defenderStrongVs.Contains(Attacker.TeamName))
			defenderDefense *= 2;
		
		Defender.TakeDamage(Game, Mathf.Max(0, attackerAttack - defenderDefense));

		if (!Defender.Exists)
			Defender.DefensePile.ForEach(c => c.DoAction(Game, Defender, Attacker, c.OnDie));

		Attacker.DiscardPile.AddRange(Attacker.AttackPile);
		Attacker.AttackPile.Clear();

		Defender.DiscardPile.AddRange(Defender.DefensePile);
		Defender.DefensePile.Clear();
	}
}
