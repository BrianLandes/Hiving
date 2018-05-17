using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MessageOnGrab : MonoBehaviour {

	public string message = "";

	public void GetGrabbed(object playerController) {
		Debug.Log(message);
		Fungus.Flowchart.BroadcastFungusMessage(message);
	}

	public void GetGrabbedReleased() {

	}
}
