using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;

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
	public bool IsMoving;
	float speed;
	Vector2 targetPosition;

	void Start() {
		DrawPile = GameObject.Find("DrawPile").GetComponent<RectTransform>();
		HandPile = GameObject.Find("HandPile").GetComponent<RectTransform>();
		AttackPile = GameObject.Find("AttackPile").GetComponent<RectTransform>();
		DefensePile = GameObject.Find("DefensePile").GetComponent<RectTransform>();
		DiscardPile = GameObject.Find("DiscardPile").GetComponent<RectTransform>();
	}

	List<Card> lastPile = new List<Card>();
	int lastIndex = -99;

	void Update() {
		if (IsMoving) {
			transform.position = Vector2.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
			if (Vector2.Distance(transform.position, targetPosition) < 0.001f) {
				IsMoving = false;
			}
		}

		if (lastPile.IndexOf(Card) == lastIndex)
			return;

		var targetTransform = DiscardPile;

		if (Player.DrawStack.Contains(Card)) {
			lastPile = Player.DrawStack;
			targetTransform = DrawPile;
		} else if (Player.HandStack.Contains(Card)) {
			lastPile = Player.HandStack;
			targetTransform = HandPile;
		} else if (Player.AttackStack.Contains(Card)) {
			lastPile = Player.AttackStack;
			targetTransform = AttackPile;
		} else if (Player.DefenseStack.Contains(Card)) {
			lastPile = Player.DefenseStack;
			targetTransform = DefensePile;
		} else if (Player.DiscardStack.Contains(Card)) {
			lastPile = Player.DiscardStack;
			targetTransform = DiscardPile;
		}

		lastIndex = lastPile.IndexOf(Card);

		transform.SetParent(targetTransform);
		if (targetTransform == HandPile) {
			var w = targetTransform.sizeDelta.x + 60;
			var x = -1f * lastIndex / Player.MaximumHandCards * w + w / 2 + 30;
			targetPosition = targetTransform.position + new Vector3(x, 18, 0);
		} else
			targetPosition = targetTransform.position + new Vector3(0, lastIndex * 4 + 18, 0);

		IsMoving = true;
		speed = Vector2.Distance(transform.position, targetPosition) * 5f;
	}

	public void Initialize(Card card, Creature holder, Instantiator instantiator) {
		Card = card;
		Player = holder;
		Instantiator = instantiator;
		title.text = card.Name;
	}
}
