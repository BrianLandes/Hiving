using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour {

	public float speed = 2.0f;

	public GameObject explosionPrefab;

	public float range = 3.0f;
	public float force = 5.0f;
	[SerializeField]
	private LayerMask m_WhatIsHittable;

	private Transform propPoint;
	private Transform propDirection;
	private Transform grabPoint;

	private Rigidbody2D mRigidBody;

	private const int raytraceCount = 12;

	private bool gettingGrabbed = false;

	private bool isPlayers = false;

	// Use this for initialization
	void Start () {
		propPoint = transform.Find("PropulsionPoint");
		propDirection = transform.Find("PropulsionDirection");
		grabPoint = transform.Find("GrabPoint");

		mRigidBody = GetComponent<Rigidbody2D>();
	}

	public Vector2 Position {
		get { return new Vector2(transform.position.x, transform.position.y); }
	}

	// Update is called once per frame
	void Update () {
		Vector2 v = CommonUtils.NormalVector(propPoint.position, propDirection.position);
		mRigidBody.AddForceAtPosition(v * speed, propPoint.position, ForceMode2D.Impulse);
		//mRigidBody.MovePosition(Position + v * speed);

		if (CommonUtils.Distance(transform.position, GameManager.Instance.playerController.transform.position) > 50) {
			Destroy(gameObject);
		}
	}

	public void GetGrabbed( object playerController ) {
		PlayerController pc = playerController as PlayerController;
		pc.tentacle.AttachEnd(mRigidBody, grabPoint.position);
		pc.carryMouseSpring.connectedAnchor = mRigidBody.transform.InverseTransformPoint(grabPoint.position);

		gettingGrabbed = true;
		isPlayers = true;
	}

	public void GetGrabbedReleased() {
		gettingGrabbed = false;
	}

	public void OnTriggerEnter2D(Collider2D collider) {
		if (!collider.isTrigger) {
			TriggerExplosion();
		}
	}

	bool hitPlayer = false;
	bool hitBoss = false;

	public void TriggerExplosion() {
		GameObject explosion = Instantiate(explosionPrefab);
		explosion.transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f );
		explosion.SetActive(true);

		hitPlayer = false;
		hitBoss = false;

		for (int i = 0; i < raytraceCount; i++) {
			float theta = (float)i / (float)raytraceCount * Mathf.PI * 2.0f;

			RaytraceBlast(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
		}

		if (hitPlayer) {
			//Debug.Log("HitPlayer");
			PlayerController playerController = GameManager.Instance.playerController;
			Vector2 v = CommonUtils.NormalVector(transform.position, playerController.transform.position);
			playerController.Damage(1, v, 30);

		}

		if (hitBoss) {
			//Debug.Log("HitBossWithRocket");
			Fungus.Flowchart.BroadcastFungusMessage("HitBossWithRocket");
		}

		//if (removeOnExplode)
			Destroy(gameObject);
	}

	public void RaytraceBlast(Vector2 direction) {
		RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, range, m_WhatIsHittable);

		foreach (var hit in hits) {
			if (hit.collider.gameObject != gameObject) {

				Rigidbody2D body = hit.collider.gameObject.GetComponent<Rigidbody2D>();
				if (body != null) {
					//Debug.Log("Blast");
					body.AddForceAtPosition(direction * force, hit.point, ForceMode2D.Impulse);
				}

				if (isPlayers) {
					DieFromBombs dieBombs = hit.collider.gameObject.GetComponent<DieFromBombs>();
					if (dieBombs != null) {
						dieBombs.DieFromBlast();
					}

					if (hit.collider.gameObject.CompareTag("Boss")) {
						hitBoss = true;
					}
					
				} else if (hit.collider.gameObject.tag.Equals("Player")) {
					hitPlayer = true;
				}
			}
		}
	}
}
