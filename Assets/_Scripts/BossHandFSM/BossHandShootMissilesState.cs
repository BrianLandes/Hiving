using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandShootMissilesState : StateMachineBehaviour {

	BossHandFSM bossScript;

	public float readyTime = 1.0f;
	public float shootTime = 1.0f;

	private float timer = 0;
	private bool readying = true;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		bossScript.PlayReadyShootMissile();
		timer = 0;
		readying = true;
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		if (readying) {
			if (timer >= readyTime) {
				bossScript.PlayShootMissile();
				bossScript.FireRocketEast();
				timer = 0;
				readying = false;
			}
		} else {
			if (timer >= shootTime) {
				bossScript.PlayReadyShootMissile();
				timer = 0;
				readying = true;
			}
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
