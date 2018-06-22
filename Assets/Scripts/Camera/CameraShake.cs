using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour {
	public IEnumerator Shake(float duration, float magnitude){
		Vector3 originalPos = transform.position;
		float timer = 0f;
		while (timer < duration) {
			float x = Random.Range (-1f, 1f) * magnitude + transform.position.x;
			float y = Random.Range (-1f, 1f) * magnitude + transform.position.y;
			transform.localPosition = new Vector3 (x, y, transform.localPosition.z);
			timer += Time.deltaTime;
			yield return null;
		}
		transform.localPosition = originalPos;
	}
}
