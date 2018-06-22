using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class GameOverScreen : MonoBehaviour {
	public Text score;
	private AudioSource audio;
	public AudioClip gameOverMusic;
	void Start(){
		score.text = PlayerPrefs.GetInt ("highscore", 0).ToString();
		audio = GetComponent<AudioSource> ();
		audio.PlayOneShot (gameOverMusic);
	}
}
