using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingDoor : MonoBehaviour {

	public bool startClosed = true;

	public bool closed = false;

	private Transform closedPosition;
	private Transform openPosition;
	private GameObject doorObject;

	// Use this for initialization
	void Start () {
		closedPosition = transform.Find("ClosedPosition");
		openPosition = transform.Find("OpenPosition");
		doorObject = transform.Find("DoorObject").gameObject;

		closed = startClosed;

		if ( startClosed )
			Close();
		else
			Open();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void Open() {
		StopAllCoroutines();
		StartCoroutine(_lerpPosition(doorObject,openPosition));
		closed = false;
	}

	public void Close() {
		StopAllCoroutines();
		StartCoroutine(_lerpPosition(doorObject, closedPosition));
		closed = true;
	}

	public void Toggle() {
		if (closed)
			Open();
		else
			Close();
	}

	static IEnumerator _lerpPosition(GameObject targetObject, Transform targetPosition, float fadeBackTime = 1.0f) {
		Vector3 originalPosition = targetObject.transform.position;
		float t = 0.0f;
		while (targetObject != null && targetObject.transform.position != targetPosition.position) {
			t += Time.deltaTime;
			targetObject.transform.position = Vector3.Lerp(originalPosition, targetPosition.position, t / fadeBackTime);
			yield return null;
		}
	}
}
