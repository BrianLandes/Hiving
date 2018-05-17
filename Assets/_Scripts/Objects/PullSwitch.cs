using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullSwitch : MonoBehaviour {

	public GameObject[] targets;

	public float triggerLength = 2.0f;

	public enum Type {
		Open,
		Close,
		Toggle,
		Nothing
	}

	[SerializeField]
	public Type onPull = Type.Open;
	[SerializeField]
	public Type onRelease = Type.Nothing;

	private float lastLength = 0.0f;
	private SpringJoint2D spring;

	// Use this for initialization
	void Start () {
		spring = GetComponentInChildren<SpringJoint2D>();
	}

	private void OnEnable() {
		transform.Find("Handle").localPosition = Vector3.zero;
	}

	// Update is called once per frame
	void Update () {
		float d = CommonUtils.Distance( spring.transform.position, spring.connectedBody.transform.position );
		if (lastLength < triggerLength && d >= triggerLength) {
			switch (onPull) {

				case Type.Open:
					//Debug.Log("Open");
					Call("Open");
					break;
				case Type.Close:
					Call("Close");
					break;
				case Type.Toggle:
					Call("Toggle");
					break;
				default:
				case Type.Nothing:
					break;
			}
		} else if (lastLength > triggerLength && d <= triggerLength) {
			switch (onRelease) {
				default:
				case Type.Nothing:
					break;
				case Type.Open:
					Call("Open");
					break;
				case Type.Close:
					Call("Close");
					break;
				case Type.Toggle:
					Call("Toggle");
					break;
			}
		}

		lastLength = d;
	}

	void Call(string message) {
		foreach (var go in targets) {
			go.SendMessage(message);
		}
	}
}
