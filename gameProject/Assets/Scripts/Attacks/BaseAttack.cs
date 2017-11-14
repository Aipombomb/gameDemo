using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseAttack : MonoBehaviour {

	public string attackName; // name of current performed attack
	public string description; // description of the weapon
	public float attackDamage; // base damage
	public float attackCost; // Mana
}
