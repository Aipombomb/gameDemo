using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroStateMachine : MonoBehaviour {

	public enum TurnState {
		PROCESSING,
		ADDTOLIST,
		WAITING,
		SELECTING,
		ACTION,
		DEAD
	}

	private BattleStateMachine BSM;
	public BaseHero hero;
	public EnemyStateMachine targetEnemy;
	public GameObject selectedAttackerTarget;

	// Hero panel
	private BattleScreenPlayerStats BSPlayerStats;
	public GameObject heroPanel;

	public TurnState currentState;
	private float currentCooldown=0f;  // cooldown
	private float maxCooldown=3f;
	public GameObject selector;
	private HandleTurns thisPlayerHandleTurn;

	//IENumerator
	public GameObject enemyToAttack;
	private bool actionStarted = false;
	private Vector3 startPosition;
	private float animSpeed = 10f; // animation to run
	private float attackDistance = 1.5f;
	private HandleTurns myAttack;

	// Dead
	private bool alive;

	void Start() {
		hero = this.gameObject.GetComponent<BaseHero>();
		// Create panel, fill in info
		CreateHeroPanel();
		// may be useful with luck / speed stat
		//currentCooldown = Random.Range(0f, 2.5f);
		selector.SetActive (false);
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
		currentState = TurnState.PROCESSING;
		startPosition = transform.position;
		alive = true;
		selectedAttackerTarget = BSM.enemiesInBattle [0];
		BSM.enemiesInBattle[0].transform.Find("Selector").gameObject.SetActive(true); // sets selector to first enemy by default
	}

	void Update() {

		if (Input.GetMouseButtonDown (0)) {
			SelectCharacter ();
		}

		switch (currentState) {
		case(TurnState.PROCESSING):
			UpgradeProgressBar();
			break;
		case(TurnState.ADDTOLIST):
			BSM.HerosToManage.Add(this.gameObject);
			currentState = TurnState.WAITING;
			break;
		case(TurnState.WAITING):
			break;
		case(TurnState.SELECTING):
			break;
		case(TurnState.ACTION):
			// if multiple enemies targeting one enemy, they'll wait so no more than one attack can hit the player at a time.
			// not 100% sure this will work if both enemies attack at the same time, but seems to be working so far.
			StartCoroutine (TimeForAction ());				
			break;
		case(TurnState.DEAD):
			if (!alive) {
				return; // dont do anything
			} else {
				// change tag of the player - can use to revive
				this.gameObject.tag = "DeadHero";
				// not attackable anymore
				BSM.herosInBattle.Remove(this.gameObject);
				// not managable
				BSM.HerosToManage.Remove(this.gameObject);
				// deactive attack options / maybe enable button to revive?
				BSM.actionPanel.SetActive(false);
				// deactivate selector

				//remove enemy from hero's target if it was being targeted
				if (BSM.herosInBattle.Count > 0) {
					for (int i = 0; i < BSM.performList.Count; i++) {
						if (BSM.performList [i].attackersTarget == this.gameObject) {
							BSM.performList [i].attackersTarget = BSM.herosInBattle [Random.Range (0, BSM.herosInBattle.Count)];
						}
						if (BSM.performList [i].attackerGameObject == this.gameObject) {
							BSM.performList.Remove (BSM.performList [i]);
						}
					}
				}
				// change color / sprite / dead animation
				//this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);
				Debug.Log("HERO IS DEAD");
				alive = false;
				BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
			}
			break;
		}
	}

	private void SelectCharacter() {
		Ray interactionRay = Camera.main.ScreenPointToRay(Input.mousePosition);
		RaycastHit interactionInfo;
		if (Physics.Raycast (interactionRay, out interactionInfo, Mathf.Infinity)) {
			GameObject interactedObject = interactionInfo.collider.gameObject;
			if (interactedObject.tag == "Enemy") {
				selectedAttackerTarget.GetComponent<EnemyStateMachine> ().selector.SetActive (false);				
				selectedAttackerTarget = interactedObject;
				selectedAttackerTarget.GetComponent<EnemyStateMachine> ().selector.SetActive (true);
				Debug.Log ("Selected: " + interactedObject.name);
			} else if (interactedObject.tag == "Player") {
				// fix later for healing select
				selectedAttackerTarget.GetComponent<HeroStateMachine> ().selector.SetActive (false);
				selectedAttackerTarget = interactedObject;
				selectedAttackerTarget.GetComponent<HeroStateMachine> ().selector.SetActive (true);
				Debug.Log ("Selected: " + interactedObject.name);
			} else {
				Debug.Log ("Didn't select character. Selected: " + interactedObject.name);
			}
		}
	}

	void UpgradeProgressBar() {
		currentState = TurnState.ADDTOLIST;
	}

	private IEnumerator TimeForAction() {
		if (actionStarted) {
			yield break;
		}
		actionStarted = true;
		// animate the hero near the enemy to attack
		Vector3 enemyPosition = new Vector3(enemyToAttack.transform.position.x + attackDistance, 
			this.transform.position.y, enemyToAttack.transform.position.z);
		while (MoveTowardsEnemy (enemyPosition)) {
			yield return null;
		}
		// wait a bit
		yield return new WaitForSeconds(0.5f);

		// do damage
		DoDamage();

		// animate back to position
		Vector3 firstPosition = startPosition;
		while (MoveTowardsStart (firstPosition)) {
			yield return null;
		}

		// remove performer from list
		BSM.performList.RemoveAt(0);

		if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE) {
			// Reset BSM => Wait		
			currentCooldown = Random.Range (0f, 1f); // Can start as much as one second faster > different random attack times (need better solution)
			currentState = TurnState.PROCESSING;
		} else {
			currentState = TurnState.WAITING;
		}
		// end coroutine
		actionStarted = false;
	}

	private bool MoveTowardsEnemy(Vector3 target) {
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	private bool MoveTowardsStart(Vector3 target) {
		return target != (transform.position = Vector3.MoveTowards(transform.position, target, animSpeed * Time.deltaTime));
	}

	void DoDamage() {
		//float calc_damage = 5 + myAttack.chosenAttack.attackDamage;
		float calc_damage = hero.Strength;
		enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
	}

	public void TakeDamage(float damageAmount) {
		hero.CurrentHealth -= (int) damageAmount; // not sure if it should cast int
		//Debug.Log ("Hero HP: " + hero.CurrentHealth);
		if (hero.CurrentHealth <= 0) {
			hero.CurrentHealth = 0;
			currentState = TurnState.DEAD;
		}
		UpdateHeroPanel();
	}

	void CreateHeroPanel() {
		heroPanel = GameObject.Find("TopInfoPanel");
		BSPlayerStats = heroPanel.GetComponent<BattleScreenPlayerStats>();
		BSPlayerStats.PlayerName.text = hero.PlayerName;
		BSPlayerStats.PlayerHP.text = "HP: " + hero.CurrentHealth;
		BSPlayerStats.PlayerMP.text = "MP: " + hero.CurrentMP;
	}

	// Update stats on damage / heal
	void UpdateHeroPanel() {
		BSPlayerStats.PlayerHP.text = "HP: " + hero.CurrentHealth;
		BSPlayerStats.PlayerMP.text = "MP: " + hero.CurrentMP;
	}
}
