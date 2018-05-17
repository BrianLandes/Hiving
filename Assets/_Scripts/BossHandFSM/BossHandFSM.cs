using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandFSM : MonoBehaviour {

	public Sprite idle;
	public Sprite charge;
	public Sprite slam;

	public GameObject bombPrefab;

	public float chargeDistance = 4;

	public bool shootMissilesRight = true;
	public bool chargePlayer = false;
	public bool catchUpToPlayer = false;

	private SpriteRenderer renderer;

	public Vector3 targetPosition;
	public Direction targetDirection;

	private bool deactivated = false;
	public float deactivatedMaxTime = 8f;
	private float deactivatedTimer = 0f;

	private Transform damageCollider;
	private Transform pullSwitch;
	private Animator animator;
	private Rigidbody2D myBody;
	private BoxCollider2D mainCollider;

	public float rocketTime = 12;
	public float rocketSpeed = 0.15f;

	private float rocketStyle = 0;
	private Transform propPoint;
	private Transform propDirection;
	private Transform grabPoint;

	public GameObject explosionPrefab;

	private AudioSource audioSource;
	bool isStageThree = false;
	public enum Direction {
		NORTH,
		EAST,
		WEST,
		SOUTH
	}

	private Transform bombSpawn;
	private Transform launchDirection;

	// Use this for initialization
	void OnEnable () {
		renderer = GetComponent<SpriteRenderer>();
		animator = GetComponent<Animator>();
		myBody = GetComponent<Rigidbody2D>();
		mainCollider = GetComponent<BoxCollider2D>();
		audioSource = GetComponent<AudioSource>();
		damageCollider = transform.Find("DamageCollider");
		pullSwitch = transform.Find("PullSwitch");
		bombSpawn = transform.Find("BombSpawnPoint");
		launchDirection = transform.Find("LaunchDirection");
		PlayIdle();

		propPoint = transform.Find("RocketStyle/PropulsionPoint");
		propDirection = transform.Find("RocketStyle/PropulsionDirection");
		grabPoint = transform.Find("RocketStyle/GrabPoint");
	}
	
	// Update is called once per frame
	void Update () {
		
		if ( rocketStyle > 0 ) {
			rocketStyle -= Time.deltaTime;
			Vector2 v = CommonUtils.NormalVector(propPoint.position, propDirection.position);
			myBody.AddForceAtPosition(v * rocketSpeed, propPoint.position, ForceMode2D.Impulse);
			if ( rocketStyle <= 0 ) {
				Reactivate();
			}
		}


		if (deactivated) {
			deactivatedTimer -= Time.deltaTime;
			if ( deactivatedTimer <= 0.0f ) {
				Reactivate();
			}
			if (!isStageThree && (transform.position.y < -50 || transform.position.x < -34
									|| transform.position.y > 54)) {
				Reactivate();
			}
		}

	}

	public void SetTargetPosition(Vector3 v) {
		targetPosition = v;
	}
	public void SetTargetDirection(Direction v) {
		targetDirection = v;
	}

	public void PlayCharge() {
		renderer.sprite = charge;
		damageCollider.gameObject.SetActive(true);
	}

	public void PlayIdle() {
		renderer.sprite = idle;
		damageCollider.gameObject.SetActive(false);
	}

	public void PlayReadyShootMissile() {
		renderer.sprite = charge;
		damageCollider.gameObject.SetActive(false);
	}

	public void PlayShootMissile() {
		renderer.sprite = slam;
		damageCollider.gameObject.SetActive(false);
	}

	public Direction GetDirectionToTargetPosition() {
		float x = targetPosition.x - transform.position.x;
		float y = targetPosition.y - transform.position.y;
		if (Mathf.Abs(x) > Mathf.Abs(y)) {
			if (x > 0) {
				return Direction.EAST;
			} else
				return Direction.WEST;
		} else {
			if (y > 0) {
				return Direction.NORTH;
			} else
				return Direction.SOUTH;
		}
	}
	public Direction GetRandomDirection() {
		switch (Random.Range(0,4)) {
			case 0:
			default:
				return Direction.NORTH;
			case 1:
				return Direction.EAST;
			case 2:
				return Direction.WEST;
			case 3:
				return Direction.SOUTH;
		}
	}

	public Vector2 DirectionAsVector( Direction d ) {
		switch (d) {
			case BossHandFSM.Direction.NORTH:
			default:
				return Vector2.up;
			case BossHandFSM.Direction.EAST:
				return Vector2.right;
			case BossHandFSM.Direction.WEST:
				return Vector2.left;
			case BossHandFSM.Direction.SOUTH:
				return Vector2.down;
		}
	}

	public void FireRocketEast() {
		GameObject newBomb = Instantiate(bombPrefab);
		newBomb.transform.position = bombSpawn.position;
		newBomb.transform.rotation = Quaternion.Euler(0, 0, CommonUtils.ThetaBetweenD(bombSpawn.position, launchDirection.position) + 180);
		Rigidbody2D bombBody = newBomb.GetComponent<Rigidbody2D>();
		bombBody.velocity = (launchDirection.position - bombSpawn.position).normalized * 2.0f;
	}

	public void Deactivate() {

		PlayIdle();
		SetBodyTypeDisabled();
		mainCollider.enabled = true;
		pullSwitch.gameObject.SetActive(false);
		audioSource.Play();
		deactivated = true;
		animator.SetTrigger("Deactivated");
		deactivatedTimer = deactivatedMaxTime;
	}

	public void Reactivate() {
		PlayIdle();
		SetBodyTypeRegular();
		mainCollider.enabled = false;
		pullSwitch.gameObject.SetActive(true);
		//animator.SetBool("Reactivated", true);
		animator.SetTrigger("Reactivated");
		deactivated = false;
		rocketStyle = 0;
	}

	public void SetBodyTypeRegular() {
		myBody.bodyType = RigidbodyType2D.Kinematic;
		myBody.velocity = Vector2.zero;
		myBody.angularVelocity = 0.0f;
		myBody.angularDrag = 0f;
		myBody.drag = 0;
		myBody.gravityScale = 1;
	}

	public void SetBodyTypeDisabled() {
		myBody.bodyType = RigidbodyType2D.Dynamic;
		myBody.velocity = Vector2.zero;
		myBody.angularVelocity = 2.0f;
		myBody.angularDrag = 0f;
		myBody.drag = 0;
		myBody.gravityScale = 1;
	}

	public void SetBodyTypeRocket() {
		myBody.bodyType = RigidbodyType2D.Dynamic;
		myBody.velocity = Vector2.zero;
		myBody.angularVelocity = 0.0f;
		myBody.angularDrag = 10f;
		myBody.drag = 0;
		myBody.gravityScale = 0;
	}

	public void Open() {
		Deactivate();
	}

	public void Close() {
		
	}

	public void Toggle() {
	
	}

	public void GetGrabbed(object playerController) {
		SetBodyTypeRocket();
		PlayerController pc = playerController as PlayerController;
		pc.tentacle.AttachEnd(myBody, grabPoint.position);
		pc.carryMouseSpring.connectedAnchor = myBody.transform.InverseTransformPoint(grabPoint.position);
		PlayReadyShootMissile();
		rocketStyle = rocketTime;
	}

	public void GetGrabbedReleased() {
		rocketStyle = rocketTime;
	}

	public void StartStageTwo() {
		chargePlayer = true;
		pullSwitch.gameObject.SetActive(true);
	}

	public void OnTriggerEnter2D(Collider2D collision) {
		if (rocketStyle > 0 && collision.gameObject.CompareTag("Boss")) {
			GameObject explosion = Instantiate(explosionPrefab);
			explosion.transform.position = new Vector2(transform.position.x, transform.position.y + 1.0f);
			explosion.transform.localScale = new Vector3(3, 3, 1);
			explosion.SetActive(true);
			Reactivate();
			//Debug.Log("Trigger");
			Fungus.Flowchart.BroadcastFungusMessage("HitBossWithHand");
		}
	}

	public void StartStageThree() {
		catchUpToPlayer = true;
		isStageThree = true;
		//pullSwitch.gameObject.SetActive(false);
	}

	public void Pause() {
		animator.enabled = false;
	}

	public void Unpause() {
		animator.enabled = true;
	}
}
