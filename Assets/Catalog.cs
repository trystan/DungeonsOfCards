using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Pack {
	public string Name;
	public List<Card> Cards;
}

public class Catalog {
	public List<Card> Cards = new List<Card>() {
		new Card() { Name = "Gold", GoldCost = 1, CardType = CardType.Normal },

		new Card() { Name = "Quick attack", GoldCost = 4, CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw3 },
		new Card() { Name = "Ready attack", GoldCost = 4, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
		new Card() { Name = "Quick defense", GoldCost = 4, CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw3 },
		new Card() { Name = "Ready defense", GoldCost = 4, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
		new Card() { Name = "Idle", GoldCost = 0, CardType = CardType.Normal },

		new Card() { Name = "Attack +1", GoldCost = 3, CardType = CardType.Attack, CombatBonus = 1 },
		new Card() { Name = "Attack +2", GoldCost = 4, CardType = CardType.Attack, CombatBonus = 2 },
		new Card() { Name = "Attack +3", GoldCost = 5, CardType = CardType.Attack, CombatBonus = 3 },
		new Card() { Name = "Attack +4", GoldCost = 6, CardType = CardType.Attack, CombatBonus = 4 },
		new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },

		new Card() { Name = "Defense +1", GoldCost = 3, CardType = CardType.Defense, CombatBonus = 1 },
		new Card() { Name = "Defense +2", GoldCost = 4, CardType = CardType.Defense, CombatBonus = 2 },
		new Card() { Name = "Defense +3", GoldCost = 5, CardType = CardType.Defense, CombatBonus = 3 },
		new Card() { Name = "Defense +4", GoldCost = 6, CardType = CardType.Defense, CombatBonus = 4 },
		new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },

		new Card() { Name = "Regenerate", GoldCost = 3, CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health },
		new Card() { Name = "Draw 3", GoldCost = 3, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },

		new Card() { Name = "Freeze closest", GoldCost = 4, CardType = CardType.Normal, StrongVs = "Lizards", OnUse = CardSpecialEffect.DamageClosest },
		new Card() { Name = "Burn closest", GoldCost = 4, CardType = CardType.Normal, StrongVs = "Plants", OnUse = CardSpecialEffect.DamageClosest },
		new Card() { Name = "Focus", GoldCost = 3, CardType = CardType.Attack, OnInHand = CardSpecialEffect.IncreaseHandSize },
		new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
		new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
		new Card() { Name = "Stab +1", GoldCost = 4, CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },

		new Card() { Name = "Turn undead", CardType = CardType.Normal, OnUse = CardSpecialEffect.TurnUndead },
		new Card() { Name = "Holy attack +1", GoldCost = 4, CardType = CardType.Attack, CombatBonus = 1, StrongVs = "Undead" },
		new Card() { Name = "Holy defense +1", GoldCost = 4, CardType = CardType.Defense, CombatBonus = 1, StrongVs = "Undead" },
		new Card() { Name = "Mass heal", GoldCost = 3, CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam },
		new Card() { Name = "Pray", CardType = CardType.Normal, OnUse = CardSpecialEffect.Pray },
	};

	public Card Card(string name) {
		var prototype = Cards.SingleOrDefault(c => c.Name == name);

		if (prototype == null)
			throw new ArgumentException("Card " + name + " does not exist in this catalog");

		var copy = new Card();

		foreach (var field in prototype.GetType().GetFields())
			field.SetValue(copy, field.GetValue(prototype));

		return copy;
	}

	public Item CardItem(int x, int y, Card card) {
		card.WorldPointOrigin = new Vector3(x,y,0);
		return new Item() {
			Position = new Point(x,y),
			SpriteName = "DawnLike/Items/Scroll:card",
			Card = card,
		};
	}

	public Item PackItem(int x, int y, Pack pack) {
		pack.Cards.ForEach(c => c.WorldPointOrigin = new Vector3(x,y,0));
		return new Item() {
			Position = new Point(x,y),
			SpriteName = "DawnLike/Items/Book:deck of cards",
			Pack = pack,
		};
	}

	public Pack MerchantPack() {
		return new Pack() {
			Name = "Merchant",
			Cards = new List<Card>() {
				Card("Quick attack"),
				Card("Quick attack"),
				Card("Ready attack"),
				Card("Ready attack"),
				Card("Quick defense"),
				Card("Quick defense"),
				Card("Ready defense"),
				Card("Ready defense"),
				Card("Idle"),
				Card("Idle"),
			}
		};
	}

	public Pack AdventurerPack() {
		return new Pack() {
			Name = "Adventurer",
			Cards = new List<Card>() {
				Card("Attack +2"),
				Card("Attack +2"),
				Card("Defense +2"),
				Card("Defense +2"),
				Card("Gold"),
				Card("Gold"),
				Card("Regenerate"),
				Card("Regenerate"),
				Card("Draw 3"),
				Card("Draw 3"),
			}
		};
	}

	public Pack WizardPack() {
		return new Pack() {
			Name = "Wizard",
			Cards = new List<Card>() {
				Card("Freeze closest"),
				Card("Burn closest"),
				Card("Draw 3"),
				Card("Draw 3"),
				Card("Focus"),
				Card("Focus"),
			}
		};
	}

	public Pack PriestPack() {
		return new Pack() {
			Name = "Priest",
			Cards = new List<Card>() {
				Card("Turn undead"),
				Card("Turn undead"),
				Card("Holy attack +1"),
				Card("Holy attack +1"),
				Card("Holy defense +1"),
				Card("Holy defense +1"),
				Card("Mass heal"),
				Card("Mass heal"),
				Card("Pray"),
				Card("Pray"),
			}
		};
	}

	public Pack RoguePack() {
		return new Pack() {
			Name = "Rogue",
			Cards = new List<Card>() {
				Card("Miss"),
				Card("Miss"),
				Card("Focus"),
				Card("Focus"),
				Card("Draw 3"),
				Card("Draw 3"),

				Card("Miss"),
				Card("Miss"),
				Card("Evade"),
				Card("Evade"),
				Card("Stab +1"),
				Card("Stab +1"),
			}
		};
	}

	public Pack AttackPack() {
		return new Pack() {
			Name = "Attack",
			Cards = new List<Card>() {
				Card("Attack +2"),
				Card("Attack +2"),
				Card("Attack +3"),
				Card("Attack +3"),
				Card("Attack focus"),
				Card("Attack focus"),
				Card("Quick attack"),
				Card("Quick attack"),
				Card("Ready attack"),
				Card("Ready attack"),
			}
		};
	}

	public Pack DefensePack() {
		return new Pack() {
			Name = "Defense",
			Cards = new List<Card>() {
				Card("Defense +2"),
				Card("Defense +2"),
				Card("Defense +3"),
				Card("Defense +3"),
				Card("Defense focus"),
				Card("Defense focus"),
				Card("Quick defense"),
				Card("Quick defense"),
				Card("Ready defense"),
				Card("Ready defense"),
			}
		};
	}

	public Pack GenericPack() {
		return new Pack() {
			Name = "Basic",
			Cards = new List<Card>() {
				Card("Attack +1"),
				Card("Attack +1"),
				Card("Attack +2"),
				Card("Attack +2"),
				Card("Defense +1"),
				Card("Defense +1"),
				Card("Defense +2"),
				Card("Defense +2"),
				Card("Idle"),
				Card("Idle"),
			}
		};
	}

	public Pack UndeadPack() {
		return new Pack() {
			Name = "Undead",
			Cards = new List<Card>() {
				Card("Attack +1"),
				Card("Attack +1"),
				Card("Defense +1"),
				Card("Defense +1"),
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
				Card("Attack +2"),
				Card("Attack +2"),
				Card("Defense +2"),
				Card("Defense +2"),
				new Card() { Name = "Bash +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				new Card() { Name = "Bash +1", CardType = CardType.Attack, DoesStopCombat = true, CombatBonus = 1 },
				Card("Idle"),
				Card("Idle"),
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
				Card("Evade"),
				Card("Evade"),
				Card("Idle"),
				Card("Idle"),
			}
		};
	}

	public Pack GhostPack() {
		return new Pack() {
			Name = "Ghost",
			Cards = new List<Card>() {
				Card("Attack +1"),
				Card("Attack +1"),
				Card("Miss"),
				Card("Miss"),
				Card("Idle"),
				Card("Idle"),
			}
		};
	}

	public Pack ZombiePack() {
		return new Pack() {
			Name = "Zombie",
			Cards = new List<Card>() {
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
				Card("Regenerate"),
				Card("Regenerate"),
				new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1Health },
				new Card() { Name = "Rot", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Lose1Health },
				new Card() { Name = "Rot", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Lose1Health },
				Card("Idle"),
				Card("Idle"),
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
				Card("Regenerate"),
				Card("Regenerate"),
				Card("Mass heal"),
				Card("Mass heal"),
				Card("Draw 3"),
				Card("Draw 3"),
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
				Card("Attack +3"),
				Card("Attack +3"),
				Card("Defense +2"),
				Card("Defense +2"),
			}
		};
	}

	public Pack FungusPack() {
		return new Pack() {
			Name = "Fungus",
			Cards = new List<Card>() {
				Card("Ready attack"),
				Card("Ready attack"),
				Card("Ready defense"),
				Card("Ready defense"),
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
				Card("Draw 3"),
				Card("Draw 3"),
				Card("Attack +2"),
				Card("Attack +2"),
				Card("Defense +2"),
				Card("Defense +2"),
				Card("Attack +3"),
				Card("Attack +3"),
				Card("Defense +3"),
				Card("Defense +3"),
			}
		};
	}

	public List<Card> Packs(Point start, params Pack[] packs) {
		var deck = new List<Card>();
		foreach (var pack in packs)
			deck.AddRange(pack.Cards);

		deck.ForEach(c => c.WorldPointOrigin = new Vector3(start.X, start.Y, 0));
		return Util.Shuffle(deck);
	}

	public Creature Merchant(int x, int y) {
		var pack = Util.Shuffle(new List<Pack>() { GenericPack(), AdventurerPack(), AttackPack(), DefensePack(), PriestPack(), WizardPack() })[0];
		return new Creature() {
			Name = "Merchant",
			Position = new Point(x, y),
			Ai = new MerchantAi() {
				Name = pack.Name + " pack merchant",
				CardsForSale = pack.Cards.Where(c => c.Name != "Gold").GroupBy(c => c.Name).Select(kv => kv.First()).ToList(),
			},
			TeamName = "Merchant",
			SpriteName = "DawnLike/Characters/Player0:merchant",
			AttackValue = 3,
			MaximumAttackCards = 8,
			DefenseValue = 3,
			MaximumDefenseCards = 8,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 12,
			DrawStack = Packs(new Point(x,y), AttackPack(), DefensePack(), RoguePack(), MerchantPack()),
		};
	}

	public Creature Player(int x, int y) {
		return new Creature() {
			Name = "Hero",
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
			DrawStack = Packs(new Point(x,y), AttackPack(), AdventurerPack()),
		};
	}

	public Creature Skeleton(int x, int y) {
		return new Creature() {
			Name = "Skeleton",
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
			DrawStack = Packs(new Point(x,y), UndeadPack(), SkeletonPack()),
		};
	}

	public Creature Zombie(int x, int y) {
		return new Creature() {
			Name = "Zombie",
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
			DrawStack = Packs(new Point(x,y), UndeadPack(), ZombiePack()),
		};
	}

	public Creature Vampire(int x, int y) {
		return new Creature() {
			Name = "Vampire",
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
			DrawStack = Packs(new Point(x,y), UndeadPack(), VampirePack()),
		};
	}

	public Creature Ghost(int x, int y) {
		return new Creature() {
			Name = "Ghost",
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
			DrawStack = Packs(new Point(x,y), UndeadPack(), GhostPack()),
		};
	}

	public Creature TreePerson(int x, int y) {
		return new Creature() {
			Name = "Tree",
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
			DrawStack = Packs(new Point(x,y), FloraPack(), TreePack()),
		};
	}

	public Creature LeafPerson(int x, int y) {
		return new Creature() {
			Name = "Leaf",
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
			DrawStack = Packs(new Point(x,y), FloraPack(), FloraPack()),
		};
	}

	public Creature ShroomPerson(int x, int y) {
		return new Creature() {
			Name = "Shroom",
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
			DrawStack = Packs(new Point(x,y), FloraPack(), FungusPack()),
		};
	}

	public Creature MossMan(int x, int y) {
		return new Creature() {
			Name = "Moss man",
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
			DrawStack = Packs(new Point(x,y), FloraPack(), MossPack()),
		};
	}

	public Creature RogueLizard(int x, int y) {
		return new Creature() {
			Name = "Rogue lizard",
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
			DrawStack = Packs(new Point(x,y), GenericPack(), RoguePack()),
		};
	}

	public Creature AttackLizard(int x, int y) {
		return new Creature() {
			Name = "Spear lizard",
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
			DrawStack = Packs(new Point(x,y), GenericPack(), AttackPack()),
		};
	}

	public Creature DefenseLizard(int x, int y) {
		return new Creature() {
			Name = "Shield lizard",
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
			DrawStack = Packs(new Point(x,y), GenericPack(), DefensePack()),
		};
	}

	public Creature PriestLizard(int x, int y) {
		return new Creature() {
			Name = "Priest lizard",
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
			DrawStack = Packs(new Point(x,y), GenericPack(), PriestPack()),
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
