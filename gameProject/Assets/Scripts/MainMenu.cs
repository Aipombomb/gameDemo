using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

	public string levelToLoad = "BattleScene";
		
	public void Play () {
		SceneManager.LoadScene(levelToLoad);
	}

	// Other method names
	/*
	public void () {

	}*/
}
