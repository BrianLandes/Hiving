using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFromBombs : MonoBehaviour {

	public GameObject bloodSplatterPrefab;

	public float explosionScale = 2;
	public float explosionRotation = 0;

	bool dying = false;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void DieFromBlast() {
		if (!dying) {
			GameObject explosion = Instantiate(bloodSplatterPrefab);
			explosion.transform.position = transform.position;
			explosion.transform.localScale = new Vector3(explosionScale, explosionScale, 1);
			explosion.transform.Rotate(new Vector3(0, 0, explosionRotation));
			explosion.SetActive(true);
			dying = true;
			Destroy(gameObject);
		}
	}
}
