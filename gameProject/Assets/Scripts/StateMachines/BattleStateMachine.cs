using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BattleStateMachine : MonoBehaviour {
	// Handles the turn based battle

	public enum PerformAction
	{
		WAIT,
		TAKEACTION,
		PERFORMACTION,
		CHECKALIVE,
		WIN,
		LOSE
	}

	public enum HeroGUI
	{
		ACTIVATE,
		WAITING,
		INPUT1,
		INPUT2,
		DONE
	}

	public PerformAction battleStates;
	public HeroGUI heroInput;
	public HeroStateMachine playerObject;

	public List<HandleTurns> performList = new List<HandleTurns>();
	public List<GameObject> herosInBattle = new List<GameObject>();
	public List<GameObject> herosBeingAttacked = new List<GameObject>();
	public List<GameObject> enemiesInBattle = new List<GameObject>();
	public List<GameObject> enemiesBeingAttacked = new List<GameObject>();

	public List<GameObject> HerosToManage = new List<GameObject>();
	public HandleTurns HeroChoice;
	//public GameObject EnemySelectPanel;
	public Button basicAttackButton;

	// Spawn points
	public List<Transform> spawnPoints = new List<Transform>();

	void Awake () {
		/*
		for(int i = 0; i < GameManager.instance.enemyAmount; i++) {
			GameObject newEnemy = Instantiate(GameManager.instance.enemiesToBattle[i], spawnPoints[i].position, Quarternion.identity) as GameObject;
			//newEnemy.name = newEnemy.GetComponent<EnemyStateMachine>().enemy; // prob don't need
			enemiesInBattle.Add(newEnemy); 
		}
		*/
		battleStates = PerformAction.WAIT;
		herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("Player"));
		herosInBattle.AddRange(GameObject.FindGameObjectsWithTag("SupportPlayer"));
		enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // added in awake, remove after
		heroInput = HeroGUI.ACTIVATE;
		basicAttackButton.interactable = false;
	}
	
	void Update () {
		switch (battleStates) 
		{
		case(PerformAction.WAIT):
			if(performList.Count > 0) {
				battleStates = PerformAction.TAKEACTION;
			}
			break;
		case(PerformAction.TAKEACTION):
			if (performList.Count > 0) {
				GameObject performer = GameObject.Find (performList [0].attacker);
				if (performer.tag == "DeadEnemy" || performer.tag == "DeadHero") {
					Debug.Log("Found dead player");
					performList.Remove(performList[0]);
				} else if (performList [0].type == "Enemy") {
					EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine> ();
					if (herosBeingAttacked.Contains(performList[0].targetGameObject)) {
						//Debug.Log("Hero being attacked by " + performList[0].attacker);
						performList.Insert (performList.Count-1, performList [0]); // move to end of the list
						performList.Remove(performList[0]);
					} else {
						ESM.heroToAttack = performList [0].targetGameObject;
						herosBeingAttacked.Add(ESM.heroToAttack);
						ESM.currentState = EnemyStateMachine.TurnState.ACTION;
						performList.RemoveAt (0);
					}
				} else if (performList [0].type == "Hero") {
					HeroStateMachine HSM = performer.GetComponent<HeroStateMachine> ();
					HSM.enemyToAttack = performList [0].targetGameObject;
					enemiesBeingAttacked.Add (HSM.enemyToAttack);
					HSM.currentState = HeroStateMachine.TurnState.ACTION;
					performList.RemoveAt (0);
				} else if (performList [0].type == "SupportPlayer") {
					SupportStateMachine SSM = performer.GetComponent<SupportStateMachine> ();
					if (enemiesBeingAttacked.Contains (performList [0].targetGameObject)) {
						performList.Insert (performList.Count-1, performList [0]); // move to end of the list
						performList.Remove(performList[0]);
					} else {
						SSM.enemyToAttack = performList [0].targetGameObject;
						enemiesBeingAttacked.Add (SSM.enemyToAttack);
						SSM.currentState = SupportStateMachine.TurnState.ACTION;
						performList.RemoveAt (0);
					}
				}
			} 
			//else {
				//battleStates = PerformAction.PERFORMACTION; // Makes it so action must finish before another player can perform an action (wait)
			//}
			break;
		case(PerformAction.PERFORMACTION):
			break;

		case(PerformAction.CHECKALIVE):
			if (herosInBattle.Count < 1) {
				battleStates = PerformAction.LOSE;
			} else if (enemiesInBattle.Count < 1) {
				battleStates = PerformAction.WIN;
			} else {
				heroInput = HeroGUI.ACTIVATE;
			}
			break;
		case(PerformAction.WIN):
			Debug.Log ("WINNER WINNER WINNER");
			for(int i = 0; i < herosInBattle.Count; i++) {
				if(herosInBattle[i].tag == "Player")
					herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
				else
					herosInBattle[i].GetComponent<SupportStateMachine>().currentState = SupportStateMachine.TurnState.WAITING;
			}
			break;

		case(PerformAction.LOSE):
			Debug.Log ("YOU LOST GAME OVER");
			for(int i = 0; i < enemiesInBattle.Count; i++) {
				enemiesInBattle[i].GetComponent<EnemyStateMachine>().currentState = EnemyStateMachine.TurnState.WAITING;
			}
			break;

		}


		switch (heroInput) {
		case(HeroGUI.ACTIVATE):
			if (HerosToManage.Count > 0) {
				HeroChoice = new HandleTurns();
				basicAttackButton.interactable = true;
				heroInput = HeroGUI.WAITING;
			}
			break;
		case(HeroGUI.WAITING):
			if(HerosToManage[0].GetComponent<HeroStateMachine>().selectedTargetCharacter == null) {
				Debug.Log("NULLED");
				HerosToManage[0].GetComponent<HeroStateMachine>().selectedTargetCharacter = enemiesInBattle[0]; // selects first enemy by default
			}
			ChooseEnemyInput(HerosToManage[0].GetComponent<HeroStateMachine>().selectedTargetCharacter);
			break;
		case(HeroGUI.INPUT1):
			break;
		case(HeroGUI.INPUT2):
		break;
		case(HeroGUI.DONE):
			HeroInputDone();
			break;
		}
	}

	public void CollectActions(HandleTurns inputTurn) {
		performList.Add(inputTurn);
	}

	public void Input1() {
		// basic attack
		HeroChoice.attacker = HerosToManage[0].name;
		HeroChoice.attackerGameObject = HerosToManage[0];
		HeroChoice.type = "Hero";
		basicAttackButton.interactable = false;
		playerObject = HerosToManage [0].GetComponent<HeroStateMachine>();
		playerObject.targetEnemy = HeroChoice.targetGameObject.GetComponent<EnemyStateMachine>(); // change to selected enemy
		heroInput = HeroGUI.DONE;
	}

	public void ChooseEnemyInput(GameObject choosenEnemy) {
		HeroChoice.targetGameObject = choosenEnemy;
		//Debug.Log ("Hero choice: " + HeroChoice.targetGameObject.name);
	}

	void HeroInputDone() {
		performList.Add(HeroChoice);
		HerosToManage.RemoveAt(0);
		heroInput = HeroGUI.ACTIVATE;
	}
}
