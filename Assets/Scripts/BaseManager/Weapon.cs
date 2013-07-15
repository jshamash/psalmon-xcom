using System;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Weapon
{
	string name;
	[System.NonSerialized] Texture2D image;
	string imagePath;
	
	// Hours remaining until this weapon can be researched
	double researchSpawnTime;
	
	// True when this weapon is being researched
	Boolean isResearching;
	
	int assignedScientists;
	double timeToResearch;
	
	int cost;
	int storageSpace;
	int productionLimit;
	double timeToProduce;
	
	// Weapon stats
	int damage;
	int range;
	int maxAmmo;
	int actionPoints;
	int angle;
	
	// For armour only
	bool isArmour;
	double protection;
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Weapon"/> class.
	/// </summary>
	/// <param name='name'>
	/// Name.
	/// </param>
	/// <param name='daysToSpawn'>
	/// Days until this weapon can be researched (once all prerequisites have been researched)
	/// </param>
	/// <param name='daysToResearch'>
	/// Days to research (one man days).
	/// </param>
	public Weapon (string name, string imgPath, bool isArmour, int daysToSpawn, int daysToResearch, int cost, int storageSpace,
					int productionLimit, int daysToProduce, int damage, int range, int maxAmmo, int actionPoints, int angle,
					double protection)
	{
		this.name = name;
		this.imagePath = imgPath;
		this.image = Resources.Load(imgPath) as Texture2D;
		researchSpawnTime = 24*daysToSpawn;
		timeToResearch = 24*daysToResearch;
		isResearching = false;
		assignedScientists = 0;
		
		this.cost = cost;
		this.storageSpace = storageSpace;
		this.productionLimit = productionLimit;
		this.timeToProduce = 24*daysToProduce;
		
		this.isArmour = isArmour;
		if (isArmour)
		{
			// Everything is 0 except protection
			this.damage = 0;
			this.range = 0;
			this.maxAmmo = 0;
			this.actionPoints = 0;
			this.angle = 0;
			this.protection = protection;
		}
		else
		{
			// Everything has values except protection
			this.damage = damage;
			this.range = range;
			this.maxAmmo = maxAmmo;
			this.actionPoints = actionPoints;
			this.angle = angle;
			this.protection = 0;
		}
	}
	
	//Constructor for equipment menus
	/*public Weapon (string name, Texture2D img, bool isArmour, int cost, int storageSpace, int damage, int range, int maxAmmo, int actionPoints, int angle,double protection)
	{
		this.name = name;
		this.image =img;
		researchSpawnTime = 0.0;
		timeToResearch = 0;
		isResearching = false;
		assignedScientists = 0;
		
		this.cost = cost;
		this.storageSpace = storageSpace;
		this.productionLimit = 0;
		this.timeToProduce = 0;
		
		this.isArmour = isArmour;
		if (isArmour)
		{
			// Everything is 0 except protection
			this.damage = 0;
			this.range = 0;
			this.maxAmmo = 0;
			this.actionPoints = 0;
			this.angle = 0;
			this.protection = protection;
		}
		else
		{
			// Everything has values except protection
			this.damage = damage;
			this.range = range;
			this.maxAmmo = maxAmmo;
			this.actionPoints = actionPoints;
			this.angle = angle;
			this.protection = 0;
		}
	}*/
	
	#region Getters
	public string getName()
	{
		return this.name;
	}
	
	public Texture2D getImage()
	{
		setImage();
		return image;
	}
	
	public int getCost()
	{
		return cost;
	}
	
	public int getStorageSpace()
	{
		return storageSpace;
	}
	
	public int getProductionLimit()
	{
		return productionLimit;
	}
	
	public int getDamage() {
		return damage;
	}
		
	public int getRange()
	{
		return range;
	}
	
	public int getMaxAmmo() {
		return maxAmmo;
	}
	
	public int getActionPoints() {
		return actionPoints;
	}
	
	public int getAngle() {
		return angle;
	}
	
	public double getProtection() {
		return protection;
	}
	
	public bool isArmourType() {
		return isArmour;
	}	

	
	/* Special setter for serialization 
	 * NOTE: To be executed after deserialization
	 */
	public void setImage(){
		this.image = Resources.Load(imagePath) as Texture2D;
	}
	#endregion
	
	/// <summary>
	/// Gets the production time in hours.
	/// </summary>
	/// <returns>
	/// The production time.
	/// </returns>
	public double getProductionTime()
	{
		return timeToProduce;
	}
	
	// Returns true when this research should spawn.
	public virtual bool advanceTime(double hours)
	{
		if (isResearching)
		{
			// Decrement research time.
			timeToResearch -= assignedScientists*hours;
		}
		
		if (researchSpawnTime <= 0) return false;
		
		researchSpawnTime -= hours;
		if (researchSpawnTime <= 0) return true;
		
		return false;
	}
	
	public double getTimeToResearch()
	{
		return timeToResearch;
	}
	
	public int getDaysToResearch()
	{
		return (int) (timeToResearch/24.0);
	}
	
	public bool isResearchable()
	{
		return researchSpawnTime <= 0;
	}
	
	public bool researchFinished()
	{
		return timeToResearch <= 0;
	}
	
	public int getAssignedScientists()
	{
		return assignedScientists;
	}
	
	public void assignScientist()
	{
		assignedScientists++;
		isResearching = true;
	}
	
	public void removeScientist()
	{
		if (assignedScientists > 0)
			assignedScientists--;

		if (assignedScientists == 0)
			isResearching = false;
	}
	
	public bool researchInProgress() {
		return isResearching;
	}
	
}

