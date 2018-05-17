using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackWorldPosition : MonoBehaviour {

	public Transform target;

	public Vector3 offset = Vector3.left;

	public float screenBounds = 10f;

	public bool disappearWhenOffScreen = false;
	public bool disappearWhenOnScreen = false;

	private RectTransform rect;

	private bool visible = true;

	// Use this for initialization
	void Awake() {
		rect = GetComponent<RectTransform>();
	}

	// Update is called once per frame
	void Update() {
		TrackPosition();
	}



	public void TrackPosition() {
		transform.position = Camera.main.WorldToScreenPoint(target.position + offset);

		float left = rect.rect.width * 0.5f + screenBounds;
		float right = Screen.width - rect.rect.width * 0.5f - screenBounds;
		float top = Screen.height - rect.rect.height * 0.5f - screenBounds;
		float bottom = rect.rect.height * 0.5f + screenBounds;

		bool outOfBounds = false;

		if (transform.position.x < left) {
			transform.position = new Vector2(left, transform.position.y);
			outOfBounds = true;
		}


		if (transform.position.x > right) { 
			transform.position = new Vector2(right, transform.position.y);
			outOfBounds = true;
		}

		if (transform.position.y > top) {
			transform.position = new Vector2(transform.position.x, top);
			outOfBounds = true;
		}

		if (transform.position.y < bottom) {
			transform.position = new Vector2(transform.position.x, bottom);
			outOfBounds = true;
		}

		if ( visible ) {

			if ( outOfBounds && disappearWhenOffScreen ) {
				CommonUtils.DisableChildren(transform);
				visible = false;
			}
			if (!outOfBounds && disappearWhenOnScreen) {
				CommonUtils.DisableChildren(transform);
				visible = false;
			}
		} else {
			if (!outOfBounds && disappearWhenOffScreen) {
				CommonUtils.EnableChildren(transform);
				visible = true;
			}
			if (outOfBounds && disappearWhenOnScreen) {
				CommonUtils.EnableChildren(transform);
				visible = true;
			}
		}
	}
}
