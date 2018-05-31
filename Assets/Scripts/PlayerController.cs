using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {
	[Header("Player properties")]
	public float speed;
	public float jumpForce;
	[Header("Extra player properties")]
	public float extraYCollide;
	[Header("Gravity modifiers")]
	public float jumpMultiplier;
	public float gravityIncreaser;
	[Header("Shoot properties")]
	[SerializeField]
	private Transform bulletSpawnPoint;
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
		//Reset values with ground
		if (IsGrounded ()) {
			jumped = false;
		}
		//Input values
		float horizontal = Input.GetAxis ("Horizontal"); 
		bool vertical = Input.GetButtonDown ("Jump");
		//Horizontal move
		if (horizontal!= 0) {
			player.velocity = new Vector3 (speed * horizontal * Time.deltaTime, player.velocity.y, 0f);
		}
		//Jump
		if (vertical && !jumped) {
			player.AddForce (Vector3.up * jumpForce);
			jumped = true;
		}
		//Fix jump
		if (player.velocity.y > 0) {
			player.velocity += Vector3.up * Physics.gravity.y * (jumpMultiplier - 1) * Time.deltaTime;
		}
		else if (player.velocity.y < 0) {
			player.velocity += Vector3.up * Physics.gravity.y * (gravityIncreaser - 1) * Time.deltaTime;	
		}
		//Shoot
		if (Input.GetKeyDown (KeyCode.Z)) {
			
		}
	}

	bool IsGrounded(){
		return Physics.Raycast (transform.position, -Vector3.up, collider.bounds.extents.y + extraYCollide);
	}

	void Shoot(){
		GameObject bullet = pool.Recycle ();
		Rigidbody bulletRB = bullet.GetComponent<Rigidbody> ();
		bulletRB.AddForce (bulletSpawnPoint.forward,ForceMode.Impulse);
	}
}
