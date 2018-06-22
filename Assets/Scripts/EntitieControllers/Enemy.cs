using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {
	[Header("Properties")]
	public int life;
	private int currentLife;
	[Header("Movement")]
	public float totalDistanceRange;
	public float speed;
	private bool patrolWaiting = false;
	public float rotSpeed;
	public float waitTime;
	private float waitTimer;
	private float currentWaitTimer;
	private float localY;
	[Header("Shoot")]
	public float watchDistance;
	private bool shooting = false;
	public float shootForce;
	public float shootTimer = 0.5f;
	private float currentShootTimer = 0;
	[Header("Audio")]
	private AudioSource audio;
	public AudioClip[] monsterScreamsClip;
	[Header("Extras")]
	[SerializeField]
	private Pool eyeBullets;
	[SerializeField]
	private Transform eye;
	private Rigidbody enemy;
	[SerializeField]
	private GameObject player;
	[SerializeField]
	private Vector3 pointA;
	[SerializeField]
	private Vector3 pointB;
	[SerializeField]
	Vector3 move;
	[SerializeField]
	private Vector3 target;

	void OnDrawGizmosSelected(){
		Gizmos.color = Color.cyan;
		Gizmos.DrawRay (transform.position, Vector3.right * totalDistanceRange / 2);
		Gizmos.color = Color.green;
		Gizmos.DrawRay (transform.position, -Vector3.right * totalDistanceRange / 2);
		Gizmos.color = Color.black;
		Gizmos.DrawRay (transform.position, (player.transform.position - eye.position).normalized * watchDistance);
	}

	void OnDrawGizmos(){
		Gizmos.color = Color.grey;
		Gizmos.DrawSphere (pointA, 0.2f);
		Gizmos.DrawSphere (pointB, 0.2f);
	}

	void Start () {
		enemy = GetComponent<Rigidbody> ();
		currentLife = life;
		player = GameObject.FindGameObjectWithTag ("Player");
		audio = GetComponent<AudioSource> ();
		//Local y
		localY = enemy.position.y;
		//Calculate Points with raycast or default values
		CalculatePoints ();
		target = (Random.Range(0,1) > 0.5) ? pointA : pointB;
		move = (target-enemy.position).normalized;
		//Eye transform
		eye = transform.GetChild (0);
	}

	void Update () {
		float deltaTime = Time.deltaTime;
		//Checking distance with Player and players life
		if (Vector3.Distance (player.transform.position, enemy.position) < watchDistance && player.GetComponent<PlayerController>().life>0) {
			shooting = true;
			if (currentShootTimer > shootTimer) {
				Shoot ();
				currentShootTimer = 0;
			} else
				currentShootTimer += deltaTime;
		} else
			shooting = false;

		//If it falls, it recalculate points
		if (enemy.position.y != localY) {
			CalculatePoints ();
		}

		//Checking distance beetween points
		if (enemy.position.x >= pointB.x && target == pointB) {
			target = pointA;
			ChangeDirection ();
		} else if (enemy.position.x <= pointA.x && target == pointA) {
			target = pointB;
			ChangeDirection ();
		}

		//Evaluating if is patroling or waiting
		if (!patrolWaiting && !shooting) {
			enemy.MovePosition (enemy.position + (move * speed * deltaTime));
		}
		else
			currentWaitTimer += deltaTime;

		//Continue with Patrol
		if (currentWaitTimer > waitTime) {
			patrolWaiting = false;
			currentWaitTimer = 0;
		}
	}
	void Shoot(){
		audio.PlayOneShot (monsterScreamsClip [Random.Range (0, monsterScreamsClip.Length - 1)]);
		GameObject energy = eyeBullets.Recycle (eye.position, eye.rotation);
		Rigidbody energyRb = energy.GetComponent<Rigidbody> ();
		energyRb.velocity = Vector3.zero;
		energyRb.AddForce((player.transform.position-eye.position).normalized * shootForce);
	}

	void ChangeRotation(){
		if (target == pointA) {
			transform.rotation = Quaternion.Euler (0,-90.0f, 0);
		} else if (target ==pointB) {
			transform.rotation = Quaternion.Euler(0,-270.0f, 0);
		}
	}

	void ChangeDirection(){
		move = (target-enemy.position).normalized;
		ChangeRotation ();
		patrolWaiting = true;
	}

	void CalculatePoints(){
		RaycastHit hit;
		if(Physics.Raycast(transform.position,-Vector3.right,out hit,totalDistanceRange/2)){
			pointA = hit.point + (Vector3.right  * GetComponent<BoxCollider>().bounds.extents.x) + Vector3.right;
		}else{
			pointA = enemy.position - Vector3.right * (totalDistanceRange / 2);
		}
		if (Physics.Raycast (transform.position, Vector3.right, out hit, totalDistanceRange / 2)) {
			pointB = hit.point - (Vector3.right * GetComponent<BoxCollider> ().bounds.extents.x) - Vector3.right;
		} else {
			pointB = enemy.position + Vector3.right * (totalDistanceRange / 2);
		}
	}

	public void MakeDamage(int damage){
		if (currentLife > 0) {
			currentLife -= damage;
			GameManager._instance.IncreaseScore (10);
		} else {
			currentLife = life;
			gameObject.SetActive (false);
			GameManager._instance.IncreaseScore (50);
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Bullet")) {
			other.gameObject.SetActive (false);
			MakeDamage (1);
		}
	}
}
