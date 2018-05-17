using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PressureButton : MonoBehaviour {
	public GameObject[] targets;

	public float triggerLength = 1.0f;

	public enum Type {
		Open,
		Close,
		Toggle,
		Nothing
	}

	[SerializeField]
	public Type onPush = Type.Open;
	[SerializeField]
	public Type onRelease = Type.Nothing;

	private float lastLength = 0.0f;
	private SliderJoint2D slider;
	// Use this for initialization
	void Start () {
		slider = GetComponentInChildren<SliderJoint2D>();
	}
	
	// Update is called once per frame
	void Update () {
		//float d = CommonUtils.Distance(slider.transform.position, slider.connectedBody.transform.position);
		float d = slider.jointTranslation;
		if (lastLength > triggerLength && d <= triggerLength) {
			switch (onPush) {

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
		} else if (lastLength < triggerLength && d >= triggerLength) {
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
