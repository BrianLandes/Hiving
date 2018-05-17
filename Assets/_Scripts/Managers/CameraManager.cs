using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets._2D;

public class CameraManager : GenericSingletonClass<CameraManager> {

	public static void ChangeZoom(float newSize, float zoomTime = 1.0f) {
		CameraManager.Instance.StartCoroutine(_lerpCameraZoom(newSize, zoomTime));
	}

	public static void SetDamping( float newDamping ) {
		GetCameraFollow().damping = newDamping;
	}

	public void SetTarget2(Transform newTarget) {
		GetCameraFollow().target = newTarget;
	}

	public void SetTargetToPlayer2() {
		SetDamping(0.1f);
		GetCameraFollow().target = GameManager.Instance.playerController.transform;
	}

	public static void SetTarget( Transform newTarget ) {
		GetCameraFollow().target = newTarget;
	}

	public static void SetTargetToPlayer() {
		SetDamping(0.1f);
		GetCameraFollow().target = GameManager.Instance.playerController.transform;
	}

	public static Camera2DFollow GetCameraFollow() {
		return Camera.main.gameObject.GetComponent<Camera2DFollow>();
	}

	public static Camera GetCamera() {
		return Camera.main;
	}

	static IEnumerator _lerpCameraZoom(float newSize, float fadeBackTime = 1.0f) {
		Camera camera = GetCamera();
		float originalPosition = camera.orthographicSize;
		float t = 0.0f;
		while (camera != null && camera.orthographicSize != newSize) {
			t += Time.deltaTime;
			float v = Mathf.Min(1f, t / fadeBackTime);

			camera.orthographicSize = originalPosition + (newSize - originalPosition) * v;
			yield return null;
		}
	}
}
