using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelCuller : MonoBehaviour {

	public bool culled = false;

	public void Start() {
		if (culled) {
			DisableChildren();
		} else {
			EnableChildren();
		}
	}

	public void OnEnabled() {
		if (culled) {
			DisableChildren();
		} else {
			EnableChildren();
		}
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			EnableChildren();
		}
	}

	public void OnTriggerExit2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			DisableChildren();
		}
	}

	public void OnTriggerStay2D(Collider2D collider) {
		if (collider.gameObject.CompareTag("Player")) {
			if (culled) {
				EnableChildren();
			}
		}
	}

	public void EnableChildren() {
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
		}
		culled = false;
	}

	public void DisableChildren() {
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}
		culled = true;
	}
}
