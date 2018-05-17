using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(PlatformBuilder))]
public class PlatformBuilderEditor : Editor {

	PlatformBuilder platform;

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	bool selected = false;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		platform = target as PlatformBuilder;
		if (GUILayout.Button("Build")) {
            platform.Build();
		}
		if (GUILayout.Button("Clear")) {
            platform.Clear();
		}
		if (GUILayout.Button("Create New Platform on End")) {
			GameObject newPlatform = Object.Instantiate(platform.gameObject, platform.transform.parent);
			newPlatform.transform.position = platform.GetEndPoint();
			PlatformBuilder pb = newPlatform.GetComponent<PlatformBuilder>();
			pb.Clear();
			pb.length = 5f;
			Selection.activeGameObject = newPlatform;
		}
	}

	private void OnSceneGUI() {
		platform = target as PlatformBuilder;
		Vector2 point = platform.GetEndPoint();
		float size = HandleUtility.GetHandleSize(point);
		Handles.color = Color.white;

		if (selected) {
			EditorGUI.BeginChangeCheck();
			point = Handles.PositionHandle(point, Quaternion.identity);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(platform, "Move Point");
				EditorUtility.SetDirty(platform);
				platform.SetLengthFromEndPoint(point);
			}
		} else if (Handles.Button(point, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotCap)) {
			selected = true;
			Repaint();
		}

		Handles.color = Color.gray;
		Handles.DrawLine(platform.transform.position, point);
	}
}
