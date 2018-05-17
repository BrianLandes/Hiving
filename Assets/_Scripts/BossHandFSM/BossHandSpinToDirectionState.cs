using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHandSpinToDirectionState : StateMachineBehaviour {

	public BossHandFSM.Direction direciton;
	public int overRotations = 2;
	public float spinTime = 1;

	BossHandFSM bossScript;

	private float startRotation;
	private float targetRotation;
	private float timer = 0;

	override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		bossScript = animator.gameObject.GetComponent<BossHandFSM>();

		startRotation = animator.transform.rotation.eulerAngles.z;
		startRotation = startRotation % 360;
		animator.transform.rotation = Quaternion.Euler(0, 0, startRotation);
		switch (direciton) {
			case BossHandFSM.Direction.NORTH:
				targetRotation = 90;
				break;
			case BossHandFSM.Direction.EAST:
				targetRotation = 0;
				break;
			case BossHandFSM.Direction.WEST:
				targetRotation = 180;
				break;
			case BossHandFSM.Direction.SOUTH:
				targetRotation = 270;
				break;
		}
		if (startRotation > targetRotation) {
			targetRotation -= 360 * overRotations;
		} else {
			targetRotation += 360 * overRotations;
		}
		timer = 0;
		animator.SetBool("LeaveState", false);
	}

	override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
		timer += Time.deltaTime;
		//float z = Mathf.Lerp(startRotation,targetRotation,timer/spinTime);
		float z = Ease.Value(Ease.Type.IN_OUT, timer, startRotation, targetRotation, spinTime);
		bossScript.transform.rotation = Quaternion.Euler(0, 0, z);

		if (timer / spinTime > 1.0f) {
			bossScript.transform.rotation = Quaternion.Euler(0, 0, targetRotation);
			animator.SetBool("LeaveState", true);
		}
	}

	override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}

	override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {

	}
}
