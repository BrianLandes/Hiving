using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PlatBuilderWindow : EditorWindow {

	public PlatformPack pack;

	// Add menu named "My Window" to the Window menu
	[MenuItem("Window/Platform Builder")]
	static void Init() {
		// Get existing open window or if none, make a new one:
		PlatBuilderWindow window = (PlatBuilderWindow)EditorWindow.GetWindow(typeof(PlatBuilderWindow));
		window.Show();
	}

	void OnGUI() {
		pack = (PlatformPack)EditorGUI.ObjectField(new Rect(3, 3, position.width - 6, 20), "Platform Pack", pack, typeof(PlatformPack));

		if (GUI.Button(new Rect(3, 25, position.width - 6, 20), "Rebuild All")) {
			RebuildAll();
		}
	}

	void RebuildAll() {
		PlatformBuilder[] platBuilders = FindObjectsOfType<PlatformBuilder>() as PlatformBuilder[];
		foreach (var plat in platBuilders) {
			plat.pack = pack;
			plat.Build();
		}
	}
}
