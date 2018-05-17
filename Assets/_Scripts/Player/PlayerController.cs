using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour {

	[SerializeField]
	private float m_MaxSpeed = 10f;                    // The fastest the player can travel in the x axis.
	[SerializeField]
	private float m_GrapplePullSpeed = 1f;
	[SerializeField]
	private float m_StretchSpeed = .2f;
	[SerializeField]
	private float m_MaxStretchDistance = 15f;
	[SerializeField]
	private float m_JumpForce = 400f;                  // Amount of force added when the player jumps.
	//[Range(0, 1)]
	//[SerializeField]
	//private float m_CrouchSpeed = .36f;  // Amount of maxSpeed applied to crouching movement. 1 = 100%
	public bool m_AirControl = false;                 // Whether or not a player can steer while jumping;
	[SerializeField]
	private LayerMask m_WhatIsGround;                  // A mask determining what is ground to the character
	[SerializeField]
	private LayerMask m_WhatIsGrabbable;

	//public float wallJumpHorizontalOverrideTime = 0.4f;

	public int mouseButton = 0;

	public bool tentacleEnabled = false;
	public bool movementEnabled = true;
	public bool onlyTentacleEnabled = false;

	public GameObject mouseObject;
	public GameObject targetObject;
	private Rigidbody2D mouseBody;
	private SpriteRenderer targetRenderer;

	bool useDragonBones = false;

	//public GameObject swordSwing;

	public Tentacle tentacle;
	private Transform tentacleEnd;
	//private SpringJoint2D idleSpring;
	private SpringJoint2D grappleSpring;
	private SpringJoint2D carryTentSpring;
	[HideInInspector]
	public SpringJoint2D carryMouseSpring;
	
	private Transform m_GroundCheck;    // A position marking where to check if the player is grounded.
	private Transform m_WallCheck;    // A position marking where to check if the player is grounded.
	const float k_GroundedRadius = .2f; // Radius of the overlap circle to determine if grounded
	private bool m_Grounded;            // Whether or not the player is grounded.
	private bool m_Walled;            // Whether or not the player is grounded.
	private Transform m_CeilingCheck;   // A position marking where to check for ceilings
	const float k_CeilingRadius = .01f; // Radius of the overlap circle to determine if the player can stand up
	private Animator m_Anim;            // Reference to the player's animator component.
	DragonBonesController dbController;
	private Rigidbody2D m_Rigidbody2D;
	private bool m_FacingRight = true;  // For determining which way the player is currently facing.

	//private float wallJumpOverrideDelay = 0.0f;

	private bool m_Jump;
	private bool grappling = false;
	private bool gripping = false;
	//private bool m_Shrink = false;
	private Vector2 relativeTentPos;
	private GameObject grippingObject;

	//private Vector2 movementMod = Vector2.zero;

	private float knockBackForceX = 0f;
	private float wallJumpForceX = 0.0f;

	//Health of the character
	public int maxHealth = 6;
	[SerializeField]
	public int currentHealth;

	public float afterDamageInvincibleTime = 0.2f;
	private float afterDamageTimer = 0f;


	public AudioClip playerHitSound;
	private AudioSource audioSource;

	public float HealthPercentage {
		get {
			return (float)currentHealth / (float)maxHealth;
		}
	}

	// Use this for initialization
	void Start () {
		// Setting up references.
		m_GroundCheck = transform.Find("GroundCheck");
		m_CeilingCheck = transform.Find("CeilingCheck");
		m_WallCheck = transform.Find("WallCheck");
		tentacleEnd = tentacle.transform.Find("EndPoint");
		//idleSpring = tentacleEnd.GetComponent<SpringJoint2D>();
		m_Anim = GetComponent<Animator>();
		audioSource = GetComponent<AudioSource>();
		m_Anim.enabled = !useDragonBones;
		
		if (!useDragonBones) {
			transform.Find("Armature").gameObject.SetActive(false);
			GetComponent<SpriteRenderer>().enabled = true;
		} else {
			dbController = GetComponentInChildren<DragonBonesController>();
			dbController.enabled = useDragonBones;
			transform.Find("Armature").gameObject.SetActive(true);
			GetComponent<SpriteRenderer>().enabled = false;
		}
		m_Rigidbody2D = GetComponent<Rigidbody2D>();
		mouseBody = mouseObject.GetComponentInChildren<Rigidbody2D>();
		targetRenderer = mouseObject.GetComponentInChildren<SpriteRenderer>();

		currentHealth = maxHealth;
	}
	
	// Update is called once per frame
	void Update () {
		

		afterDamageTimer -= Time.deltaTime;

		//wallJumpOverrideDelay -= Time.deltaTime;
		if (!m_Jump) {
			// Read the jump input in Update so button presses aren't missed.
			m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
		}
		//m_Shrink = Input.GetButton("Shrink");
		if (tentacleEnabled) {
			RaycastHit2D hit = UpdateTentTargetPosition();

			if (Input.GetMouseButtonDown(mouseButton) ) {

				
				if (hit.collider != null) {
					Rigidbody2D rigidBody = hit.collider.gameObject.GetComponentInChildren<Rigidbody2D>();
					
					if (rigidBody != null && (rigidBody.bodyType == RigidbodyType2D.Static || rigidBody.bodyType == RigidbodyType2D.Kinematic)) {
						Grapple(rigidBody, hit.point);

					} else if (rigidBody != null && rigidBody.bodyType == RigidbodyType2D.Dynamic) {
						Grip(rigidBody, hit.point);
					}

					//if (hit.collider.gameObject.CompareTag("Boss")) {
					//	BossFsmScript bossScript = hit.collider.transform.parent.parent.parent.GetComponentInChildren<BossFsmScript>();
					//	bossScript.GetGrabbed();
					//}
				}
			}
			if ((grappling || gripping) && Input.GetMouseButtonUp(mouseButton)) {
				StopGripping();
			}
		}
		//if (Input.GetMouseButtonDown(1)) {
		//	swordSwing.SetActive(true);
		//}

	}

	public void Grapple( Rigidbody2D grappledBody, Vector3 grappledPoint, bool playSound = true ) {
		if (grappledBody.gameObject.GetComponent<MessageOnGrab>()) {
			grappledBody.gameObject.SendMessage("GetGrabbed", this);
		}
		Fungus.Flowchart.BroadcastFungusMessage("PlayerGrappling");
		//tentacle.endPoint.position = new Vector3(hit.point.x, hit.point.y, -5f);
		tentacle.SetGrabbing(grappledBody.gameObject, grappledPoint, playSound);
		tentacle.joint.distance = CommonUtils.Distance(transform.position, grappledPoint);

		tentacle.AttachEnd(grappledBody, grappledPoint);

		grappling = true;
	}

	public void Grip(Rigidbody2D grippedBody, Vector3 gripPoint, bool playSound = true) {
		tentacle.AttachEnd(grippedBody, gripPoint);
		grippingObject = grippedBody.gameObject;
		carryMouseSpring = mouseBody.gameObject.AddComponent<SpringJoint2D>();
		carryMouseSpring.autoConfigureDistance = false;
		carryMouseSpring.distance = 0.1f;
		carryMouseSpring.dampingRatio = 0.6f;
		carryMouseSpring.frequency = 1f;

		carryMouseSpring.connectedBody = grippedBody;
		carryMouseSpring.connectedAnchor = grippedBody.transform.InverseTransformPoint(gripPoint);

		gripping = true;
		if (playSound)
			tentacle.PlaySuctionNoise();
		grippedBody.gameObject.SendMessage("GetGrabbed", this);
	}

	public void StopGripping() {
		if ( tentacle.Grabbing && tentacle.GrabbedObject!=null &&
			tentacle.GrabbedObject.GetComponent<JubileeFSMScript>()) {
			tentacle.GrabbedObject.SendMessage("GetGrabbedReleased");
		}

			tentacle.StopGrabbing();
		//if (grappleSpring != null) {
		//	if (carryMouseSpring.connectedBody != null &&
		//		carryMouseSpring.connectedBody.gameObject != null &&
		//		carryMouseSpring.connectedBody.gameObject.GetComponent<JubileeFSMScript>() != null)
		//		carryMouseSpring.connectedBody.gameObject.SendMessage("GetGrabbedReleased");
		//	Destroy(grappleSpring);
		//	grappleSpring = null;
		//}

		//if (carryTentSpring != null) {
		//	Destroy(carryTentSpring);
		//	carryTentSpring = null;
		//}



		if (carryMouseSpring != null) {
			if (carryMouseSpring.connectedBody!=null && carryMouseSpring.connectedBody.gameObject != null)
				carryMouseSpring.connectedBody.gameObject.SendMessage("GetGrabbedReleased");
			Destroy(carryMouseSpring);
			carryMouseSpring = null;
		}

		tentacle.AttachEnd(mouseBody, mouseObject.transform.position);
		//idleSpring.enabled = true;
		//tentacle.endPoint.SetParent(transform);
		grappling = false;
		gripping = false;
	}

	private RaycastHit2D UpdateTentTargetPosition() {
		//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector2 mousePosition = CursorManager.GetCursorWorldPosition();
		Vector2 mouseDirection = CommonUtils.NormalVector(transform.position, mousePosition);
		float mouseDistance = CommonUtils.Distance(transform.position, mousePosition);

		mouseObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
		targetObject.SetActive(false);
		if (gripping) {
			if (grippingObject == null || !grippingObject.activeSelf)
				StopGripping();
			//if ( CommonUtils.Distance( grippingObject.transform.position, transform.position ) > m_MaxStretchDistance*2.0f ) 
			if ( CommonUtils.Distance( tentacle.GetEndPosition(), transform.position ) > m_MaxStretchDistance*2.0f)
					StopGripping();
			//targetRenderer.color = Color.white;
			//float d = CommonUtils.Distance(transform.position, mousePosition);
			//if (d < m_MaxStretchDistance) {
			//	mouseObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
			//} else {
			//	mouseObject.transform.position = new Vector3(transform.position.x + mouseDirection.x * m_MaxStretchDistance, transform.position.y + mouseDirection.y * m_MaxStretchDistance, 0);
			//}
			return new RaycastHit2D();
		} else {
			// Cast from cursor position to player position
			Vector2 playerDirection = CommonUtils.NormalVector(mousePosition, transform.position);
			RaycastHit2D hit = Physics2D.Raycast(mousePosition, playerDirection, mouseDistance, m_WhatIsGrabbable);


			if (hit.collider == null) {
				//Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				//Vector2 mouseDirection = CommonUtils.NormalVector(transform.position, mousePosition);
				// cast from cursor position out to max distance
				hit = Physics2D.Raycast(transform.position, mouseDirection, m_MaxStretchDistance, m_WhatIsGrabbable);
			}

			if (hit.collider != null) {
				targetObject.SetActive(true);
				targetObject.transform.position = hit.point;
				//targetRenderer.color = Color.green;
			} else {
				//targetRenderer.color = Color.white;
				//float d = CommonUtils.Distance(transform.position, mousePosition);
				//if (d < m_MaxStretchDistance) {
				//	mouseObject.transform.position = new Vector3(mousePosition.x, mousePosition.y, 0);
				//} else {
				//	mouseObject.transform.position = new Vector3(transform.position.x + mouseDirection.x * m_MaxStretchDistance, transform.position.y + mouseDirection.y * m_MaxStretchDistance, 0);
				//}
			}

			return hit;
		}
	}

	private void FixedUpdate() {
		m_Grounded = false;
		m_Walled = false;
		// The player is grounded if a circlecast to the groundcheck position hits anything designated as ground
		// This can be done using layers instead but Sample Assets will not overwrite your project settings.
		Collider2D[] colliders = Physics2D.OverlapCircleAll(m_GroundCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				m_Grounded = true;
				break;
			}
		}

		colliders = Physics2D.OverlapCircleAll(m_WallCheck.position, k_GroundedRadius, m_WhatIsGround);
		for (int i = 0; i < colliders.Length; i++) {
			if (colliders[i].gameObject != gameObject) {
				m_Walled = true;
				break;
			}
		}
		if (useDragonBones)
			dbController.ground = m_Grounded;
		m_Anim.SetBool("Ground", m_Grounded);

		// Set the vertical animation
		if (useDragonBones)
			dbController.vSpeed = m_Rigidbody2D.velocity.y;
		m_Anim.SetFloat("vSpeed", m_Rigidbody2D.velocity.y);
		// 
		relativeTentPos = CommonUtils.NormalVector(transform.position, tentacleEnd.position);
		//Debug.Log(relativeTentPos);
		// Read the inputs.
		//bool crouch = Input.GetKey(KeyCode.LeftControl);
		float h = CrossPlatformInputManager.GetAxis("Horizontal");
		float v = CrossPlatformInputManager.GetAxis("Vertical");
		
		// Pass all parameters to the character control script.
		Move(h, v, m_Jump);
		m_Jump = false;
	}

	public void Move(float h, float v, bool jump) {

		if (!movementEnabled) {
			h = 0;
			v = 0;
			jump = false;
		}

		knockBackForceX *= 0.9f;
		wallJumpForceX *= 0.94f;

		// Move the character
		if (!grappling) {

			//only control the player if grounded or airControl is turned on
			if (m_Grounded || m_AirControl) {

				// The Speed animator parameter is set to the absolute value of the horizontal input.
				if (useDragonBones)
					dbController.speed = Mathf.Abs(h);
				m_Anim.SetFloat("Speed", Mathf.Abs(h));

			

				if (Mathf.Abs(h) > 0)
					m_Rigidbody2D.velocity = new Vector2(h * m_MaxSpeed + knockBackForceX + wallJumpForceX, m_Rigidbody2D.velocity.y);
				//m_Rigidbody2D.velocity += movementMod;

				//m_Rigidbody2D.AddForce(new Vector2(h * m_MaxSpeed, 0), ForceMode2D.Impulse);
				//if ( m_Grounded && h == 0.0f )
				//	m_Rigidbody2D.AddForce(new Vector2(-m_Rigidbody2D.velocity.x, 0));
			

				// If the input is moving the player right and the player is facing left...
				if (h > 0 && !m_FacingRight) {
					// ... flip the player.
					Flip();
				}
				// Otherwise if the input is moving the player left and the player is facing right...
				else if (h < 0 && m_FacingRight) {
					// ... flip the player.
					Flip();
				}
			}
		} else {
			m_Rigidbody2D.AddForce(new Vector2(h * m_GrapplePullSpeed, v * m_GrapplePullSpeed), ForceMode2D.Impulse);
			//if (!m_Shrink) {
			if (Mathf.Abs(relativeTentPos.x) > Mathf.Abs(relativeTentPos.y)) {
				if (relativeTentPos.x > 0)
					tentacle.joint.distance -= h * m_StretchSpeed;
				else
					tentacle.joint.distance += h * m_StretchSpeed;
			} else {
				if (relativeTentPos.y > 0)
					tentacle.joint.distance -= v * m_StretchSpeed;
				else
					tentacle.joint.distance += v * m_StretchSpeed;
			}
			if (tentacle.joint.distance > m_MaxStretchDistance)
				tentacle.joint.distance = m_MaxStretchDistance;
			//} else {
			//	tentacle.joint.distance = 0.0001f;
			//}
		}
		if (!m_Grounded && m_Walled && jump ) {
			m_Walled = false;
			//wallJumpOverrideDelay = wallJumpHorizontalOverrideTime;
			m_Rigidbody2D.velocity = new Vector2(transform.localScale.x * -m_MaxSpeed * 1.3f, m_JumpForce);
			//m_Rigidbody2D.velocity = Vector2.zero;
			//m_Rigidbody2D.AddForce(new Vector2(0, m_JumpForce));
			
			wallJumpForceX = transform.localScale.x * -m_MaxSpeed * 1.3f;
		}
		// If the player should jump...
		if (m_Grounded && jump && (!useDragonBones || (useDragonBones && dbController.ground))) { //m_Anim.GetBool("Ground")
									  // Add a vertical force to the player.
			m_Grounded = false;
			if (useDragonBones)
				dbController.ground = false;
			m_Anim.SetBool("Ground", false);
			//m_Rigidbody2D.AddForce(new Vector2(0f, m_JumpForce));
			m_Rigidbody2D.velocity = new Vector2(m_Rigidbody2D.velocity.x, m_JumpForce);
		}
	}


	private void Flip() {
		// Switch the way the player is labelled as facing.
		m_FacingRight = !m_FacingRight;

		// Multiply the player's x local scale by -1.
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}

	//public void AddMovement(Vector2 mod) {
	//	movementMod += mod;
	//}

	public void Death() {
		//SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
		Fungus.Flowchart.BroadcastFungusMessage("PlayerDied");
	}

	public void StopMoving() {
		m_Rigidbody2D.velocity = Vector2.zero;
	}

	public void Damage(int damageAmount, Vector2 kbDirection, float knockbackForce = 10 ) {
		//Debug.Log("Damage");
		if (afterDamageTimer <= 0) {
			currentHealth -= damageAmount;
			PlayDamagedNoise();
			//Checks player's health
			if (currentHealth > maxHealth) {
				currentHealth = maxHealth;
			} else if (currentHealth < 1) {
				Death();
			}
			afterDamageTimer = afterDamageInvincibleTime;
		}

		//Vector2 v = -(m_Rigidbody2D.velocity).normalized;
		//if (source != null) {
		//	v = CommonUtils.NormalVector(source.position, transform.position);
		//}
		m_Rigidbody2D.velocity = kbDirection * knockbackForce;
		knockBackForceX = m_Rigidbody2D.velocity.x;
		//m_Rigidbody2D.velocity = Vector2.zero;
		//m_Rigidbody2D.AddForce(kbDirection * knockbackForce, ForceMode2D.Impulse);

		//StartCoroutine(Knockback(v, knockbackForce));
	}
	
    //Knockbacks character based on power and direction.
    public IEnumerator Knockback(Vector2 kbDirection, float knockbackForce = 5 ) {
        float timer = 0;
        float kbTime = .02f;

        while(kbTime > timer)
        {
            timer += Time.deltaTime;

            m_Rigidbody2D.AddForce( kbDirection * knockbackForce, ForceMode2D.Impulse);
        }

        yield return 0;
    }

	public void PlayDamagedNoise() {
		if (audioSource!=null && playerHitSound!=null) {
			audioSource.clip = playerHitSound;
			audioSource.pitch = Random.Range(1.0f - 0.05f, 1.0f + 0.05f);
			audioSource.volume = 0.9f;
			audioSource.Play();
		}
	}
}
