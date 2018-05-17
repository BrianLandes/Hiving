using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMoveToPoint : StateMachineBehaviour {
	
	float moveTime;

	Vector3 startPosition;
	Vector3 targetPosition;
	float timer;

	BossFsmScript bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossFsmScript>();
		startPosition = bossScript.bossArmature.transform.position;
		targetPosition = bossScript.NextTargetPoint().position;
		moveTime = CommonUtils.Distance(targetPosition, startPosition) / bossScript.moveSpeed;
		timer = 0;
		animator.SetBool("LeaveState", false);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		bossScript.bossArmature.transform.position = Vector3.Lerp(startPosition, targetPosition, timer / moveTime);
		if (timer>moveTime) {
			animator.SetBool("LeaveState", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}