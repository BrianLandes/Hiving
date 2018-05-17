using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundTileBuilder : MonoBehaviour {

	public Vector2 endPoint = Vector2.right*3.0f;

	public GameObject[] spritePrefabs;

	public Vector3 GetEndPoint() {
		return CommonUtils.Position(transform) + endPoint;
	}

	public void SetLengthFromEndPoint(Vector3 newPoint) {
		endPoint = transform.InverseTransformPoint(newPoint);
		endPoint = new Vector2(
			Mathf.Max( 0, endPoint.x),
			Mathf.Max( 0, endPoint.y )
		);
	}

	public void Clear() {
		int count = transform.childCount;
		//foreach (Transform child in transform) {
		for (int i = 0; i < count; i++) {
			Transform child = transform.GetChild(0);
			if (Application.isPlaying)
				Destroy(child.gameObject);
			else
				DestroyImmediate(child.gameObject);
		}
	}

	private int countX, countY;

	public void Build() {
		Clear(); 
		Transform spriteChildObject = NewBlankChildObject("Sprites");

		float y = 0;
		countY = 0;
		while (y < endPoint.y) {

			y += BuildRow(transform.position.y + y, spriteChildObject);
			countY++;
		}

		gameObject.name = string.Format("BackgroundTiles x({0},{1})", countX, countY);
	}

	float BuildRow(float bottom, Transform parent) {
		float L = endPoint.x - 0.0001f;
		countX = 0;
		Vector2 lastEndPoint = transform.position;

		float height = 0;

		while (L >= 0) {
			GameObject middlePrefab = ChooseRandom(spritePrefabs);
			float middleLength = GetLengthOfSprite(middlePrefab);
			height = GetHeightOfSprite(middlePrefab);
			lastEndPoint = CreateNewObject(middlePrefab, parent, lastEndPoint, bottom );
			L -= middleLength;
			countX++;
		}
		return height;
	}

	private Transform NewBlankChildObject(string name) {
		Transform emptyGameObject = new GameObject(name).transform;
		emptyGameObject.SetParent(transform);
		emptyGameObject.localPosition = Vector2.zero;
		return emptyGameObject;
	}

	private GameObject ChooseRandom(GameObject[] objects) {
		int i = Random.Range(0, objects.Length);
		return objects[i];
	}

	private float GetLengthOfSprite(GameObject sprite) {
		Transform leftPoint = sprite.transform.Find("LeftEndPoint");
		Transform rightPoint = sprite.transform.Find("RightEndPoint");
		return rightPoint.localPosition.x - leftPoint.localPosition.x;
	}

	private float GetHeightOfSprite(GameObject sprite) {
		Transform bottomPoint = sprite.transform.Find("BottomEndPoint");
		Transform topPoint = sprite.transform.Find("TopEndPoint");
		return topPoint.localPosition.y - bottomPoint.localPosition.y;
	}

	private Vector2 CreateNewObject(GameObject prefab, Transform parent, Vector2 lastEndPoint, float bottom) {
		GameObject newSprite = Object.Instantiate(prefab, parent);
		Transform leftPoint = newSprite.transform.Find("LeftEndPoint");
		Transform bottomPoint = newSprite.transform.Find("BottomEndPoint");
		newSprite.transform.position = lastEndPoint - new Vector2(leftPoint.localPosition.x, leftPoint.localPosition.y);
		newSprite.transform.position = new Vector2(newSprite.transform.position.x, bottom - bottomPoint.localPosition.y);
		Transform rightPoint = newSprite.transform.Find("RightEndPoint");
		return rightPoint.position;
	}
}
