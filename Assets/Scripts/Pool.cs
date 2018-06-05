using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {
	public GameObject pooledObj;
	public int amount;
	private List<GameObject> pool;
	void Start () {
		GameObject Bucket = new GameObject (pooledObj.name + "_bucket");
		pool = new List<GameObject> ();
		for (int i = 0; i < amount; i++) {
			GameObject aux = (GameObject)Instantiate (pooledObj, Vector3.zero, Quaternion.identity);
			aux.SetActive (false);
			aux.transform.SetParent (Bucket.transform);
			pool.Add (aux);
		}
	}

	int activeObj = 0;
	public GameObject Recycle(){
		for(int i = 0;i<pool.Count;i++) {
			if (!pool [i].activeInHierarchy) {
				pool [i].SetActive (true);
				activeObj++;
				Debug.Log ("Recycled obj with id: " + i);
				return  pool [i];
			}
		}
		return null;
	}
}
