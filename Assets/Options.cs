
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Options : MonoBehaviour {
	public Slider mapSize;
	public InputField playersName;
	public Slider seed;
	[SerializeField]
	private MainMenu mainMenu;
	void Start(){
		mapSize.value = PlayerPrefs.GetInt ("mapSize", 100);
		playersName.text = PlayerPrefs.GetString ("name", "Astrodude");
		seed.value = PlayerPrefs.GetInt ("seed", 500);
	}
	public void SaveConfigurations(){
		//Establecimiento de nombre
		string name;
		if (playersName.text == "")
			name = "Astrodude";
		else
			name = playersName.text;
		PlayerPrefs.SetString ("name", name);
		//Establecimiento de tamaño de mapa
		PlayerPrefs.SetInt("mapSize",(int)mapSize.value);
		//Establecimiento de semilla de mapa
		PlayerPrefs.SetInt("seed",(int)seed.value);
		mainMenu.FadeToScene ("MainMenu");
	}
}
