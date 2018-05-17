using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckpointFlag : MonoBehaviour {

	private bool triggered = false;

	void OnTriggerEnter2D(Collider2D collider) {
		if (!triggered && collider.gameObject.tag == "Player") {
			triggered = true;
			GameManager.HealPlayer();
			GetComponent<SpriteRenderer>().color = Color.grey;
		}
	}

	void OnCollisionEnter2D(Collision2D collision) {
		if (!triggered && collision.gameObject.tag == "Player") {
			triggered = true;
			GameManager.HealPlayer();
			GetComponent<SpriteRenderer>().color = Color.grey;
		}
	}

	public void GetGrabbed(object playerController) {
		if (!triggered) {
			triggered = true;
			GameManager.HealPlayer();
			GetComponent<SpriteRenderer>().color = Color.grey;
			GetComponent<AudioSource>().Play();
		}
	}

	public void GetGrabbedReleased() {

	}
}
