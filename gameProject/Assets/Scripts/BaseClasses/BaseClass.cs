using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseClass : MonoBehaviour {

	private string theName;

	private string className;
	private int strength;
	private int agility;
	private int wisdom;
	private int attackSpeed;

	private int armor;
	private int hitpoints;
	private int regeneration;

	private int currentHealth;
	private int maxHealth;
	private int currentMP;
	private int maxMP;

	public List<BaseAttack> attacks = new List<BaseAttack> ();

	public string TheName {
		get{ return theName;}
		set{ theName = value;}
	}

	public string ClassName {
		get{ return className;}
		set{ className = value;}
	}
		
	public int Strength {
		get{ return strength;}
		set{ strength = value;}
	}

	public int Agility {
		get{ return agility;}
		set{ agility = value;}
	}

	public int Wisdom {
		get{ return wisdom;}
		set{ wisdom = value;}
	}

	public int AttackSpeed {
		get{ return attackSpeed; }
		set{ attackSpeed = value; }
	}

	public int Armor {
		get{ return armor;}
		set{ armor = value;}
	}

	public int Hitpoints {
		get{ return hitpoints;}
		set{ hitpoints = value;}
	}

	public int Regeneration {
		get{ return regeneration;}
		set{ regeneration = value;}
	}

	public int CurrentHealth {
		get{ return currentHealth;}
		set{ currentHealth = value;}
	}

	public int MaxHealth {
		get{ return maxHealth;}
		set{ maxHealth = value;}
	}

	public int CurrentMP {
		get{ return currentMP;}
		set{ currentMP = value;}
	}

	public int MaxMP {
		get{ return maxMP;}
		set{ maxMP = value;}
	}
}
