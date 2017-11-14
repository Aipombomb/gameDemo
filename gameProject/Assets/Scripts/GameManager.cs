using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static GameManager instance;

	// Class Random Monster
	[System.Serializable]
	public class RegionData
	{
		public string regionName;
		public int maxAmountEnemies = 4;
		public string battleScene;
		public List <GameObject> possibleEnemies = new List<GameObject> ();
	}

	// SCENES
	public string sceneToLoad;
	public string lobbyScene; // maybe map scene

	// BATTLE
	public List<GameObject> enemiesToBattle = new List<GameObject>();
	public int enemyAmount;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void StartBattle() {
		// Amount of enemies
		enemyAmount = Random.Range(1, 4);
		// Which enemies
		for (int i = 0; i < enemyAmount; i++) {
			//enemiesToBattle.Add();
		}
	}
}
