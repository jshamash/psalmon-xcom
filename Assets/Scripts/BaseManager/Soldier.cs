using UnityEngine;
using System.Collections;

[System.Serializable]
public class Soldier {
	
	//All variables relevant to a soldier
	private int id;
	private string name;
	private int health;
	private int accuracy;
	private int mind;
	private int energy;
	private int snipePower;
	private int assaultPower;
	private int closeCombatPower;
	private int alienKilled;
	private int missionsPerformed;
	private int rank;
	private Weapon weapon;
	private Weapon armor;
	private int ammoLeft;
	private int weaponRange;
	
	//Soldier ranks
	private enum Rank {Private, Corporal, Sergeant, Lieutnant, General};
	
	private string[] ranks = {"Private","Corporal", "Sergeant", "Lieutnant", "General"};
	
	//Soldier Constructor
	public Soldier(int setId, string setName){
		id = setId;
		name = setName;
		alienKilled=0;
		missionsPerformed = 0;
		health=100;
		accuracy = 0;
		energy=100;
		mind = 5;
		snipePower=Random.Range (5,10);
		assaultPower = Random.Range (5,10);
		closeCombatPower = Random.Range (5,10);
		rank = 0;
		weapon = null;
		armor = null;
		ammoLeft = 0;
		InHospital = false;
		weaponRange = 0;
	}
	
	public bool InHospital { get; set; }
	
	//Obtain rank of soldier
	public int getRank(){
		return rank;	
	}
	
	public string getRankString(){
		return ranks[getRank()];
	}
	
	//Set the rank of soldier
	public void setRank(int newRank){
		rank=newRank;	
	}
	
	//Update new value of health
	public void updateHealth(int newHealth){
		health = newHealth;
	}
	
	//Update value of accuracy to the new value
	public void updateAccuracy(int newAccuracy){
		accuracy = newAccuracy;	
	}
	
	//Adds "adder" to the # of aliens the soldier has slain in battle
	public void addAlienKilled(int adder){
		alienKilled = alienKilled+adder;
	}
	
	//Returns the name of the soldier
	public string getName(){
		return name;	
	}
	
	//Returns the id of the soldier
	public int getId(){
		return id;	
	}
	
	public int getHealth(){
		return health;
	}
	
	//Decrease health of a soldier
	public void decreaseHealth(int ammount){
		health-= ammount;
		if(health<0)
			health=0;
	}
	
	public int getAccuracy(){
		return accuracy;	
	}
	
	public int getMind(){
		return mind;
	}
	
	public int getSnipePower(){
		return snipePower;	
	}
	
	public int getCloseCombatPower(){
		return closeCombatPower;
	}
	
	public int getAssaultPower(){
		return assaultPower;	
	}
		
	public void changeName(string newName){
		name = newName;	
	}
	
	public int getAliens(){
		return alienKilled;	
	}
	
	public int getMissions(){
		return missionsPerformed;	
	}
	
	public int getEnergy(){
		return energy;
	}
	
	public void setEnergy(int newEnergy){
		energy=newEnergy;
	}
	
	public override bool Equals (object o)
	{
		return this.Equals (o as Soldier);
	}
	
	public void setWeapon(Weapon newWeapon){
		weapon = newWeapon;
	}
	
	public Weapon getWeapon(){
		return weapon;
	}
	
	//Setters and getters for ammoLeft
	public void setAmmoLeft(int newAmmo){
		ammoLeft = newAmmo;
	}
	
	public int getAmmoLeft(){
		return ammoLeft;
	}
	
	//Setters and getters for armor
	public void setArmor(Weapon newArmor){
		armor=newArmor;
	}
	
	public Weapon getArmor(){
		return armor;	
	}
	
	public int getWeaponRange(){
		return weaponRange;
	}
	
	public void setWeaponRange(int newRange){
		weaponRange = newRange;
	}
	
	public bool Equals(Soldier s)
	{
		// If parameter is null, return false. 
        if (Object.ReferenceEquals(s, null))
        {
            return false;
        }

        // Optimization for a common success case. 
        if (Object.ReferenceEquals(this, s))
        {
            return true;
        }

        // If run-time types are not exactly the same, return false. 
        if (this.GetType() != s.GetType())
            return false;

        // Return true if the fields match. 
        // Note that the base class is not invoked because it is 
        // System.Object, which defines Equals as reference equality. 
        return (id == s.getId());			
	}
	
	public override int GetHashCode()
	{
		return id;
	}
	
	public void resetEnergy(){
		energy = 100;	
	}
}
