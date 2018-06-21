using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleDisabler : MonoBehaviour {
	ParticleSystem ps;
	AudioSource audio;

	void Start(){
		ps = GetComponent<ParticleSystem> ();
		audio = GetComponent<AudioSource> ();
	}
	// Update is called once per frame
	void Update () {
		if (!ps.isPlaying && !audio.isPlaying) {
			gameObject.SetActive (false);
		}
	}
}
