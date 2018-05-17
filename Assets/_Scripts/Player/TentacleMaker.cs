using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TentacleMaker {

	public GameObject[] sprites;

	public static Tentacle Basic( TentaclePack pack, float length = 10) {
		Tentacle tentComp = Ungenerated();
		tentComp.pack = pack;
		tentComp.alternateSprites = true;
		
		tentComp.length = length;
		tentComp.segments = (int)(length*2);
		tentComp.pullLength = 0;
		tentComp.jointWeight = 0.16f;
		tentComp.jointDrag = 0.9f;
		tentComp.useWiggle = true;
		tentComp.wiggleStrength = 0.16f;
		tentComp.wiggleSpeed = 1.0f;
		tentComp.startWidth = 1.2f;
		tentComp.endWidth = 0.5f;
		tentComp.GenerateSegments();
		
		return tentComp;
	}

	public static Tentacle Ungenerated() {
		GameObject newTentacle = new GameObject("Tentacle");
		AddAttachSpring(newTentacle);

		Transform endPoint = new GameObject("EndPoint").transform;
		endPoint.parent = newTentacle.transform;
		endPoint.localPosition = Vector2.zero;
		AddAttachSpring(endPoint.gameObject, 0.0f);

		Tentacle tentComp = newTentacle.AddComponent<Tentacle>();

		return tentComp;
	}

	public static void AddAttachSpring(GameObject go, float dampingRatio = 1.0f ) {
		SpringJoint2D newSpring = go.AddComponent<SpringJoint2D>();
		newSpring.autoConfigureConnectedAnchor = false;
		newSpring.autoConfigureDistance = false;
		newSpring.anchor = Vector2.zero;
		newSpring.connectedAnchor = Vector2.zero;
		newSpring.distance = 0.005f;
		newSpring.dampingRatio = dampingRatio; // 1 -> the spring will have no give, 0 -> the spring will stretch
		newSpring.frequency = 0;
		Rigidbody2D myBody = go.GetComponent<Rigidbody2D>();
		myBody.mass = 0.0001f;
	}

	public static void SeverTentacle(Tentacle tentacle) {
		int severPoint = (int)Mathf.Ceil(tentacle.segments * Random.Range( 0.2f, 0.5f ) );

		Tentacle baseHalf = Ungenerated();
		CopyTentSubSegments(tentacle, baseHalf, 0, severPoint);
		baseHalf.AttachBase(tentacle.connectBody);
		Transform endPoint = baseHalf.transform.Find("EndPoint");
		MakeEndDangling(endPoint);
		SpringJoint2D endSpringJoint = endPoint.GetComponent<SpringJoint2D>();
		endSpringJoint.connectedBody = tentacle.connectBody;
		endSpringJoint.distance = tentacle.length * ((float)severPoint/(float)tentacle.segments) * 0.5f;

		Tentacle endHalf = Ungenerated();
		CopyTentSubSegments(tentacle, endHalf, severPoint, tentacle.segments-1);
		MakeEndDangling(endHalf.transform);
		if (tentacle.Grabbing) {
			endHalf.AttachEnd(tentacle.endAttachSpring.connectedBody,tentacle.GetEndPosition());
			
		} else {
			Transform endHalfEndPoint = endHalf.transform.Find("EndPoint");
			MakeEndDangling(endHalfEndPoint);
			SpringJoint2D endHalfEndSpring = endHalfEndPoint.gameObject.GetComponent<SpringJoint2D>();
			endHalfEndSpring.enabled = false;

			Rigidbody2D endBody = endHalf.gameObject.GetComponent<Rigidbody2D>();
			endBody.gravityScale = 1;
			Rigidbody2D endPointBody = endHalfEndPoint.gameObject.GetComponent<Rigidbody2D>();
			endPointBody.gravityScale = 1;
		}
		SpringJoint2D spring = endHalf.gameObject.GetComponent<SpringJoint2D>();
		spring.connectedBody = endHalf.transform.Find("EndPoint").gameObject.GetComponent<Rigidbody2D>();
		spring.distance = (tentacle.length - endSpringJoint.distance) * 0.5f;

		ExplosionManager.BloodSplatter(tentacle.tentJoints[severPoint].transform.position,3,0);

		tentacle.StopGrabbing();
		GameObject.Destroy(tentacle.gameObject);
	}

	public static void CopyTentSegments(Tentacle source, Tentacle dest) {
		dest.ClearSegments();

		dest.segments = source.segments;
		dest.pack = source.pack;
		dest.alternateSprites = source.alternateSprites;
		dest.startWidth = source.startWidth;
		dest.endWidth = source.endWidth;
		dest.length = source.length;

		dest.GenerateSegments();

		for (int i = 0; i < dest.tentJoints.Count; i++) {
			TentJoint destJoint = dest.tentJoints[i];
			TentJoint sourceJoint = source.tentJoints[i];

			destJoint.transform.position = sourceJoint.transform.position;

			//destJoint.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.transform.position);
			//destJoint.gameObject.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.gameObject.transform.position);
			
		//destJoint.gameObject.GetComponent<StretchTo>().RecalculateStart();
			//destJoint.gameObject.GetComponent<StretchTo>().RunStretch();
			//destJoint.lastPosition = sourceJoint.lastPosition;
			//destJoint.velocity = sourceJoint.velocity;
			////public List<Vector2> targetPositions;
			//destJoint.tOffset = sourceJoint.tOffset;
		}

		//dest.t = source.t;
	}

	public static void CopyTentSubSegments(Tentacle source, Tentacle dest, int start, int end) {
		//if (start < 0 || end > source.segments) {
		//	throw new Exception("Can't make a sub segment with these bounds.");
		//}
		dest.ClearSegments();

		dest.segments = end - start + 1;
		dest.pack = source.pack;
		dest.alternateSprites = source.alternateSprites;
		float width_dif = source.endWidth - source.startWidth;
		dest.startWidth = source.startWidth + width_dif * ((float)start / (float)source.segments);
		dest.endWidth = source.startWidth + width_dif * ((float)end / (float)source.segments);
		dest.length = source.length * ((float)dest.segments / (float)source.segments);

		dest.GenerateSegments();

		if (start > 0) {
			dest.transform.position = source.tentJoints[start - 1].transform.position;

		} else {
			dest.transform.position = source.transform.position;
		}
		if (end < source.segments - 1) {
			dest.endPoint.position = source.tentJoints[end].transform.position;
		} else {
			dest.endPoint.position = source.endPoint.position;
		}
		dest.baseSegment.transform.localPosition = Vector2.zero;

		for (int i = 0; i < dest.tentJoints.Count; i++) {
			TentJoint destJoint = dest.tentJoints[i];
			TentJoint sourceJoint = source.tentJoints[i + start];

			destJoint.transform.position = sourceJoint.transform.position;

			//destJoint.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.transform.position);
			//destJoint.gameObject.transform.position = dest.transform.position + source.transform.InverseTransformPoint(sourceJoint.gameObject.transform.position);
			//destJoint.lastPosition = sourceJoint.lastPosition;
			//destJoint.velocity = sourceJoint.velocity;
			////public List<Vector2> targetPositions;
			//destJoint.tOffset = sourceJoint.tOffset;
		}

		//dest.AdjustJoints();

		
		//dest.t = source.t;
	}

	public static void MakeEndDangling( Transform end ) {
		Rigidbody2D endBody = end.GetComponent<Rigidbody2D>();
		endBody.bodyType = RigidbodyType2D.Dynamic;
		endBody.mass = 0.000001f;
		endBody.gravityScale = 11;
		endBody.drag = 1;
		endBody.AddForce(new Vector2(Random.Range(-.001f, .001f), Random.Range(-.001f, .001f)), ForceMode2D.Impulse);
		SpringJoint2D spring = end.GetComponent<SpringJoint2D>();
		spring.dampingRatio = .4f;
		spring.frequency = 1.0f;

		//CircleCollider2D circle = end.gameObject.AddComponent<CircleCollider2D>();
		//circle.radius = .2f;
		//end.gameObject.layer = LayerMask.NameToLayer("BackgroundObjects");
	}
}
