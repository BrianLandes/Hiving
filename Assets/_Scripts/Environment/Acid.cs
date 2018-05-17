using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Acid : MonoBehaviour {

	void OnTriggerEnter2D(Collider2D collider) {
		//AcidDamage(collider.gameObject);
	}

	void OnCollisionEnter2D(Collision2D collision) {
		//AcidDamage(collision.gameObject);
		GameObject go = collision.gameObject;
		if (go.tag == "Player") {
			ExplosionManager.BloodSplatter(go.transform.position, 2, 90);

			PlayerController playerController = GameManager.Instance.playerController;
			Vector2 v = CommonUtils.NormalVector(transform.position, playerController.transform.position );
			playerController.Damage(1, v, 20);
			//playerController.Damage(1);

			//Fungus.Flowchart.BroadcastFungusMessage(message);
		} else {
			DieFromAcid die = go.GetComponent<DieFromAcid>();
			if (die != null) {
				die.Die();
			}
		}
	}

	//void AcidDamage( GameObject go ) {
	//	if (go.tag == "Player") {
	//		GameObject explosion = Instantiate(bloodSplatterPrefab);
	//		explosion.transform.position = transform.position;
	//		explosion.transform.localScale = new Vector3(2, 2, 2);
	//		explosion.transform.Rotate(new Vector3(0, 0, 90));
	//		explosion.SetActive(true);

	//		PlayerController playerController = GameManager.Instance.playerController;
	//		playerController.Damage(1, transform, 20);
	//		//playerController.Damage(1);

	//		//Fungus.Flowchart.BroadcastFungusMessage(message);
	//	} else {
	//		DieFromAcid die = go.GetComponent<DieFromAcid>();
	//		if (die != null) {
	//			die.Die();
	//		}
	//	}
	//}
}
