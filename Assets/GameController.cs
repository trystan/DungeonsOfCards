using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;

public class GameController : MonoBehaviour {
	public Instantiator Instantiator;
	public GuiController guiController;
	public TileMesh FloorTileMesh;
	public TileMesh WallTileMesh;

	Game game;
	List<CreatureView> CreatureViews = new List<CreatureView>();
	List<CardView> CardViews = new List<CardView>();

	void Start() {
		game = new Game(20, 20) {
			Catalog = new Catalog(),
		};

		new LevelBuilder().Build(game);

		FloorTileMesh.ShowLevel(new FloorView(game));
		WallTileMesh.ShowLevel(new WallView(game));

		foreach (var c in game.Creatures)
			CreatureViews.Add(Instantiator.Add(c));

		foreach (var c in game.Player.DrawStack)
			CardViews.Add(Instantiator.Add(game, c, game.Player));
		
		Camera.main.GetComponent<CameraController>().Follow(CreatureViews[0].gameObject);

		guiController.Show(game.Player);
	}

	void Update() {
		foreach (var c in game.NewCards)
			CardViews.Add(Instantiator.Add(game, c, game.Player));
		game.NewCards.Clear();

		game.Effects.ForEach(e => e.Delay -= Time.deltaTime);
		foreach (var e in game.Effects.Where(e => e.Delay < 0).ToList()) {
			e.Callback(game);
			game.Effects.Remove(e);
		}

		foreach (var popup in game.Popups)
			Instantiator.Add(popup);
		game.Popups.Clear();

		if (CreatureViews.Any(v => v.IsMoving))
			return;

		if (game.Player.Exists) {
			var mx = 0;
			if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.L) || Input.GetKey(KeyCode.Keypad6))
				mx = 1;
			if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.H) || Input.GetKey(KeyCode.Keypad4))
				mx = -1;

			var my = 0;
			if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.Keypad8))
				my = 1;
			if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.J) || Input.GetKey(KeyCode.Keypad2))
				my = -1;
			
			var wait = Input.GetKey(KeyCode.Period) || Input.GetKey(KeyCode.Keypad5);

			if (mx != 0 || my != 0 || wait) {
				game.Player.MoveBy(game, mx, my);
				game.TakeTurn();
			}
		} else {
			game.TakeTurn();
		}
	}
}

public class Catalog {
	public List<Card> AdventurerPack() {
		return new List<Card>() {
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Gold", CardType = CardType.Normal },
			new Card() { Name = "Gold", CardType = CardType.Normal },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
			new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
		};
	}

	public List<Card> AttackPack() {
		return new List<Card>() {
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
			new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
			new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
			new Card() { Name = "Attack focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseAttackSize },
			new Card() { Name = "Quick attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw5Attack },
			new Card() { Name = "Quick attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.Draw5Attack },
			new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
			new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
		};
	}

	public List<Card> DefensePack() {
		return new List<Card>() {
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
			new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
			new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
			new Card() { Name = "Defense focus", CardType = CardType.Normal, OnInHand = CardSpecialEffect.IncreaseDefenseSize },
			new Card() { Name = "Quick defense", CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw5Defense },
			new Card() { Name = "Quick defense", CardType = CardType.Defense, OnUse = CardSpecialEffect.Draw5Defense },
			new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
			new Card() { Name = "Ready defense", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Defense },
		};
	}

	public List<Card> GenericPack() {
		return new List<Card>() {
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
		};
	}

	public List<Card> SkeletonPack() {
		return new List<Card>() {
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
			new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
		};
	}

	public List<Card> VampirePack() {
		return new List<Card>() {
			new Card() { Name = "Blood suck", CardType = CardType.Attack, OnUse = CardSpecialEffect.Vampire1 },
			new Card() { Name = "Blood suck", CardType = CardType.Attack, OnUse = CardSpecialEffect.Vampire1 },
			new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
			new Card() { Name = "Evade", CardType = CardType.Defense, DoesStopCombat = true, OnUse = CardSpecialEffect.Evade },
			new Card() { Name = "Bat form", CardType = CardType.Defense },
			new Card() { Name = "Bat form", CardType = CardType.Defense },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
		};
	}

	public List<Card> GhostPack() {
		return new List<Card>() {
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
			new Card() { Name = "Miss", CardType = CardType.Defense, DoesBlockOtherCard = true },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
		};
	}

	public List<Card> ZombiePack() {
		return new List<Card>() {
			new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
			new Card() { Name = "Skeleton", CardType = CardType.Defense, OnDie = CardSpecialEffect.SpawnSkeleton },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1 },
			new Card() { Name = "Revive", CardType = CardType.Defense, OnDie = CardSpecialEffect.Heal1 },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touched", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
			new Card() { Name = "Disease touch", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Diseased", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromEachPile } },
		};
	}

	public List<Card> FloraPack() {
		return new List<Card>() {
			new Card() { Name = "Planet attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
			new Card() { Name = "Planet attack", CardType = CardType.Attack, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
			new Card() { Name = "Planet armor", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
			new Card() { Name = "Planet armor", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToOther, 
				ExtraCard = () => new Card() { Name = "Leafs", CardType = CardType.Normal } },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Regenerate", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Regrowth", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToSelf,
				ExtraCard = () => new Card() { Name = "Regenerate", CardType = CardType.Normal, OnUse = CardSpecialEffect.Heal1 } },
			new Card() { Name = "Regrowth", CardType = CardType.Defense, OnUse = CardSpecialEffect.AddCardToSelf,
				ExtraCard = () => new Card() { Name = "Regenerate", CardType = CardType.Normal, OnUse = CardSpecialEffect.Heal1 } },
		};
	}

	public List<Card> Packs(params List<Card>[] packs) {
		var deck = new List<Card>();
		foreach (var pack in packs)
			deck.AddRange(pack);
		return Util.Shuffle(deck);
	}

	public Creature Player(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new PlayerAi(),
			TeamName = "Player",
			SpriteName = "DawnLike/Characters/Player0:Player0_1",
			AttackValue = 2,
			MaximumAttackCards = 2,
			DefenseValue = 2,
			MaximumDefenseCards = 2,
			MaximumHealth = 8,
			CurrentHealth = 8,
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
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 6,
			CurrentHealth = 6,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), SkeletonPack()),
		};
	}

	public Creature Zombie(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:zombie",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 6,
			CurrentHealth = 6,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), ZombiePack()),
		};
	}

	public Creature Vampire(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:vampire",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 6,
			CurrentHealth = 6,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), VampirePack()),
		};
	}

	public Creature Ghost(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Undead",
			SpriteName = "DawnLike/Characters/Undead0:ghost",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 6,
			CurrentHealth = 6,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), GhostPack()),
		};
	}

	public Creature TreePerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:tree",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), FloraPack()),
		};
	}

	public Creature LeafPerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Elemental0:plant elemental",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), FloraPack()),
		};
	}

	public Creature ShroomPerson(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Flora",
			SpriteName = "DawnLike/Characters/Plant0:shroom",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), FloraPack()),
		};
	}

	public Creature Lizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:lizard",
			AttackValue = 2,
			MaximumAttackCards = 2,
			DefenseValue = 2,
			MaximumDefenseCards = 2,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
			DrawStack = Packs(GenericPack(), AdventurerPack()),
		};
	}

	public Creature AttackLizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:spear lizard",
			AttackValue = 2,
			MaximumAttackCards = 2,
			DefenseValue = 2,
			MaximumDefenseCards = 2,
			MaximumHealth = 8,
			CurrentHealth = 8,
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
			AttackValue = 2,
			MaximumAttackCards = 2,
			DefenseValue = 2,
			MaximumDefenseCards = 2,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 6,
			DrawStack = Packs(GenericPack(), DefensePack()),
		};
	}

	public Creature Enemy(int x, int y) {
		return Util.Shuffle<Func<int,int,Creature>>(new List<Func<int,int,Creature>>() {
			Skeleton, Vampire, Ghost, Zombie
//			Lizard, AttackLizard, DefenseLizard,
//			TreePerson, LeafPerson, ShroomPerson,
		})[0](x, y);
	}
}

public class TextPopup {
	public float TTL = 1f;
	public string Text;
	public Vector3 WorldPosition;
	public Vector3 Offset = new Vector3(0,0.5f,0);

	public TextPopup(string text, Point worldPosition, Vector3 offset) {
		Text = text;
		WorldPosition = new Vector3(worldPosition.X, worldPosition.Y, 0);
		Offset = offset;
	}
}

public struct Point {
	public int X;
	public int Y;

	public Point(int x, int y) {
		X = x;
		Y = y;
	}

	public static Point operator +(Point a, Point b) {
		return new Point(a.X + b.X, a.Y + b.Y);
	}

	public static Point operator -(Point a, Point b) {
		return new Point(a.X - b.X, a.Y - b.Y);
	}

	public static bool operator ==(Point a, Point b) {
		return a.Equals(b);
	}

	public static bool operator !=(Point a, Point b) {
		return !a.Equals(b);
	}

	public override bool Equals(object obj) {
		if (obj == null)
			return false;
		if (ReferenceEquals (this, obj))
			return true;
		if (obj.GetType () != typeof(Point))
			return false;
		Point other = (Point)obj;
		return X == other.X && Y == other.Y;
	}

	public override int GetHashCode() {
		unchecked {
			return X.GetHashCode() ^ Y.GetHashCode();
		}
	}
}

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
			case CardSpecialEffect.Heal1: {
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
						var strength = 3 + creature.AttackValue + creature.AttackStack.Count - other.DefenseValue - other.DefenseStack.Count;
						if (strength > 0)
							cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, strength);
					}
				} else if (!game.GetTile(neighbor.X, neighbor.Y).BlocksMovement) {
					var strength = 3 - GetDanger(game, creature, neighbor);
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

public enum CardType {
	Attack, Defense, Normal
}

public enum CardSpecialEffect {
	None, Discard1FromEachPile, Heal1, Draw3, Vampire1, Evade,
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
		case CardSpecialEffect.Heal1:
			if (user.CurrentHealth < user.MaximumHealth)
				user.CurrentHealth++;
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
			break;
		default:
			throw new NotImplementedException(Name + " " + effect);
		}

		if (effect != CardSpecialEffect.None && effect == OnUse || DoesBlockOtherCard || DoesStopCombat)
			game.Popups.Add(new TextPopup(Name, user.Position, new Vector3(0,4,0)));
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

public class Creature {
	public bool Exists = true;
	public Point Position;
	public string SpriteName;
	public string TeamName;
	public AI Ai;

	public int AttackValue;
	public int MaximumAttackCards;
	public int DefenseValue;
	public int MaximumDefenseCards;
	public int MaximumHealth;
	public int CurrentHealth;
	public int MaximumHandCards;

	public List<Card> DrawStack = new List<Card>();
	public List<Card> AttackStack = new List<Card>();
	public List<Card> DefenseStack = new List<Card>();
	public List<Card> HandStack = new List<Card>();
	public List<Card> DiscardStack = new List<Card>();

	public void MoveBy(Game game, int mx, int my) {
		var next = Position + new Point(mx, my);

		if (game.GetTile(Position.X + mx, Position.Y + my).BlocksMovement) {
			EndTurn(game);
			return;
		}
		
		var other = game.GetCreature(next);

		if (other != null && other != this) {
			if (other.TeamName != TeamName)
				Attack(game, other);
		} else {
			Position += new Point(mx, my);
			Draw1Card(game);
		}

		EndTurn(game);
	}

	public void UseCard(Game game, Card card) {
		HandStack.Remove(card);

		card.DoAction(game, this, null, card.OnUse);
		game.Popups.Add(new TextPopup(card.Name, Position, new Vector3(0,4,0)));
		DiscardStack.Add(card);
	}

	void ReshuffleIntoDrawStack() {
		if (!DiscardStack.Any()) {
			DiscardStack.AddRange(AttackStack);
			AttackStack.Clear();
			DiscardStack.AddRange(DefenseStack);
			DefenseStack.Clear();
			DiscardStack.AddRange(HandStack);
			HandStack.Clear();
		}
		DrawStack.AddRange(Util.Shuffle(DiscardStack));
		DiscardStack.Clear();
	}

	public Card GetTopDrawCard() {
		if (!DrawStack.Any())
			ReshuffleIntoDrawStack();
		
		var pulledCard = DrawStack.Last();
		DrawStack.Remove(pulledCard);
		return pulledCard;
	}

	public void Draw1Card(Game game) {
		KeepCard(game, GetTopDrawCard());
	}

	public void KeepCard(Game game, Card pulledCard) {
		pulledCard.DoAction(game, this, null, pulledCard.OnDraw);

		if (pulledCard.CardType == CardType.Attack) {
			AttackStack.Add(pulledCard);
			while (AttackStack.Count > MaximumAttackCards) {
				var toDiscard = AttackStack[0];
				AttackStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		} else if (pulledCard.CardType == CardType.Defense) {
			DefenseStack.Add(pulledCard);
			while (DefenseStack.Count > MaximumDefenseCards) {
				var toDiscard = DefenseStack[0];
				DefenseStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		} else {
			HandStack.Add(pulledCard);
			pulledCard.DoAction(game, this, null, pulledCard.OnInHand);
			while (HandStack.Count > MaximumHandCards) {
				var toDiscard = HandStack[0];
				HandStack.RemoveAt(0);
				toDiscard.UndoAction(game, this, toDiscard.OnInHand);
				DiscardStack.Add(toDiscard);
			}
		}
	}

	void EndTurn(Game game) {
		while (AttackStack.Count > MaximumAttackCards) {
			var toDiscard = AttackStack[0];
			AttackStack.RemoveAt(0);
			DiscardStack.Add(toDiscard);
		}
		while (DefenseStack.Count > MaximumDefenseCards) {
			var toDiscard = DefenseStack[0];
			DefenseStack.RemoveAt(0);
			DiscardStack.Add(toDiscard);
		}
		while (HandStack.Count > MaximumHandCards) {
			var toDiscard = HandStack[0];
			HandStack.RemoveAt(0);
			toDiscard.UndoAction(game, this, toDiscard.OnInHand);
			DiscardStack.Add(toDiscard);
		}
	}

	public void TakeTurn(Game game) {
		Ai.TakeTurn(game, this);
	}

	public void Attack(Game game, Creature other) {
		new Combat(game, this, other).Resolve();
	}

	public void TakeDamage(int amount) {
		CurrentHealth -= amount;
		if (CurrentHealth <= 0)
			Die();
	}

	public void Die() {
		Exists = false;
	}
}

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

		var maxCards = Mathf.Max(Attacker.AttackStack.Count, Defender.DefenseStack.Count);

		for (var i = 0; i < maxCards; i++) {
			var defenderCard = Defender.DefenseStack.Count > i ? Defender.DefenseStack[i] : null;
			var attackerCard = Attacker.AttackStack.Count > i ? Attacker.AttackStack[i] : null;
			var blockAttack = false;
			var endsCombat = false;

			if (defenderCard != null) {
				defenderDefense += defenderCard.CombatBonus;
				blockAttack = defenderCard.DoesBlockOtherCard;
				endsCombat = defenderCard.DoesStopCombat;
					
				defenderCard.DoAction(Game, Defender, Attacker, defenderCard.OnUse);

				var popup = new TextPopup(defenderCard.Name, Defender.Position, new Vector3(0,14 * i,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f,
					Callback = g => g.Popups.Add(popup),
				});
			}

			if (attackerCard != null && !blockAttack) {
				attackerAttack += attackerCard.CombatBonus;
				endsCombat = endsCombat || attackerCard.DoesStopCombat;

				attackerCard.DoAction(Game, Attacker, Defender, attackerCard.OnUse);

				var popup = new TextPopup(attackerCard.Name, Attacker.Position, new Vector3(0,14 * i + 7,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f + 0.05f,
					Callback = g => g.Popups.Add(popup),
				});
			}

			if (endsCombat)
				break;
		}

		Defender.TakeDamage(Mathf.Max(1, attackerAttack - defenderDefense));

		if (!Defender.Exists)
			Defender.DefenseStack.ForEach(c => c.DoAction(Game, Defender, Attacker, c.OnDie));

		Attacker.DiscardStack.AddRange(Attacker.AttackStack);
		Attacker.AttackStack.Clear();

		Defender.DiscardStack.AddRange(Defender.DefenseStack);
		Defender.DefenseStack.Clear();
	}
}

public static class Util {
	public static List<T> Shuffle<T>(List<T> things) {
		var newList = new List<T>();
		var list = things.Select(x => x).ToList();
		while (list.Any()) {
			var i = UnityEngine.Random.Range(0, list.Count);
			newList.Add(list[i]);
			list.RemoveAt(i);
		}
		return newList;
	}

	public static T WeightedChoice<T>(Dictionary<T, int> choices) {
		var i = UnityEngine.Random.Range(0, choices.Values.Sum());
		foreach (var kv in choices) {
			if (i <= kv.Value)
				return kv.Key;
			i -= kv.Value;
		}
		return choices.First().Key;
	}
}

public class Tile {
	public static Tile OutOfBounds = new Tile { Index = 8, BlocksMovement = true };

	public static Tile Floor = new Tile { Index = 0 };
	public static Tile Wall = new Tile { Index = 1, BlocksMovement = true };

	public int Index;
	public bool BlocksMovement;
}

public class DelayedEffect {
	public float Delay;
	public Action<Game> Callback;
}

public class Game {
	public List<Creature> Creatures = new List<Creature>();
	public List<DelayedEffect> Effects = new List<DelayedEffect>();
	public List<TextPopup> Popups = new List<TextPopup>();
	public List<Card> NewCards = new List<Card>();
	public Catalog Catalog;
	public Creature Player;

	public int Width;
	public int Height;
	Tile[,] tiles;

	public Game(int width, int height) {
		Width = width;
		Height = height;
		tiles = new Tile[width,height];

		for (var x = 0; x < width; x++)
		for (var y = 0; y < height; y++) {
			tiles[x,y] = Tile.Wall;
		}
	}

	public Creature GetCreature(Point p) {
		return Creatures.FirstOrDefault(c => c.Exists && c.Position == p);
	}

	public Tile GetTile(int x, int y) {
		if (x >= 0 && y >= 0 && x < Width && y < Height)
			return tiles[x,y];
		else
			return Tile.OutOfBounds;
	}

	public void SetTile(int x, int y, Tile tile) {
		if (x >= 0 && y >= 0 && x < Width && y < Height)
			tiles[x,y] = tile;
	}

	public void TakeTurn() {
		foreach (var c in Creatures.ToList()) {
			if (c.Exists)
				c.TakeTurn(this);
		}

		Creatures.RemoveAll(c => !c.Exists);
	}
}
