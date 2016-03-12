using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Linq;

public class CardView : MonoBehaviour {
	public RectTransform DrawPile;
	public RectTransform HandPile;
	public RectTransform AttackPile;
	public RectTransform DefensePile;
	public RectTransform DiscardPile;
	public RectTransform CurrentlyInUsePile;
	public GameObject UseButton;

	Instantiator Instantiator;
	Game Game;
	Card Card;
	Creature Player;

	public Text title;
	public Text typeLabel;
	public Text descriptionLabel;
	public Text flavorLabel;
	public Image cardImage;

	bool isBeingExamined;
	int lastKnownTurnNumber;

	List<Card> lastPile = new List<Card>();
	int lastIndex = -99;
	public bool IsMoving;
	float speed;
	RectTransform targetTransform;
	Vector2 targetPosition;

	void Start() {
		if (!Card.Exists || !Player.Exists) {
			Instantiator.Remove(this);
			return;
		}

		DrawPile = GameObject.Find("DrawPile").GetComponent<RectTransform>();
		HandPile = GameObject.Find("HandPile").GetComponent<RectTransform>();
		AttackPile = GameObject.Find("AttackPile").GetComponent<RectTransform>();
		DefensePile = GameObject.Find("DefensePile").GetComponent<RectTransform>();
		DiscardPile = GameObject.Find("DiscardPile").GetComponent<RectTransform>();
		CurrentlyInUsePile = GameObject.Find("CurrentlyInUsePile").GetComponent<RectTransform>();
	}

	void Update() {
		if (!Card.Exists || !Player.Exists) {
			Instantiator.Remove(this);
			return;
		}

		if (IsMoving) {
			transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			if (Vector2.Distance(transform.position, targetPosition) < 0.001f) {
				IsMoving = false;
			}
		}

		if (isBeingExamined) {
			if (lastKnownTurnNumber != Game.TurnNumber)
				EndExamining();
		} else {
			if (lastPile.IndexOf(Card) == lastIndex)
				return;

			Reposition();
		}

		lastKnownTurnNumber = Game.TurnNumber;
	}

	void Reposition() {
		if (Player.DrawStack.Contains(Card)) {
			lastPile = Player.DrawStack;
			targetTransform = DrawPile;
			FaceDown();
		} else if (Player.HandStack.Contains(Card)) {
			lastPile = Player.HandStack;
			targetTransform = HandPile;
			FaceUp();
		} else if (Player.AttackStack.Contains(Card)) {
			lastPile = Player.AttackStack;
			targetTransform = AttackPile;
			FaceUp();
		} else if (Player.DefenseStack.Contains(Card)) {
			lastPile = Player.DefenseStack;
			targetTransform = DefensePile;
			FaceUp();
		} else if (Player.DiscardStack.Contains(Card)) {
			lastPile = Player.DiscardStack;
			targetTransform = DiscardPile;
			FaceUp();
		} else {
			lastPile = new List<Card>();
			targetTransform = CurrentlyInUsePile;
		}

		if (targetTransform == CurrentlyInUsePile)
			lastIndex = -99;
		else
			lastIndex = lastPile.IndexOf(Card);

		if (targetTransform == HandPile) {
			var w = targetTransform.sizeDelta.x + 60;
			var x = -1f * lastIndex / Player.MaximumHandCards * w + w / 2 + 30;
			targetPosition = targetTransform.position + new Vector3(x, 18, 0);
		} else
			targetPosition = targetTransform.position + new Vector3(0, lastIndex + 18, 0);

		transform.SetParent(targetTransform);
		IsMoving = true;
		speed = Vector2.Distance(transform.position, targetPosition) * 5f;

		var cards = new List<CardView>();
		foreach (Transform child in targetTransform.transform) {
			var view = child.GetComponent<CardView>();
			if (view != null) {
				cards.Add(view);
			}
		}
		foreach (var c in cards.OrderBy(c => lastPile.IndexOf(c.Card)).Reverse())
			c.transform.SetAsFirstSibling();
	}

	public void Initialize(Game game, Card card, Creature holder, Instantiator instantiator) {
		Game = game;
		Card = card;
		Player = holder;
		Instantiator = instantiator;
		title.text = card.Name;
		typeLabel.text = "";
		descriptionLabel.text = "";
		flavorLabel.text = "";
		transform.position = Camera.main.WorldToScreenPoint(card.WorldPointOrigin);
		FaceDown();
		UseButton.SetActive(false);
	}

	public void UseButtonClicked() {
		Player.UseCard(Game, Card);
		EndExamining();
		Game.TakeTurn();
	}

	void FaceUp() {
		title.gameObject.SetActive(true);
		cardImage.color = Color.white;
	}

	void FaceDown() {
		title.gameObject.SetActive(false);
		cardImage.color = Color.gray;
	}

	public void Clicked() {
		if (Player.DrawStack.Contains(Card)) {
			Player.MoveBy(Game,0,0);
			Game.TakeTurn();
		} else if (isBeingExamined) {
			EndExamining();
		} else {
			foreach (Transform c in CurrentlyInUsePile) {
				var view = c.GetComponent<CardView>();
				if (view != null)
					view.EndExamining();
			}
			BeginExamining();
		}
	}

	void BeginExamining() {
		isBeingExamined = true;
		targetPosition = CurrentlyInUsePile.transform.position;
		transform.SetParent(CurrentlyInUsePile.transform);
		IsMoving = true;
		speed = Vector2.Distance(transform.position, targetPosition) * 5f;
		lastKnownTurnNumber = Game.TurnNumber;

		(transform as RectTransform).sizeDelta = new Vector2(240, 300);
		descriptionLabel.text = "";

		if (Card.CardType == CardType.Attack)
			typeLabel.text += "<b>Attack card</b>\n";
		else if (Card.CardType == CardType.Defense)
			typeLabel.text += "<b>Defense card</b>\n";
		else
			typeLabel.text = "";

		descriptionLabel.text = "";
		if (Card.Description != null)
			descriptionLabel.text += Card.Description + "\n\n";
		
		AddLine("On death", Card.OnDie);
		AddLine("On draw", Card.OnDraw);
		AddLine("While in hand", Card.OnInHand);
		AddLine("On use", Card.OnUse);

		if (Card.DoesBlockOtherCard)
			descriptionLabel.text += "\n<b>Blocks opponents attack card.</b>";

		if (Card.DoesStopCombat)
			descriptionLabel.text += "\n<b>Ends combat.</b>";
		
		if (Card.StrongVs != null)
			descriptionLabel.text += "\n<b>Doubles total damage vs " + Card.StrongVs + ".</b>";

		if (Card.FlavorText != null)
			flavorLabel.text = "<i>" + Card.FlavorText + "</i>";
		else
			flavorLabel.text = "";

		UseButton.SetActive(Card.OnUse != CardSpecialEffect.None && Player.HandStack.Contains(Card));
	}

	void EndExamining() {
		isBeingExamined = false;
		UseButton.SetActive(false);
		Reposition();

		(transform as RectTransform).sizeDelta = new Vector2(80, 100);
		typeLabel.text = "";
		descriptionLabel.text = "";
		flavorLabel.text = "";
	}

	void AddLine(string when, CardSpecialEffect effect) {
		if (effect == CardSpecialEffect.None)
			return;

		var text = "";
		switch (effect) {
		case CardSpecialEffect.AddCardToOther: 
			text = "Add a " + Card.ExtraCard().Name + " to opponent's draw pile."; break;
		case CardSpecialEffect.AddCardToSelf: 
			text = "Add a " + Card.ExtraCard().Name + " to your draw pile."; break;
		case CardSpecialEffect.DamageClosest: 
			text = "Damage the closest creature within 5 spaces."; break;
		case CardSpecialEffect.Discard1FromEachPile: 
			text = "Discard 1 card from your attack pile, defense pile, and hand."; break;
		case CardSpecialEffect.Draw3:
			text = "Draw 3 cards."; break;
		case CardSpecialEffect.Draw5Attack:
			text = "Draw 5 cards. Keep any attack cards and discard the rest."; break;
		case CardSpecialEffect.Draw5Defense:
			text = "Draw 5 cards. Keep any defense cards and discard the rest."; break;
		case CardSpecialEffect.Evade: 
			text = "Step away from combat."; break;
		case CardSpecialEffect.Heal1Health:
			text = "Recover 1 health."; break;
		case CardSpecialEffect.HealTeam:
			text = "All allies within 5 spaces recover health."; break;
		case CardSpecialEffect.IncreaseAttackSize:
			text = "Increase the number of attack cards by 1."; break;
		case CardSpecialEffect.IncreaseDefenseSize:
			text = "Increase the number of defense cards by 1."; break;
		case CardSpecialEffect.IncreaseHandSize:
			text = "Increase the number of cards in your hand by 1."; break;
		case CardSpecialEffect.Lose1Health:
			text = "Lose 1 health."; break;
		case CardSpecialEffect.Pray:
			text = "Let the gods decide what will happen to you."; break;
		case CardSpecialEffect.SpawnSkeleton:
			text = "If you die, come back as a skeleton."; break;
		case CardSpecialEffect.TurnUndead:
			text = "Damage all undead within 5 spaces."; break;
		case CardSpecialEffect.Vampire1:
			text = "Recover 1 health."; break;
		}
		
		descriptionLabel.text += "<b>" + when + ":</b> " + text + "\n";
	}
}
