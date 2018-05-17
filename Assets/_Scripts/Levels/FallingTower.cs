using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTower : MonoBehaviour {

	public float fallSpeed = -1.0f;

	private Rigidbody2D myBody;

	// Use this for initialization
	void Start () {
		myBody = GetComponent<Rigidbody2D>();
		ConnectAllChildren();
	}
	
	// Update is called once per frame
	void Update () {
		myBody.angularVelocity = fallSpeed;
	}

	public void ConnectAllChildren() {
		Rigidbody2D[] theirBodies = GetComponentsInChildren<Rigidbody2D>();
		foreach (var body in theirBodies) {
			if ( body.bodyType == RigidbodyType2D.Static)
			body.bodyType = RigidbodyType2D.Kinematic;
			//RelativeJoint2D joint = body.gameObject.AddComponent<RelativeJoint2D>();
			//FixedJoint2D joint = body.gameObject.AddComponent<FixedJoint2D>();
			//joint.connectedBody = myBody;
		}

		//BoxCollider2D[] colliders = GetComponentsInChildren<BoxCollider2D>();
		//foreach (var collider in colliders) {
		//	collider.gameObject.layer = LayerMask.NameToLayer("NavTerrain");
		//}
	}
}
