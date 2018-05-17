using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisableAfterAnimation : MonoBehaviour {

	public float delay = 0f;

	private float timer = 0f;

	// Use this for initialization
	void OnEnable() {
		timer = this.GetComponent<Animation>().clip.length + delay;
	}

	void Update() {
		if (timer > 0) {
			timer -= Time.deltaTime;
			if (timer <= 0) {
				gameObject.SetActive(false);
			}
		}
	}
}
