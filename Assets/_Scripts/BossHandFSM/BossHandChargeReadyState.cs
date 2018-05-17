using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandChargeReadyState : StateMachineBehaviour {

	BossHandFSM bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		
		switch (bossScript.targetDirection) {
			case BossHandFSM.Direction.NORTH:
				bossScript.SetTargetPosition(CommonUtils.Position(bossScript.transform) + Vector2.up * bossScript.chargeDistance);
				break;
			case BossHandFSM.Direction.EAST:
				bossScript.SetTargetPosition(CommonUtils.Position(bossScript.transform) + Vector2.right * bossScript.chargeDistance);
				break;
			case BossHandFSM.Direction.WEST:
				bossScript.SetTargetPosition(CommonUtils.Position(bossScript.transform) + Vector2.left * bossScript.chargeDistance);
				break;
			case BossHandFSM.Direction.SOUTH:
				bossScript.SetTargetPosition(CommonUtils.Position(bossScript.transform) + Vector2.down * bossScript.chargeDistance);
				break;
		}

		
		animator.SetBool("LeaveState", true);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
