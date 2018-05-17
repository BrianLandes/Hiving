using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(MovingPlatform))]
public class MovingPlatformEditor : Editor {

	MovingPlatform platform;
	int selectedIndex = -1;

	private const float handleSize = 0.04f;
	private const float pickSize = 0.06f;

	public override void OnInspectorGUI() {
		DrawDefaultInspector();
		platform = target as MovingPlatform;
		if (GUILayout.Button("Add new waypoint")) {
			Undo.RecordObject(platform, "Add Point");
			Vector2 v = Vector2.zero;
			if (platform.path.Count > 0)
				v = platform.path[platform.path.Count - 1] + Vector2.right;
			platform.path.Add(v);

			EditorUtility.SetDirty(platform);
			//UnityEditor.SceneManagement.EditorSceneManager.MarkAllScenesDirty();
			//Repaint();
		}

		if (selectedIndex != -1) {
			if (GUILayout.Button("Remove waypoint")) {
				Undo.RecordObject(platform, "Remove Point");

				platform.path.RemoveAt(selectedIndex);
				selectedIndex = -1;

				EditorUtility.SetDirty(platform);
			}
		}
	}

	private void OnSceneGUI() {
		platform = target as MovingPlatform;
		Vector2 lastPoint = Vector2.zero;
		for (int i = 0; i < platform.path.Count; i++) {
			Vector2 p1 = ShowPoint(i);
			if (i > 0) {
				Handles.color = Color.gray;

				Handles.DrawLine(lastPoint, p1);
			}
			lastPoint = p1;
		}
	}

	private Vector2 ShowPoint(int index) {
		Vector2 point = platform.transform.TransformPoint(platform.path[index]);
		if (Application.isPlaying) {
			point = platform.path[index] + platform.startPostion;
		}

		float size = HandleUtility.GetHandleSize(point);
		Handles.color = Color.white;
		if (selectedIndex == index) {
			EditorGUI.BeginChangeCheck();
			point = Handles.PositionHandle(point, Quaternion.identity);
			if (EditorGUI.EndChangeCheck()) {
				Undo.RecordObject(platform, "Move Point");
				EditorUtility.SetDirty(platform);
				platform.path[index] = platform.transform.InverseTransformPoint(point);
			}
		} else if (Handles.Button(point, Quaternion.identity, size * handleSize, size * pickSize, Handles.DotCap)) {
			selectedIndex = index;
			Repaint();
		}
		return point;
	}
}
