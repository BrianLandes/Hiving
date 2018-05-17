using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformBuilder : MonoBehaviour {

    public enum Type {
        Horizontal,
        Vertical,
    }

    public Type type;
    public float length = 1;

	public bool spikesOnTop = false;
	public bool spikesOnBottom = false;
	public bool spikesOnLeft = false;
	public bool spikesOnRight = false;

	public PlatformPack pack;

	const float sliderSize = 0.2f;

	private PhysicsMaterial2D slipperyPM;

	private float platformLeftSide = 0f;
	private float platformRightSide = 0f;
	private float platformTopSide = 0f;
	private float platformBottomSide = 0f;

	public Vector3 GetEndPoint() {
        switch (type) {
            case Type.Horizontal:
                return transform.position + new Vector3(length, 0);
            case Type.Vertical:
            default:
                return transform.position + new Vector3(0, length);
        }
    }

    public void SetLengthFromEndPoint(Vector3 endPoint) {
        switch (type) {
            case Type.Horizontal:
                length = endPoint.x - transform.position.x;
                break;
            case Type.Vertical:
            default:
                length = endPoint.y - transform.position.y;
                break;
        }
        length = Mathf.Max(length, 0);
    }

    public void Clear() {
        int count = transform.childCount;
        //foreach (Transform child in transform) {
        for ( int i = 0; i < count; i ++ ) {
            Transform child = transform.GetChild(0);
            if ( Application.isPlaying )
                Destroy(child.gameObject);
            else
                DestroyImmediate(child.gameObject);
        }
    }

    public void Build() {
        Clear();

		float rot_z = transform.rotation.eulerAngles.z;

		transform.rotation = Quaternion.Euler(0, 0, 0);

        switch( type ) {
            case Type.Horizontal:
                BuildHorizontal();
                break;
            case Type.Vertical:
				BuildVertical();
				break;
        }

		if (spikesOnTop) BuildSpikesTop();
		if (spikesOnBottom) BuildSpikesBottom();
		if (spikesOnRight) BuildSpikesRight();
		if (spikesOnLeft) BuildSpikesLeft();

		transform.rotation = Quaternion.Euler(0, 0, rot_z);
	}

    private void BuildHorizontal() {
        Transform spriteChildObject = NewBlankChildObject("Sprites");

        float L = length-0.0001f;
        GameObject leftEndPrefab = ChooseRandom(pack.horizontalLeftEndSpritePrefabs);
        float leftLength = GetLengthOfSprite(leftEndPrefab);

        GameObject rightEndPrefab = ChooseRandom(pack.horizontalRightEndSpritePrefabs);
        float rightLength = GetLengthOfSprite(rightEndPrefab);

        Vector2 lastEndPoint = transform.position;
		platformLeftSide = transform.position.x;

		int count = 1;

        lastEndPoint = CreateNewHorObject(leftEndPrefab, spriteChildObject, lastEndPoint);
        L -= leftLength;

		float height = 0;

        while ( L >= rightLength  ) {
            GameObject middlePrefab = ChooseRandom(pack.horizontalMiddleSpritePrefabs);
            float middleLength = GetLengthOfSprite(middlePrefab);
			height = GetHeightOfSprite(middlePrefab);
            lastEndPoint = CreateNewHorObject(middlePrefab, spriteChildObject, lastEndPoint);
            L -= middleLength;
			count++;
        }

        lastEndPoint = CreateNewHorObject(rightEndPrefab, spriteChildObject, lastEndPoint);
        length = lastEndPoint.x - transform.position.x;
		platformRightSide = lastEndPoint.x;
		platformTopSide = transform.position.y + height * 0.5f;
		platformBottomSide = transform.position.y - height * 0.5f;

		// Create the colliders
		Transform colliderChildObject = NewBlankChildObject("Colliders");
        Transform colliderObject = NewBlankChildObject("Collider");
        colliderObject.SetParent(colliderChildObject);
        colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

        Rigidbody2D rBody = colliderObject.gameObject.AddComponent<Rigidbody2D>();
        rBody.bodyType = RigidbodyType2D.Static;

        BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();
        
        boxCollider.offset = new Vector2(length * 0.5f, 0);
        boxCollider.size = new Vector2(length - sliderSize*2f, 1);

        CreateSlider(colliderChildObject, lastEndPoint - new Vector2(sliderSize*0.5f,0));
        CreateSlider(colliderChildObject, new Vector2(sliderSize * 0.5f + transform.position.x, transform.position.y));

		gameObject.name = string.Format("HorPlat x{0}", count);
	}

	private void BuildVertical() {
		Transform spriteChildObject = NewBlankChildObject("Sprites");

		float L = length - 0.0001f;
		//GameObject leftEndPrefab = ChooseRandom(horizontalLeftEndSpritePrefabs);
		//float leftLength = GetLengthOfSprite(leftEndPrefab);

		//GameObject rightEndPrefab = ChooseRandom(horizontalRightEndSpritePrefabs);
		//float rightLength = GetLengthOfSprite(rightEndPrefab);

		Vector2 lastEndPoint = transform.position;
		platformBottomSide = transform.position.y;

		int count = 1;

		//lastEndPoint = CreateNewHorObject(leftEndPrefab, spriteChildObject, lastEndPoint);
		//L -= leftLength;

		float width = 0f;

		while (L >= 0) {
			GameObject middlePrefab = ChooseRandom(pack.verticalMiddleSpritePrefabs);
			width = GetLengthOfSprite(middlePrefab);
			float middleLength = GetHeightOfSprite(middlePrefab);

			lastEndPoint = CreateNewVertObject(middlePrefab, spriteChildObject, lastEndPoint);
			L -= middleLength;
			count++;
		}

		//lastEndPoint = CreateNewHorObject(rightEndPrefab, spriteChildObject, lastEndPoint);
		length = lastEndPoint.y - transform.position.y;
		platformTopSide = lastEndPoint.y;
		platformLeftSide = transform.position.x - width * 0.5f;
		platformRightSide = transform.position.x + width * 0.5f;

		// Create the colliders
		Transform colliderChildObject = NewBlankChildObject("Colliders");
		Transform colliderObject = NewBlankChildObject("Collider");
		colliderObject.SetParent(colliderChildObject);
		colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

		Rigidbody2D rBody = colliderObject.gameObject.AddComponent<Rigidbody2D>();
		rBody.bodyType = RigidbodyType2D.Static;

		BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

		boxCollider.offset = new Vector2( 0, length * 0.5f);
		boxCollider.size = new Vector2(width, length);
		boxCollider.sharedMaterial = Slippery();

		gameObject.name = string.Format("VertPlat x{0}", count);
	}

	private void BuildSpikesTop() {
		Transform spikesChild = GetSpikesChild();
		if (type == Type.Vertical) {
			GameObject newSprite = Object.Instantiate(pack.spikeShortPrefab, spikesChild);
			Transform bottomPoint = newSprite.transform.Find("BottomEndPoint");
			newSprite.transform.position = new Vector2(transform.position.x - bottomPoint.localPosition.x,
				platformTopSide - bottomPoint.localPosition.y);

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = newSprite.transform.localPosition;
			boxCollider.size = new Vector2(platformRightSide - platformLeftSide - sliderSize, GetHeightOfSprite(newSprite));

			colliderObject.gameObject.AddComponent<Acid>();
		} else {
			GameObject longSpikePrefab = pack.spikeLongPrefab;

			float L = length - 0.0001f;

			Vector2 lastEndPoint = new Vector2(transform.position.x, platformTopSide);

			float prefabWidth = GetLengthOfSprite(longSpikePrefab);
			float prefabHeight = GetHeightOfSprite(longSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(longSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 0);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y - prefabHeight * 0.5f);
				lastEndPoint = new Vector2(lastEndPoint.x + prefabWidth, lastEndPoint.y);
				L -= prefabWidth;
			}

			GameObject shortSpikePrefab = pack.spikeShortPrefab;

			prefabWidth = GetLengthOfSprite(shortSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(shortSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 0);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y - prefabHeight * 0.5f);
				lastEndPoint = new Vector2(lastEndPoint.x + prefabWidth, lastEndPoint.y);
				L -= prefabWidth;
			}

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = new Vector2(length * 0.5f, platformTopSide - transform.position.y + prefabHeight * 0.5f);
			boxCollider.size = new Vector2(platformRightSide - platformLeftSide - sliderSize, prefabHeight);

			colliderObject.gameObject.AddComponent<Acid>();
		}
	}

	private void BuildSpikesBottom() {
		Transform spikesChild = GetSpikesChild();
		if (type == Type.Vertical) {
			GameObject newSprite = Object.Instantiate(pack.spikeShortPrefab, spikesChild);
			newSprite.transform.localEulerAngles = new Vector3(0, 0, 180);
			Transform bottomPoint = newSprite.transform.Find("TopEndPoint");
			newSprite.transform.position = new Vector2(transform.position.x - bottomPoint.localPosition.x,
				transform.position.y - bottomPoint.localPosition.y);

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = newSprite.transform.localPosition;
			boxCollider.size = new Vector2(platformRightSide - platformLeftSide - sliderSize, GetHeightOfSprite(newSprite));

			colliderObject.gameObject.AddComponent<Acid>();
		} else {
			GameObject longSpikePrefab = pack.spikeLongPrefab;

			float L = length - 0.0001f;

			Vector2 lastEndPoint = new Vector2(transform.position.x, platformBottomSide);

			float prefabWidth = GetLengthOfSprite(longSpikePrefab);
			float prefabHeight = GetHeightOfSprite(longSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(longSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 180);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y + prefabHeight * 0.5f);
				lastEndPoint = new Vector2(lastEndPoint.x + prefabWidth, lastEndPoint.y);
				L -= prefabWidth;
			}

			GameObject shortSpikePrefab = pack.spikeShortPrefab;

			prefabWidth = GetLengthOfSprite(shortSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(shortSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 180);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y + prefabHeight * 0.5f);
				lastEndPoint = new Vector2(lastEndPoint.x + prefabWidth, lastEndPoint.y);
				L -= prefabWidth;
			}

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = new Vector2( length * 0.5f, platformBottomSide - transform.position.y - prefabHeight * 0.5f);
			boxCollider.size = new Vector2(platformRightSide - platformLeftSide - sliderSize, prefabHeight);

			colliderObject.gameObject.AddComponent<Acid>();
		}
	}

	private void BuildSpikesRight() {
		Transform spikesChild = GetSpikesChild();
		if (type == Type.Horizontal) {
			GameObject newSprite = Object.Instantiate(pack.spikeShortPrefab, spikesChild);
			newSprite.transform.localEulerAngles = new Vector3(0, 0, 270);
			Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
			newSprite.transform.position = new Vector2(platformRightSide - bottomPoint.localPosition.x,
				transform.position.y - bottomPoint.localPosition.y);

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = newSprite.transform.localPosition;
			boxCollider.size = new Vector2(GetLengthOfSprite(newSprite), platformTopSide - platformBottomSide - sliderSize);

			colliderObject.gameObject.AddComponent<Acid>();
		} else {
			GameObject longSpikePrefab = pack.spikeLongPrefab;

			float L = length - 0.0001f;

			Vector2 lastEndPoint = new Vector2(platformRightSide, transform.position.y);

			float prefabWidth = GetLengthOfSprite(longSpikePrefab);
			float prefabHeight = GetHeightOfSprite(longSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(longSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 270);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.y - prefabHeight * 0.5f, bottomPoint.localPosition.x);
				lastEndPoint = new Vector2(lastEndPoint.x, lastEndPoint.y + prefabWidth);
				L -= prefabWidth;
			}

			GameObject shortSpikePrefab = pack.spikeShortPrefab;

			prefabWidth = GetLengthOfSprite(shortSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(shortSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 270);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.y - prefabHeight * 0.5f, bottomPoint.localPosition.x);
				lastEndPoint = new Vector2(lastEndPoint.x, lastEndPoint.y + prefabWidth);
				L -= prefabWidth;
			}

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = new Vector2(platformRightSide - transform.position.x + prefabHeight * 0.5f, length * 0.5f);
			boxCollider.size = new Vector2(prefabHeight, platformTopSide - platformBottomSide - sliderSize);

			colliderObject.gameObject.AddComponent<Acid>();
		}
	}

	private void BuildSpikesLeft() {
		Transform spikesChild = GetSpikesChild();
		if (type == Type.Horizontal) {
			GameObject newSprite = Object.Instantiate(pack.spikeShortPrefab, spikesChild);
			newSprite.transform.localEulerAngles = new Vector3(0, 0, 90);
			Transform bottomPoint = newSprite.transform.Find("RightEndPoint");
			newSprite.transform.position = new Vector2(platformLeftSide - bottomPoint.localPosition.x,
				transform.position.y - bottomPoint.localPosition.y);

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = newSprite.transform.localPosition;
			boxCollider.size = new Vector2(GetLengthOfSprite(newSprite), platformTopSide - platformBottomSide - sliderSize);

			colliderObject.gameObject.AddComponent<Acid>();
		} else {
			GameObject longSpikePrefab = pack.spikeLongPrefab;

			float L = length - 0.0001f;

			Vector2 lastEndPoint = new Vector2(platformLeftSide, transform.position.y);

			float prefabWidth = GetLengthOfSprite(longSpikePrefab);
			float prefabHeight = GetHeightOfSprite(longSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(longSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 90);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.y + prefabHeight*0.5f, bottomPoint.localPosition.x);
				lastEndPoint = new Vector2(lastEndPoint.x, lastEndPoint.y + prefabWidth);
				L -= prefabWidth;
			}

			GameObject shortSpikePrefab = pack.spikeShortPrefab;
			
			prefabWidth = GetLengthOfSprite(shortSpikePrefab);

			while (L >= prefabWidth) {

				GameObject newSprite = Object.Instantiate(shortSpikePrefab, spikesChild);
				newSprite.transform.localEulerAngles = new Vector3(0, 0, 90);
				Transform bottomPoint = newSprite.transform.Find("LeftEndPoint");
				newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.y + prefabHeight * 0.5f, bottomPoint.localPosition.x);
				lastEndPoint = new Vector2(lastEndPoint.x, lastEndPoint.y + prefabWidth);
				L -= prefabWidth;
			}

			Transform colliderChildObject = GetCollidersChild();

			Transform colliderObject = NewBlankChildObject("SpikeCollider");
			colliderObject.SetParent(colliderChildObject);
			//colliderObject.gameObject.layer = LayerMask.NameToLayer("NavTerrain");

			BoxCollider2D boxCollider = colliderObject.gameObject.AddComponent<BoxCollider2D>();

			boxCollider.offset = new Vector2(platformLeftSide - transform.position.x - prefabHeight*0.5f, length*0.5f);
			boxCollider.size = new Vector2(prefabHeight, platformTopSide - platformBottomSide - sliderSize);

			colliderObject.gameObject.AddComponent<Acid>();
		}
	}

	private Transform GetSpikesChild() {
		Transform spikesChild = transform.Find("Spikes");
		if (spikesChild == null) {
			spikesChild = NewBlankChildObject("Spikes");

		}
		return spikesChild;
	}

	private Transform GetCollidersChild() {
		Transform colliderChildObject = transform.Find("Colliders");
		if (colliderChildObject == null) {
			colliderChildObject = NewBlankChildObject("Colliders");

		}
		return colliderChildObject;
	}

	private Transform NewBlankChildObject(string name) {
        Transform emptyGameObject = new GameObject(name).transform;
        emptyGameObject.SetParent(transform);
        emptyGameObject.localPosition = Vector2.zero;
        return emptyGameObject;
    }

    private GameObject ChooseRandom ( GameObject[] objects ) {
        int i = Random.Range(0, objects.Length);
        return objects[i];
    }

    private float GetLengthOfSprite( GameObject sprite ) {
        Transform leftPoint = sprite.transform.Find("LeftEndPoint");
        Transform rightPoint = sprite.transform.Find("RightEndPoint");
        return rightPoint.localPosition.x - leftPoint.localPosition.x;
    }

	private float GetHeightOfSprite(GameObject sprite) {
		Transform bottomPoint = sprite.transform.Find("BottomEndPoint");
		Transform topPoint = sprite.transform.Find("TopEndPoint");
		return topPoint.localPosition.y - bottomPoint.localPosition.y;
	}

	private Vector2 CreateNewHorObject( GameObject prefab, Transform parent, Vector2 lastEndPoint ) {
        GameObject newSprite = Object.Instantiate(prefab, parent);
        Transform leftPoint = newSprite.transform.Find("LeftEndPoint");
        newSprite.transform.position = lastEndPoint - new Vector2(leftPoint.localPosition.x, leftPoint.localPosition.y);
        Transform rightPoint = newSprite.transform.Find("RightEndPoint");
        return rightPoint.position;
    }

	private Vector2 CreateNewVertObject(GameObject prefab, Transform parent, Vector2 lastEndPoint) {
		GameObject newSprite = Object.Instantiate(prefab, parent);
		Transform bottomPoint = newSprite.transform.Find("BottomEndPoint");
		newSprite.transform.position = lastEndPoint - new Vector2(bottomPoint.localPosition.x, bottomPoint.localPosition.y);
		Transform topPoint = newSprite.transform.Find("TopEndPoint");
		return topPoint.position;
	}

	private void CreateSlider( Transform collidersParent, Vector2 endPoint ) {
        Transform sliderObject = NewBlankChildObject("Slider");
        sliderObject.SetParent(collidersParent);
        sliderObject.position = endPoint;

        BoxCollider2D boxCollider = sliderObject.gameObject.AddComponent<BoxCollider2D>();

		//boxCollider.offset = new Vector2(length * 0.5f, 0);
		boxCollider.size = new Vector2(sliderSize, 1);
		boxCollider.sharedMaterial = Slippery();

		//boxCollider.sharedMaterial = Resources.Load<PhysicsMaterial2D>("Slippery");
	}

	private PhysicsMaterial2D Slippery() {
		if (slipperyPM == null) {
			slipperyPM = new PhysicsMaterial2D("Slippery");
			slipperyPM.friction = 0.0f;
		}
		return slipperyPM;
	}
}
