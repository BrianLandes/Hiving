using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandReadyShootMissilesState : StateMachineBehaviour {

	BossHandFSM bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		animator.SetBool("LeaveState", false);

		PlayerController pc = GameManager.Instance.playerController;

		if (bossScript.transform.position.y > pc.transform.position.y) {

			bossScript.SetTargetPosition(
				CommonUtils.Position(pc.transform)
				+ Vector2.left
				* bossScript.chargeDistance
				+ Vector2.down
				* bossScript.chargeDistance);
		} else {
			bossScript.SetTargetPosition(
				CommonUtils.Position(pc.transform)
				+ Vector2.left
				* bossScript.chargeDistance
				+ Vector2.up
				* bossScript.chargeDistance);
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
