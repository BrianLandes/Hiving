using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level02 : MonoBehaviour {

	public GameObject explosionPrefab;

	public List<GameObject> hallwayFloorPlatforms;
	public Transform explosionTarget;

	public GameObject nurse;
	public Tentacle nurseTentacle;
	public Tentacle nurseRippedTentacle;

	public GameObject mechHand;

	public void TriggerExplosion() {
		//GameManager.Instance.DisableMovement();
		//GameManager.Instance.DisableTentacle();

		GameObject explosion = Instantiate(explosionPrefab);
		explosion.transform.position = explosionTarget.position;
		explosion.transform.localScale = new Vector3(3, 3, 3);
		explosion.SetActive(true);

		int backObjectsLayer = LayerMask.NameToLayer("BackgroundObjects");

		foreach (var platform in hallwayFloorPlatforms) {

			Rigidbody2D body = platform.GetComponentInChildren<Rigidbody2D>();
			platform.layer = backObjectsLayer;
			body.bodyType = RigidbodyType2D.Dynamic;
			Transform forceTarget = platform.transform.Find("ForceTarget");
			Transform forceVector = platform.transform.Find("ForceVector");
			Vector2 v = forceVector.position - forceTarget.position;
			body.AddForceAtPosition(v, forceTarget.position, ForceMode2D.Impulse);
		}
	}

	public void SetNurseDanglingFromMecha() {
		Transform tentAttachPoint = nurse.transform.Find("TentacleAttachPoint");
		nurseTentacle.AttachBase(nurse.GetComponent<Rigidbody2D>(), tentAttachPoint.position);
		nurseRippedTentacle.AttachBase(nurse.GetComponent<Rigidbody2D>(), tentAttachPoint.position);

		nurseTentacle.pullLength = 7f;
		nurseTentacle.length = 5f;
		nurseTentacle.SetGrabbing(mechHand, mechHand.transform.position);
		nurseTentacle.AttachEnd(mechHand.GetComponent<Rigidbody2D>(), mechHand.transform.position);
		nurseTentacle.ClearSegments();
		nurseTentacle.GenerateSegments();

		Rigidbody2D nurseBody = nurse.GetComponent<Rigidbody2D>();

		nurseBody.mass = 30.0f;

		nurseRippedTentacle.AttachEnd(nurseBody, nurse.transform.position);

		nurse.transform.position = mechHand.transform.position;
		//nurseTentacle.basePoint.transform.position = nurse.transform.position;
		nurseTentacle.TeleportToBase();

		nurseTentacle.endPoint.transform.position = mechHand.transform.position;
	}
}
