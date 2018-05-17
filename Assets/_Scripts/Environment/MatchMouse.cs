using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchMouse : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		Vector3 v = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		v.z = 0;
		transform.position = v;
	}
}
