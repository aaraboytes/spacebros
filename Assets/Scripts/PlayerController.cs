using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Player properties")]
	//Horizontal move
	public int life;
	private int currentLife;
	public float speed;
	public float dashSpeed;
	private float horizontal;
	[SerializeField]
	private bool tapped = false;
	public float tapDelay = 0.5f;
	private int tapCount = 0;
	[SerializeField]
	private float currentTapDelay;
	//Jump
	public float jumpForce;
	//Climb
	public float climbDistance;
	public float climbTime;
	private float currentClimbTime;
	private bool walled = false;
	[Header("Extra player properties")]
	public float extraXCollide;
	public float extraYCollide;
	[SerializeField]
	private GameObject dashParticle;
	[Header("Gravity modifiers")]
	public float jumpMultiplier;
	public float gravityIncreaser;
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

	private bool jumped;
	private Rigidbody player;
	private Collider collider;

	void Start(){
		player = GetComponent<Rigidbody> ();
		collider = GetComponent<CapsuleCollider> ();
	}

	void Update(){
		if (!anim.GetCurrentAnimatorStateInfo(0).IsName("GunOut")) {
			gunOut = true;
		}
		if (gunOut) {
			float deltaTime = Time.deltaTime; 	//Delta time optimized
			bool isGrounded = IsGrounded ();		//isGrounded optimized
			//Reset values with ground
			if (isGrounded) {
				walled = false;
				jumped = false;
				anim.SetBool ("jump", false);
			}

			//Input values
			horizontal = Input.GetAxis ("Horizontal");
			bool vertical;
			if (Input.GetButtonDown ("Jump") || Input.GetKeyDown (KeyCode.UpArrow))
				vertical = true;
			else
				vertical = false;

			//Horizontal move
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
				if (horizontal > 0)
					gameObject.transform.rotation = Quaternion.Euler (0, 0, 0);
				else if (horizontal < 0)
					gameObject.transform.rotation = Quaternion.Euler (0, 180.0f, 0);
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

			//Jump
			if (vertical && !jumped) {
				player.AddForce (Vector3.up * jumpForce);
				jumped = true;
				anim.SetBool ("jump", true);
				dashParticle.GetComponent<ParticleSystem> ().Pause ();
			}

			//Fix jump
			if (player.velocity.y > 0 && (!Input.GetButton ("Jump") && !Input.GetKey (KeyCode.UpArrow))) {
				player.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * deltaTime;
			} else if (player.velocity.y < 0 && !walled) {
				player.velocity += Vector3.up * Physics.gravity.y * (gravityIncreaser - 1) * deltaTime;	
			}

			//Wall jump
			if (isWallingLeft () && !isGrounded && horizontal < 0) {
				walled = true;
			} else if (isWallingRight () && !isGrounded && horizontal > 0) {
				walled = true;
			} else
				walled = false;	

			//Climb
			if (walled && vertical) {
				//player.velocity = Vector3.zero;
				player.MovePosition (transform.position + Vector3.up * climbDistance * deltaTime);
				currentClimbTime += deltaTime;
				if (currentClimbTime > climbTime) {
					currentClimbTime = 0;
					player.MovePosition (transform.position + Vector3.up * climbDistance * deltaTime);
				}
			} else if (walled) {
				//Salto hacia afuera + gravedad añadida

			} else if (!walled) {
				currentClimbTime = climbTime;
			}

			//Shoot
			currentShootCadence += deltaTime;
			if (Input.GetKey (KeyCode.Z)) {
				anim.SetBool ("shoot", true);
				if (currentShootCadence > shootCadence) {
					Shoot ();
					currentShootCadence = 0;
				}
			} else
				anim.SetBool ("shoot", false);

			//
			currentLaunchCadence += deltaTime;
			if (Input.GetKey (KeyCode.C)) {
				if (currentLaunchCadence > lauchCadence) {
					Grenades ();
					currentLaunchCadence = 0;
				}
			}

			//Animaciones
			anim.SetFloat ("velocity", Mathf.Abs (player.velocity.x + player.velocity.z));
			anim.SetFloat ("velocityY", player.velocity.y);
		}
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
		GameObject bullet = bulletPool.Recycle (bulletSpawnPoint.transform.position,Quaternion.identity);
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
		if (currentLife <= 0) {
			anim.SetBool ("dead", true);
			this.enabled = false;
		}
	}

}
