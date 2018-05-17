using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStage3LevelGenerator : MonoBehaviour {

	public GameObject[] outsidePrefabs;
	public GameObject[] insidePrefabs;

	public GameObject finalStagePrefab;

	public Vector2 lastEndPoint;
	private float lastLastEndPointY;

	private PlayerController playerController;

	private Rigidbody2D myBody;

	private bool generating = true;

	// Use this for initialization
	void Start () {
		myBody = GetComponent<Rigidbody2D>();
		playerController = GameManager.Instance.playerController;

		lastLastEndPointY = transform.position.y;
		lastEndPoint = transform.position;
		GenerateNextSection();
		GenerateNextSection();
		//BuildFinalStage();
		GenerateNextSection();
		//GenerateNextSection();
		//GenerateNextSection();
		//GenerateNextSection();
		//GenerateNextSection();
		//GenerateNextSection();
	}

	// Update is called once per frame
	void Update () {
		if (generating && playerController.transform.position.y + 10 > lastLastEndPointY ) {

			GenerateNextSection();
		}

		//myBody.angularVelocity = -1.0f;
	}

	public void GenerateNextSection() {
		lastLastEndPointY = lastEndPoint.y;
		CreateNewOutsideObject(ChooseRandom(insidePrefabs), transform, lastEndPoint);
		lastEndPoint = CreateNewOutsideObject(ChooseRandom(outsidePrefabs), transform, lastEndPoint);
	}

	private Vector2 CreateNewOutsideObject(GameObject prefab, Transform parent, Vector2 lastEndPoint) {
		GameObject newSprite = Object.Instantiate(prefab, parent);
		Transform bottomPoint = newSprite.transform.Find("BottomEndPoint");
		newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y);
		Transform topPoint = newSprite.transform.Find("TopEndPoint");

		//Rigidbody2D[] theirBodies = newSprite.GetComponentsInChildren<Rigidbody2D>();
		//foreach (var body in theirBodies) {
		//	body.bodyType = RigidbodyType2D.Kinematic;
		//	//RelativeJoint2D joint = body.gameObject.AddComponent<RelativeJoint2D>();
		//	FixedJoint2D joint = body.gameObject.AddComponent<FixedJoint2D>();
		//	joint.connectedBody = myBody;
		//}

		//BoxCollider2D[] colliders = newSprite.GetComponentsInChildren<BoxCollider2D>();
		//foreach (var collider in colliders) {
		//	collider.gameObject.layer = LayerMask.NameToLayer("NavTerrain");
		//}

		return topPoint.position;
	}

	private GameObject ChooseRandom(GameObject[] objects) {
		int i = Random.Range(0, objects.Length);
		return objects[i];
	}

	public void BuildFinalStage() {

		for ( int i = 0; i < 4; i ++ ) {
			GenerateNextSection();
		}

		GameObject newSprite = Object.Instantiate(finalStagePrefab, transform);
		Transform bottomPoint = newSprite.transform.Find("BottomEndPoint");
		newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y);
		lastEndPoint = newSprite.transform.Find("TopEndPoint").position;

		generating = false;
	}
}
