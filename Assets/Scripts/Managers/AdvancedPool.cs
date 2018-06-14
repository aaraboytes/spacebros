using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class AdvancedPool<T>:MonoBehaviour
{
	protected GameObject poolObj;
	private Stack<GameObject> activeObjects;
	private Stack<GameObject> unactiveObjects;
	protected int amount;

	void Start(){
		GameObject bucket = new GameObject (poolObj.name + "_bucket");
		for (int i = 0; i < amount; i++) {
			GameObject aux = (GameObject)Instantiate (poolObj, Vector3.zero, Quaternion.identity);
			aux.SetActive (false);
			aux.transform.SetParent (bucket.transform);
			unactiveObjects.Push (aux);
		}
	}

	public GameObject Recycle(){
		GameObject returnObj = null;
		if (unactiveObjects.Count > 0) {
			returnObj = unactiveObjects.Pop ();
			activeObjects.Push (returnObj);
		}
		return returnObj;
	}

	public void Diactivate(GameObject obj){
		obj.SetActive (false);
		unactiveObjects.Push (obj);
	}
}

