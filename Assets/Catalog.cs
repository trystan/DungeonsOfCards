using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Pack {
	public string Name;
	public List<Card> Cards;
}

public class Catalog {
	public Pack AdventurerPack() {
		return new Pack() {
			Name = "Adventurer",
			Cards = new List<Card>() {
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Gold", CardType = CardType.Normal },
				new Card() { Name = "Gold", CardType = CardType.Normal },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
			}
		};
	}

	public Pack WizardPack() {
		return new Pack() {
			Name = "Wizard",
			Cards = new List<Card>() {
				new Card() { Name = "Freeze closest", CardType = CardType.Normal, StrongVs = "Lizards", OnUse = CardSpecialEffect.DamageClosest },
				new Card() { Name = "Freeze closest", CardType = CardType.Normal, StrongVs = "Lizards", OnUse = CardSpecialEffect.DamageClosest },
				new Card() { Name = "Burn closest", CardType = CardType.Normal, StrongVs = "Plants", OnUse = CardSpecialEffect.DamageClosest },
				new Card() { Name = "Burn closest", CardType = CardType.Normal, StrongVs = "Plants", OnUse = CardSpecialEffect.DamageClosest },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Focus", CardType = CardType.Attack, OnInHand = CardSpecialEffect.IncreaseHandSize },
				new Card() { Name = "Focus", CardType = CardType.Attack, OnInHand = CardSpecialEffect.IncreaseHandSize },
			}
		};
	}

	public Pack PriestPack() {
		return new Pack() {
			Name = "Priest",
			Cards = new List<Card>() {
				new Card() { Name = "Turn undead", CardType = CardType.Normal, OnUse = CardSpecialEffect.TurnUndead },
				new Card() { Name = "Turn undead", CardType = CardType.Normal, OnUse = CardSpecialEffect.TurnUndead },
				new Card() { Name = "Holy attack +1", CardType = CardType.Attack, CombatBonus = 1, StrongVs = "Undead" },
				new Card() { Name = "Holy attack +1", CardType = CardType.Attack, CombatBonus = 1, StrongVs = "Undead" },
				new Card() { Name = "Holy defense +1", CardType = CardType.Defense, CombatBonus = 1, StrongVs = "Undead" },
				new Card() { Name = "Holy defense +1", CardType = CardType.Defense, CombatBonus = 1, StrongVs = "Undead" },
				new Card() { Name = "Mass heal", CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam },
				new Card() { Name = "Mass heal", CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam },
				new Card() { Name = "Pray", CardType = CardType.Normal, OnUse = CardSpecialEffect.Pray },
				new Card() { Name = "Pray", CardType = CardType.Normal, OnUse = CardSpecialEffect.Pray },
			}
		};
	}

	public Pack RoguePack() {
		return new Pack() {
			Name = "Rogue",
			Cards = new List<Card>() {
				new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
				new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
				new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
				new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
				new Card() { Name = "Stab +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				new Card() { Name = "Stab +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				new Card() { Name = "Focus", CardType = CardType.Attack, OnInHand = CardSpecialEffect.IncreaseHandSize },
				new Card() { Name = "Focus", CardType = CardType.Attack, OnInHand = CardSpecialEffect.IncreaseHandSize },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
			}
		};
	}

	public Pack AttackPack() {
		return new Pack() {
			Name = "Attack",
			Cards = new List<Card>() {
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
				new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
				new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
				new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
				new Card() { Name = "Quick attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Quick attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
				new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
			}
		};
	}

	public Pack DefensePack() {
		return new Pack() {
			Name = "Defense",
			Cards = new List<Card>() {
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
				new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
				new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
				new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
				new Card() { Name = "Quick defense", CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Quick defense", CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
				new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
			}
		};
	}

	public Pack GenericPack() {
		return new Pack() {
			Name = "Basic",
			Cards = new List<Card>() {
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
				new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Idle", CardType = CardType.Normal },
				new Card() { Name = "Idle", CardType = CardType.Normal },
			}
		};
	}

	public Pack UndeadPack() {
		return new Pack() {
			Name = "Undead",
			Cards = new List<Card>() {
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
				new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
				new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile },
				new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile },
				new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
				new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
				new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
				new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			}
		};
	}

	public Pack SkeletonPack() {
		return new Pack() {
			Name = "Skeleton",
			Cards = new List<Card>() {
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Bash +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				new Card() { Name = "Bash +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				new Card() { Name = "Idle", CardType = CardType.Normal },
				new Card() { Name = "Idle", CardType = CardType.Normal },
			}
		};
	}

	public Pack VampirePack() {
		return new Pack() {
			Name = "Vampire",
			Cards = new List<Card>() {
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Blood suck", CardType = CardType.Attack, OnUse = CardSpecialEffect.Vampire1 },
				new Card() { Name = "Blood suck", CardType = CardType.Attack, OnUse = CardSpecialEffect.Vampire1 },
				new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
				new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
				new Card() { Name = "Idle", CardType = CardType.Normal },
				new Card() { Name = "Idle", CardType = CardType.Normal },
			}
		};
	}

	public Pack GhostPack() {
		return new Pack() {
			Name = "Ghost",
			Cards = new List<Card>() {
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
				new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
				new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
				new Card() { Name = "Idle", CardType = CardType.Normal },
				new Card() { Name = "Idle", CardType = CardType.Normal },
			}
		};
	}

	public Pack ZombiePack() {
		return new Pack() {
			Name = "Zombie",
			Cards = new List<Card>() {
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Rot", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Lose1Health },
				new Card() { Name = "Rot", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Lose1Health },
				new Card() { Name = "Idle", CardType = CardType.Normal },
				new Card() { Name = "Idle", CardType = CardType.Normal },
			}
		};
	}

	public Pack FloraPack() {
		return new Pack() {
			Name = "Flora",
			Cards = new List<Card>() {
				new Card() { Name = "Plant attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
				new Card() { Name = "Plant attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
				new Card() { Name = "Plant armor", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
				new Card() { Name = "Plant armor", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
					ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Mass heal", CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam },
				new Card() { Name = "Mass heal", CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
			}
		};
	}

	public Pack TreePack() {
		return new Pack() {
			Name = "Tree",
			Cards = new List<Card>() {
				new Card() { Name = "Defensive branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
				new Card() { Name = "Defensive branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
				new Card() { Name = "Attack branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
				new Card() { Name = "Attack branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
				new Card() { Name = "Holding branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseHandSize },
				new Card() { Name = "Holding branches", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseHandSize },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			}
		};
	}

	public Pack FungusPack() {
		return new Pack() {
			Name = "Fungus",
			Cards = new List<Card>() {
				new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
				new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
				new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
				new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
				new Card() { Name = "Fungal revival", CardType = CardType.Normal, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Fungal revival", CardType = CardType.Normal, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Fungal feeding", CardType = CardType.Attack, OnUse = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Fungal feeding", CardType = CardType.Attack, OnUse = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Fungal regrowth", CardType = CardType.Defense, OnUse = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Fungal regrowth", CardType = CardType.Defense, OnUse = CardSpecialEffect.Heal1Health },
			}
		};
	}

	public Pack MossPack() {
		return new Pack() {
			Name = "Moss",
			Cards = new List<Card>() {
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
				new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
				new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
				new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
				new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
			}
		};
	}

	public List<Card> Packs(params Pack[] packs) {
		var deck = new List<Card>();
		foreach (var pack in packs)
			deck.AddRange(pack.Cards);
		return Util.Shuffle(deck);
	}

	public Creature Player(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new PlayerAi(),
			TeamName = "Player",
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
			DrawStack = Packs(AttackPack(), AdventurerPack()),
		};
	}

	public Creature Skeleton(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:skeleton",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 5,
			DrawStack = Packs(UndeadPack(), SkeletonPack()),
		};
	}

	public Creature Zombie(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:zombie",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 5,
			DrawStack = Packs(UndeadPack(), ZombiePack()),
		};
	}

	public Creature Vampire(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:vampire",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 5,
			DrawStack = Packs(UndeadPack(), VampirePack()),
		};
	}

	public Creature Ghost(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:ghost",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 5,
			DrawStack = Packs(UndeadPack(), GhostPack()),
		};
	}

	public Creature TreePerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:tree",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 6,
			MaximumHealth = 12,
			CurrentHealth = 12,
			MaximumHandCards = 6,
			DrawStack = Packs(FloraPack(), TreePack()),
		};
	}

	public Creature LeafPerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Elemental0:plant elemental",
			AttackValue = 4,
			MaximumAttackCards = 6,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 12,
			CurrentHealth = 12,
			MaximumHandCards = 6,
			DrawStack = Packs(FloraPack(), FloraPack()),
		};
	}

	public Creature ShroomPerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:shroom",
			AttackValue = 4,
			MaximumAttackCards = 4,
			DefenseValue = 4,
			MaximumDefenseCards = 4,
			MaximumHealth = 12,
			CurrentHealth = 12,
			MaximumHandCards = 8,
			DrawStack = Packs(FloraPack(), FungusPack()),
		};
	}

	public Creature MossMan(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:moss man",
			AttackValue = 4,
			MaximumAttackCards = 5,
			DefenseValue = 4,
			MaximumDefenseCards = 5,
			MaximumHealth = 12,
			CurrentHealth = 12,
			MaximumHandCards = 7,
			DrawStack = Packs(FloraPack(), MossPack()),
		};
	}

	public Creature RogueLizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:lizard",
			AttackValue = 3,
			MaximumAttackCards = 4,
			DefenseValue = 3,
			MaximumDefenseCards = 4,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
			DrawStack = Packs(GenericPack(), RoguePack()),
		};
	}

	public Creature AttackLizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:spear lizard",
			AttackValue = 3,
			MaximumAttackCards = 4,
			DefenseValue = 3,
			MaximumDefenseCards = 4,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
			DrawStack = Packs(GenericPack(), AttackPack()),
		};
	}

	public Creature DefenseLizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:shield lizard",
			AttackValue = 3,
			MaximumAttackCards = 4,
			DefenseValue = 3,
			MaximumDefenseCards = 4,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
			DrawStack = Packs(GenericPack(), DefensePack()),
		};
	}

	public Creature PriestLizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:chief lizard",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 5,
			DrawStack = Packs(GenericPack(), PriestPack()),
		};
	}

	public Creature Enemy(int x, int y) {
		return Util.Shuffle<Func<int,int,Creature>>(new List<Func<int,int,Creature>>() {
			Skeleton, Vampire, Ghost, Zombie,
			RogueLizard, AttackLizard, DefenseLizard, PriestLizard,
			TreePerson, LeafPerson, ShroomPerson, MossMan,
		})[0](x, y);
	}
}
