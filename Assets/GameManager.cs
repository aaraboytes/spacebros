using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {
	[Header("UI")]
	public static GameManager _instance;
	public Text playersName;
	public Text scoreText;
	public Slider lifeSlider;
	public Image[] grenades;
	public GameObject pausePanel;
	[Header("Data")]
	private int score;
	public bool gameStart = false;
	public bool pause = false;
	[Header("Background Music")]
	private AudioSource audio;
	public AudioClip[] backgroundClips;
	void Awake(){
		if (_instance == null) {
			_instance = this;
		}
	}
	void Start(){
		playersName.text = PlayerPrefs.GetString ("name", "Astrodude");
		audio = GetComponent<AudioSource> ();
		audio.clip = backgroundClips [Random.Range (0, backgroundClips.Length)];
		audio.Play ();
	}

	void Update(){
		if (Input.GetButtonDown ("Submit") || Input.GetKeyDown(KeyCode.Escape)) {
			if (!pause)
				Pause ();
			else
				Continue ();
		}
	}

	public void IncreaseScore(int extraScore){
		score += extraScore;
		if (scoreText != null) {
			scoreText.text=score.ToString ();
		}
	}
	public void ModifyLifeSlider(int maxLife,int currentLife){
		if (lifeSlider != null) {
			lifeSlider.value = (float)currentLife /(float)maxLife;
		}
	}
	public void ModifyGrenadesAmount(int remainingGrenades){
		for (int i = 0; i < grenades.Length; i++) {
			if (i > remainingGrenades-1) {
				grenades [i].gameObject.SetActive (false);
			}
		}
	}

	public void Pause(){
		if (gameStart && !pause) {
			pausePanel.SetActive (true);
			pause = true;
			Time.timeScale = 0;
		}
	}

	public void Continue(){
		Time.timeScale = 1;
		pausePanel.SetActive (false);
		pause = false;
	}

	public void GameOver(){
		if (score > PlayerPrefs.GetInt ("score")) {
			PlayerPrefs.SetInt ("score", score);
		}
		gameStart = false;
	}
}
