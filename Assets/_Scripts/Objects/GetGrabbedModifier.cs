using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GetGrabbedModifier : MonoBehaviour {

	public void GetGrabbed(object playerController) {
		PlayerController pc = playerController as PlayerController;
		Vector3 hitPoint = pc.tentacle.GetEndPosition();
		pc.StopGripping();
		
		pc.Grapple(GetComponent<Rigidbody2D>(), hitPoint, false);
		pc.tentacle.joint.distance = 0.1f;
	}

	public void GetGrabbedReleased() {

	}
}
