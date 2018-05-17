using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DragonBones;

public class DragonBonesController : MonoBehaviour {

	UnityArmatureComponent armature;

	public bool ground = false;
	public float speed = 0.0f;
	public float vSpeed = 0.0f;

	public State state = State.Idle;

	public const float transTime = 0.1f;

	public enum State {
		Idle,
		Walk,
		Jump
	}

	// Use this for initialization
	void Start () {
		armature = GetComponent<UnityArmatureComponent>();
		//armature.animation.GotoAndPlayByTime("Walk",2.0f);
		armature.animation.FadeIn("Walk",1);
	}
	
	// Update is called once per frame
	void Update () {
		switch (state) {
			case State.Idle:
				if (!ground) {
					ChangeState(State.Jump);
				} else if (Mathf.Abs(speed) >= 0.01f) {
					ChangeState(State.Walk);
				}
				break;
			case State.Walk:
				if (!ground) {
					ChangeState(State.Jump);
				} else if (Mathf.Abs(speed) < 0.01f) {
					ChangeState(State.Idle);
				}
				break;
			case State.Jump:
				if (ground) {
					ChangeState(State.Idle);
				}
				break;
		}
	}

	void ChangeState(State newState) {
		switch (newState) {
			case State.Idle:
				armature.animation.FadeIn("Idle", transTime);
				break;
			case State.Walk:
				armature.animation.FadeIn("Walk", transTime);
				break;
			case State.Jump:
				armature.animation.FadeIn("MidAir", transTime);
				break;
		}
		state = newState;
	}
}
