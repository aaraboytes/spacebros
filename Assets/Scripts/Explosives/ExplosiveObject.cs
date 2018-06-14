using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour {
	public int objLife;
	public int damage;
	public float radius;
	public float force;
	[SerializeField]
	private Pool explosionEffect;
	[SerializeField]
	private Pool damageEffect;
	public AudioClip damageSound;
	public AudioClip destroySound;
	private AudioSource audio;
	private int currentObjLife;
	void OnEnable(){
		currentObjLife = objLife;
	}
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(transform.position, radius);
	}
	void Start(){
		audio = GetComponent<AudioSource> ();
		currentObjLife = objLife;
	}
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Bullet")) {
			if (currentObjLife > 0) {
				currentObjLife--;
				//damageEffect.Recycle(other.transform.position,Quaternion.Inverse(other.transform.rotation));
				audio.PlayOneShot (damageSound, 0.5f);
			} else {
				Explossion ();
			}
			other.gameObject.SetActive (false);
		}
	}
	public void makeDamage(int damage){
		currentObjLife -= damage;
		if (currentObjLife <= 0) {
			Explossion ();
		}
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
	void Explossion(){
		audio.PlayOneShot (destroySound, 0.8f);
		Explode ();
	}
}
