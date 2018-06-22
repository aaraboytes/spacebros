using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScorePoint : MonoBehaviour {
	void OnTriggerEnter(Collider other){
		if(other.CompareTag("Player")){
			GameManager._instance.IncreaseScore(50);
			this.gameObject.SetActive (false);
		}
	}
}
