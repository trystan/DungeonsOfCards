using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {
	GameObject target;

	Vector3 offset = new Vector3(0,0,-10);

	void Update () {
		transform.position = Vector3.Slerp(transform.position, target.transform.position + offset, 5f * Time.deltaTime);
	}

	public void GoTo(Vector3 position) {
		transform.position = position + offset;
	}

	public void Follow(GameObject go) {
		target = go;
		GoTo(go.transform.position);
	}
}
