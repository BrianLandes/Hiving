using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandChargePlayerState : StateMachineBehaviour {
	
	BossHandFSM bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();

		BossHandFSM.Direction d = bossScript.GetRandomDirection();
		bossScript.SetTargetPosition(
			CommonUtils.Position(GameManager.Instance.playerController.transform)
			- bossScript.DirectionAsVector( d )
			* bossScript.chargeDistance);
		//switch (d) {
		//	case BossHandFSM.Direction.NORTH:
		//		bossScript.SetTargetPosition(CommonUtils.Position(GameManager.Instance.playerController.transform) + Vector2.down * bossScript.chargeDistance);
		//		break;
		//	case BossHandFSM.Direction.EAST:
		//		bossScript.SetTargetPosition(CommonUtils.Position(GameManager.Instance.playerController.transform) + Vector2.left * bossScript.chargeDistance);
		//		break;
		//	case BossHandFSM.Direction.WEST:
		//		bossScript.SetTargetPosition(CommonUtils.Position(GameManager.Instance.playerController.transform) + Vector2.right * bossScript.chargeDistance);
		//		break;
		//	case BossHandFSM.Direction.SOUTH:
		//		bossScript.SetTargetPosition(CommonUtils.Position(GameManager.Instance.playerController.transform) + Vector2.up * bossScript.chargeDistance);
		//		break;
		//}

		
		bossScript.SetTargetDirection(d);
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
