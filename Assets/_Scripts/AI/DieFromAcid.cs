using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DieFromAcid : MonoBehaviour {

	public GameObject bloodSplatterPrefab;

	bool dying = false;

	public void Die() {
		if (!dying) {
			GameObject explosion = Instantiate(bloodSplatterPrefab);
			explosion.transform.position = transform.position;
			explosion.transform.localScale = new Vector3(2, 2, 2);
			explosion.transform.Rotate(new Vector3(0, 0, 90));
			explosion.SetActive(true);
			dying = true;
			Destroy(gameObject);
		}
	}
}
