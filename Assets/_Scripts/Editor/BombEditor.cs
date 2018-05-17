using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Bomb))]
public class BombEditor : Editor {

	Bomb bomb;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		bomb = target as Bomb;

		if (GUILayout.Button("Explode")) {
			bomb.removeOnExplode = false;
			bomb.TriggerExplosion();
		}

	}
}
