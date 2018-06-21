 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grenade : MonoBehaviour {
	[Header("Properties")]
	public float timer;
	private float currentTimer;
	public float radius;
	public float force;
	public int damage;
	[Header("Pool")]
	[SerializeField]
	private Pool explosionEffect;
	private bool exploded = false;
	[Header("CameraEffect")]
	public CameraShake camShake;
	public float duration;
	public float magnitude;

	void OnEnable(){
		exploded = false;
		currentTimer = timer;
	}

	void OnDrawGizmosSelected() {
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(transform.position, radius);
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
		StartCoroutine(camShake.Shake(duration,magnitude));
		explosionEffect.Recycle (transform.position, Quaternion.identity);
		Collider[] colliders = Physics.OverlapSphere (transform.position,radius);
		foreach (Collider obj in colliders) {
			ExplosiveObject explosiveObj = obj.GetComponent<ExplosiveObject> ();
			if (explosiveObj != null) {
				explosiveObj.makeDamage (damage);
			}
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
}
