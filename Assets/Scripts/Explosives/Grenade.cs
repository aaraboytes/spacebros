 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
	public float timer;
	private float currentTimer;
	public float radius;
	public float force;
	public int damage;
	[SerializeField]
	private Pool explosionEffect;
	private bool exploded = false;

	void OnEnable(){
		exploded = false;
		currentTimer = timer;
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, radius);
	}

	void Start(){
		currentTimer = timer;
	}

	void Update () {
		if (currentTimer <= 0 && !exploded) {
			exploded = true;
			Explode ();
		} else
			currentTimer -= Time.deltaTime;
	}

	void Explode(){
		explosionEffect.Recycle (transform.position, Quaternion.identity);
		Collider[] colliders = Physics.OverlapSphere (transform.position,radius);
		foreach (Collider obj in colliders) {
			Rigidbody rb = obj.GetComponent<Rigidbody> ();
			if (rb != null) {
				rb.AddExplosionForce (force, transform.position, radius);
			}
			DestructibleObject destrObj = obj.GetComponent<DestructibleObject> ();
			if (destrObj != null) {
				destrObj.makeDamage (damage);
			}
			PlayerController player = obj.GetComponent<PlayerController> ();
			if (player != null) {
				player.MakeDamage (damage);
			}
		}
		gameObject.SetActive (false);
	}
}
