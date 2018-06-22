using UnityEngine;

public class PlayerEventsCaller : MonoBehaviour {
	[SerializeField]
	private PlayerController playerScript;
	private MainMenu mainMenu;
	void Start(){
		mainMenu = FindObjectOfType<MainMenu> ();
	}
	public void GoToGameOverScreen(){
		if (mainMenu != null)
			mainMenu.FadeToScene ("GameOver");
	}
	public void EndClimb(){
		Debug.Log ("Ending Climb");
		playerScript.EndClimb ();
	}
	public void StartClimb(){
		Debug.Log ("StartingClimb");
		playerScript.StartClimb ();
	}
}
