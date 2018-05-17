using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleTreads : MonoBehaviour {

	public int tentaclesPerSide = 1;
	public LayerMask m_WhatIsGrabbable;

	public float reach = 10f;
	public float minReach = 3f;
	public float stretchSpeed = 1f;
	public float stretchSpeedIncrease = 0.02f;
	public float speedBoost = 0.2f;
	public bool climbing = false;

	public Vector2 climbDirection = Vector2.right;

	public TentaclePack pack;

	private List<Tentacle> rightSideTentacles = new List<Tentacle>();
	private List<Tentacle> leftSideTentacles = new List<Tentacle>();
	private Rigidbody2D myBody;

	private float speedBoostActual = 0.0f;

	public float sectionDelay = 1.0f;

	float leftSideDelay = 0.0f;
	//float rightSideDelay = 0.0f;

	private Transform leftIdle;
	private Transform rightIdle;

	private bool pressPlayerIntoSpikes = false;

	// Use this for initialization
	void Start () {
		myBody = GetComponentInParent<Rigidbody2D>();

		leftIdle = transform.Find("LeftIdle");
		rightIdle = transform.Find("RightIdle");
		
		for (int i = 0; i < tentaclesPerSide; i++) {

			Tentacle tentacle = TentacleMaker.Basic(pack,9);
			tentacle.transform.parent = transform;
			tentacle.transform.localPosition = Vector2.zero;
			tentacle.AttachBase(myBody, myBody.transform.position);
			tentacle.AttachEnd(myBody, RightIdlePosition());
			rightSideTentacles.Add(tentacle);
		}

		for (int i = 0; i < tentaclesPerSide; i++) {

			Tentacle tentacle = TentacleMaker.Basic(pack,9);
			tentacle.transform.parent = transform;
			tentacle.transform.localPosition = Vector2.zero;
			tentacle.AttachBase(myBody, myBody.transform.position);
			tentacle.AttachEnd(myBody, LeftIdlePosition());
			leftSideTentacles.Add(tentacle);
		}
		
	}
	
	public Vector2 LeftIdlePosition() {
		return CommonUtils.Position(leftIdle) + Random.insideUnitCircle * 2.0f;
	}

	public Vector2 RightIdlePosition() {
		return CommonUtils.Position(rightIdle) + Random.insideUnitCircle * 2.0f;
	}

	// Update is called once per frame
	void Update () {
		speedBoostActual -= Time.deltaTime*0.01f;
		if (speedBoostActual < 0)
			speedBoostActual = 0;
		leftSideDelay -= Time.deltaTime;
		//rightSideDelay -= Time.deltaTime;
		
		if (climbing) {
			foreach (var tentacle in rightSideTentacles) {
				UpdateTent(tentacle, RightIdlePosition());
				if (leftSideDelay <= 0.0f) {
					bool grabbed = TryGrapple(tentacle, RightGrappleDirection());
					if (grabbed) {
						leftSideDelay = sectionDelay;
					}
				}
			}

			foreach (var tentacle in leftSideTentacles) {
				UpdateTent(tentacle, LeftIdlePosition());
				if (leftSideDelay <= 0.0f) {
					bool grabbed = TryGrapple(tentacle, LeftGrappleDirection());
					if (grabbed) {
						leftSideDelay = sectionDelay;
					}
				}
			}
		}

		if (pressPlayerIntoSpikes) {
			foreach (var tentacle in rightSideTentacles) {
				if (tentacle.GrabbedObject.CompareTag("Player")) {
					tentacle.joint.connectedAnchor = new Vector2(tentacle.joint.connectedAnchor.x, tentacle.joint.connectedAnchor.y + Time.deltaTime*0.5f );
				}
			}
			foreach (var tentacle in leftSideTentacles) {
				if (tentacle.GrabbedObject.CompareTag("Player")) {
					tentacle.joint.connectedAnchor = new Vector2(tentacle.joint.connectedAnchor.x, tentacle.joint.connectedAnchor.y + Time.deltaTime * 0.5f);
				}
			}
		}
	}

	public Vector2 RightGrappleDirection() {
		float cTheta = CommonUtils.VectorToAngle(climbDirection);
		cTheta -= Random.Range(.1f, Mathf.PI * 0.25f);
		return CommonUtils.AngleToVector(cTheta);
	}

	public Vector2 LeftGrappleDirection() {
		float cTheta = CommonUtils.VectorToAngle(climbDirection);
		cTheta += Random.Range(.1f, Mathf.PI * 0.25f);
		return CommonUtils.AngleToVector(cTheta);
	}

	public void UpdateTent(Tentacle tentacle, Vector3 idle) {
		Vector2 n = CommonUtils.NormalVector(transform.position, tentacle.endPoint.position);
		//float theta = CommonUtils.ThetaBetween(n, climbDirection);
		float nAsAngle = CommonUtils.VectorToAngle(n);
		float dAsAngle = CommonUtils.VectorToAngle(climbDirection);
		float angleDifference = dAsAngle-nAsAngle;
		// TODO: account for values that are pointing left/west, that either give -pi or pi
		float d = CommonUtils.Distance(transform.position, tentacle.endPoint.position);
		if (tentacle.Grabbing) {
			if (d > reach * 0.5f && Mathf.Abs(angleDifference) > Mathf.PI*2f/4f ) {
				
				tentacle.StopGrabbing();
				tentacle.AttachEnd(myBody, idle);
			} else {
				
				if (d > 1.0f) {
					if (Mathf.Abs(angleDifference) < Mathf.PI * 0.5f) {
						tentacle.joint.distance -= stretchSpeed + speedBoostActual;
					} else {
						tentacle.joint.distance += stretchSpeed + speedBoostActual;
					}
				}
			}
		}
	}

	public bool TryGrapple(Tentacle tentacle, Vector2 direction) {
		if (!tentacle.Grabbing) {
			RaycastHit2D[] hits = Physics2D.RaycastAll(transform.position, direction, reach, m_WhatIsGrabbable);
			foreach (var hit in hits) {
				if (hit.collider != null) {
					Rigidbody2D rigidBody = hit.collider.gameObject.GetComponentInChildren<Rigidbody2D>();

					if (rigidBody != null && (rigidBody.bodyType == RigidbodyType2D.Static || rigidBody.bodyType == RigidbodyType2D.Kinematic)) {
						float distance = CommonUtils.Distance(transform.position, hit.point);
						if (distance > minReach) {
							Grapple(tentacle, rigidBody, hit.point);
							return true;
						}
					}
				}
			}
		}
		return false;
	}

	public void Grapple(Tentacle tentacle, Rigidbody2D grappledBody, Vector3 grappledPoint, bool playSound = true) {
		//StartCoroutine(_waitForTentEndToGrab(tentacle, grappledBody, grappledPoint, playSound));
		tentacle.SetGrabbing(grappledBody.gameObject, grappledPoint, playSound);
		tentacle.joint.distance = CommonUtils.Distance(tentacle.transform.position, grappledPoint);
		tentacle.AttachEnd(grappledBody, grappledPoint);
	}

	public void SpiderWeb() {
		foreach (var tentacle in rightSideTentacles) {
			tentacle.StopGrabbing();
			TryGrapple(tentacle, Random.insideUnitCircle.normalized );
		}

		foreach (var tentacle in leftSideTentacles) {
			tentacle.StopGrabbing();
			TryGrapple(tentacle, Random.insideUnitCircle.normalized);
		}
	}

	//static IEnumerator _waitForTentEndToGrab(Tentacle tentacle, Rigidbody2D grappledBody, Vector3 grappledPoint, bool playSound = true) {
	//	Transform endPoint = tentacle.transform.Find("EndPoint");
	//	while (CommonUtils.Distance( endPoint.position, grappledBody.transform.position) > 0.1f) {
	//		Debug.Log("waiting");
	//		yield return null;
	//	}
	//	tentacle.SetGrabbing(grappledBody.gameObject, grappledPoint, playSound);
	//	tentacle.joint.distance = CommonUtils.Distance(tentacle.transform.position, grappledPoint);
	//}

	public void StopClimbing() {
		climbing = false;
	}

	public void StartClimbing() {
		climbing = true;
	}

	public void SpeedBoost() {
		speedBoostActual = speedBoost;
	}

	public void PressPlayerIntoSpikes() {
		pressPlayerIntoSpikes = true;

		foreach (var tentacle in rightSideTentacles) {
			if (tentacle.GrabbedObject.CompareTag("Player")) {
				tentacle.joint.connectedAnchor = new Vector2(tentacle.joint.connectedAnchor.x, tentacle.joint.connectedAnchor.y + 5);
			}
		}
		foreach (var tentacle in leftSideTentacles) {
			if (tentacle.GrabbedObject.CompareTag("Player")) {
				tentacle.joint.connectedAnchor = new Vector2(tentacle.joint.connectedAnchor.x, tentacle.joint.connectedAnchor.y + 5);
			}
		}
	}

	public void SeverTentacle() {
		if (leftSideTentacles.Count == 0 && rightSideTentacles.Count == 0)
			return;

		if (leftSideTentacles.Count == rightSideTentacles.Count) {
			if ( Random.value > 0.5 ) {
				TentacleMaker.SeverTentacle(leftSideTentacles[0]);
				leftSideTentacles.RemoveAt(0);
			} else {
				TentacleMaker.SeverTentacle(rightSideTentacles[0]);
				rightSideTentacles.RemoveAt(0);
			}
		} else if (leftSideTentacles.Count > rightSideTentacles.Count) {
			TentacleMaker.SeverTentacle(leftSideTentacles[0]);
			leftSideTentacles.RemoveAt(0);
		} else {
			TentacleMaker.SeverTentacle(rightSideTentacles[0]);
			rightSideTentacles.RemoveAt(0);
		}

		stretchSpeed += stretchSpeedIncrease;
	}

	public void SeverAllTentacles() {
		while (leftSideTentacles.Count > 0 || rightSideTentacles.Count > 0)
			SeverTentacle();
	}

	public void AttachToPlayer() {
		bool attached = false;
		StopClimbing();
		GameManager.Instance.DisableAirControl();

		foreach( var tentacle in leftSideTentacles) {
			attached = TryAttachToPlayer(tentacle);
			if (attached)
				break;
		}

		if (!attached) {
			foreach (var tentacle in rightSideTentacles) {
				attached = TryAttachToPlayer(tentacle);
				if (attached)
					break;
			}
		}

		if (!attached) {
			foreach (var tentacle in leftSideTentacles) {
				attached = TryUnGrabAndAttachToPlayer(tentacle);
				if (attached)
					break;
			}
		}

		if (!attached) {
			foreach (var tentacle in rightSideTentacles) {
				attached = TryUnGrabAndAttachToPlayer(tentacle);
				if (attached)
					break;
			}
		}
	}

	bool TryAttachToPlayer( Tentacle tentacle ) {
		if (!tentacle.Grabbing) {
			PlayerController pc = GameManager.Instance.playerController;
			Rigidbody2D playersBody = pc.gameObject.GetComponent<Rigidbody2D>();

			Vector2 grabPoint = pc.transform.position;

			StartCoroutine(_waitForTentEndToGrabPlayer(tentacle, playersBody, grabPoint, true));
			tentacle.AttachEnd(playersBody, grabPoint);

			return true;
		}
		return false;
	}

	bool TryUnGrabAndAttachToPlayer(Tentacle tentacle) {
		if (!tentacle.GrabbedObject.CompareTag("Player")) {
			tentacle.StopGrabbing();
			PlayerController pc = GameManager.Instance.playerController;
			Rigidbody2D playersBody = pc.gameObject.GetComponent<Rigidbody2D>();

			Vector2 grabPoint = pc.transform.position;

			StartCoroutine(_waitForTentEndToGrabPlayer(tentacle, playersBody, grabPoint, true));
			tentacle.AttachEnd(playersBody, grabPoint);

			return true;
		}
		return false;
	}

	IEnumerator _waitForTentEndToGrabPlayer(Tentacle tentacle, Rigidbody2D grappledBody,
						Vector3 grappledPoint, bool playSound = true) {
		Transform endPoint = tentacle.transform.Find("EndPoint");
		while (CommonUtils.Distance(endPoint.position, grappledBody.transform.position) > 0.1f) {
			yield return null;
		}
		tentacle.SetGrabbing(grappledBody.gameObject, grappledPoint, playSound);
		tentacle.joint.distance = 1;
		tentacle.SetGrabbingAnchor(CommonUtils.Position(rightIdle) + Random.insideUnitCircle);
	}
}
