using UnityEngine;

public class DestructibleObject : MonoBehaviour {
	public int objLife;
	public GameObject damageEffect;
	public GameObject destroyEffect;
	private int currentObjLife;
	private Rigidbody rb;
	void OnEnable(){
		currentObjLife = objLife;
	}
	void Start(){
		rb = GetComponent<Rigidbody> ();
		rb.isKinematic = true;
		currentObjLife = objLife;
	}
	void OnTriggerEnter(Collider other){
		if (other.CompareTag ("Bullet")) {
			rb.isKinematic = false;
			other.gameObject.SetActive (false);
			if (currentObjLife > 0) {
				currentObjLife--;
				Instantiate (damageEffect, transform.position, transform.rotation);
			} else {
				Instantiate (destroyEffect, transform.position, transform.rotation);
				gameObject.SetActive (false);
			}
		}
		if (other.CompareTag ("EnemyBullet")) {
			other.gameObject.SetActive (false);
		}
	}
	public void makeDamage(int damage){
		rb.isKinematic = false;
		currentObjLife -= damage;
		if (currentObjLife <= 0) {
			gameObject.SetActive (false);
		}
	}
}
