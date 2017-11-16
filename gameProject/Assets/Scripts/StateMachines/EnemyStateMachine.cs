using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStateMachine : MonoBehaviour {

	private BattleStateMachine BSM;
	public BaseEnemy enemy;

	public enum TurnState {
		PROCESSING,
		CHOOSEACTION,
		WAITING,
		ACTION,
		DEAD
	}

	public TurnState currentState;
	private float currentCooldown=0f;
	private float maxCooldown=0.1f;

	// this game object
	private Vector3 startPosition;
	public GameObject selector;

	//time for action stuff
	private bool actionStarted = false;
	public GameObject heroToAttack;
	private float animSpeed = 10f;
	private float attackDistance = 1.5f;
	private HandleTurns myAttack;

	// Dead
	private bool alive;

	void Start() {
		enemy = this.gameObject.GetComponent<BaseEnemy>();
		currentCooldown = Random.Range (0f, 0.2f);
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
			if (BSM.HerosToManage.Count == 0) {
				ChooseAction ();
				currentState = TurnState.WAITING;
			}
			break;
		case(TurnState.WAITING):
			break;
		case(TurnState.ACTION):
			StartCoroutine (TimeForAction ());
			break;
		case(TurnState.DEAD):
			if (!alive) {
				return; // dont do anything
			} else {
				// change tag of the player - can use to revive
				this.gameObject.tag = "DeadEnemy";
				// not attackable anymore
				BSM.enemiesInBattle.Remove(this.gameObject);
				// deactivate selector
				selector.SetActive(false);

				//remove enemy from hero's target if it was being targeted
				if (BSM.enemiesInBattle.Count > 0) {
					for (int i = 0; i < BSM.performList.Count; i++) {
						if (BSM.performList [i].attackersTarget == this.gameObject) {
							int rand = Random.Range (0, BSM.enemiesInBattle.Count);
							BSM.performList [i].attackersTarget = BSM.enemiesInBattle[rand];
							BSM.HeroChoice.attackersTarget = BSM.enemiesInBattle[rand];
							BSM.HeroChoice.attackerGameObject.GetComponent<HeroStateMachine>().selectedAttackerTarget = 
								BSM.HeroChoice.attackersTarget;
						}
						if (BSM.performList [i].attackerGameObject == this.gameObject) {
							BSM.performList.Remove (BSM.performList [i]);
						}
					}
				}
					
				// change color / sprite / dead animation
				this.gameObject.GetComponent<MeshRenderer>().material.color = new Color32(105,105,105,255);

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
		myAttack.type = "Enemy";
		myAttack.attackerGameObject = this.gameObject;
		myAttack.attackersTarget = BSM.herosInBattle[Random.Range (0, BSM.herosInBattle.Count)];

		// choosing attack
		int randAtk = Random.Range (0, enemy.attacks.Count);
		myAttack.chosenAttack = enemy.attacks[randAtk];
		BSM.CollectActions(myAttack);
	}

	 private IEnumerator TimeForAction() {
		if (actionStarted) {
			yield break;
		}
		actionStarted = true;

		// animate the enemy near the hero to attack
		Vector3 heroPosition = new Vector3(heroToAttack.transform.position.x - attackDistance, 
			this.transform.position.y, heroToAttack.transform.position.z);
		while (MoveTowardsEnemy (heroPosition)) {
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
			//BSM.battleStates = BattleStateMachine.PerformAction.WAIT;

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
		heroToAttack.GetComponent<HeroStateMachine> ().TakeDamage (calc_damage);
	}

	public void TakeDamage(float damageAmount) {
		enemy.CurrentHealth -= (int)damageAmount;
		if (enemy.CurrentHealth <= 0) {
			enemy.CurrentHealth = 0;
			Debug.Log (enemy.name + " HP: " + enemy.CurrentHealth);
			currentState = TurnState.DEAD;
		}
	}
}
