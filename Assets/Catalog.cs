using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class Pack {
	public string Name;
	public List<Card> Cards;
}

public class Catalog {
	private List<Card> amuletCards = new List<Card>();

	public List<Card> Cards = new List<Card>();

	public Catalog() {
		Cards.AddRange(new List<Card>() {
			new Card() { Name = "Gold", GoldCost = 1, CardType = CardType.Normal,
				SpriteName = "DawnLike/Items/Money:gold",
				Description = "Gold cards don't do anything themselves, but can be traded with merchants for other cards.",
				FlavorText = "Why else would anyone go adventuring?", },

			new Card() { Name = "Quick attack", GoldCost = 4, CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw3,
				SpriteName = "DawnLike/Objects/Effect0:quick",
				FlavorText = "Victory goes to those who are quick to recover after an attack.", },
			new Card() { Name = "Ready attack", GoldCost = 4, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack,
				SpriteName = "DawnLike/Objects/Effect0:quick", },
			new Card() { Name = "Quick defense", GoldCost = 4, CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw3,
				SpriteName = "DawnLike/Objects/Effect0:quick",
				FlavorText = "Victory goes to those who are quick to recover after an attack.", },
			new Card() { Name = "Ready defense", GoldCost = 4, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense,
				SpriteName = "DawnLike/Objects/Effect0:quick", },
			new Card() { Name = "Idle", GoldCost = 0, CardType = CardType.Normal },

			new Card() { Name = "Attack +1", GoldCost = 3, CardType = CardType.Attack, CombatBonus = 1,
				SpriteName = "DawnLike/Items/MedWep:sword",
				Description = "Increase attack by 1.", },
			new Card() { Name = "Attack +2", GoldCost = 4, CardType = CardType.Attack, CombatBonus = 2,
				SpriteName = "DawnLike/Items/MedWep:sword",
				Description = "Increase attack by 2.", },
			new Card() { Name = "Attack +3", GoldCost = 5, CardType = CardType.Attack, CombatBonus = 3,
				SpriteName = "DawnLike/Items/MedWep:sword",
				Description = "Increase attack by 3.", },
			new Card() { Name = "Attack +4", GoldCost = 6, CardType = CardType.Attack, CombatBonus = 4,
				SpriteName = "DawnLike/Items/MedWep:sword",
				Description = "Increase attack by 4.", },
			new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize,
				SpriteName = "DawnLike/Items/MedWep:sword", },

			new Card() { Name = "Defense +1", GoldCost = 3, CardType = CardType.Defense, CombatBonus = 1,
				SpriteName = "DawnLike/Items/Shield:shield",
				Description = "Increase defense by 1.", },
			new Card() { Name = "Defense +2", GoldCost = 4, CardType = CardType.Defense, CombatBonus = 2,
				SpriteName = "DawnLike/Items/Shield:shield",
				Description = "Increase defense by 2.", },
			new Card() { Name = "Defense +3", GoldCost = 5, CardType = CardType.Defense, CombatBonus = 3,
				SpriteName = "DawnLike/Items/Shield:shield",
				Description = "Increase defense by 3.", },
			new Card() { Name = "Defense +4", GoldCost = 6, CardType = CardType.Defense, CombatBonus = 4,
				SpriteName = "DawnLike/Items/Shield:shield",
				Description = "Increase defense by 4.", },
			new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize,
				SpriteName = "DawnLike/Items/Shield:shield", },

			new Card() { Name = "Regenerate", GoldCost = 3, CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1Health,
				SpriteName = "DawnLike/Items/Potion:health", },
			new Card() { Name = "Draw 3", GoldCost = 3, CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3,
				SpriteName = "DawnLike/Items/Scroll:card", },

			new Card() { Name = "Freeze closest", GoldCost = 4, CardType = CardType.Normal, StrongVs = "Lizards", OnUse = CardSpecialEffect.DamageClosest,
				SpriteName = "DawnLike/Items/Wand:freeze wand", },
			new Card() { Name = "Burn closest", GoldCost = 4, CardType = CardType.Normal, StrongVs = "Plants", OnUse = CardSpecialEffect.DamageClosest,
				SpriteName = "DawnLike/Items/Wand:fire wand", },
			new Card() { Name = "Blink", GoldCost = 3, CardType = CardType.Normal, OnUse = CardSpecialEffect.Blink,
				SpriteName = "DawnLike/Items/Wand:blink wand", },
			new Card() { Name = "Focus", GoldCost = 3, CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseHandSize,
				SpriteName = "DawnLike/Items/Scroll:card", },
			new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true,
				SpriteName = "DawnLike/Objects/Effect0:miss", },
			new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade,
				SpriteName = "DawnLike/Objects/Effect0:miss", },
			new Card() { Name = "Stab +1", GoldCost = 4, CardType = CardType.Attack, CombatBonus = 1, OnUse = CardSpecialEffect.Discard1FromEachPile,
				SpriteName = "DawnLike/Items/ShortWep:sword" },

			new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton,
				SpriteName = "DawnLike/Characters/Undead0:skeleton" },

			new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1Health,
				SpriteName = "DawnLike/Items/Potion:big", },
			new Card() { Name = "Rot", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Lose1Health },

			new Card() { Name = "Turn undead", CardType = CardType.Normal, OnUse = CardSpecialEffect.TurnUndead,
				SpriteName = "DawnLike/Characters/Undead0:dark skeleton", },
			new Card() { Name = "Holy attack +1", GoldCost = 4, CardType = CardType.Attack, CombatBonus = 1, StrongVs = "Undead",
				SpriteName = "DawnLike/Items/ShortWep:holy" },
			new Card() { Name = "Holy defense +1", GoldCost = 4, CardType = CardType.Defense, CombatBonus = 1, StrongVs = "Undead",
				SpriteName = "DawnLike/Items/Shield:holy" },
			new Card() { Name = "Mass heal", GoldCost = 3, CardType = CardType.Normal, OnUse = CardSpecialEffect.HealTeam,
				SpriteName = "DawnLike/Items/Potion:big", },
			new Card() { Name = "Pray", CardType = CardType.Normal, OnUse = CardSpecialEffect.Pray,
					SpriteName = "DawnLike/Items/Book:pray", },

			new Card() { Name = "Amulet of attack -2", CardType = CardType.Attack, CombatBonus = -2,
				SpriteName = "DawnLike/Items/Amulet:amulet",
				Description = "One of three amulets you came here for. Decreases attack by 2.",
				FlavorText = "I saw its glow and was transformed. I lost all interest in attacking." },
			new Card() { Name = "Amulet of defense -2", CardType = CardType.Defense, CombatBonus = -2,
				SpriteName = "DawnLike/Items/Amulet:amulet",
				Description = "One of three amulets you came here for. Decreases defense by 2.",
				FlavorText = "I saw its glow and was transformed. I lost all interest in living." },
			new Card() { Name = "Amulet of hands -1", CardType = CardType.Normal, OnInHand = CardSpecialEffect.Discard1FromEachPile,
				SpriteName = "DawnLike/Items/Amulet:amulet",
				Description = "One of three amulets you came here for. Forces you to discard.",
				FlavorText = "I saw its glow and was transformed. I lost all interest in things." },

			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther,
				SpriteName = "DawnLike/Characters/Undead0:disease",
				ExtraCard = () => Card("Diseased") },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther,
				SpriteName = "DawnLike/Characters/Undead0:disease",
				ExtraCard = () => Card("Diseased") },
			new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile,
				SpriteName = "DawnLike/Characters/Undead0:disease",
				FlavorText = "You weren't fighting undead, were you?" },
			
			new Card() { Name = "Leafs", CardType = CardType.Normal,
				SpriteName = "DawnLike/Characters/Plant0:leafs",
				FlavorText = "Stupid plant people dumping useless leafs everywhere." },

			new Card() { Name = "Plant attack +1", CardType = CardType.Attack, CombatBonus = 1, OnUse = CardSpecialEffect.AddCardToOther, 
				SpriteName = "DawnLike/Characters/Plant0:leafs",
				ExtraCard = () => Card("Leafs") },
			new Card() { Name = "Plant armor +1", CardType = CardType.Defense, CombatBonus = 1, OnUse = CardSpecialEffect.AddCardToOther, 
				SpriteName = "DawnLike/Characters/Plant0:leafs",
				ExtraCard = () => Card("Leafs") },

			new Card() { Name = "Thief's getaway", CardType = CardType.Normal, 
				SpriteName = "DawnLike/items/Boot:boot",
				OnInHand = CardSpecialEffect.IncreaseAllSizes,
				OnUse = CardSpecialEffect.Blink },
			new Card() { Name = "Spear +3", CardType = CardType.Normal, 
				SpriteName = "DawnLike/Items/LongWep:spear",
				OnUse = CardSpecialEffect.AddCardToSelf, ExtraCard = () => Card("Attack +1"),
				OnInHand = CardSpecialEffect.IncreaseAttackValue3 },
			new Card() { Name = "Shield +3", CardType = CardType.Normal, 
				SpriteName = "DawnLike/Items/Shield:big",
				OnUse = CardSpecialEffect.AddCardToSelf, ExtraCard = () => Card("Defense +1"),
				OnInHand = CardSpecialEffect.IncreaseDefenseValue3 },
			new Card() { Name = "Vestments", CardType = CardType.Normal, 
				SpriteName = "DawnLike/Items/Armor:vestments",
				OnDraw = CardSpecialEffect.Heal1Health,
				OnUse = CardSpecialEffect.HealTeam,
				OnInHand = CardSpecialEffect.IncreaseAllSizes },

			new Card() { Name = "Vampire bite +1", CardType = CardType.Attack, CombatBonus = 1, OnUse = CardSpecialEffect.VampireBite,
					SpriteName = "DawnLike/Characters/Undead0:vampire bite", },
			new Card() { Name = "Ghost form +1", CardType = CardType.Defense, CombatBonus = 1, OnUse = CardSpecialEffect.GhostForm,
				SpriteName = "DawnLike/Characters/Undead0:ghost form", },
			new Card() { Name = "Bones", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAllStats,
				SpriteName = "DawnLike/Characters/Undead0:skeleton 2", },
			new Card() { Name = "Zombie rot", CardType = CardType.Normal,
				SpriteName = "DawnLike/Characters/Undead0:zombie",
				OnDraw = CardSpecialEffect.AddCardToOther, ExtraCard = () => Card("Diseased"),
				OnUse = CardSpecialEffect.HealTeam },

			new Card() { Name = "Branches", CardType = CardType.Normal, 
				SpriteName = "DawnLike/Characters/Plant0:tree",
				OnUse = CardSpecialEffect.Draw3,
				OnInHand = CardSpecialEffect.IncreaseAllSizes },
			new Card() { Name = "Bloom", CardType = CardType.Normal, OnUse = CardSpecialEffect.AddCardToOther, ExtraCard = () => Card("Leafs"),
				SpriteName = "DawnLike/Characters/Plant0:bloom" },
			new Card() { Name = "Spores", CardType = CardType.Defense,
				SpriteName = "DawnLike/Characters/Plant0:spores",
				OnDraw = CardSpecialEffect.AddCardToSelf,
				OnUse = CardSpecialEffect.AddCardToSelf, ExtraCard = () => Card("Regenerate") },
			new Card() { Name = "Charge", CardType = CardType.Normal, OnUse = CardSpecialEffect.ChargeNearest,
				SpriteName = "DawnLike/Characters/Plant0:moss man", },

			new Card() { Name = "Aura +2", GoldCost = 4, CardType = CardType.Attack, 
				StrongVs = "Undead", CombatBonus = 2,
				OnDraw = CardSpecialEffect.HealTeam,
				OnUse = CardSpecialEffect.AddCardToOther, ExtraCard = () => Card("Idle"),
				SpriteName = "DawnLike/Objects/Effect0:quick", },
			new Card() { Name = "Poisoned", GoldCost = 0, CardType = CardType.Normal, 
				OnDraw = CardSpecialEffect.Lose1Health, },
			new Card() { Name = "Dart +2", GoldCost = 4, CardType = CardType.Normal, 
				OnUse = CardSpecialEffect.DamageClosest, ExtraCard = () => Card("Poisoned"),
				SpriteName = "DawnLike/Items/Ammo:dart", },
			new Card() { Name = "Purge", GoldCost = 5, CardType = CardType.Normal, 
				OnUse = CardSpecialEffect.DestoryChosenCard,
				SpriteName = "DawnLike/Items/Ammo:dart", },
			new Card() { Name = "Curse", GoldCost = 4, CardType = CardType.Normal, 
				OnUse = CardSpecialEffect.DamageClosest, ExtraCard = () => Card("Cursed"),
				SpriteName = "DawnLike/Objects/Effect0:quick", },
			new Card() { Name = "Cursed", GoldCost = 0, CardType = CardType.Normal, 
				OnInHand = CardSpecialEffect.ReduceAllSizes, 
				SpriteName = "DawnLike/Objects/Effect0:quick" },
		});

		amuletCards = Cards.Where(c => c.Name.StartsWith("Amulet of ")).ToList();
	}

	public Card AmuletCard(int number) {
		return amuletCards[number];
	}

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

	public Pack GoldPack() {
		return new Pack() {
			Name = "Gold",
			Cards = new List<Card>() {
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
				Card("Gold"),
			}
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
				Card("Freeze closest"),
				Card("Burn closest"),
				Card("Burn closest"),
				Card("Draw 3"),
				Card("Draw 3"),
				Card("Focus"),
				Card("Focus"),
				Card("Blink"),
				Card("Blink"),
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

	public Pack BasicPack() {
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
				Card("Skeleton"),
				Card("Skeleton"),
				Card("Disease touched"),
				Card("Disease touched"),
				Card("Disease touch"),
				Card("Disease touch"),
			}
		};
	}

	public Pack FloraPack() {
		return new Pack() {
			Name = "Flora",
			Cards = new List<Card>() {
				Card("Plant attack +1"),
				Card("Plant attack +1"),
				Card("Plant armor +1"),
				Card("Plant armor +1"),
				Card("Regenerate"),
				Card("Regenerate"),
				Card("Mass heal"),
				Card("Mass heal"),
				Card("Draw 3"),
				Card("Draw 3"),
			}
		};
	}

	public Pack RarePack() {
		return new Pack() {
			Name = "Rare",
			Cards = new List<Card>() {
				Card("Skeleton"),
				Card("Skeleton"),
				Card("Attack +4"),
				Card("Attack +4"),
				Card("Defense +4"),
				Card("Defense +4"),
				Card("Attack +3"),
				Card("Attack +3"),
				Card("Defense +3"),
				Card("Defense +3"),
			}
		};
	}

	public Pack EquipmentPack() {
		return new Pack() {
			Name = "Equipment",
			Cards = new List<Card>() {
				Card("Spear +3"),
				Card("Spear +3"),
				Card("Shield +3"),
				Card("Shield +3"),
				Card("Vestments"),
				Card("Vestments"),
				Card("Thief's getaway"),
				Card("Thief's getaway"),
				Card("Dart +2"),
				Card("Dart +2"),
			}
		};
	}

	public Creature Init(Creature creature) {
		var deck = new List<Card>();
		deck.AddRange(creature.DrawPile);
		creature.DrawPile.Clear();

		foreach (var pack in creature.Packs)
			deck.AddRange(pack.Cards);

		deck.ForEach(c => c.WorldPointOrigin = new Vector3(creature.Position.X, creature.Position.Y, 0));
		creature.DrawPile.AddRange(Util.Shuffle(deck));

		return creature;
	}

	public Creature Merchant(int x, int y) {
		var cost5Pack = new Pack() {
			Name = "5 gold",
			Cards = Util.Shuffle(Cards.Where(c => c.GoldCost == 5).Select(c => c.Name)).Take(5).Select(n => Card(n)).ToList(),
		};
		var cost4Pack = new Pack() {
			Name = "4 gold",
			Cards = Util.Shuffle(Cards.Where(c => c.GoldCost == 4).Select(c => c.Name)).Take(5).Select(n => Card(n)).ToList(),
		};
		var cost3Pack = new Pack() {
			Name = "3 gold",
			Cards = Util.Shuffle(Cards.Where(c => c.GoldCost == 3).Select(c => c.Name)).Take(5).Select(n => Card(n)).ToList(),
		};
		var cost2Pack = new Pack() {
			Name = "2 gold",
			Cards = Util.Shuffle(Cards.Where(c => c.GoldCost == 2).Select(c => c.Name)).Take(5).Select(n => Card(n)).ToList(),
		};

		var pack = Util.Shuffle(new List<Pack>() { 
			BasicPack(), AdventurerPack(), EquipmentPack(), 
			AttackPack(), DefensePack(), PriestPack(), RoguePack(), WizardPack(),
			RarePack(), cost2Pack, cost3Pack, cost4Pack, cost5Pack,
		})[0];

		return Init(new Creature() {
			Name = "Merchant",
			Packs = new [] { DefensePack(), AttackPack() },
			Position = new Point(x, y),
			Ai = new MerchantAi() {
				Name = pack.Name + " pack merchant",
				CardsForSale = pack.Cards.Where(c => c.Name != "Gold").GroupBy(c => c.Name).Select(kv => kv.First()).ToList(),
			},
			TeamName = "Merchant",
			SpriteName = "DawnLike/Characters/Player0:merchant",
			AttackValue = 5,
			MaximumAttackCards = 8,
			DefenseValue = 5,
			MaximumDefenseCards = 8,
			MaximumHealth = 20,
			CurrentHealth = 20,
			MaximumHandCards = 12,
		});
	}

	public Creature Witch(int x, int y) {
		return Init(new Creature() {
			Name = "Witch",
			Packs = new [] { PriestPack(), WizardPack() },
			DrawPile = new List<Card>() { Card("Curse"), Card("Curse") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Human",
			SpriteName = "DawnLike/Characters/Player0:witch",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 5,
		});
	}

	public Creature Monk(int x, int y) {
		return Init(new Creature() {
			Name = "Monk",
			Packs = new [] { RoguePack(), PriestPack() },
			DrawPile = new List<Card>() { Card("Purge"), Card("Purge") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Human",
			SpriteName = "DawnLike/Characters/Player0:monk",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 5,
		});
	}

	public Creature Assassin(int x, int y) {
		return Init(new Creature() {
			Name = "Assassin",
			Packs = new [] { AttackPack(), RoguePack() },
			DrawPile = new List<Card>() { Card("Dart +2"), Card("Dart +2") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Human",
			SpriteName = "DawnLike/Characters/Player0:assassin",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 5,
		});
	}

	public Creature Palladin(int x, int y) {
		return Init(new Creature() {
			Name = "Palladin",
			Packs = new [] { DefensePack(), PriestPack() },
			DrawPile = new List<Card>() { Card("Aura +2"), Card("Aura +2") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Human",
			SpriteName = "DawnLike/Characters/Player0:palladin",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 5,
		});
	}

	public Creature Skeleton(int x, int y) {
		return Init(new Creature() {
			Name = "Skeleton",
			Packs = new [] { UndeadPack(), AttackPack() },
			DrawPile = new List<Card>() { Card("Bones"), Card("Bones") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:skeleton",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
		});
	}

	public Creature Zombie(int x, int y) {
		return Init(new Creature() {
			Name = "Zombie",
			Packs = new [] { UndeadPack(), DefensePack() },
			DrawPile = new List<Card>() { Card("Zombie rot"), Card("Zombie rot") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:zombie",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
		});
	}

	public Creature Vampire(int x, int y) {
		return Init(new Creature() {
			Name = "Vampire",
			Packs = new [] { UndeadPack(), WizardPack() },
			DrawPile = new List<Card>() { Card("Vampire bite +1"), Card("Vampire bite +1") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:vampire",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
		});
	}

	public Creature Ghost(int x, int y) {
		return Init(new Creature() {
			Name = "Ghost",
			Packs = new [] { UndeadPack(), RoguePack() },
			DrawPile = new List<Card>() { Card("Ghost form +1"), Card("Ghost form +1") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:ghost",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
		});
	}

	public Creature TreePerson(int x, int y) {
		return Init(new Creature() {
			Name = "Tree",
			Packs = new [] { FloraPack(), BasicPack() },
			DrawPile = new List<Card>() { Card("Branches"), Card("Branches") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:tree",
			AttackValue = 1,
			MaximumAttackCards = 3,
			DefenseValue = 1,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
		});
	}

	public Creature LeafPerson(int x, int y) {
		return Init(new Creature() {
			Name = "Leaf",
			Packs = new [] { FloraPack(), BasicPack() },
			DrawPile = new List<Card>() { Card("Bloom"), Card("Bloom") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Elemental0:plant elemental",
			AttackValue = 1,
			MaximumAttackCards = 3,
			DefenseValue = 1,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
		});
	}

	public Creature ShroomPerson(int x, int y) {
		return Init(new Creature() {
			Name = "Shroom",
			Packs = new [] { FloraPack(), BasicPack() },
			DrawPile = new List<Card>() { Card("Spores"), Card("Spores") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:shroom",
			AttackValue = 1,
			MaximumAttackCards = 3,
			DefenseValue = 1,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
		});
	}

	public Creature MossMan(int x, int y) {
		return Init(new Creature() {
			Name = "Moss man",
			Packs = new [] { FloraPack(), BasicPack() },
			DrawPile = new List<Card>() { Card("Charge"), Card("Charge") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:moss man",
			AttackValue = 1,
			MaximumAttackCards = 3,
			DefenseValue = 1,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 6,
		});
	}

	public Creature RogueLizard(int x, int y) {
		return Init(new Creature() {
			Name = "Rogue lizard",
			Packs = new [] { BasicPack(), RoguePack() },
			DrawPile = new List<Card>() { Card("Thief's getaway"), Card("Thief's getaway") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:lizard",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 4,
		});
	}

	public Creature AttackLizard(int x, int y) {
		return Init(new Creature() {
			Name = "Spear lizard",
			Packs = new [] { BasicPack(), AttackPack() },
			DrawPile = new List<Card>() { Card("Spear +3"), Card("Spear +3") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:spear lizard",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 4,
		});
	}

	public Creature DefenseLizard(int x, int y) {
		return Init(new Creature() {
			Name = "Shield lizard",
			Packs = new [] { BasicPack(), DefensePack() },
			DrawPile = new List<Card>() { Card("Shield +3"), Card("Shield +3") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:shield lizard",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 4,
		});
	}

	public Creature PriestLizard(int x, int y) {
		return Init(new Creature() {
			Name = "Priest lizard",
			Packs = new [] { BasicPack(), PriestPack() },
			DrawPile = new List<Card>() { Card("Vestments"), Card("Vestments") },
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:chief lizard",
			AttackValue = 2,
			MaximumAttackCards = 3,
			DefenseValue = 2,
			MaximumDefenseCards = 3,
			MaximumHealth = 10,
			CurrentHealth = 10,
			MaximumHandCards = 4,
		});
	}

	public Creature Enemy(int x, int y) {
		return Util.Shuffle<Func<int,int,Creature>>(new List<Func<int,int,Creature>>() {
			Skeleton, Vampire, Ghost, Zombie,
			RogueLizard, AttackLizard, DefenseLizard, PriestLizard,
			TreePerson, LeafPerson, ShroomPerson, MossMan,
			Witch, Monk, Palladin, Assassin,
		})[0](x, y);
	}
}
