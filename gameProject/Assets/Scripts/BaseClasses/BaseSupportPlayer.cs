using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BaseSupportPlayer : BaseClass {

	public enum TypeElement
	{
		FIRE,
		WATER,
		WIND,
		EARTH,
		ELECTRIC
	}

	public TypeElement supportPlayerTypeElement;

	public virtual void setAttacks() {

	}
}
