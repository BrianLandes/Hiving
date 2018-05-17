using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandFloatToPositionState : StateMachineBehaviour {

	public float speed = 3;

	BossHandFSM bossScript;

	private float projectedTime = 0;
	private float timer = 0;
	Vector2 n, start;
	float length = 0;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();
		animator.SetBool("LeaveState", false);
		length = (bossScript.transform.position - bossScript.targetPosition).magnitude;
		projectedTime = length / speed;
		timer = 0;
		start = bossScript.transform.position;
		n = CommonUtils.NormalVector(bossScript.transform.position, bossScript.targetPosition);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		float l = Ease.Value(Ease.Type.IN_OUT, timer, 0, length, projectedTime);
		//bossScript.transform.position = CommonUtils.Position( bossScript.transform) + n * speed *Time.deltaTime;
		bossScript.transform.position = start + n * l;

		if (timer>projectedTime) {
			animator.SetBool("LeaveState", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
