using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour {
	public int objLife;
	public int damage;
	public float radius;
	public float force;
	public GameObject explosionEffect;
	public GameObject damageEffect;
	public AudioClip damageSound;
	public AudioClip destroySound;
	private AudioSource audio;
	private int currentObjLife;
	void OnEnable(){
		currentObjLife = objLife;
	}
	void Start(){
		audio = GetComponent<AudioSource> ();
		currentObjLife = objLife;
	}
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Bullet")) {
			other.gameObject.SetActive (false);
			if (currentObjLife > 0) {
				currentObjLife--;
				Instantiate (damageEffect, transform.position, transform.rotation);
				audio.PlayOneShot (damageSound, 0.5f);
			} else {
				Explossion ();
			}
		}
	}
	public void makeDamage(int damage){
		currentObjLife -= damage;
		if (currentObjLife <= 0) {
			Explossion ();
		}
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
	void Explossion(){
		audio.PlayOneShot (destroySound, 0.8f);
		Instantiate (explosionEffect, transform.position, transform.rotation);
		Explode ();
	}
}
