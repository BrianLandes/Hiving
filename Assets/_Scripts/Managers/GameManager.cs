using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class GameManager : GenericSingletonClass<GameManager> {

	public PlayerController playerController;

	//public bool debuggin = false;

	//public GameObject tentacle;

	public bool skipIntro = false;
	public static bool Paused { get; set; }

	// Boss Stage persistance variables

	public static bool passedStageThree = false;
	public static int bossStageDiedCount = 0;

	public void Start() {
		//DisableTentacle();
		//tentacle.SetActive(false);
	}
	
	public void StartLevelRight() {
		if (skipIntro && Application.isEditor) {
			Fungus.Flowchart.BroadcastFungusMessage("QuickSkipStart");
		} else {
			Fungus.Flowchart.BroadcastFungusMessage("StartIntro");
		}
	}

	public void SetTimeScale(float scale) {
		Time.timeScale = scale;
	}

	public void EnableMovement() {
		playerController.movementEnabled = true;
	}

	public void EnableTentacle() {
		playerController.tentacleEnabled = true;
	}

	public void DisableMovement() {
		playerController.movementEnabled = false;
	}

	public void DisableTentacle() {
		playerController.tentacleEnabled = false;
	}

	public void SetGrabbingNothing() {
		playerController.StopGripping();
	}

	public void StopMoving() {
		playerController.StopMoving();
	}

	public void EnableAirControl() {
		playerController.m_AirControl = true;
	}

	public void DisableAirControl() {
		playerController.m_AirControl = false;
	}


	public static void HealPlayer() {
		Instance.playerController.currentHealth = Instance.playerController.maxHealth;
	}

	public void SnapCameraAndTentacleToPlayer() {
		Camera.main.transform.position = playerController.transform.position;
		Camera2DFollow follow = Camera.main.GetComponent<Camera2DFollow>();
		follow.Reset();
		playerController.tentacle.transform.position = playerController.transform.position;
		playerController.tentacle.TeleportToBase();
	}

	public static void CopyTentSegments(Tentacle source, Tentacle dest) {
		dest.ClearSegments();

		dest.segments = source.segments;
		dest.startWidth = source.startWidth;
		dest.endWidth = source.endWidth;
		dest.length = source.length;

		dest.GenerateSegments();

		for (int i = 0; i < dest.tentJoints.Count; i++) {
			TentJoint destJoint = dest.tentJoints[i];
			TentJoint sourceJoint = source.tentJoints[i];

			destJoint.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.transform.position);
			destJoint.gameObject.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.gameObject.transform.position);
			//destJoint.gameObject.GetComponent<StretchTo>().RecalculateStart();
			//destJoint.gameObject.GetComponent<StretchTo>().RunStretch();
			//destJoint.lastPosition = sourceJoint.lastPosition;
			//destJoint.velocity = sourceJoint.velocity;
			////public List<Vector2> targetPositions;
			//destJoint.tOffset = sourceJoint.tOffset;
		}

		//dest.t = source.t;
	}

	public static void CopyTentSubSegments(Tentacle source, Tentacle dest, int start, int end) {
		if (start < 0 || end > source.segments) {
			throw new Exception("Can't make a sub segment with these bounds.");
		}
		dest.ClearSegments();

		dest.segments = end - start + 1;
		float width_dif = source.endWidth - source.startWidth;
		dest.startWidth = source.startWidth + width_dif * ((float)start / (float)source.segments);
		dest.endWidth = source.startWidth + width_dif * ((float)end / (float)source.segments);
		dest.length = source.length * ((float)dest.segments / (float)source.segments);

		dest.GenerateSegments();

		for (int i = 0; i < dest.tentJoints.Count; i++) {
			TentJoint destJoint = dest.tentJoints[i];
			TentJoint sourceJoint = source.tentJoints[i + start];

			destJoint.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.transform.position);
			destJoint.gameObject.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.gameObject.transform.position);
			//destJoint.lastPosition = sourceJoint.lastPosition;
			//destJoint.velocity = sourceJoint.velocity;
			////public List<Vector2> targetPositions;
			//destJoint.tOffset = sourceJoint.tOffset;
		}


		if (start > 0) {
			dest.transform.position = source.tentJoints[start - 1].transform.position;

		} else {
			dest.transform.position = source.transform.position;
		}
		if (end < source.segments - 1) {
			dest.endPoint.position = source.tentJoints[end].transform.position;
		} else {
			dest.endPoint.position = source.endPoint.position;
		}
		dest.baseSegment.transform.localPosition = Vector2.zero;
		//dest.t = source.t;
	}

	public void CameraZoomBoss() {
		StopAllCoroutines();
		StartCoroutine(_lerpCameraPosition(Camera.main.gameObject.GetComponent<Camera>(), 15f, 0.8f));
	}

	public void CameraZoomNormal() {
		StopAllCoroutines();
		StartCoroutine(_lerpCameraPosition(Camera.main.gameObject.GetComponent<Camera>(), 7f, 0.8f));
	}

	public void CameraZoomDeath() {
		StopAllCoroutines();
		StartCoroutine(_lerpCameraPosition(Camera.main.gameObject.GetComponent<Camera>(), 3f, 0.8f));
	}

	public void SetCameraZoom( float zoom ) {
		StopAllCoroutines();
		StartCoroutine(_lerpCameraPosition(Camera.main.gameObject.GetComponent<Camera>(), zoom, 0.8f));
	}


	public void Restart() {
		playerController.currentHealth = playerController.maxHealth;
	}

	static IEnumerator _lerpCameraPosition(Camera camera, float newSize, float fadeBackTime = 1.0f) {
		float originalPosition = camera.orthographicSize;
		float t = 0.0f;
		while (camera != null && camera.orthographicSize != newSize) {
			t += Time.deltaTime;
			float v = Mathf.Min(1f, t / fadeBackTime);

			camera.orthographicSize = originalPosition + (newSize - originalPosition) * v;
			yield return null;
		}
	}

	public void PassedStageThree() {
		passedStageThree = true;
	}

	public void ResetBossLevel() {
		passedStageThree = false;
		bossStageDiedCount = 0;
	}

	public void OccurBossStage() {
		if (!passedStageThree) {
			if (bossStageDiedCount>=2)
				Fungus.Flowchart.BroadcastFungusMessage("StartStage1v2");
			else
				Fungus.Flowchart.BroadcastFungusMessage("StartStage1v1");
			bossStageDiedCount++;
		} else {
			Fungus.Flowchart.BroadcastFungusMessage("StartStage3");
		}
	}
}
