 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
	public float timer;
	private float currentTimer;
	public float radius;
	public float force;
	public int damage;
	public GameObject explosionEffect;
	private bool exploded = false;

	void OnEnable(){
		exploded = false;
		currentTimer = timer;
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
		Instantiate (explosionEffect, transform.position, Quaternion.identity);
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
		}
		gameObject.SetActive (false);
	}
}
