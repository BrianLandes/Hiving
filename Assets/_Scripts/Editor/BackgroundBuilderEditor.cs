using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(BackgroundTileBuilder))]
public class BackgroundBuilderEditor : Editor {

	BackgroundTileBuilder platform;

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	bool selected = false;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		platform = target as BackgroundTileBuilder;
		if (GUILayout.Button("Build")) {
			platform.Build();
		}
		if (GUILayout.Button("Clear")) {
			platform.Clear();
		}
		//if (GUILayout.Button("Create New Platform on End")) {

		//}
	}

	private void OnSceneGUI() {
		platform = target as BackgroundTileBuilder;
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
		Vector2 v = platform.transform.position;
		Handles.DrawLine(platform.transform.position, new Vector2( point.x, v.y) );
		Handles.DrawLine(point, new Vector2( point.x, v.y) );

		Handles.DrawLine(new Vector2(v.x, point.y), point);
		Handles.DrawLine(new Vector2(v.x, point.y), v);
	}
}
