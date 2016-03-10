using UnityEngine;
using System.Collections;

public class StartController : MonoBehaviour {
	
	void Update () {
		if (Input.GetKeyDown(KeyCode.Return))
			UnityEngine.SceneManagement.SceneManager.LoadScene(1, UnityEngine.SceneManagement.LoadSceneMode.Single);	
	}
}
