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
	public List<GameObject> enemiesInBattle = new List<GameObject>();

	public List<GameObject> HerosToManage = new List<GameObject>();
	public HandleTurns HeroChoice;

	//public GameObject selectedTargetCharacter; // Selected enemy

	public GameObject actionPanel;
	public Transform ActionPanelSpacer;
	public GameObject actionButton;
	public List<GameObject> atkBtns = new List<GameObject> ();
	// Spawn points
	public List<Transform> spawnPoints = new List<Transform>();

	//
	private int totalBattleCount;
	private bool roundOpen;

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
		enemiesInBattle.AddRange(GameObject.FindGameObjectsWithTag("Enemy")); // added in awake, remove after
		heroInput = HeroGUI.ACTIVATE;
		totalBattleCount = herosInBattle.Count + enemiesInBattle.Count;
		Debug.Log("Total: " + totalBattleCount);
		roundOpen = true;
	}
	
	void Update () {
		switch (battleStates) 
		{
		case(PerformAction.WAIT):
			// can also check if HerosToManage.Count == 0
			if(performList.Count == totalBattleCount) {
				roundOpen = false;
				battleStates = PerformAction.TAKEACTION;
			}
			break;
		case(PerformAction.TAKEACTION):
			if (performList.Count > 0) {
				GameObject performer = GameObject.Find (performList [0].attacker);
				if (performList [0].type == "Enemy") {
					EnemyStateMachine ESM = performer.GetComponent<EnemyStateMachine> ();
					for(int i = 0; i < herosInBattle.Count; i++) {
						if (performList [0].attackersTarget == herosInBattle [i]) {
							ESM.heroToAttack = performList [0].attackersTarget;
							ESM.currentState = EnemyStateMachine.TurnState.ACTION;
							break;
						} 
						else { // old target is dead
							performList[0].attackersTarget = herosInBattle[Random.Range(0, herosInBattle.Count)];
							ESM.heroToAttack = performList [0].attackersTarget;
							ESM.currentState = EnemyStateMachine.TurnState.ACTION;
						}
					}
				}
				else if (performList [0].type == "Hero") {
					HeroStateMachine HSM = performer.GetComponent<HeroStateMachine> ();
					HSM.enemyToAttack = performList [0].attackersTarget;
					HSM.currentState = HeroStateMachine.TurnState.ACTION;
				}
			}
			else {
				roundOpen = true;
				heroInput = HeroGUI.ACTIVATE;
				battleStates = PerformAction.WAIT; // Makes it so action must finish before another player can perform an action (wait)
			}
			break;
		case(PerformAction.PERFORMACTION):
			break;

		case(PerformAction.CHECKALIVE):
			if (herosInBattle.Count < 1) {
				battleStates = PerformAction.LOSE;
			} else if (enemiesInBattle.Count < 1) {
				battleStates = PerformAction.WIN;
			} else {
				ClearAttackPanel();
				totalBattleCount = herosInBattle.Count + enemiesInBattle.Count; // re-evaluate total battle count
				Debug.Log("TOTAL: + " + totalBattleCount.ToString());
				battleStates  = PerformAction.TAKEACTION;
			}
			break;
		case(PerformAction.WIN):
			Debug.Log ("WINNER WINNER WINNER");
			for(int i = 0; i < herosInBattle.Count; i++) {
				herosInBattle[i].GetComponent<HeroStateMachine>().currentState = HeroStateMachine.TurnState.WAITING;
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
			if (HerosToManage.Count > 0 && roundOpen) {
				HeroChoice = new HandleTurns();
				actionPanel.SetActive (true);
				CreateAttackButtons();
				heroInput = HeroGUI.WAITING;
			}
			break;
		case(HeroGUI.WAITING):
			
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

		// update to pick correct attack
		//HeroChoice.chosenAttack = HerosToManage [0].GetComponent<HeroStateMachine>().hero.attacks[0];
		ChooseEnemyInput(HerosToManage[0].GetComponent<HeroStateMachine>().selectedAttackerTarget);
		actionPanel.SetActive(false);
		heroInput = HeroGUI.DONE;
	}

	public void ChooseEnemyInput(GameObject choosenEnemy) {
		if (choosenEnemy == null || choosenEnemy.tag == "DeadEnemy") {
			Debug.Log ("NULLED");
			choosenEnemy = enemiesInBattle[0];
		}
		HeroChoice.attackersTarget = choosenEnemy;
		HeroChoice.attackersTarget.GetComponent<EnemyStateMachine>().selector.SetActive(true);
	}

	void HeroInputDone() {
		performList.Add(HeroChoice);
		// clean the attack panel
		ClearAttackPanel ();
		HerosToManage.RemoveAt(0);
		HeroChoice.attackersTarget.GetComponent<EnemyStateMachine>().selector.SetActive(false);
		heroInput = HeroGUI.ACTIVATE;
	}

	void ClearAttackPanel() {
		actionPanel.SetActive(false);
		Debug.Log ("Clear");
		foreach (GameObject atkBtn in atkBtns) {
			Destroy (atkBtn);
		}
		atkBtns.Clear();
	}

	void CreateAttackButtons() {
		Debug.Log ("Create");
		GameObject AttackButton = Instantiate (actionButton) as GameObject;
		Text AttackButtonText = AttackButton.transform.Find ("ActionButtonText").gameObject.GetComponent<Text>();
		AttackButtonText.text = "AttackName";
		AttackButton.GetComponent<Button>().onClick.AddListener( () => Input1() );
		AttackButton.transform.SetParent(ActionPanelSpacer, false);
		atkBtns.Add(AttackButton);
		ChooseEnemyInput(HerosToManage[0].GetComponent<HeroStateMachine>().selectedAttackerTarget);
	}
}
