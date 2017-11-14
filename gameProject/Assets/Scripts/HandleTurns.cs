using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class HandleTurns {

	public string attacker; // Name of attacker
	public string type; //
	public GameObject attackerGameObject; // who was performing the attack
	public GameObject targetGameObject; // who the attacker is targeting

	// which attack is performed
	public BaseAttack chosenAttack;
}
