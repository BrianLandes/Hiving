using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentJoint : MonoBehaviour {

	public Tentacle parentTentacle;

	public int index;
	//public Vector2 position;
	//public Vector2 lastPosition;
	public Vector2 velocity;
	public List<Vector2> targetPositions = new List<Vector2>();
	public float tOffset = 0;

	public Transform nextPoint;
	public Transform lastPoint;

	//public Rigidbody2D myRigidBody;
	public StretchTo stretchTo;

	//public Vector2 targetPosition;

	public void Initialize(Tentacle parent, int index) {
		parentTentacle = parent;
		this.index = index;
		tOffset = (float)index / (float)parentTentacle.segments * Mathf.PI * 2.0f;

		//myRigidBody = gameObject.AddComponent<Rigidbody2D>();
		//myRigidBody.bodyType = RigidbodyType2D.Kinematic;
		//myRigidBody.gravityScale = 0;

		float w = parentTentacle.GetTentSegmentWidth(index);

		GameObject stretchSegment = Object.Instantiate(parentTentacle.GetTentSegmentPrefab(index), transform);
		stretchSegment.name = string.Format("Segment {0}", index);
		stretchSegment.transform.localScale = new Vector3(1.0f, w, 1.0f);
		stretchSegment.transform.localPosition = Vector2.zero;
		stretchTo = stretchSegment.GetComponent<StretchTo>();

		GameObject roundSegment = Object.Instantiate(parent.pack.tentacleSpriteRounderPrefab, transform);
		roundSegment.name = string.Format("Rounder {0}", index);
		roundSegment.transform.localScale = new Vector3(w,w, 1.0f);
		roundSegment.transform.localPosition = new Vector3(0,0,1);

		lastPoint = parentTentacle.GetTentSegmentLastPoint(index);
		nextPoint = parentTentacle.endPoint;
		parentTentacle.UpdateTentSegmentNextPointMaybe(this);
	}

	public Vector2 Position {
		get { return new Vector2(transform.position.x, transform.position.y); }
	}

	public float HeadLength() {
		//if ( lastPoint!=null )
			return CommonUtils.Distance(lastPoint.transform.position, transform.position);
		//return 0.0f;
	}

	public float TailLength() {
		return CommonUtils.Distance(nextPoint.transform.position, transform.position);
	}

	public void UpdateVelocity() {

		//Vector2 v = myRigidBody.velocity;
		Vector2 v = velocity;

		v *= 1.0f - parentTentacle.jointDrag;
		v.y -= parentTentacle.jointWeight;
		

		//myRigidBody.velocity = v;
		velocity = v;
	}
}
