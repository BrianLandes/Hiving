using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tentacle : MonoBehaviour {

	public Rigidbody2D connectBody;

	//public Transform basePoint;
	public Transform endPoint;

	public bool alternateSprites = true;

	//public GameObject[] tentacleSprites;
	//public GameObject tentacleSpriteRounderPrefab;

	public int segments = 18;
	public float length = 10;
	public float pullLength = 6;
	public float jointWeight = 0.005f;
	public float jointDrag = 0.1f;

	public bool useWiggle = true;
	public float wiggleStrength = 0.01f;
	public float wiggleSpeed = 1.0f;

	public float startWidth = 1.2f;
	public float endWidth = 0.5f;

	bool ShowPoints = false;
	
	public List<TentJoint> tentJoints = new List<TentJoint>();
	
	float segmentLength;

	public int prewarm = 3;

	public bool Grabbing {
		get; set;
	}

	GameObject grabbedObject;
	public GameObject GrabbedObject { get { return grabbedObject; } set { } }
	[HideInInspector]
	public SpringJoint2D joint;

	private SpringJoint2D baseAttachSpring;
	public SpringJoint2D endAttachSpring;

	public GameObject baseSegment;

	public bool autoGenerateSegments = false;

	public float WiggleTime { get; private set; }
	bool segmentsGenerated = false;

	//public AudioClip[] suctionNoises;
	//public AudioClip[] unsuctionNoises;

	public TentaclePack pack;

	private AudioSource audioSource;

	// Use this for initialization
	void OnEnable() {
		if (endPoint == null)
			endPoint = transform.Find("EndPoint");
		baseAttachSpring = GetComponent<SpringJoint2D>();
		endAttachSpring = endPoint.GetComponent<SpringJoint2D>();

		//AttachBase(connectBody);
		//joints = new List<Joint>();

		WiggleTime = Random.Range(0.0f, 10.0f);

		if (!segmentsGenerated && autoGenerateSegments) {
			if (pack == null)
				pack = GetComponent<TentaclePack>();
			GenerateSegments();
			for (int i = 0; i < prewarm; i++) {
				AdjustJoints();
			}
		}

		

		audioSource = endPoint.gameObject.AddComponent<AudioSource>();
		audioSource.spatialBlend = 1.0f;
		audioSource.minDistance = 1;
		audioSource.maxDistance = 30;
	}

	public void GenerateSegments() {
		segmentLength = length / (float)segments;

		Transform emptyGameObject = new GameObject("Segments").transform;
		emptyGameObject.SetParent(transform);
		emptyGameObject.localPosition = Vector2.zero;

		for (int i = 0; i < segments; i++) {
			GameObject newSegment = new GameObject(string.Format("Joint {0}", i));
			newSegment.transform.SetParent(emptyGameObject);
			newSegment.transform.localPosition = new Vector3(i*0.1f,i * 0.01f, -0.001f);
			TentJoint newJoint = newSegment.AddComponent<TentJoint>();
			newJoint.Initialize(this, i);
			
			tentJoints.Add(newJoint);
		}

		int o = 0;

		baseSegment = Object.Instantiate(pack.tentacleSprites[o], emptyGameObject);
		baseSegment.transform.parent = emptyGameObject;
		baseSegment.name = "Base Segment " + transform.name;
		baseSegment.transform.position = transform.position;
		baseSegment.transform.localScale = new Vector3(1.0f, startWidth, 1);
		baseSegment.GetComponent<StretchTo>().targetPoint = tentJoints[0].gameObject.transform;
		//baseSegment.transform.SetParent(basePoint);
		//baseSegment.transform.localPosition = Vector3.zero;
		//baseSegment.GetComponent<StretchTo>().RecalculateStart();

		segmentsGenerated = true;
	}

	public GameObject GetTentSegmentPrefab(int index) {
		int s = 0;
		if (alternateSprites) {
			s = index % pack.tentacleSprites.Length + 1;
			if (s >= pack.tentacleSprites.Length)
				s = 0;
		} else {
			s = ((int)(((float)index) / ((float)segments) * pack.tentacleSprites.Length));
		}
		return pack.tentacleSprites[s];
	}

	public float GetTentSegmentWidth(int index) {
		return endWidth + (startWidth - endWidth) * ((float)(segments - index) / (float)segments);
	}

	public Transform GetTentSegmentLastPoint(int index) {
		if (index == 0)
			return transform;
		return tentJoints[index - 1].transform;
	}

	public void UpdateTentSegmentNextPointMaybe(TentJoint tentJoint) {
		int i = tentJoint.index;
		if (i > 0) {
			tentJoints[i - 1].nextPoint = tentJoint.transform;
			tentJoints[i - 1].stretchTo.targetPoint = tentJoint.transform;
			tentJoints[i - 1].stretchTo.RecalculateStart();
		}
		if (i == segments - 1) {
			tentJoint.stretchTo.targetPoint = endPoint.transform;
			tentJoint.stretchTo.RecalculateStart();
		}
	}

	public void ClearSegments() {
		if (segmentsGenerated) {
			tentJoints.Clear();
			GameObject segmentFolderObject = transform.Find("Segments").gameObject;
			Destroy(segmentFolderObject);

			Destroy(baseSegment);

			segmentsGenerated = false;
		}

	}

	// Update is called once per frame
	void Update() {

		WiggleTime += Time.deltaTime * wiggleSpeed;
		
		UpdateVelocities();

		AdjustJoints();

		ApplyVelocities();


		if (ShowPoints) {
			// draw little circles on each of the joints
			for (int i = 0; i < segments; i++) {
				DrawPoint(tentJoints[i].transform.position);
			}
		}
	}

	public void AdjustJoints() {
		int n = 0;
		while (AnyJointsTooLong() && n < 50) {
			foreach (TentJoint j in tentJoints) {
				j.targetPositions.Clear();
			}
			ForwardPass();
			BackwardPass();
			if (useWiggle && n == 0)
				WigglePass();
			foreach (TentJoint j in tentJoints) {
				float x = 0;
				float y = 0;
				foreach (var v in j.targetPositions) {
					x += v.x;
					y += v.y;
				}
				//j.myRigidBody.MovePosition(new Vector2(x / j.targetPositions.Count, y / j.targetPositions.Count));
				j.transform.position = new Vector2(x / j.targetPositions.Count, y / j.targetPositions.Count);
				//j.targetPosition = new Vector2(x / j.targetPositions.Count, y / j.targetPositions.Count);
			}
			n++;
		}
	}

	bool AnyJointsTooLong() {
		foreach (TentJoint j in tentJoints) {
			//if (j.lastJoint == null && Distance(basePoint.position, j.position) > segmentLength) {
			//	return true;
			//}
			if (j.HeadLength() > segmentLength || j.TailLength() > segmentLength) {
				return true;
			}
		}
		return false;
	}
	
	void UpdateVelocities() {
		foreach (TentJoint j in tentJoints) {
			j.UpdateVelocity();
		}
	}

	Vector2 v;

	void ApplyVelocities() {
		foreach (TentJoint j in tentJoints) {
			v = j.transform.position;
			j.transform.position = j.Position + j.velocity;
			//j.myRigidBody.MovePosition(j.Position + j.myRigidBody.velocity );
			j.transform.localPosition = new Vector3(j.transform.localPosition.x, j.transform.localPosition.y, -0.00001f * j.index);

			j.velocity += v - j.Position;
		}
	}

	void ForwardPass() {

		for (int i = tentJoints.Count - 1; i >= 0; i--) {
			//for (int i = 0; i < tentJoints.Count; i++) {
			TentJoint thisJoint = tentJoints[i];
			Vector2 tailPoint = thisJoint.nextPoint.position;
			Vector2 thisPoint = thisJoint.transform.position;
			//float pull = (float)(segments - i) / (float)segments;
			//float taut = 0.99f;
			//float pull = (Mathf.Sin(WiggleTime + thisJoint.tOffset) + 1.0f) * 0.5f;
			//float s = segmentLength - segmentLength * pull * taut;
			float s = segmentLength;
			////float d = Mathf.Min(s, Distance(headPoint, thisPoint));
			////Debug.Log(string.Format("s: {0}, d: {1}",s,d));
			if ( thisJoint.TailLength() > s) {
				float theta = CommonUtils.ThetaBetween(tailPoint, thisPoint);
				Vector2 newPos = new Vector2(tailPoint.x + Mathf.Cos(theta) * s, tailPoint.y + Mathf.Sin(theta) * s);
				thisJoint.targetPositions.Add(newPos);
				//joint.position.x = headPoint.x + Mathf.Cos(theta) * s;
				//joint.position.y = headPoint.y + Mathf.Sin(theta) * s;
			} else {
				thisJoint.targetPositions.Add(thisJoint.transform.position);
			}
		}

	}

	void BackwardPass() {

		for (int i = 0; i < tentJoints.Count; i++) {
			TentJoint thisJoint = tentJoints[i];
			Vector2 headPoint = thisJoint.lastPoint.position;

			float s = segmentLength;

			if (thisJoint.HeadLength() > s) {
				float theta = CommonUtils.ThetaBetween(headPoint, thisJoint.transform.position);
				Vector2 newPos = new Vector2(headPoint.x + Mathf.Cos(theta) * s, headPoint.y + Mathf.Sin(theta) * s);
				thisJoint.targetPositions.Add(newPos);

			} else {
				thisJoint.targetPositions.Add(thisJoint.transform.position);
			}
		}
	}

	public Vector2 TentSlope {
		get; set;
	}

	void WigglePass() {
		float t = CommonUtils.Distance(transform.position, endPoint.transform.position) / length;
		t = Mathf.Min(t, 1.0f);
		TentSlope = CommonUtils.NormalVector(transform.position, endPoint.position);
		for (int i = 0; i < tentJoints.Count; i++) {
			TentJoint thisJoint = tentJoints[i];

			float w = Mathf.Sin(WiggleTime + thisJoint.tOffset) * wiggleStrength * t;
			v = thisJoint.Position;
			v.x += w * TentSlope.y;
			v.y -= w * TentSlope.x;

			thisJoint.targetPositions.Add(v);

		}
	}
	
	public void AttachBase(Rigidbody2D body, Vector3 worldSpaceAttachPoint) {
		if (baseAttachSpring != null) {
			baseAttachSpring.connectedBody = body;
			connectBody = body;
			baseAttachSpring.connectedAnchor = body.transform.InverseTransformPoint(worldSpaceAttachPoint);
		}
	}

	public void AttachBase(Rigidbody2D body) {
		AttachBase(body, body.transform.position);
	}

	public void AttachEnd(Rigidbody2D body, Vector3 worldSpaceAttachPoint) {
		if (endAttachSpring != null) {
			endAttachSpring.enabled = true;
			endAttachSpring.connectedBody = body;
			endAttachSpring.connectedAnchor = body.transform.InverseTransformPoint(worldSpaceAttachPoint);
		}
	}

	public Vector3 GetEndPosition() {
		if (endAttachSpring != null && endAttachSpring.enabled) {
			return endAttachSpring.connectedBody.transform.TransformPoint(endAttachSpring.connectedAnchor);
		}
		return Vector3.zero;
	}

	public void SetGrabbing(GameObject grabable, Vector3 grabPoint, bool playSound = true) {
		if (grabbedObject != null)
			StopGrabbing();
		//targetTaut = 0.8f;
		grabbedObject = grabable;
		Rigidbody2D gBody = grabable.GetComponent<Rigidbody2D>();
		if (gBody != null) {
			joint = grabable.AddComponent<SpringJoint2D>();

			joint.enableCollision = true;
			joint.autoConfigureDistance = false;
			//			joint.connectedBody = GetComponentInChildren<Rigidbody2D>();
			joint.connectedBody = connectBody;
			joint.anchor = grabable.transform.InverseTransformPoint(grabPoint);
			//			joint.connectedAnchor = connectBody.transform.position;
			//			joint.connectedAnchor = grabable.transform.InverseTransformPoint(connectBody.transform.position);
			joint.distance = pullLength;
			joint.dampingRatio = 0.6f;
			joint.frequency = 0.9f;

			if (playSound)
				PlaySuctionNoise();
			Grabbing = true;
			//			SetGrabbingAnchor(new Vector2(0, 0));
		}

	}

	public void StopGrabbing() {
		//targetTaut = 0.0f;
		if (grabbedObject != null) {
			Destroy(joint);
			grabbedObject = null;
			joint = null;
		}
		Grabbing = false;
	}

	public void SetGrabbingAnchor(Vector2 newAnchorWorldPosition) {
		if (grabbedObject != null && joint != null) {
			joint.connectedAnchor = connectBody.transform.InverseTransformPoint(newAnchorWorldPosition);
		}
	}

	void DrawPoint(Vector2 point) {
		int c = 8;
		float size = 0.2f;

		float step = Mathf.PI / (c * 0.5f);
		for (int i = 0; i < c; i++) {

			Vector2 V1 = new Vector2(Mathf.Cos(step * i) * size, Mathf.Sin(step * i) * size);
			Vector2 V2 = new Vector2(Mathf.Cos(step * (i + 1)) * size, Mathf.Sin(step * (i + 1)) * size);
			Debug.DrawLine(point + V1, point + V2);
		}
	}

	public void TeleportToBase() {
		for (int i = 0; i < tentJoints.Count; i++) {
			TentJoint joint = tentJoints[i];
			joint.transform.position = transform.position;
			//joint.lastPosition = basePoint.transform.position;
			joint.velocity = Vector2.zero;
		}
	}
	
	public void PlaySuctionNoise() {
		if (pack.suctionNoises.Length > 0) {
			audioSource.clip = pack.suctionNoises[Random.Range(0, pack.suctionNoises.Length)];
			audioSource.pitch = Random.Range(1.0f - 0.05f, 1.0f + 0.05f);
			audioSource.volume = 1f;
			audioSource.Play();
		}
	}
}
