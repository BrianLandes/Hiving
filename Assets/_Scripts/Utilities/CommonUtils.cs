using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class CommonUtils {

	public static float Distance(Vector2 start, Vector2 end) {
		return Mathf.Sqrt(Mathf.Pow(start.x - end.x, 2) + Mathf.Pow(start.y - end.y, 2));
	}

	public static float ThetaBetween(Vector2 start, Vector2 end) {
		return Mathf.Atan2(end.y - start.y, end.x - start.x);
	}

	public static float ThetaBetweenD(Vector2 start, Vector2 end) {
		return Mathf.Atan2(end.y - start.y, end.x - start.x)*Mathf.Rad2Deg;
	}

	public static Vector3 SetZ( Vector3 vector, float z) {
		//vector = new Vector3(vector.x, vector.y, z);
		return new Vector3(vector.x, vector.y, z);
	}

	public static string NewUniqueId() {
		return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
	}

	public static Vector2 NormalVector(Vector2 from, Vector2 to) {
		return (to - from).normalized;
	}

	public static Vector2 Displacement(Vector2 startVelocity, float time) {
		Vector2 v = new Vector2();
		v.x = startVelocity.x * time;
		v.y = startVelocity.y * time + 0.5f * Physics2D.gravity.y * (time * time);
		return v;
	}

	public static float DisplacementY(float startVelocity, float time) {
		return startVelocity * time + 0.5f * Physics2D.gravity.y * (time * time);
	}

	public static float JumpHeight(float jumpForce) {
		float time = -jumpForce / Physics2D.gravity.y;
		return jumpForce * time + 0.5f * Physics2D.gravity.y * (time * time);
	}

	public static float VerticalSpeed(float jumpForce, float time) {
		//Debug.Log(string.Format("Vertical speed: force: {0}, time: {1}, result: {2}",jumpForce, time, jumpForce + Physics2D.gravity.y * time));
		return jumpForce + Physics2D.gravity.y * time;
	}

	public static float FallTime(float y) {
		return Mathf.Sqrt(2f * y / Physics2D.gravity.y);
	}

	public static Vector2 TrajectoryVerticalSpeed(Vector2 targetPoint, float horizontalSpeed) {
		float t = Mathf.Abs( targetPoint.x ) / horizontalSpeed;

		float y = targetPoint.y;
		float peakt = TrajectoryPeakTime(y);
		if (t < peakt) {
			t = peakt;
		}
		float g = Physics2D.gravity.y;
		return new Vector2(targetPoint.x/t,
			y / t - 0.5f*g*t);
	}

	public static float TrajectoryPeakTime(float peaky) {
		float g = Physics2D.gravity.y;
		return Mathf.Sqrt(-2*peaky/g);
	}

	/**
		returns the time it will take to reach displacementY with jumpForce (upwards only)
	*/
	public static float JumpTime(float jumpForce, float displacementY) {
		float s = jumpForce;
		float g = Physics2D.gravity.y;
		float y = displacementY;
		float n = -s + Mathf.Sqrt(s*s + 2 * g * y );
		return n / g;
	}

	/**
		returns the time it will take to reach displacementY with jumpForce (downwards only)
	*/
	public static float JumpTimePostPeak(float jumpForce, float displacementY) {
		float s = jumpForce;
		float g = Physics2D.gravity.y;
		float y = displacementY;
		float n = -s - Mathf.Sqrt(s * s + 2 * g * y);
		return n / g;
	}

	public static float JumpPeakTime(float jumpForce) {
		return -jumpForce / Physics2D.gravity.y;
	}
	
	public static Vector2 Position(Transform transform) {
		return new Vector2(transform.position.x, transform.position.y);
	}

	public static void SetTextLabel(GameObject go, string labelName, string text ) {
		Text nameText = go.transform.Find(labelName).GetComponent<Text>();
		nameText.text = text;
	}

	public static T RandomChoice<T>(IList<T> set) {
		int c = UnityEngine.Random.Range(0,set.Count);
		return set[c];
	}

	public static T RandomChoice<T>(T[] set) {
		int c = UnityEngine.Random.Range(0, set.Length);
		return set[c];
	}

	public static Transform GetChild(Transform parent, string name) {
		List<Transform> openList = new List<Transform>();
		openList.AddRange(parent.GetComponentsInChildren<Transform>());
		foreach (var child in openList) {
			if (child.gameObject.name == name) {
				return child;
			}
		}

		return null;
	}

	public static Vector3 ClampWorldPositionToScreen(Vector3 worldPos) {
		Vector3 temp = Camera.main.WorldToScreenPoint(worldPos);
		temp.x = Mathf.Max(0, temp.x);
		temp.x = Mathf.Min(Screen.width, temp.x);
		temp.y = Mathf.Max(0, temp.y);
		temp.y = Mathf.Min(Screen.height, temp.y);
		return Camera.main.ScreenToWorldPoint(temp);
	}

	public static Vector3 GetMouseScreenPositionClamped() {
		Vector3 temp = Input.mousePosition;
		temp.x = Mathf.Max(0, temp.x);
		temp.x = Mathf.Min(Screen.width, temp.x);
		temp.y = Mathf.Max(0, temp.y);
		temp.y = Mathf.Min(Screen.height, temp.y);
		return temp;
	}

	public static string AsString(float[] numbers) {
		string result = "";
		foreach (float item in numbers) {
			result += item.ToString() + ",";
		}

		return result;
	}

	public static void EnableChildren(Transform transform) {
		foreach (Transform child in transform) {
			child.gameObject.SetActive(true);
		}
	}

	public static void DisableChildren(Transform transform) {
		foreach (Transform child in transform) {
			child.gameObject.SetActive(false);
		}
	}

	public static float VectorToAngle( Vector2 direction ) {
		return Mathf.Atan2(direction.y, direction.x);
	}

	public static Vector2 AngleToVector( float radians ) {
		return new Vector2( Mathf.Cos(radians), Mathf.Sin(radians) );
	}
}
