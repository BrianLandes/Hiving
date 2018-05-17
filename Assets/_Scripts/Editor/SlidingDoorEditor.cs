using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(SlidingDoor))]
public class SlidingDoorEditor : Editor {
	SlidingDoor door;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		door = target as SlidingDoor;
		if (GUILayout.Button("Toggle")) {
			door.Toggle();
		}
	}
}
