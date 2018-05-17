using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandChargeForwardState : StateMachineBehaviour {

	public float speed = 20;

	public float maxDistance = 20;

	BossHandFSM bossScript;

	private float projectedTime = 0;
	private float timer = 0;
	Vector2 n, start;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		bossScript.PlayCharge();
		
		projectedTime = maxDistance / speed;
		timer = 0;
		start = bossScript.transform.position;

		n = bossScript.DirectionAsVector(bossScript.targetDirection);

		animator.SetBool("LeaveState", false);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		
	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		float l = Ease.Value(Ease.Type.IN_OUT, timer, 0, maxDistance, projectedTime);
		//bossScript.transform.position = CommonUtils.Position( bossScript.transform) + n * speed *Time.deltaTime;
		bossScript.transform.position = start + n * l;

		if (timer > projectedTime) {
			bossScript.PlayIdle();
			animator.SetBool("LeaveState", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
