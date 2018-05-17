using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

	public float impactTrigger = 10.0f;
	public float range = 3.0f;
	public float force = 5.0f;
	public GameObject explosionPrefab;
	[SerializeField]
	private LayerMask m_WhatIsHittable;
	public bool removeOnExplode = true;
	private Rigidbody2D body;

	private const int raytraceCount = 12;

	private bool gettingGrabbed = false;

	private bool isPlayers = false;
	
	// Use this for initialization
	void Start () {
		body = GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void OnCollisionEnter2D(Collision2D collision) {
		if (!gettingGrabbed && body.velocity.magnitude > impactTrigger) {
			TriggerExplosion();
		}
	}

	public void TriggerExplosion() {
		GameObject explosion = Instantiate(explosionPrefab);
		explosion.transform.position = transform.position;
		explosion.SetActive(true);

		for (int i = 0; i < raytraceCount; i++) {
			float theta = (float)i / (float)raytraceCount * Mathf.PI*2.0f;

			RaytraceBlast(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
		}

		if ( removeOnExplode )
			Destroy(gameObject);
	}

	public void RaytraceBlast(Vector2 direction) {
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, range, m_WhatIsHittable);
		
		foreach (var hit in hits) {
			if (hit.collider.gameObject != gameObject) {
				
				Rigidbody2D body = hit.collider.gameObject.GetComponent<Rigidbody2D>();
				if (body != null) {
					//Debug.Log("Blast");
					body.AddForceAtPosition(direction* force, hit.point, ForceMode2D.Impulse);
				}

				if (isPlayers) {
					DieFromBombs dieBombs = hit.collider.gameObject.GetComponent<DieFromBombs>();
					if (dieBombs != null) {
						dieBombs.DieFromBlast();
					}
				}
			}
		}
	}

	public void GetGrabbed() {
		gettingGrabbed = true;
		isPlayers = true;
	}

	public void GetGrabbedReleased() {
		gettingGrabbed = false;
	}
}
