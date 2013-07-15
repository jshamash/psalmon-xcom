using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;

[System.Serializable]
public class SavedGameState	{
	
	
	#region constants
	public const double TIME_SLOW = 0.1;
	public const double TIME_FAST = 1.0;
	#endregion
	
	#region variables	
	//HERE WE WILL NEED TO STORE ALL VARIABLES THAT ARE IMPORTANT TO OUR GAME
	public int money;
	public Funding funding;
	public System.DateTime time;
	public List<string> pendingRequests = new List<string>();
	public int currentBase;
	public int availableId;
	
	public int missionsSuccess;
	public int missionsFailed;
	public int CiviliansKilled;
	public int CiviliansSaved;
	
	// Weapons that are available to buy/produce
	public List<Weapon> availableWeapons = new List<Weapon>(AllWeapons.initialWeapons);
	
	// Weapons that have yet to be researched
	public Queue<Weapon>[] unresearchedWeapons;
	/*=
	{
		new Queue<Weapon>(AllWeapons.shotgunUpgrades),
		new Queue<Weapon>(AllWeapons.armourUpgrades),
		new Queue<Weapon>(AllWeapons.rifleUpgrades),
		new Queue<Weapon>(AllWeapons.sniperUpgrades)
	};
	*/
	
	//List of names initiation in startState() 
	public string[] nameList;
	
	// Bases
	public List<Base> bases;
	
	
	//Important variable to know which level we are currently at
	public string activeLevel;
	
	#endregion
	
	
	public static gameManager instance;
	
	public SavedGameState () {
		money = gameManager.Instance.getMoney();
		funding = gameManager.Instance.getFunding();
		time = gameManager.Instance.getTime();
		pendingRequests = gameManager.Instance.getPendingRequests();
		currentBase = gameManager.Instance.getCurrentBaseNum();
		availableId = gameManager.Instance.getAvailableId();
		availableWeapons = gameManager.Instance.getAvailableWeapons();
		unresearchedWeapons = gameManager.Instance.getUnresearchedWeapons();
		nameList = gameManager.Instance.getNameList();
		// set random with new
		bases = gameManager.Instance.getBases();
		activeLevel = gameManager.Instance.getLevel();
		missionsSuccess = gameManager.Instance.getMissionSuccess();
		missionsFailed = gameManager.Instance.getMissionFailed();
		CiviliansKilled = gameManager.Instance.CiviliansKilled;
		CiviliansSaved = gameManager.Instance.CiviliansSaved;
	}
}

