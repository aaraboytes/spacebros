using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour {
	public Animator anim;
	public string scene;
	public void FadeToScene(string scene){
		anim.SetTrigger ("FadeOut");
		this.scene = scene;
	}
	public void ExitGame(){
		anim.SetTrigger ("FadeOut");
		Application.Quit ();
	}
	public void OnFadeComplete(){
		SceneManager.LoadScene (scene);
	}
}
