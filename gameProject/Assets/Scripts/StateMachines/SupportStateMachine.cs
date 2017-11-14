using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportStateMachine : MonoBehaviour {

	private BattleStateMachine BSM;
	public BaseSupportPlayer supportPlayer;
	private EnemyStateMachine targetEnemy;

	public enum TurnState {
		PROCESSING,
		CHOOSEACTION,
		WAITING,
		ACTION,
		DEAD
	}

	public TurnState currentState;
	private float currentCooldown=0f;
	private float maxCooldown=3f;

	// this game object
	private Vector3 startPosition;
	public GameObject selector;

	//time for action stuff
	private bool actionStarted = false;
	public GameObject enemyToAttack;
	private float animSpeed = 10f;
	private float attackDistance = 1.5f;
	private HandleTurns myAttack;

	// Dead
	private bool alive;

	void Start() {
		supportPlayer = this.gameObject.GetComponent<BaseSupportPlayer>();
		currentCooldown = Random.Range (0.2f, 1.2f);
		selector.SetActive (false);
		currentState = TurnState.PROCESSING;
		BSM = GameObject.Find("BattleManager").GetComponent<BattleStateMachine>();
		startPosition = transform.position;
		alive = true;
	}

	void Update() {
		switch (currentState) {
		case(TurnState.PROCESSING):
			UpgradeProgressBar();
			break;
		case(TurnState.CHOOSEACTION):
			ChooseAction();
			currentState = TurnState.WAITING;
			break;
		case(TurnState.WAITING):
			break;
		case(TurnState.ACTION):
			if(myAttack.targetGameObject.tag != "DeadEnemy") {
				if (targetEnemy.currentState != EnemyStateMachine.TurnState.ACTION) {
					// if multiple enemies targeting one enemy, they'll wait so no more than one attack can hit the player at a time.
					// not 100% sure this will work if both enemies attack at the same time, but seems to be working so far.
					StartCoroutine (TimeForAction ());
				}
			}
			else {
				//target died
				if (BSM.enemiesInBattle.Count > 0)
					myAttack.targetGameObject = BSM.enemiesInBattle [Random.Range (0, BSM.enemiesInBattle.Count)];
				else
					Debug.Log ("WINNER");
			}
			break;
		case(TurnState.DEAD):
			if (!alive) {
				return; // dont do anything
			} else {
				// change tag of the player - can use to revive
				this.gameObject.tag = "DeadHero";
				// not attackable anymore
				BSM.herosInBattle.Remove(this.gameObject);

				// deactivate selector

				// if in perform list / done input > remove him
				// change this

				// change color / sprite / dead animation
				this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);
				// reset hero input
				BSM.heroInput = BattleStateMachine.HeroGUI.ACTIVATE;
				alive = false;
				BSM.battleStates = BattleStateMachine.PerformAction.CHECKALIVE;
			}
			break;
		}
	}

	void UpgradeProgressBar() {

		if(currentState == TurnState.PROCESSING) {
			currentCooldown = currentCooldown + Time.deltaTime;
			if (currentCooldown >= maxCooldown) {
				currentState = TurnState.CHOOSEACTION;
			}
		}
	}

	void ChooseAction() {
		myAttack = new HandleTurns();
		myAttack.attacker = this.gameObject.name;
		myAttack.type = "SupportPlayer";
		myAttack.attackerGameObject = this.gameObject;
		myAttack.targetGameObject = BSM.enemiesInBattle[Random.Range (0, BSM.enemiesInBattle.Count)];

		// Choosing attack
		int randAtk = Random.Range (0, supportPlayer.attacks.Count);
		myAttack.chosenAttack = supportPlayer.attacks[randAtk];
		targetEnemy = myAttack.targetGameObject.GetComponent<EnemyStateMachine>();
		BSM.CollectActions(myAttack);
	}

	 private IEnumerator TimeForAction() {
		if (actionStarted) {
			yield break;
		}
		actionStarted = true;

		// animate the enemy near the hero to attack
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

		// remove hero from being attacked list 
		BSM.enemiesBeingAttacked.Remove(enemyToAttack);

		if (BSM.battleStates != BattleStateMachine.PerformAction.WIN && BSM.battleStates != BattleStateMachine.PerformAction.LOSE) {
			// Reset BSM => Wait
			BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

			// Reset this enemy state
			//currentCooldown = 0f; // Enemy attack at the same time
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
		/*enemy.Strength*/
		float calc_damage = 5 + myAttack.chosenAttack.attackDamage;
		enemyToAttack.GetComponent<EnemyStateMachine>().TakeDamage(calc_damage);
	}

	public void TakeDamage(float damageAmount) {
		supportPlayer.CurrentHealth -= (int)damageAmount;
		if (supportPlayer.CurrentHealth <= 0) {
			supportPlayer.CurrentHealth = 0;
			Debug.Log ("Supp HP: " + supportPlayer.CurrentHealth);
			currentState = TurnState.DEAD;
		}
	}
}
