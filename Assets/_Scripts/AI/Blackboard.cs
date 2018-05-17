using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Blackboard : MonoBehaviour {

	public float speed = 1f;
	public bool canFly = false;
	public float minStopDistance = 0.1f;

	public bool hasDestination = false;
	public Vector2 destination;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetDestination(Vector2 newDes) {
		destination = newDes;
		hasDestination = true;
	}

	public void RemoveDestination() {
		hasDestination = false;
	}
}
