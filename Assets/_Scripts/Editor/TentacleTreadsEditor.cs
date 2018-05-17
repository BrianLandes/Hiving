using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TentacleTreads))]
public class TentacleTreadsEditor : Editor {

	TentacleTreads treads;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		treads = target as TentacleTreads;
		if (GUILayout.Button("Sever")) {
			treads.SeverTentacle();
		}
		if (GUILayout.Button("Attach to player")) {
			treads.AttachToPlayer();
		}
	}
}
