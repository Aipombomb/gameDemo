using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonDog : BaseEnemy {

	public DemonDog() {
		ClassName = "Demon";
		Strength = 9;
		Wisdom = 13;
		Agility = 12;
		AttackSpeed = 30;
		Hitpoints = 90;
		Armor = 5;
		Regeneration = 0;
		MaxHealth = 30;
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
