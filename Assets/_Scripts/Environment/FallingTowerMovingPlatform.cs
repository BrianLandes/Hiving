﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallingTowerMovingPlatform : MonoBehaviour {

	[Tooltip("Whether the waypoints will be traversed in reverse when the end is reached OR simply started from the beginning.")]
	public bool pingPong = true;
	public bool autoStart = true;

	public float speed = 1f;

	public float minDistance = 0.1f;

	public List<Transform> path = new List<Transform>();

	private bool forward = true;
	private int currentWaypoint = 0;

	private Rigidbody2D m_Rigidbody2D;

	Vector2 v;

	private bool on = true;

	// Use this for initialization
	void Start() {
		m_Rigidbody2D = GetComponent<Rigidbody2D>();

		on = autoStart;
	}

	// Update is called once per frame
	void Update() {
		int c = path.Count;
		if (c == 0)
			return;

		if (on) {

			v = path[currentWaypoint].position;
			if (CommonUtils.Distance(transform.position, v) < minDistance) {
				currentWaypoint += forward ? 1 : -1;
				if (currentWaypoint == c) {
					if (pingPong) {
						forward = false;
						currentWaypoint -= 2;
					} else {
						currentWaypoint = 0;
					}
				} else if (currentWaypoint == -1) {
					forward = true;
					currentWaypoint = 1;
				}
				v = path[currentWaypoint].position;
			}
		}
	}

	public void FixedUpdate() {
		if (on) {
			Vector2 n = CommonUtils.NormalVector(transform.position, v);
			m_Rigidbody2D.velocity = new Vector2(n.x * speed, n.y * speed);
		} else {
			m_Rigidbody2D.velocity = Vector2.zero;
		}

	}

	public void Open() {
		on = true;
	}

	public void Close() {
		on = false;
	}

	public void Toggle() {

	}

	public void TeleportToStart() {
		transform.position = path[0].position;
		forward = true;
		currentWaypoint = 0;
	}
}
