using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TextPopupView : MonoBehaviour {
	public Text Label;

	Instantiator Instantiator;
	TextPopup Popup;
	Vector3 offset;

	void Update() {
		Popup.TTL -= Time.deltaTime;
		offset += new Vector3(0, Time.deltaTime * 48, 0);
		transform.position = Camera.main.WorldToScreenPoint(Popup.WorldPosition) + offset;

		if (Popup.TTL < 0)
			Instantiator.Remove(this);
	}

	public void Initialize(TextPopup popup, Instantiator instantiator) {
		Instantiator = instantiator;
		Popup = popup;
		Label.text = popup.Text;
		offset = popup.Offset;
		transform.position = Camera.main.WorldToScreenPoint(Popup.WorldPosition) + offset;
	}
}
