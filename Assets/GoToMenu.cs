using UnityEngine;
using UnityEngine.SceneManagement;
public class GoToMenu : MonoBehaviour {
	public void GoToMainMenu(){
		SceneManager.LoadScene("MainMenu");
	}
}
