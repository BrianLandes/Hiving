using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFsmScript : MonoBehaviour {

	public float moveSpeed = 1.0f;
	public Transform[] targetPoints;
	public GameObject bossArmature;

	List<Transform> tps = new List<Transform>();

	MechaPirateDragonBones dragonBones;
	Animator animator;
	Rigidbody2D myBody;

	bool stageThree = false;
	bool disabled = false;
	// Use this for initialization
	void Start () {
		dragonBones = bossArmature.GetComponent<MechaPirateDragonBones>();
		animator = GetComponent<Animator>();
		myBody = bossArmature.GetComponentInChildren<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
		//if (disabled && myBody.transform.position.y < -50) {
		//	myBody.MovePosition(Vector2.up * 10.0f);
		//}
	}

	public Transform NextTargetPoint() {
		if ( tps.Count == 0 ) {
			foreach (var point in targetPoints) {
				tps.Add(point);
			}
		}
		int chosenIndex = Random.Range(0, tps.Count-1);
		Transform value = tps[chosenIndex];
		tps.RemoveAt(chosenIndex);
		return value;
	}

	//public void HitWithRocket() {
	//	//Debug.Log("HitWithRocket");
	//	dragonBones.PlayDamaged();
	//	animator.SetTrigger("Damaged");
	//}

	public void Play(string name) {
		dragonBones.Play(name);
	}

	public void StartStageThree() {
		animator.SetBool("FlyAway", true);
		animator.SetBool("IsStageThree", true);
		
		stageThree = true;
	}

	//public void Ragdoll() {
	//	_ragdoll(bossArmature.transform.Find("Slots"));
	//	animator.SetTrigger("Deactivated");
	//	disabled = true;
	//}

	//void _ragdoll(Transform parent) {
	//	foreach (Transform child in parent) {
	//		if ( child.gameObject.CompareTag("Boss" ))
	//			child.gameObject.layer = LayerMask.NameToLayer("BackgroundObjects");
	//		if (!child.gameObject.name.Equals("Body")) {
	//			Rigidbody2D body = child.gameObject.GetComponent<Rigidbody2D>();
	//			if (body != null) {
	//				body.bodyType = RigidbodyType2D.Dynamic;
	//			}
	//		}
	//		if (child.gameObject.name.Equals("HappyFace")) {
	//			Destroy(child.gameObject);
	//		} 
	//		_ragdoll(child);
	//	}
	//}

	public void GetGrabbed() {
		if ( stageThree ) {
			//myBody.bodyType = RigidbodyType2D.Dynamic;
			animator.SetTrigger("Grabbed");
		}
	}
}
