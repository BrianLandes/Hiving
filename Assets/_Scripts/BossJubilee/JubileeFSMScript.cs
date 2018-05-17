using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class JubileeFSMScript : MonoBehaviour {

	//public Vector3 targetPosition;
	public GameObject painFace;
	public float grabTime = 3.0f;

	public Slider grabTimerSlider;
	private Rigidbody2D myBody;

	private TentacleTreads treads;

	bool gettingGrabbed = false;
	float grabTimer = 0.0f;

	public bool finalStage = false;
	private BossStage3LevelGenerator levelGenerator;

	private float redirectionDelay = 0f;

	public float leftSideOfTower = -6;
	public float rightSideOfTower = 25;

	// Use this for initialization
	void Awake () {
		myBody = GetComponent<Rigidbody2D>();
		treads = GetComponentInChildren<TentacleTreads>();
	}
	
	// Update is called once per frame
	void Update () {
		if (gettingGrabbed && treads.climbing) {
			grabTimer += Time.deltaTime;
			grabTimerSlider.value = grabTimer / grabTime;

			if ( grabTimer >= grabTime ) {
				Fungus.Flowchart.BroadcastFungusMessage("JubileeGetGrabbedWhole");
				GetGrabbedReleased();
			}
		}

		if (finalStage && treads.climbing) {

			float d = CommonUtils.Distance(transform.position, levelGenerator.lastEndPoint);
			if ( d < 20f )

				SetVelocity(CommonUtils.NormalVector(transform.position, levelGenerator.lastEndPoint) * 4);

			if (d < 2.0f) {
				SetVelocity(Vector2.zero);
				myBody.MovePosition(levelGenerator.lastEndPoint);
				treads.StopClimbing();
				treads.SpiderWeb();
				Fungus.Flowchart.BroadcastFungusMessage("JubileeSpiderWebbed");
			}
		}

		redirectionDelay -= Time.deltaTime;
		if (redirectionDelay <= 0) {
			ChooseNextTargetPosition();
			redirectionDelay = 2.0f;
		}
	}

	public void FixedUpdate() {
		KeepWithinTower();
	}

	public void PutBehindFace() {
		SpriteRenderer renderer = painFace.GetComponent<SpriteRenderer>();
		renderer.sortingOrder = 2;
		Rigidbody2D faceBody = painFace.GetComponent<Rigidbody2D>();
		faceBody.bodyType = RigidbodyType2D.Static;
		myBody.bodyType = RigidbodyType2D.Static;
		transform.position = painFace.transform.position;
		treads.StopClimbing();
		CameraManager.SetDamping(1.0f);
		CameraManager.ChangeZoom(7.0f, 0.8f);
		CameraManager.SetTarget(transform);
	}

	public void BigReveal() {
		ExplosionManager.CreateExplosion(transform.position, 4.0f);
	}

	public void StartStageThree() {
		myBody.bodyType = RigidbodyType2D.Dynamic;
		treads.StartClimbing();
	}

	public void StartFinalStage() {
		finalStage = true;
		levelGenerator = GameObject.FindGameObjectWithTag("Level3Generator")
			.GetComponent<BossStage3LevelGenerator>();
	}

	public void GetCrushed() {
		gameObject.layer = LayerMask.NameToLayer("BackgroundObjects");
		GameManager.Instance.EnableAirControl();
		//treads.SeverAllTentacles();
		myBody.constraints = RigidbodyConstraints2D.None;
		Destroy(GetComponent<GetGrabbedModifier>());
	}

	public void ResetPlayerCamera() {
		CameraManager.SetTargetToPlayer();
	}

	public void ChooseNextTargetPosition() {
		if (finalStage) {
			treads.climbDirection = CommonUtils.NormalVector(transform.position, levelGenerator.lastEndPoint);
		} else {
			float x = 0;
			float y = 1;
			if (transform.position.x > 25)
				x = -1f;
			//targetPosition = transform.position + Vector3.left*5.0f;
			else if (transform.position.x < -6)
				x = 1f;
			//treads.climbDirection = Vector2.right;
			//targetPosition = transform.position + Vector3.right * 5.0f;
			//else
			//	treads.climbDirection = Vector2.up;
			//targetPosition = transform.position + Vector3.up*5.0f;
			treads.climbDirection = new Vector2(x, y);
			treads.climbDirection = treads.climbDirection.normalized;
		}

		
	}

	public void SetVelocity(Vector2 velocity) {
		myBody.velocity = velocity;
	}

	public void AddForce(Vector2 force) {
		myBody.AddForce(force);
	}

	public void KeepWithinTower() {
		if (transform.position.x > rightSideOfTower + 20) {
			myBody.velocity = new Vector2(-4, 1);
		} else if (transform.position.x < leftSideOfTower - 20) {
			myBody.velocity = new Vector2(4, 1);
		}
	}

	public void GetGrabbed(object playerController) {
		PlayerController pc = playerController as PlayerController;
		Vector3 hitPoint = pc.tentacle.GetEndPosition();
		pc.StopGripping();

		pc.Grapple(GetComponent<Rigidbody2D>(), hitPoint, false);
		pc.tentacle.joint.distance = 0.1f;

		if (!finalStage) {
			grabTimerSlider.gameObject.SetActive(true);
			TrackWorldPosition twp = grabTimerSlider.gameObject.GetComponent<TrackWorldPosition>();
			twp.TrackPosition();

			gettingGrabbed = true;
			grabTimer = 0;

			Fungus.Flowchart.BroadcastFungusMessage("JubileeGetGrabbed");
		}
	}
	
	public void GetGrabbedReleased() {
		grabTimerSlider.gameObject.SetActive(false);

		gettingGrabbed = false;
	}
}
