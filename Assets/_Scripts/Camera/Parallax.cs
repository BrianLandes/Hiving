using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parallax : MonoBehaviour {

	public GameObject camera;

	[Range(0,1)]
	public float scale = 1.0f;

	Vector3 startPosition;
	Vector3 camStartPosition;

	// Use this for initialization
	void Start () {
		startPosition = transform.position;
		camStartPosition = camera.transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = (camera.transform.position - camStartPosition)*scale + startPosition;


	}
}
