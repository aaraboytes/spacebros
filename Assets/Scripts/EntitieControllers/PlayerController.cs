using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Player properties")]
	//Horizontal move
	public int life;
	[SerializeField]
	private int currentLife;
	public float speed;
	public float dashSpeed;
	private float horizontal;
	private bool tapped = false;
	public float tapDelay = 0.5f;
	private int tapCount = 0;
	private float currentTapDelay;
	//Jump
	private bool vertical;
	public float jumpForce;
	public float jumpDelay = 0.1f;
	private float currentJumpDelay = 0;
	//Climb
	[SerializeField]
	public float climbSpeed;
	private bool walled = false;
	private bool climbLevel = false;
	private Vector3 lastTouchedWall;
	[Header("Gameplay Properties")]
	public int grenadesAmount = 3;
	[Header("Extra player properties")]
	public float extraXCollide;
	public float extraYCollide;
	public float rotationToRight = 0;
	public float rotationToLeft = 180;
	[SerializeField]
	public GameObject playerModel;
	[SerializeField]
	private GameObject dashParticle;
	[Header("Gravity modifiers")]
	public float jumpMultiplier;
	public float gravityIncreaser;
	[Header("KnockBack")]
	public float knockBackForce = 300;
	public float upKnockBackForce = 300;
	[Header("Shoot properties")]
	[SerializeField]
	private Transform bulletSpawnPoint;
	public float shootForce;
	public float shootCadence;
	private float currentShootCadence = 0;
	[Header("Grenades properties")]
	public float averageLauchForce;
	public float lauchCadence;
	private float currentLaunchCadence;
	public float difference;
	[Header("Pooling")]
	[SerializeField]
	private Pool bulletPool;
	[SerializeField]
	private Pool grenadesPool;
	[Header("Animations")]
	[SerializeField]
	private Animator anim;
	private bool gunOut = false;
	[Header("AudioEffects")]
	private AudioSource audio;
	public AudioClip[] damageClips;
	public AudioClip shootClip;

	private bool jumped;
	private Rigidbody player;
	private Collider collider;

	void Start(){
		player = GetComponent<Rigidbody> ();
		collider = GetComponent<CapsuleCollider> ();
		audio = GetComponent<AudioSource> ();
		currentLife = life;
		GameManager._instance.ModifyLifeSlider (life, currentLife);
	}

	void Update(){
		//Chequep de animacion de sacar arma
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GunOut")) {
			gunOut = true;
		}

		if (gunOut && !climbLevel && !GameManager._instance.pause) {
			float deltaTime = Time.deltaTime; 	//Delta time optimized
			bool isGrounded = IsGrounded ();		//isGrounded optimized

			//Reset values with ground
			if (isGrounded) {
				walled = false;
				currentJumpDelay += deltaTime;
				if (currentJumpDelay > jumpDelay) {
					jumped = false;
					currentJumpDelay = 0;
				}
				anim.SetBool ("jump", false);
				anim.SetBool ("climb", false);
			}

			//Input values
			horizontal = Input.GetAxis ("Horizontal");
			if (Input.GetAxis("Vertical")>=0.5f||Input.GetButtonDown("AButton") || Input.GetKeyDown (KeyCode.UpArrow))
				vertical = true;
			else
				vertical = false;

			//********************************************Horizontal move**********************************
			if (horizontal != 0) {
				if (Input.GetAxisRaw ("Horizontal") == 0) {
					tapCount++;
				}
				//Dash
				if (tapped && currentTapDelay > 0 && tapCount > 1) {
					player.velocity = new Vector3 (dashSpeed * horizontal * deltaTime, player.velocity.y, 0f);	
					currentTapDelay = tapDelay;
					dashParticle.GetComponent<ParticleSystem> ().Play ();
					//Normal run
				} else {
					player.velocity = new Vector3 (speed * horizontal * deltaTime, player.velocity.y, 0f);
				}
				if (!tapped) {
					tapped = true;
					currentTapDelay = tapDelay;
				}
				//Flip character
				if (!climbLevel && !walled) {
					if (horizontal > 0)
						gameObject.transform.rotation = Quaternion.Euler (0, rotationToRight, 0);
					else if (horizontal < 0)
						gameObject.transform.rotation = Quaternion.Euler (0, rotationToLeft, 0);
				}
			} else {
				dashParticle.GetComponent<ParticleSystem> ().Pause ();
			}
			if (tapped) {
				currentTapDelay -= Time.deltaTime;
			}
			if (currentTapDelay <= 0) {
				tapped = false;
				tapCount = 0;
			}
			//*******************************************************************************************************
			//******************************************Vertical*****************************************************
			//Jump
			if (vertical && !jumped) {
				Jump ();
			}

			//Fix jump
			if (player.velocity.y > 0 && (!Input.GetButton("AButton") && !Input.GetKey (KeyCode.UpArrow))) {
				player.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * deltaTime;
			} else if (player.velocity.y < 0 && !walled) {
				player.velocity += Vector3.up * Physics.gravity.y * (gravityIncreaser - 1) * deltaTime;	
			}

			//Start Climb
			if ((isWallingLeft ()||isWallingRight()) && !isGrounded && horizontal!=0) {
				walled = true;
				anim.SetBool ("climb", true);
				RaycastHit hit;
				if(Physics.Raycast(transform.position,transform.rotation.y == rotationToRight? Vector3.right:-Vector3.right,out hit,1.0f)){
					lastTouchedWall = hit.collider.gameObject.transform.position;
				}
			} else
				walled = false;	

			//Climb
			if (walled) {
				Climb ();
			} else {
				anim.SetBool ("climb", false);
			}
			//***********************************************************************************************
			//Shoot
			currentShootCadence += deltaTime;
			if (Input.GetKey (KeyCode.Z) || Input.GetAxis("Shoot")<-0.1f) {
				anim.SetBool ("shoot", true);
				if (currentShootCadence > shootCadence) {
					Shoot ();
					currentShootCadence = 0;
				}
			} else
				anim.SetBool ("shoot", false);

			//Throw Grenades
			currentLaunchCadence += deltaTime;
			if ((Input.GetKey (KeyCode.C)|| Input.GetAxis("Shoot")>0.9f) && grenadesAmount>0) {
				if (currentLaunchCadence > lauchCadence) {
					Grenades ();
					currentLaunchCadence = 0;
					grenadesAmount--;
					GameManager._instance.ModifyGrenadesAmount (grenadesAmount);
				}
			}

			//Animaciones
			anim.SetFloat ("velocity", Mathf.Abs (player.velocity.x + player.velocity.z));
			anim.SetFloat ("velocityY", player.velocity.y);
		}
	}
	void Jump(){
		player.AddForce (Vector3.up * jumpForce);
		jumped = true;
		anim.SetBool ("jump", true);
		dashParticle.GetComponent<ParticleSystem> ().Pause ();
	}

	void Climb(){
		//Checking if there is no wall in front of player when hes climbing
		RaycastHit hit;
		if (isWallingRight ()) {
			if (!Physics.Raycast (new Vector3 (transform.position.x, transform.position.y + collider.bounds.extents.y, 0f), Vector3.right, out hit, 1.0f)) {
				Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y + collider.bounds.extents.y, 0f), Vector3.right * 0.5f, Color.blue);
				anim.SetBool ("climbLevel", true);
			} else {
				lastTouchedWall = hit.collider.gameObject.transform.position;
			}
		} else if (isWallingLeft ()) {
			if (!Physics.Raycast (new Vector3 (transform.position.x, transform.position.y + collider.bounds.extents.y, 0f), -Vector3.right, out hit, 1.0f)) {
				Debug.DrawRay (new Vector3 (transform.position.x, transform.position.y + collider.bounds.extents.y, 0f), -Vector3.right * 0.5f, Color.blue);
				anim.SetBool ("climbLevel", true);
			} else {
				lastTouchedWall = hit.collider.gameObject.transform.position;
			}
		}
		//Climb movement
		if (vertical) {
			player.MovePosition (player.position + (Vector3.up * climbSpeed * Time.deltaTime));
		}
	}
	public void StartClimb(){
		player.isKinematic = true;
		climbLevel = true;
	}
	public void EndClimb(){
		anim.SetBool ("climbLevel", false);
		Debug.Log ("Ending Climb in player controller" + this.name);
		player.isKinematic = false;
		climbLevel = false;
		player.position = new Vector3 (lastTouchedWall.x, lastTouchedWall.y + 1, .0f);
	}

	float timer = 0;
	IEnumerator MoveToClimbPosition(){
		if(timer > 0.5f) {
			playerModel.transform.SetParent (null);
			player.position = new Vector3 (transform.position.x + (transform.rotation.y == rotationToRight ? 0.5f : -0.5f), 0.5f, 0.0f);
			playerModel.transform.SetParent (player.transform);
			timer = 0;
			yield return null;
		}
		timer+=Time.deltaTime;
	}

	bool isWallingRight(){
		return Physics.Raycast (transform.position, Vector3.right, collider.bounds.extents.x + extraXCollide);
	}

	bool isWallingLeft(){
		return Physics.Raycast (transform.position, -Vector3.right, collider.bounds.extents.x + extraXCollide);
	}

	bool IsGrounded(){
		return Physics.Raycast (transform.position, -Vector3.up, collider.bounds.extents.y + extraYCollide);
	}

	void Shoot(){
		audio.PlayOneShot (shootClip,0.5f);
		GameObject bullet = bulletPool.Recycle (bulletSpawnPoint.transform.position,Quaternion.Euler(0,90*(transform.rotation.eulerAngles.y==rotationToRight?1:-1),0));
		Rigidbody bulletRB = bullet.GetComponent<Rigidbody> ();
		bulletRB.velocity = Vector3.zero;
		bulletRB.AddForce (bulletSpawnPoint.transform.right * shootForce);
	}

	void Grenades(){
		GameObject grenade1 = grenadesPool.Recycle (bulletSpawnPoint.transform.position,Quaternion.identity);
		GameObject grenade2 = grenadesPool.Recycle (bulletSpawnPoint.transform.position,Quaternion.identity);
		GameObject grenade3 = grenadesPool.Recycle (bulletSpawnPoint.transform.position,Quaternion.identity);
		Rigidbody gRb1 = grenade1.GetComponent<Rigidbody> ();
		Rigidbody gRb2 = grenade2.GetComponent<Rigidbody> ();
		Rigidbody gRb3 = grenade3.GetComponent<Rigidbody> ();
		int dir = horizontal > 0 ? 1 : -1;
		gRb1.AddForce (new Vector3(dir,0.5f,0f) * (averageLauchForce - difference));
		gRb2.AddForce (new Vector3(dir,0.5f,0f) * averageLauchForce);
		gRb3.AddForce (new Vector3(dir,0.5f,0f) * (averageLauchForce + difference));
	}

	public void MakeDamage(int damage){
		currentLife -= damage;
		GameManager._instance.ModifyLifeSlider (life, currentLife);
		if (currentLife > 0) {
			audio.PlayOneShot (damageClips [Random.Range (0, damageClips.Length - 1)]);
		}
		else{
			anim.SetBool ("dead", true);
			this.enabled = false;
			GameManager._instance.GameOver ();
		}
	}

	void OnTriggerEnter(Collider other){
		if(other.CompareTag("EnemyBullet")){
			if (currentLife > 0) {
				other.gameObject.SetActive (false);
				Vector3 knockBack = (transform.position - other.transform.position).normalized;
				player.AddForce ((knockBack * knockBackForce) + (Vector3.up * upKnockBackForce));
				MakeDamage (other.GetComponent<EnemyBullet> ().damage);
			}
		}
	}

	public void RestorePlayer(){
		currentLife = life;
		grenadesAmount = 3;
	}
}
