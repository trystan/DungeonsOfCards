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
			new Card() { Name = "Regenerate 1", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Regenerate 1", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Heal1 },
			new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
			new Card() { Name = "Draw 3", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw3 },
		};
	}

	public List<Card> AttackPack() {
		return new List<Card>() {
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Attack +1", CardType = CardType.Attack, CombatBonus = 1 },
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Attack +2", CardType = CardType.Attack, CombatBonus = 2 },
			new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
			new Card() { Name = "Attack +3", CardType = CardType.Attack, CombatBonus = 3 },
			new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
			new Card() { Name = "Ready attack", CardType = CardType.Normal, OnUse = CardSpecialEffect.Draw5Attack },
		};
	}

	public List<Card> DefensePack() {
		return new List<Card>() {
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Defense +1", CardType = CardType.Defense, CombatBonus = 1 },
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Defense +2", CardType = CardType.Defense, CombatBonus = 2 },
			new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
			new Card() { Name = "Defense +3", CardType = CardType.Defense, CombatBonus = 3 },
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
			new Card() { Name = "Idle", CardType = CardType.Normal },
			new Card() { Name = "Idle", CardType = CardType.Normal },
			new Card() { Name = "Fumble", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromHand },
			new Card() { Name = "Fumble", CardType = CardType.Normal, OnDraw = CardSpecialEffect.Discard1FromHand },
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
			AttackValue = 1,
			MaximumAttackCards = 1,
			DefenseValue = 1,
			MaximumDefenseCards = 1,
			MaximumHealth = 5,
			CurrentHealth = 5,
			MaximumHandCards = 3,
			DrawStack = Packs(GenericPack(), SkeletonPack()),
		};
	}

	public Creature Lizard(int x, int y) {
		return new Creature() {
			Position = new Point(x, y),
			Ai = new ComputerAi(),
			TeamName = "Lizards",
			SpriteName = "DawnLike/Characters/Player0:lizard",
			AttackValue = 2,
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
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
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
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
			MaximumAttackCards = 1,
			DefenseValue = 2,
			MaximumDefenseCards = 1,
			MaximumHealth = 8,
			CurrentHealth = 8,
			MaximumHandCards = 4,
			DrawStack = Packs(GenericPack(), DefensePack()),
		};
	}

	public Creature Enemy(int x, int y) {
		return Util.Shuffle<Func<int,int,Creature>>(new List<Func<int,int,Creature>>() {
			Skeleton, Lizard, AttackLizard, DefenseLizard
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
					+ creature.MaximumDefenseCards - creature.DefenseStack.Count
					+ 1;
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
				if (x == 0 && y == 0)
					cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, 2);
				
				var neighbor = creature.Position + new Point(x,y);

				var other = game.GetCreature(neighbor);
				if (other != null) {
					if (other.TeamName != creature.TeamName) {
						var strength = 5 + creature.AttackValue + creature.AttackStack.Count - other.DefenseValue - other.DefenseStack.Count;
						cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, strength);
					}
				} else if (!game.GetTile(neighbor.X, neighbor.Y).BlocksMovement) {
					cardsToPlay.Add(new Card() { Name = "AI_MOVE " + x + "x" + y }, 1);
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
}

public enum CardType {
	Attack, Defense, Normal
}

public enum CardSpecialEffect {
	None, Discard1FromHand, Heal1, Draw3, Draw5Attack, Draw5Defense, SpawnSkeleton
}

public class Card {
	public string Name;
	public CardType CardType;

	public int CombatBonus;
	public bool DoesBlockOtherCard;

	public CardSpecialEffect OnDraw = CardSpecialEffect.None;
	public CardSpecialEffect OnDie = CardSpecialEffect.None;
	public CardSpecialEffect OnUse = CardSpecialEffect.None;
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
			Draw1Card();
		}

		EndTurn(game);
	}

	public void UseCard(Game game, Card card) {
		HandStack.Remove(card);

		switch (card.OnUse) {
		case CardSpecialEffect.Heal1:
			if (CurrentHealth < MaximumHealth)
				CurrentHealth++;
			break;
		case CardSpecialEffect.Draw3:
			game.Effects.Add(new DelayedEffect() {
				Delay = 0.1f,
				Callback = g => this.Draw1Card(),
			});
			game.Effects.Add(new DelayedEffect() {
				Delay = 0.2f,
				Callback = g => this.Draw1Card(),
			});
			game.Effects.Add(new DelayedEffect() {
				Delay = 0.3f,
				Callback = g => this.Draw1Card(),
			});
			game.Effects.Add(new DelayedEffect() {
				Delay = 0.4f,
				Callback = g => DiscardStack.Add(card),
			});
			game.Popups.Add(new TextPopup(card.Name, Position, new Vector3(0,4,0)));
			return;
		case CardSpecialEffect.Draw5Attack:
			Action<Game> drawAttack = g => {
				var c = this.GetTopDrawCard();
				if (c.CardType == CardType.Attack)
					this.KeepCard(c);
				else
					this.DiscardStack.Add(c);
			};
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.3f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.4f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.5f, Callback = drawAttack });
			game.Effects.Add(new DelayedEffect() { Delay = 0.6f, Callback = g => DiscardStack.Add(card) });
			game.Popups.Add(new TextPopup(card.Name, Position, new Vector3(0,4,0)));
			return;
		case CardSpecialEffect.Draw5Defense:
			Action<Game> drawDefense = g => {
				var c = this.GetTopDrawCard();
				if (c.CardType == CardType.Defense)
					this.KeepCard(c);
				else
					this.DiscardStack.Add(c);
			};
			game.Effects.Add(new DelayedEffect() { Delay = 0.1f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.2f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.3f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.4f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.5f, Callback = drawDefense });
			game.Effects.Add(new DelayedEffect() { Delay = 0.6f, Callback = g => DiscardStack.Add(card) });
			game.Popups.Add(new TextPopup(card.Name, Position, new Vector3(0,4,0)));
			return;
		default:
			throw new NotImplementedException(card.Name + " " + card.OnDraw);
		}

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

	Card GetTopDrawCard() {
		if (!DrawStack.Any())
			ReshuffleIntoDrawStack();
		
		var pulledCard = DrawStack.Last();
		DrawStack.Remove(pulledCard);
		return pulledCard;
	}

	void Draw1Card() {
		var pulledCard = GetTopDrawCard();

		switch (pulledCard.OnDraw) {
		case CardSpecialEffect.None:
			break;
		case CardSpecialEffect.Heal1:
			if (CurrentHealth < MaximumHealth)
				CurrentHealth++;
			break;
		case CardSpecialEffect.Discard1FromHand:
			if (HandStack.Any()) {
				var i = UnityEngine.Random.Range(0, HandStack.Count);
				DiscardStack.Add(HandStack[i]);
				HandStack.RemoveAt(i);
			}
			break;
		case CardSpecialEffect.SpawnSkeleton:
			break;
		default:
			throw new NotImplementedException(pulledCard.Name + " " + pulledCard.OnDraw);
		}

		KeepCard(pulledCard);
	}

	void KeepCard(Card pulledCard) {
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
			while (HandStack.Count > MaximumHandCards) {
				var toDiscard = HandStack[0];
				HandStack.RemoveAt(0);
				DiscardStack.Add(toDiscard);
			}
		}
	}

	void EndTurn(Game game) {
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

			if (defenderCard != null) {
				defenderDefense += defenderCard.CombatBonus;
				blockAttack = defenderCard.DoesBlockOtherCard;

				var popup = new TextPopup(defenderCard.Name, Defender.Position, new Vector3(0,14 * i,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f,
					Callback = g => g.Popups.Add(popup),
				});
			}

			if (attackerCard != null && !blockAttack) {
				attackerAttack += attackerCard.CombatBonus;

				var popup = new TextPopup(attackerCard.Name, Attacker.Position, new Vector3(0,14 * i + 7,0));
				Game.Effects.Add(new DelayedEffect() {
					Delay = i * 0.1f + 0.05f,
					Callback = g => g.Popups.Add(popup),
				});
			}
		}

		Defender.TakeDamage(Mathf.Max(1, attackerAttack - defenderDefense));

		if (!Defender.Exists) {
			foreach (var card in Defender.DefenseStack) {
				switch (card.OnDie) {
				case CardSpecialEffect.None:
					break;
				case CardSpecialEffect.Discard1FromHand:
					break;
				case CardSpecialEffect.SpawnSkeleton:
					Game.Creatures.Add(Game.Catalog.Skeleton(Defender.Position.X, Defender.Position.Y));
					break;
				default:
					throw new NotImplementedException(card.Name + " " + card.OnDie);
				}
			}
		}

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
