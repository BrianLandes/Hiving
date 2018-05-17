using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[ExecuteInEditMode]
public class StretchTo : MonoBehaviour {

	public Transform targetPoint;
	public Transform endPoint;

	public bool horStretch = true;
	public bool verStretch = true;

	//public bool useOriginalRotation = false;

	float startRotation;
	Vector3 startScale;
	float endDistance;
	
	void Start () {
		RecalculateStart();
	}

	public void RecalculateStart() {
		startRotation = CommonUtils.ThetaBetweenD(Vector3.zero, transform.InverseTransformPoint(endPoint.position));
		//startRotation = CommonUtils.ThetaBetweenD(transform.position, endPoint.position);
		startScale = transform.localScale;
		endDistance = CommonUtils.Distance(transform.position, endPoint.position);
	}

	// Update is called once per frame
	void Update () {
		RunStretch();
	}

	public void RunStretch() {
		if (targetPoint == null || endPoint == null)
			return;
		//if (useOriginalRotation )
		transform.rotation = Quaternion.Euler(0, 0, CommonUtils.ThetaBetweenD(transform.position, targetPoint.position) - startRotation);
		//else
		//	transform.rotation = Quaternion.Euler(0, 0, CommonUtils.ThetaBetweenD(transform.position, targetPoint.position) );
		float d = CommonUtils.Distance(transform.position, targetPoint.position) / endDistance;
		//if ( Application.platform != RuntimePlatform.WindowsEditor) {
		float x = startScale.x;
		if (horStretch)
			x = d;
		float y = startScale.y;
		if (verStretch)
			y = d;
		transform.localScale = new Vector3(x, y, 1.0f);
		//}
	}
}
