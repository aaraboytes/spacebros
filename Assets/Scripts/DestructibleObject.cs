using UnityEngine;

public class DestructibleObject : MonoBehaviour {
	public int objLife;
	public GameObject damageEffect;
	public GameObject destroyEffect;
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
				audio.PlayOneShot (destroySound, 0.5f);
				Instantiate (destroyEffect, transform.position, transform.rotation);
				gameObject.SetActive (false);
			}
		}
	}
}
