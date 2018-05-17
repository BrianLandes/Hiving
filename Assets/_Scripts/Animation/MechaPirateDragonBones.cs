using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;


public class MechaPirateDragonBones : MonoBehaviour {

	UnityArmatureComponent armature;

	// Use this for initialization
	void Start () {
		armature = GetComponent<UnityArmatureComponent>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void PlaySetBomb() {
		armature.animation.FadeIn("SetBomb", 0.1f, 1);
	}

	public void PlayClimb() {
		armature.animation.FadeIn("Climb", 0.1f);
	}

	public void PlayDangleNurse() {
		armature.animation.FadeIn("DangleNurse", 0.1f, 1);
	}

	public void PlayDamaged() {
		armature.animation.FadeIn("Damaged", 0.1f);
	}

	public void PlayIdle() {
		armature.animation.FadeIn("Idle", 0.1f);
	}

	public void Play( string name ) {
		armature.animation.FadeIn(name, 0.1f);
	}

	public void PlayDangleNurseIdle() {
		armature.animation.FadeIn("DangleNurseIdle", 0.1f);
	}

	public void PlayPutNurseAway() {
		armature.animation.FadeIn("PutNurseAway", 0.1f, 1);
	}
}
