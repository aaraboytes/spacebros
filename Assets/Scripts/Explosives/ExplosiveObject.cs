using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveObject : MonoBehaviour {
	[Header("Parameters")]
	public int objLife;
	public int damage;
	public float radius;
	public float force;
	[SerializeField]
	[Header("Pool")]
	private Pool explosionEffect;
	[SerializeField]
	private Pool damageEffect;
	[Header("Camera effect")]
	public CameraShake camShake;
	public float duration;
	public float magnitude;
	[Header("Audio")]
	public AudioClip damageSound;
	public AudioClip destroySound;
	private AudioSource audio;
	private int currentObjLife;
	void OnEnable(){
		currentObjLife = objLife;
	}
	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
	}
	void Start(){
		audio = GetComponent<AudioSource> ();
		currentObjLife = objLife;
	}
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Bullet")) {
			if (currentObjLife > 0) {
				currentObjLife--;
				damageEffect.Recycle(other.transform.position,Quaternion.Inverse(other.transform.rotation));
			} else {
				Explossion ();
			}
			other.gameObject.SetActive (false);
		}
		if(other.CompareTag("EnemyBullet")){
			other.gameObject.SetActive (false);
			currentObjLife = 0;
			Explossion ();
		}
	}
	public void makeDamage(int damage){
		currentObjLife -= damage;
		if (currentObjLife <= 0) {
			Explossion ();
		}
	}
	void Explode(){
		StartCoroutine(camShake.Shake(duration,magnitude));
		explosionEffect.Recycle (transform.position, Quaternion.identity);
		Collider[] colliders = Physics.OverlapSphere (transform.position,radius);
		foreach (Collider obj in colliders) {
			DestructibleObject destrObj = obj.GetComponent<DestructibleObject> ();
			if (destrObj != null) {
				destrObj.makeDamage (damage);
			}
			PlayerController player = obj.GetComponent<PlayerController> ();
			if (player != null) {
				player.MakeDamage (damage);
			}
			Enemy enemy = obj.GetComponent<Enemy> ();
			if (enemy != null) {
				enemy.MakeDamage (damage);
			}
		}
		gameObject.SetActive (false);
	}
	void Explossion(){
		audio.PlayOneShot (destroySound, 0.8f);
		Explode ();
	}
}
