using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorManager : GenericSingletonClass<CursorManager> {

	public GameObject cursorSprite;

	public float mouseSpeed = 5f;
	public float maxMouseDistance = 7;

	private Vector2 cursorScreenPosition;
	private Vector2 cursorWorldPosition;

	private bool debug = false;
	private bool mainMenu = false;

	public void Start() {
		Cursor.lockState = CursorLockMode.Locked;

		ResetCursorPosition();
	}

	public void ResetCursorPosition() {
		cursorScreenPosition = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
		cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition);
	}

	public void Update() {
		
		if (Input.GetButtonUp("Tab"))
			debug = !debug;

		if ( !GameManager.Paused && !debug && !mainMenu)
			Cursor.lockState = CursorLockMode.Locked;
		else
			Cursor.lockState = CursorLockMode.None;
		//if (Application.isEditor && Input.GetButtonUp("Cancel")) {
		//	//Application.Quit();
		//	UnityEditor.EditorApplication.isPlaying = false;
		//}

		cursorScreenPosition += new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")) * mouseSpeed ;
		cursorWorldPosition = Camera.main.ScreenToWorldPoint(cursorScreenPosition);

		PlayerController pc = GameManager.Instance.playerController;
		Vector2 playerPosition = pc.transform.position;

		float distance = CommonUtils.Distance(playerPosition, cursorWorldPosition);
		if ( distance > maxMouseDistance ) {
			cursorWorldPosition = playerPosition + 
				(cursorWorldPosition- playerPosition).normalized * maxMouseDistance;

			cursorScreenPosition = Camera.main.WorldToScreenPoint(cursorWorldPosition);
		}
		
		cursorSprite.transform.position = CommonUtils.SetZ(cursorWorldPosition, 0);

	}

	public void ToggleMainMenu() {
		mainMenu = !mainMenu;
	}

	public static Vector2 GetCursorWorldPosition() {
		return Instance.cursorWorldPosition;
	}
}
