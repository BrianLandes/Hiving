﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowBombs : MonoBehaviour {

	public GameObject bombPrefab;

	public float range = 5;
	public float bombsPerSecond = 1;

	private float timer;
	private float rate = 0f;
	private GameObject player;

	private bool gettingGrabbed = false;

	private Transform bombSpawn;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.

	// Use this for initialization
	void Start() {
		bombSpawn = transform.Find("BombSpawnPoint");
	}

	public Vector2 Position {
		get { return new Vector2(transform.position.x, transform.position.y); }
	}

	// Update is called once per frame
	void Update () {
		if (player == null) {
			player = GameObject.FindGameObjectWithTag("Player");
		}
		if (!gettingGrabbed) {
			if (m_FacingRight && player.transform.position.x < transform.position.x) {
				Flip();
			} else if (!m_FacingRight && player.transform.position.x > transform.position.x) {
				Flip();
			}

			float d = CommonUtils.Distance(player.transform.position, transform.position);
			if (d < range) {
				rate = 1f / bombsPerSecond;
				timer += Time.deltaTime;
				if (timer > rate) {
					timer -= rate;
					Vector2 offset = bombSpawn.position;
					Vector2 vect = new Vector2 (player.transform.position.x - offset.x, player.transform.position.y - offset.y );

					if (Mathf.Abs(vect.x) > 0.1f) {

						GameObject newBomb = Instantiate(bombPrefab);

						newBomb.transform.position = offset;
						Rigidbody2D body = newBomb.GetComponent<Rigidbody2D>();
						if (body != null) {
							body.velocity = CommonUtils.TrajectoryVerticalSpeed(vect, 5f);
						}
					}
				}
			} else {
				timer = 0;
			}
		}
		
	}

	public void GetGrabbed() {
		gettingGrabbed = true;
	}

	public void GetGrabbedReleased() {
		gettingGrabbed = false;
	}

	private void Flip() {
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
}
