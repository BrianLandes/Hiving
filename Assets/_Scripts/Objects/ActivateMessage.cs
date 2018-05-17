using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateMessage : MonoBehaviour {

	public string openMessage = "";
	public string closeMessage = "";

	public void Open() {
		Fungus.Flowchart.BroadcastFungusMessage(openMessage);
	}

	public void Close() {
		Fungus.Flowchart.BroadcastFungusMessage(closeMessage);
	}

	public void Toggle() {
		
	}
}
