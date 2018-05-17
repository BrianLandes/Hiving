using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateWaitForPlayer : StateMachineBehaviour {

	public float distance = 4;

	PlayerController player;
	BossFsmScript bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		animator.SetBool("LeaveState", false);
		animator.SetBool("FlyAway", false);
		player = GameManager.Instance.playerController;
		bossScript = animator.gameObject.GetComponent<BossFsmScript>();
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		if (bossScript.bossArmature.transform.position.y - player.transform.position.y < distance) {
			animator.SetBool("LeaveState", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
