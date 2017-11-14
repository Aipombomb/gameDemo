using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseThiefClass : BaseHero {
// change to name of character

	public BaseThiefClass() {
		PlayerName = "Aipombomb";
		ClassName = "Thief";
		Strength = 9;
		Wisdom = 11;
		Agility = 20;
		AttackSpeed = 45;
		Hitpoints = 90;
		Armor = 5;
		Regeneration = 0;
		MaxHealth = 70;
		CurrentHealth = MaxHealth;
		MaxMP = 15;
		CurrentMP = MaxMP;
	}
}
