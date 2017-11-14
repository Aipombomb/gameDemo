using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Supporto : BaseSupportPlayer {

	public Supporto() {
		ClassName = "Support";
		Strength = 9;
		Wisdom = 13;
		Agility = 12;
		AttackSpeed = 30;
		Hitpoints = 90;
		Armor = 5;
		Regeneration = 0;
		MaxHealth = 50;
		CurrentHealth = MaxHealth;
		MaxMP = 15;
		CurrentMP = MaxMP;
	}

	void Start() {
		setAttacks();
	}

	public override void setAttacks() {
		attacks.Add(Resources.Load<BaseAttack>("AttackAbilities/MeleeAbility/SlashAbility"));
	}
}
