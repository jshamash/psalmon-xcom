using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Aircraft
{

	public static readonly int MAX_SOLDIERS = 8;
	public static readonly int PRICE = 50000;
	
	private string pilot = null;
	private List<Soldier> soldiers = new List<Soldier>();
	private List<Weapon> weapons = new List<Weapon>();
	
	public void addSoldier(Soldier s) {
		if (soldiers.Count < MAX_SOLDIERS && !soldiers.Contains (s))
			soldiers.Add(s);
		else
			Debug.Log("Problem adding soldier to aircraft");
	}
	
	public void addPilot(string p) {
		if (pilot == null)
			pilot = p;
		else
			Debug.Log("Problem adding pilot to aircraft -- already has pilot");
	}
	
	public void removeSoldier(Soldier s) {
		if (soldiers.Contains (s))
			soldiers.Remove(s);
		else
			Debug.Log("Problem removing soldier from aircraft: soldier not in aircraft");
	}
	
	// NOTE: what if pilot's name is changed then we try to remove it?
	public void removePilot() {
		if (pilot != null)
			pilot = null;
		else
			Debug.Log("Problem removing pilot from aircraft: no pilot in aircraft");
	}
	
	public string getPilot() {
		return pilot;
	}
	
	public bool contains(Soldier s) {
		return soldiers.Contains (s);
	}
	
	public bool contains(string p) {
		return pilot == p;
	}
	
	public bool hasPilot() {
		return pilot != null;
	}
	
	public int numSoldiers() {
		return soldiers.Count;
	}
	
	//Return the list of soldiers
	public List<Soldier> getSoldiers(){
		return soldiers;	
	}
	
	//Return the list of weapons
	public List<Weapon> getWeapons(){
		return weapons;	
	}
	
	//Add a weapon/armor to the weapons list
	public void addWeapon(Weapon weapon){
		weapons.Add(weapon);
	}
	
	//Remove a weapon from the weapons list, based on a given string.
	//@ return weapon removed from list
	public Weapon removeWeapon(string name){
		Debug.Log("Size of list: "+weapons.Count);
		Debug.Log("Object name to find: "+name);
		
		//First find weapon with same name
		Weapon tempWeapon=null;// = weapons.Find(mc=>mc.getName ().Equals(name));
		Weapon [] temp =new Weapon[weapons.Count];
		weapons.CopyTo (temp,0);
		List<Weapon> newWeaponList = new List<Weapon>();
		bool foundOne=false;
		for(int i =0;i<weapons.Count;i++){
			if(!foundOne){
				if(temp[i].getName().Equals(name)){
					tempWeapon = temp[i];
					//temp[i] = null;
					foundOne=true;
				}
			}
			weapons = newWeaponList;
			Debug.Log("Name of weapon/armor: "+temp[i].getName ());
		}
		
		//Create an instance of the weapon on soldier
		/*Weapon returnWeapon = new Weapon(tempWeapon.getName (), tempWeapon.getImage(), tempWeapon.isArmourType(),
					tempWeapon.getCost(), tempWeapon.getStorageSpace(), tempWeapon.getDamage(), tempWeapon.getRange(), tempWeapon.getMaxAmmo(), tempWeapon.getActionPoints(),
					tempWeapon.getAngle(), tempWeapon.getProtection());*/
		
		//Remove weapon
 		/*weapons.Remove(tempWeapon);*/
		
		if(tempWeapon==null){
			Debug.Log("problem here, weapon not assigned");
		}
		
		//Return copy instance of weapon
		return tempWeapon;
	}
}

