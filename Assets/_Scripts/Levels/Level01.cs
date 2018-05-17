using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class Level01 : MonoBehaviour {

	public GameObject roomFloorPlatform;
	public GameObject roomWallPlatform;
	public GameObject nurse;
	public Tentacle tentacle;
	public Tentacle playerTentacle;
	public Tentacle tentacleSecond;
	public Tentacle tentacleRipped;

	public GameObject explosionPrefab;
	public GameObject bloodSplatterPrefab;
	public Transform explosionTarget;

	public Camera2DFollow camera;

	public GameObject nurseGrabPoint;

	public Sprite rippedTentacle;

	public Transform swingTarget;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void AttachTentacleToNurse() {
		Transform tentAttachPoint = nurse.transform.Find("TentacleAttachPoint");
		tentacle.AttachBase(nurse.GetComponent<Rigidbody2D>(), tentAttachPoint.position );
		tentacleSecond.AttachBase(nurse.GetComponent<Rigidbody2D>(), tentAttachPoint.position);
		tentacleRipped.gameObject.SetActive(true);
		tentacleRipped.AttachBase(nurse.GetComponent<Rigidbody2D>(), tentAttachPoint.position);
		tentacleRipped.gameObject.SetActive(false);
		Transform tentIdle = nurse.transform.Find("Tentacle Idle");
		tentacle.AttachEnd(tentIdle.GetComponent<Rigidbody2D>(), tentIdle.position);

		Transform tentIdle2 = nurse.transform.Find("Tentacle Idle (1)");
		tentacleSecond.AttachEnd(tentIdle2.GetComponent<Rigidbody2D>(), tentIdle2.position);
	}

	public void DropRoomFloor() {
		Rigidbody2D body = roomFloorPlatform.GetComponentInChildren<Rigidbody2D>();
		body.bodyType = RigidbodyType2D.Dynamic;

		body = roomWallPlatform.GetComponentInChildren<Rigidbody2D>();
		body.bodyType = RigidbodyType2D.Dynamic;
		body.AddForceAtPosition(Vector2.left * 6f, Vector2.up * 3.0f, ForceMode2D.Impulse);
	}

	public void SetOffFirstExplosion() {
		TriggerExplosion(explosionTarget.position);
	}

	public void TriggerExplosion( Vector3 position ) {
		GameObject explosion = Instantiate(explosionPrefab);
		explosion.transform.position = position;
		explosion.transform.localScale = new Vector3(3, 3, 3) ;
		explosion.SetActive(true);

		//for (int i = 0; i < raytraceCount; i++) {
		//	float theta = (float)i / (float)raytraceCount * Mathf.PI * 2.0f;

		//	RaytraceBlast(new Vector2(Mathf.Cos(theta), Mathf.Sin(theta)));
		//}

		//if (removeOnExplode)
		//	Destroy(gameObject);
	}

	public void FocusCameraOnTentacle() {
		camera.target = GetATentacleSegment().transform;
		StartCoroutine(_lerpCameraPosition(camera.gameObject.GetComponent<Camera>(), 2f, 0.4f));

		Rigidbody2D body = roomFloorPlatform.GetComponentInChildren<Rigidbody2D>();
		body.bodyType = RigidbodyType2D.Static;
	}

	static IEnumerator _lerpCameraPosition(Camera camera, float newSize, float fadeBackTime = 1.0f) {
		float originalPosition = camera.orthographicSize;
		float t = 0.0f;
		while (camera != null && camera.orthographicSize != newSize) {
			t += Time.deltaTime;
			float v = Mathf.Min( 1f, t / fadeBackTime);

			camera.orthographicSize = originalPosition + (newSize - originalPosition) * v;
			yield return null;
		}
	}

	public GameObject GetATentacleSegment() {
		return tentacle.tentJoints[3].gameObject;
	}

	public GameObject GetOneTentacleSegmentUp() {
		return tentacle.tentJoints[4].gameObject;
	}

	public void SetNurseGrabbingLedge() {
		tentacle.pullLength = 4f;
		tentacle.SetGrabbing(nurseGrabPoint.transform.parent.gameObject, nurseGrabPoint.transform.position);
		tentacle.AttachEnd(nurseGrabPoint.transform.parent.gameObject.GetComponent<Rigidbody2D>(), nurseGrabPoint.transform.position);
		
	}

	public void SetNurseGrabbingPlayer() {
		tentacleSecond.pullLength = 1f;
		GameObject player = GameManager.Instance.playerController.gameObject;
		//Debug.Log(player.GetComponent<Rigidbody2D>());
		tentacleSecond.SetGrabbing(player, player.transform.position);
		tentacleSecond.AttachEnd(player.GetComponent<Rigidbody2D>(), player.transform.position);
	}

	public void RipTentacle() {
		GameObject tentSegment = GetATentacleSegment();
		tentSegment.GetComponentInChildren<SpriteRenderer>().sprite = rippedTentacle;
		GameObject explosion = Instantiate(bloodSplatterPrefab);
		explosion.transform.position = tentSegment.transform.position;
		explosion.transform.localScale = new Vector3(-1, 1, 1);
		explosion.SetActive(true);
	}

	public void PutPlayerOnTentacle() {
		GameObject tent = GetOneTentacleSegmentUp();
		swingTarget.parent = tent.transform;
		swingTarget.localPosition = Vector2.zero;
	}

	public void FocusCameraOnPlayer() {
		GameObject player = GameManager.Instance.playerController.gameObject;
		camera.target = player.transform;
		StartCoroutine(_lerpCameraPosition(camera.gameObject.GetComponent<Camera>(), 7f, 0.4f));
	}
	
	public void StopGrabbing() {

		tentacle.StopGrabbing();
		tentacleSecond.StopGrabbing();
	}

	public void AttachTentaclePlayer() {
		playerTentacle.gameObject.SetActive(true);
		
		playerTentacle.pullLength = 2f;
		playerTentacle.SetGrabbing(nurseGrabPoint.transform.parent.gameObject, nurseGrabPoint.transform.position);
		playerTentacle.AttachEnd(nurseGrabPoint.transform.parent.gameObject.GetComponent<Rigidbody2D>(), nurseGrabPoint.transform.position);

		GameManager.CopyTentSubSegments(tentacle, playerTentacle, 4, tentacle.segments - 1);
		//GameManager.CopyTentSegments(tentacle, playerTentacle);

		//playerTentacle.joints = tentacle.joints;
		//foreach(var joint in tentacle.joints) {

		//	joint.gameObject.transform.SetParent(playerTentacle.transform);
		//}
		//playerTentacle.baseSegment = tentacle.baseSegment;
		//playerTentacle.baseSegment.transform.SetParent(playerTentacle.basePoint);
		//playerTentacle.baseSegment.transform.localPosition = Vector3.zero;
		//
		//playerTentacle.baseSegment.transform.localPosition = Vector3.zero;
		//playerTentacle.baseSegment.GetComponent<StretchTo>().RecalculateStart();
		tentacleRipped.gameObject.SetActive(true);
		GameManager.CopyTentSubSegments(tentacle, tentacleRipped, 0, 4);
		tentacle.gameObject.SetActive(false);

		GameObject tentSegment = GetATentacleSegment();
		GameObject explosion = Instantiate(bloodSplatterPrefab);
		explosion.transform.position = tentSegment.transform.position;
		explosion.transform.localScale = new Vector3(-1, 1, 1);
		explosion.SetActive(true);

		GameObject player = GameManager.Instance.playerController.gameObject;
		//tentacle.AttachBase(player.GetComponent<Rigidbody2D>(), player.transform.position);
		//Transform tentIdle = nurse.transform.Find("Tentacle Idle");
		//tentacle.AttachEnd(tentIdle.GetComponent<Rigidbody2D>(), tentIdle.position);
		//tentacle.pullLength = 2f;
		//tentacle.SetGrabbing(nurseGrabPoint.transform.parent.gameObject, nurseGrabPoint.transform.position);
		//tentacle.AttachEnd(nurseGrabPoint.transform.parent.gameObject.GetComponent<Rigidbody2D>(), nurseGrabPoint.transform.position);

		Transform tentIdle2 = nurse.transform.Find("Tentacle Idle (1)");

		Rigidbody2D tentBody = tentIdle2.GetComponent<Rigidbody2D>();
		tentBody.bodyType = RigidbodyType2D.Dynamic;
		tentIdle2.transform.position = player.transform.position;
		tentacleSecond.AttachEnd(tentBody, tentIdle2.position);
	}

	public void SetPlayerGrabbingLedge() {
		playerTentacle.gameObject.SetActive(true);

		playerTentacle.pullLength = 2f;
		playerTentacle.SetGrabbing(nurseGrabPoint.transform.parent.gameObject, nurseGrabPoint.transform.position);
		playerTentacle.AttachEnd(nurseGrabPoint.transform.parent.gameObject.GetComponent<Rigidbody2D>(), nurseGrabPoint.transform.position);
	}
}
