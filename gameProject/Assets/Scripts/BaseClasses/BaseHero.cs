using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseHero : BaseClass {

	private string playerName;
	private int playerLevel;
	private string playerElement;
	private BaseClass playerClass;
	private string playerClassName;

	public string PlayerName {
		get{ return playerName; }
		set{ playerName = value; }
	}

	public int PlayerLevel {
		get{ return playerLevel; }
		set{ playerLevel = value; }
	}

	public string PlayerElement {
		get{ return playerElement; }
		set{ playerElement = value; }
	}

	public BaseClass PlayerClass {
		get{ return playerClass; }
		set{ playerClass = value; }
	}

	public string PlayerClassName {
		get{ return playerClassName; }
		set{ playerClassName = value; }
	}
}
