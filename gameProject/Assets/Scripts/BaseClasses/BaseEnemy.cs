using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseEnemy : BaseClass, IEnemy {

	public int Experience { get; set; }
	public int ID { get; set; }

	public enum TypeElement
	{
		FIRE,
		WATER,
		WIND,
		EARTH,
		ELECTRIC
	}

	public TypeElement enemyTypeElement;

	public virtual void setAttacks() {

	}

	public void Die() {

	}


}
