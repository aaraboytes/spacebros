using UnityEngine;

public class FinishPoint : MonoBehaviour {
	public MainMenu mainMenu;
	public GameObject finishEffect;
	void OnTriggerEnter(){
		PlayerPrefs.SetInt ("seed", (int)Random.Range (0, 1000));
		mainMenu.FadeToScene ("Game");
		GameManager._instance.LevelUp ();
	}
}
