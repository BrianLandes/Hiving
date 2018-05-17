using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickFixPosition : MonoBehaviour {

	public void OnEnable() {
		transform.position = transform.parent.position;
	}
}
