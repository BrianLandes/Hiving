using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpringStretchWatch : MonoBehaviour {

	public AudioClip springStretch;
	public AudioClip springSnap;

	public float minStretchLength = 1;
	public float maxStretchLength = 5;

	public bool hitBelowMinStretchLength = true;

	private SpringJoint2D spring;
	private AudioSource audioSource;

	private bool snapped = false;

	// Use this for initialization
	void Start () {
		spring = GetComponent<SpringJoint2D>();
		audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {
		if (spring == null) {
			if (!snapped) {
				snapped = true;
				PlaySnapNoise();
				transform.Find("Cable").gameObject.SetActive(false);
				Fungus.Flowchart.BroadcastFungusMessage("BossNeckSnapped");
			}
		} else {
			float springLength = CommonUtils.Distance(spring.connectedBody.transform.TransformPoint(spring.connectedAnchor), transform.position);
			if (springLength < minStretchLength) {
				hitBelowMinStretchLength = true;
			} else if (springLength > maxStretchLength && hitBelowMinStretchLength) {
				hitBelowMinStretchLength = false;
				PlayStretchNoise();
			}
		}
		if (transform.position.y < -50) {
			Fungus.Flowchart.BroadcastFungusMessage("HeadInKillZone");
		}
	}

	public void PlayStretchNoise() {
		if (audioSource != null && springStretch != null) {
			audioSource.clip = springStretch;
			audioSource.pitch = Random.Range(1.0f - 0.05f, 1.0f + 0.05f);
			audioSource.volume = 0.9f;
			audioSource.Play();
		}
	}

	public void PlaySnapNoise() {
		if (audioSource != null && springSnap != null) {
			audioSource.clip = springSnap;
			audioSource.pitch = Random.Range(1.0f - 0.05f, 1.0f + 0.05f);
			audioSource.volume = 0.9f;
			audioSource.Play();
		}
	}
}
