using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillZone : MonoBehaviour {

	public string message = "PlayerFellIntoKillZone";

	void OnTriggerEnter2D(Collider2D collider) {
		if (collider.gameObject.tag == "Player") {
			Fungus.Flowchart.BroadcastFungusMessage(message);
		}
	}
}
