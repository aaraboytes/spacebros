using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pool : MonoBehaviour {
	public GameObject pooledObj;
	public int amount;
	private List<GameObject> pool;
	void Start () {
		GameObject Bucket = new GameObject (pooledObj.name + "_bucket");
		for (int i = 0; i < amount; i++) {
			GameObject aux = (GameObject)Instantiate (pooledObj, Vector3.zero, Quaternion.identity);
			aux.SetActive (false);
			aux.transform.parent = Bucket.transform;
			pool.Add (aux);
		}
	}
	public GameObject Recycle(){
		foreach (GameObject o in pool) {
			if (!o.activeInHierarchy) {
				o.SetActive (true);
				return  o;
			} 
		}
		return null;
	}
}
