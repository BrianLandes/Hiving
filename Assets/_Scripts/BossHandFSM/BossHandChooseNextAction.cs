using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandChooseNextAction : StateMachineBehaviour {

	BossHandFSM bossScript;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		bossScript.PlayIdle();
		animator.SetBool("LeaveState", false);
		animator.SetBool("ChargePlayer", false);
		animator.SetBool("CatchUpToPlayer", false);
		//animator.SetBool("ShootMissilesRight", false);

		

		PlayerController pc = GameManager.Instance.playerController;
		if (bossScript.catchUpToPlayer && pc.transform.position.y > bossScript.transform.position.y + 10) {
			animator.SetBool("CatchUpToPlayer", true);
		} else {
			List<int> choices = new List<int>();
			if (bossScript.chargePlayer) {
				choices.Add(0);
			}
			if (bossScript.shootMissilesRight) {
				choices.Add(1);
			}
			switch (CommonUtils.RandomChoice<int>(choices)) {
				case 0: // ChargePlayer
					animator.SetBool("ChargePlayer", true);
					break;
				case 1: // ShootMissilesEast
					animator.SetTrigger("ShootMissilesEast");
					break;
			}
		}

		

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
