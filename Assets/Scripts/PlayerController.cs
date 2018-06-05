using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Player properties")]
	private float horizontal;
	public float speed;
	public float jumpForce;
	public float climbDistance;
	public float climbTime;
	private float currentClimbTime;
	[SerializeField]
	private bool walled = false;
	[Header("Extra player properties")]
	public float extraYCollide;
	public float extraXCollide;
	[Header("Gravity modifiers")]
	public float jumpMultiplier;
	public float gravityIncreaser;
	[Header("Shoot properties")]
	[SerializeField]
	private Transform bulletSpawnPoint;
	public float shootForce;
	public float shootCadence;
	private float currentShootCadence = 0;
	[Header("Pooling")]
	[SerializeField]
	private Pool pool;

	private bool jumped;
	private Rigidbody player;
	private Collider collider;

	void Start(){
		player = GetComponent<Rigidbody> ();
		collider = GetComponent<BoxCollider> ();
	}

	void Update(){
		float deltaTime = Time.deltaTime; 	//Delta time optimized
		bool isGrounded = IsGrounded();		//isGrounded optimized
		//Reset values with ground
		if (isGrounded) {
			walled = false;
			jumped = false;
		}

		//Input values
		horizontal = Input.GetAxis ("Horizontal");
		bool vertical;
		if(Input.GetButtonDown ("Jump")||Input.GetKeyDown(KeyCode.UpArrow))
			vertical = true;
		else
			vertical = false;

		//Horizontal move
		if (horizontal != 0) {
			player.velocity = new Vector3 (speed * horizontal * deltaTime, player.velocity.y, 0f);
		}

		//Jump
		if (vertical && !jumped) {
			player.AddForce (Vector3.up * jumpForce);
			jumped = true;
		}

		//Fix jump
		if (player.velocity.y > 0 && (!Input.GetButton("Jump") && !Input.GetKey(KeyCode.UpArrow))) {
			player.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * deltaTime;
		}
		else if (player.velocity.y < 0 || walled) {
			player.velocity += Vector3.up * Physics.gravity.y * (gravityIncreaser - 1) * deltaTime;	
		}

		//Wall jump
		if (isWallingLeft () && !isGrounded && horizontal<0) {
			walled = true;
		} else if (isWallingRight () && !isGrounded && horizontal>0) {
			walled = true;
		} else
			walled = false;	

		//Climb
		if (walled && vertical) {
			transform.position = new Vector3 (transform.position.x, transform.position.y + (climbDistance * deltaTime), transform.position.z);
			currentClimbTime += deltaTime;
			if (currentClimbTime > climbTime) {
				currentClimbTime = 0;
				transform.position = new Vector3 (transform.position.x, transform.position.y + (climbDistance * deltaTime), transform.position.z);
			}
		} else if (!walled) {
			currentClimbTime = climbTime;
		}

		//Shoot
		currentShootCadence += deltaTime;
		if (Input.GetKey (KeyCode.Z)) {
			if (currentShootCadence > shootCadence) {
				Shoot ();
				currentShootCadence = 0;
			}
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
		GameObject bullet = pool.Recycle ();
		bullet.transform.position = bulletSpawnPoint.transform.position;
		Rigidbody bulletRB = bullet.GetComponent<Rigidbody> ();
		bulletRB.velocity = Vector3.zero;
		bulletRB.AddForce (bulletSpawnPoint.transform.right * shootForce);
	}
}
