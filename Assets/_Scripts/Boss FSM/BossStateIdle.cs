using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateIdle : StateMachineBehaviour {

	public float idleTime = 4.0f;

	float timer = 0;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer = 0;
		animator.SetBool("LeaveIdle", false);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		if (timer >= idleTime) {
			animator.SetBool("LeaveIdle", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
