using UnityEngine;

public class Bullet : MonoBehaviour {
	public float lifeTime;
	[SerializeField]
	private float currentTime;
	void OnEnable(){
		currentTime = 0;
	}
	void Update() {
		currentTime += Time.deltaTime;
		if (currentTime > lifeTime)
			gameObject.SetActive (false);
	}
}
