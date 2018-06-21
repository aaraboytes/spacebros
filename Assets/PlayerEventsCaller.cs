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
}
