using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionManager : GenericSingletonClass<ExplosionManager> {

	public GameObject explosionPrefab;
	public GameObject bloodSplatterPrefab;

	public static void CreateExplosion( Vector2 position, float scale ) {
		GameObject explosion = Instantiate(Instance.explosionPrefab);
		explosion.transform.position = new Vector2(position.x, position.y);
		explosion.transform.localScale = new Vector3(scale, scale, 1f);
		explosion.SetActive(true);
	}

	public static void BloodSplatter(Vector2 position, float scale, float rotation) {
		GameObject explosion = Instantiate(Instance.bloodSplatterPrefab);
		explosion.transform.position = new Vector2(position.x, position.y);
		explosion.transform.localScale = new Vector3(scale, scale, 1f);
		explosion.transform.localRotation = Quaternion.Euler(0,0,rotation);
		explosion.SetActive(true);
	}
}
