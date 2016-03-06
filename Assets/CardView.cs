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

	Instantiator Instantiator;
	Card Card;
	Creature Player;

	public Text title;
	public Image cardImage;

	bool isFaceUp;
	List<Card> lastPile = new List<Card>();
	int lastIndex = -99;
	public bool IsMoving;
	float speed;
	RectTransform targetTransform;
	Vector2 targetPosition;

	void Start() {
		DrawPile = GameObject.Find("DrawPile").GetComponent<RectTransform>();
		HandPile = GameObject.Find("HandPile").GetComponent<RectTransform>();
		AttackPile = GameObject.Find("AttackPile").GetComponent<RectTransform>();
		DefensePile = GameObject.Find("DefensePile").GetComponent<RectTransform>();
		DiscardPile = GameObject.Find("DiscardPile").GetComponent<RectTransform>();
	}

	void Update() {
		if (IsMoving) {
			transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			if (Vector2.Distance(transform.position, targetPosition) < 0.001f) {
				IsMoving = false;
			}
		}

		if (lastPile.IndexOf(Card) == lastIndex)
			return;

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
		}

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

		if (targetTransform == DrawPile) {
			var cards = new List<CardView>();
			foreach (Transform child in DrawPile.transform) {
				var view = child.GetComponent<CardView>();
				if (view != null) {
					cards.Add(view);
				}
			}
			foreach (var c in cards.OrderBy(c => c.Player.DrawStack.IndexOf(c.Card)).Reverse()) {
				c.transform.SetAsFirstSibling();
			}
		}
	}

	public void Initialize(Card card, Creature holder, Instantiator instantiator) {
		Card = card;
		Player = holder;
		Instantiator = instantiator;
		title.text = card.Name;
	}

	void FaceUp() {
		isFaceUp = true;
		title.gameObject.SetActive(true);
		cardImage.color = Color.white;
	}

	void FaceDown() {
		isFaceUp = false;
		title.gameObject.SetActive(false);
		cardImage.color = Color.gray;
	}

	public void Clicked() {
		if (isFaceUp && Card.OnUse != CardSpecialEffect.None)
			Player.UseCard(Card);
	}
}
