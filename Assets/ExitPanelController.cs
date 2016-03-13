using UnityEngine;
using System.Collections;

public class ExitPanelController : MonoBehaviour {
	void Start () {
		Hide();
	}

	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.N) || Input.GetKeyDown(KeyCode.C))
			Hide();
		else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Y) || Input.GetKeyDown(KeyCode.E))
			ExitGame();
	}

	public void ExitGame() {
		Application.Quit();
	}

	public void Hide() {
		gameObject.SetActive(false);
	}

	public void Show() {
		gameObject.SetActive(true);
	}
}
