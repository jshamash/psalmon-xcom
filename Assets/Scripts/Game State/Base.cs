using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class Base {
	
	private Facility[,] facilities = new Facility[5,5];
	private string baseName;
	
	int occupiedScientists;
	private Dictionary<Weapon, int> scientistAssignment = new Dictionary<Weapon, int>();
	
	int occupiedWorkers;
	private List<ProductionOrder> activeOrders = new List<ProductionOrder>();
	
	int storageUsed;
	
	int occupiedLivingSpace;
	
	// Weapons ; amount manufactured
	private Dictionary<Weapon, int> manufacturedItems = new Dictionary<Weapon, int>();
	// Weapons ; amount purchased
	private Dictionary<Weapon, int> purchasedItems = new Dictionary<Weapon, int>();
	// Weapons ; amount equipped
	private Dictionary<Weapon, int> equippedItems = new Dictionary<Weapon, int>();
	
	//Lists of People/Items
	private LinkedList<Soldier> hiredSoldiers = new LinkedList<Soldier>();
	private LinkedList<string> hiredScientists = new LinkedList<string>();
	private LinkedList<string> hiredWorkers = new LinkedList<string>();
	private LinkedList<string> hiredPilots =  new LinkedList<string>();
	
	private Ship ship = new Ship();
	
	private int occupiedHospital = 0;
	private double timeToHeal = 0;
	
	private Aircraft aircraft = null;
	
	private int[] aliensKilled = new int[3];
	
	public Base(string name)
	{
		baseName = name;
		
		// initialize facilities to null
		for (int x = 0; x < 5; x++)
			for (int y =0; y < 5; y++)
				facilities[x,y] = null;
		
		// All bases start with a power plant
		facilities[2,2] = new Facility("Power Plant", 0, 0, "", "Facilities/Power_Plant");
		
		occupiedScientists = 0;
		occupiedWorkers = 0;
		storageUsed = 0;
		occupiedLivingSpace = 0;
	}
	
	public string getName() {
		return baseName;
	}
	
	// TODO -- MISSING ELEMENTS
	public void createInitialBase()
	{
		facilities[3,2] = AllFacilities.getFacilityByName("Storage Facility");
		facilities[1,1] = AllFacilities.getFacilityByName("Living Quarters");
		facilities[1,3] = AllFacilities.getFacilityByName("Hangar");
		
		// Add ship
		aircraft = new Aircraft();
		
		// Hire 5 soldiers
		gameManager.Instance.createNameList();
		for(int tempInt=0;tempInt<5;tempInt++){
			hiredSoldiers.AddLast (new Soldier(gameManager.Instance.getAvailableId(),gameManager.Instance.getRandomName ()));
			gameManager.Instance.increaseId();
			occupyLivingSpace();
		}
		
		//Hire one pilot
		hiredPilots.AddLast (gameManager.Instance.getRandomName ());
		occupyLivingSpace();
		
		// Assign pilots and soldiers to ship
		foreach (Soldier s in hiredSoldiers)
			aircraft.addSoldier(s);
		aircraft.addPilot(hiredPilots.First.Value);
		
		// Add 1 sniper, 1 shotgun, 3 rifles, 5 armours
		Weapon w = AllWeapons.getWeaponByName("Sniper I");
		if (w == null) Debug.Log("Weapon names have changed");
		purchasedItems.Add(w, 1);
		manufacturedItems.Add(w, 1);
		storageUsed += w.getStorageSpace();
		
		
		w = AllWeapons.getWeaponByName("Shotgun I");
		if (w == null) Debug.Log("Weapon names have changed");
		purchasedItems.Add(w, 1);
		manufacturedItems.Add(w, 1);
		storageUsed += 1*w.getStorageSpace();
		
		w = AllWeapons.getWeaponByName("Rifle I");
		if (w == null) Debug.Log("Weapon names have changed");
		purchasedItems.Add(w, 3); 
		manufacturedItems.Add(w, 3);
		storageUsed += 3*w.getStorageSpace();
		
		w = AllWeapons.getWeaponByName("Armour I");
		if (w == null) Debug.Log("Weapon names have changed");
		purchasedItems.Add(w, 5);
		manufacturedItems.Add(w, 5);
		storageUsed += 5*w.getStorageSpace();
		
		// Assign weapons to soldiers.
		//TODO item stays purchased but is marked equipped
		int index = 0;
		foreach (Soldier s in hiredSoldiers){
			//Add armor to the soldier
			w = AllWeapons.getWeaponByName("Armour I");
			
			s.setArmor(w);
			this.equip(w);
			
			//Now add weapon to the soldier
			if(index<3){
				w = AllWeapons.getWeaponByName("Rifle I");
				
				s.setWeapon(w);
				this.equip(w);
				
			}else if(index<4){
				w = AllWeapons.getWeaponByName("Shotgun I");
				
				s.setWeapon(w);
				this.equip(w);
			}else{
				w = AllWeapons.getWeaponByName("Sniper I");
				
				s.setWeapon(w);
				this.equip(w);
			}
			
			index++;
		}
			
		
	}
	
	#region Facility Info
	/// <summary>
	/// Adds the facility and charges the cost to the player.
	/// </summary>
	/// <returns>
	/// <c>True</c> if it was added
	/// <c>False</c> otherwise
	/// </returns>
	/// <param name='facility'>
	/// The Facility to add
	/// </param>
	/// <param name='pos_x'>
	/// X position in the base
	/// </param>
	/// <param name='pos_y'>
	/// Y position in the base
	/// </param>
	public bool addFacility (Facility facility, int pos_x, int pos_y)
	{
		if (!gameManager.Instance.spendMoney(facility.getCost()))
			return false;
		
		facilities[pos_x,pos_y] = facility;
		return true;
	}
	
	public Facility getFacility (int x, int y)
	{
		if (x > 4 || y > 4) return null;
		return facilities[x,y];
	}
	
	/// <summary>
	/// Gets the number of facilities having the specified name.
	/// </summary>
	/// <returns>
	/// The count.
	/// </returns>
	/// <param name='name'>
	/// Name.
	/// </param>
	public int facilityCount(string name)
	{
		int amount = 0;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if (facilities[x,y] != null && facilities[x,y].getName() == name)
					amount++;
			}
		}
		return amount;
	}
	
	public int getAvailLabSpace()
	{
		return this.getTotalLabSpace() - occupiedScientists;
	}
	
	public int getTotalLabSpace()
	{
		// Count the number of labs
		int numLabs = 0;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if (facilities[x,y] != null && facilities[x,y].getName() == "Laboratory")
					numLabs++;
			}
		}
		
		return numLabs * AllFacilities.LAB_CAPACITY;
	}
	
	public int getTotalWorkshopSpace()
	{
		// Count the number of workshops
		int numShops = 0;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if (facilities[x,y] != null && facilities[x,y].getName() == "Workshop")
					numShops++;
			}
		}
		
		return numShops * AllFacilities.WORKSHOP_CAPACITY;
	}
	
	public int getTotalStorageSpace()
	{
		// Count the number of storage spaces
		int numStorage = 0;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if (facilities[x,y] != null && facilities[x,y].getName() == "Storage Facility")
					numStorage++;
			}
		}
		
		return numStorage * AllFacilities.STORAGE_CAPACITY;
	}
	
		
	#endregion
	
	#region Research Methods
	/// <summary>
	/// Occupies one scientist in the base. Has no effect if all scientists are occupied.
	/// </summary>
	public bool occupyScientist(Weapon w)
	{
		if (occupiedScientists < this.getNumbScientists()) {
			occupiedScientists++;
			if (scientistAssignment.ContainsKey(w))
				scientistAssignment[w]++;
			else
				scientistAssignment.Add(w, 1);
			return true;
		}
		return false;
	}
	
	/// <summary>
	/// Frees one occupied scientist. Has no effect if there are no occupied scientists.
	/// </summary>
	public bool freeScientist(Weapon w)
	{
		int i;
		if (occupiedScientists > 0 && scientistAssignment.TryGetValue(w, out i))
		{
			if (i > 0)
			{
				occupiedScientists--;
				scientistAssignment[w]--;
				if (scientistAssignment[w] == 0)
				{
					scientistAssignment.Remove(w);
				}
				return true;
			}
		}
		return false;
	}
	
	/// <summary>
	/// Gets the occupied scientists.
	/// </summary>
	/// <returns>
	/// The number of occupied scientists.
	/// </returns>
	public int getOccupiedScientists()
	{
		return occupiedScientists;
	}
	
	// Frees the scientists whose project finished being researched.
	public void cleanLab()
	{
		
		List<Weapon> l = new List<Weapon>(scientistAssignment.Keys);
		foreach (Weapon w in l)
		{
			if (w.researchFinished())
			{
				occupiedScientists -= scientistAssignment[w];
				scientistAssignment.Remove(w);
			}
		}
	}
	#endregion
	
	#region Production Methods
	public int getAmountManufactured(Weapon w)
	{
		int amount;
		if (manufacturedItems.TryGetValue(w, out amount)) return amount;
		else return 0;
	}
	
	public void addManufacturedItem(Weapon w, int quantity)
	{
		if (manufacturedItems.ContainsKey(w))
			manufacturedItems[w] += quantity;
		else
			manufacturedItems.Add(w, quantity);
	}
	
	public void placeOrder(ProductionOrder o)
	{
		activeOrders.Add(o);
	}
	
	public int getOccupiedWorkers()
	{
		return occupiedWorkers;
	}
	
	public void occupyWorker()
	{
		if (occupiedWorkers < this.getNumbWorkers())  occupiedWorkers++;
	}
	
	public void freeWorker()
	{
		if (occupiedWorkers > 0) occupiedWorkers--;
	}
	
	/// <summary>
	/// Checks if an active order exists containing this weapon.
	/// </summary>
	/// <returns>
	/// True if such an order exists.
	/// </returns>
	/// <param name='w'>
	/// The weapon.
	/// </param>
	public bool orderInProgress(Weapon w)
	{
		foreach (ProductionOrder order in activeOrders) {
			if (order.getWeapon().getName() == w.getName())
				return true;
		}
		return false;
	}
	
	public List<ProductionOrder> getActiveOrders()
	{
		return this.activeOrders;
	}
	
	public void removeOrder(ProductionOrder order)
	{
		activeOrders.Remove(order);
	}
	#endregion
	
	#region Buy/Sell methods
	/// <summary>
	/// Gets a sorted list of manufactured items
	/// </summary>
	/// <returns>
	/// A list of weapons that have been manufactured in this base, sorted by name.
	/// </returns>
	public List<Weapon> getManufacturedItems()
	{
		List<Weapon> list = new List<Weapon>(manufacturedItems.Keys);
		list.Sort(
			delegate(Weapon w1, Weapon w2)
			{
				return w1.getName().CompareTo(w2.getName());
			}
		);
		return list;
	}
	
	public List<Weapon> getPurchasedItems()
	{
		List<Weapon> list = new List<Weapon>(purchasedItems.Keys);
		list.Sort(
			delegate(Weapon w1, Weapon w2)
			{
				return w1.getName().CompareTo(w2.getName());
			}
		);
		return list;
	}
	
	public int getAmountPurchased(Weapon w)
	{
		int amount;
		if (purchasedItems.TryGetValue(w, out amount)) return amount;
		else return 0;
	}
	
	/// <summary>
	/// Purchases an item. Does not check if there is enough money, storage space, or items manufactured.
	/// This is the responsibility of the caller.
	/// </summary>
	/// <param name='w'>
	/// The weapon to purchase.
	/// </param>
	public void purchaseItem(Weapon w)
	{
		if (purchasedItems.ContainsKey(w))
			purchasedItems[w]++;
		else
			purchasedItems.Add(w, 1);
		
		gameManager.Instance.spendMoney(w.getCost());
		
		storageUsed += w.getStorageSpace();
	}
	
	/// <summary>
	/// Returns an item. Has no effect if the item is not in purchased list.
	/// </summary>
	/// <param name='w'>
	/// The weapon to return.
	/// </param>
	public void returnItem(Weapon w)
	{
		int amount;
		if (purchasedItems.TryGetValue (w, out amount) && amount > 0)
		{
			purchasedItems[w]--;
			if (purchasedItems[w] == 0)
				purchasedItems.Remove(w);
			gameManager.Instance.earnMoney(w.getCost());
			storageUsed -= w.getStorageSpace();
		}
	}
	
	public int getUsedStorage()
	{
		return storageUsed;
	}
	#endregion
	
	#region Transfer Methods
	public void removeSoldiersForTransfer(EmployeeTransfer et)
	{
		foreach (Soldier s in et.getSoldiers())
		{
			if (aircraft != null && aircraft.getSoldiers().Contains(s))
				aircraft.removeSoldier(s);
			Weapon wep = s.getWeapon();
			Weapon arm = s.getArmor();
			if (wep != null)
			{
				s.setWeapon (null);
				this.dequip(wep);
			}
			if (arm != null)
			{
				s.setArmor(null);
				this.dequip(arm);
			}
			this.removeHiredSoldier(s);
			occupiedLivingSpace--;
		}
	}
	
	public List<string> removeScientistsForTransfer(EmployeeTransfer et)
	{
		List<string> removedScientists = new List<string>();
		for (int i = 0; i < et.getScientists(); i++)
		{
			string scientist = hiredScientists.Last.Value;
			hiredScientists.Remove(scientist);
			removedScientists.Add(scientist);
			occupiedLivingSpace--;
		}
		return removedScientists;
	}
	
	public List<string> removeWorkersForTransfer(EmployeeTransfer et)
	{
		List<string> removedWorkers = new List<string>();
		for (int i = 0; i < et.getWorkers(); i++)
		{
			string worker = hiredWorkers.Last.Value;
			hiredWorkers.Remove(worker);
			removedWorkers.Add(worker);
			occupiedLivingSpace--;
		}
		return removedWorkers;
	}
	
	public List<string> removePilotsForTransfer(EmployeeTransfer et)
	{
		// Unless we're removing all the pilots, we have to save the pilot on the aircraft.
		string busyPilot = null;
		if (et.getPilots() < getNumbPilots() && aircraft != null && aircraft.hasPilot())
		{
			// Remove the pilot from hired pilots
			busyPilot = aircraft.getPilot();
			hiredPilots.Remove(busyPilot);
		}
		List<string> removedPilots = new List<string>();
		for (int i = 0; i < et.getPilots(); i++)
		{
			string pilot = hiredPilots.Last.Value;
			hiredPilots.Remove(pilot);
			removedPilots.Add(pilot);
			occupiedLivingSpace--;
		}
		if (busyPilot != null) {
			// Restore the pilot to hired pilots
			hiredPilots.AddLast(busyPilot);
		}
		return removedPilots;
	}
	
	public void removeForTransfer(WeaponTransfer wt)
	{
		foreach (Weapon w in wt.getWeapons())
		{
			// Remove from manufactured items
			manufacturedItems[w] -= wt.getQuantity (w);
			if (manufacturedItems[w] == 0)
				manufacturedItems.Remove(w);
			
			// Remove from purchased items
			purchasedItems[w] -= wt.getQuantity(w);
			if (purchasedItems[w] == 0)
				purchasedItems.Remove(w);
			
			// Free storage space
			storageUsed -= w.getStorageSpace()*wt.getQuantity(w);
		}
	}
	
	public void addSoldiersForTransfer(EmployeeTransfer et)
	{
		foreach (Soldier s in et.getSoldiers())
		{
			this.addHiredSoldier (s);
			occupiedLivingSpace++;
		}
	}
	
	public void addScientistsForTransfer(EmployeeTransfer et, List<string> scientists)
	{
		if (et.getScientists() != scientists.Count) Debug.Log("Something went wrong");
		
		foreach (string s in scientists)
		{
			this.addHiredScientist(s);
			occupiedLivingSpace++;
		}
	}
	
	public void addWorkersForTransfer(EmployeeTransfer et, List<string> workers)
	{
		if (et.getWorkers() != workers.Count) Debug.Log("Something went wrong");
		
		foreach (string w in workers)
		{
			this.addHiredWorker(w);
			occupiedLivingSpace++;
		}
	}
	
	public void addPilotsForTransfer(EmployeeTransfer et, List<string> pilots)
	{
		if (et.getPilots() != pilots.Count) Debug.Log("Something went wrong");
		
		foreach (string p in pilots)
		{
			this.addHiredPilot(p);
			occupiedLivingSpace++;
		}
	}
	
	public void addForTransfer(WeaponTransfer wt)
	{
		foreach (Weapon w in wt.getWeapons())
		{
			// Add to manufactured items
			if (manufacturedItems.ContainsKey(w)) {
				manufacturedItems[w] += wt.getQuantity(w);
			}
			else {
				manufacturedItems.Add(w, wt.getQuantity(w));
			}
			
			// Add to purchased items
			if (purchasedItems.ContainsKey(w)) {
				purchasedItems[w] += wt.getQuantity(w);
			}
			else {
				purchasedItems.Add(w, wt.getQuantity(w));
			}
			
			// Occupy storage space
			storageUsed += w.getStorageSpace()*wt.getQuantity(w);
		}
	}
	#endregion
	
	#region Aircraft Methods
	public bool hasAircraft() {
		return aircraft != null;
	}
	
	public void addAircraft() {
		aircraft = new Aircraft();
	}
	
	// Returns null if base doesn't have an aircraft. 
	public Aircraft getAircraft() {
		return aircraft;
	}
	#endregion
		
	#region EMPLOYEE SCENE METHODS	
	//Following four methods are getters for number of soldiers in list of hired soldiers
	public int getNumbSoldiers(){
		return hiredSoldiers.Count;
	}
	
	public int getNumbScientists(){
		return hiredScientists.Count;	
	}
	
	public int getNumbWorkers(){
		return hiredWorkers.Count;	
	}
	
	public int getNumbPilots(){
		return hiredPilots.Count;	
	}
	
	//Following four methods are getter methods for Employee hired Lists
	public LinkedList<Soldier> getHiredSoldiers(){
		return hiredSoldiers;	
	}
	
	public LinkedList<string> getHiredScientists(){
		return hiredScientists;	
	}
	
	public LinkedList<string> getHiredWorkers(){
		return hiredWorkers;	
	}
	
	public LinkedList<string> getHiredPilots(){
		return hiredPilots;	
	}
	
	//TO BE IMPlEMENTED
	public void addHiredSoldier(Soldier toBeAdded){
		hiredSoldiers.AddLast (toBeAdded);
	}
	
	//TO BE IMPLEMENTED
	public void removeHiredSoldier(Soldier removedSoldier){
		LinkedList<Soldier> newList = new LinkedList<Soldier>();
		int id = removedSoldier.getId ();
		Soldier[] tempList = new Soldier[hiredSoldiers.Count];
		hiredSoldiers.CopyTo(tempList,0);
		for(int i = 0;i<hiredSoldiers.Count;i++){
			if(tempList[i].getId () != id){
				newList.AddLast (tempList[i]);	
			}
		}
		
		hiredSoldiers=newList;
	}
	
	//Adding a scientists to linked list of scientists
	public void addHiredScientist(string name){
		hiredScientists.AddLast(name);
	}
	
	//Remove scientist called ...
	public void removeHiredScientist(string name){
		hiredScientists.Remove (name);	
	}
	
	//Following eight methods same as previous 4
	public void addHiredWorker(string name){
		hiredWorkers.AddLast(name);	
	}
	
	public void removeHiredWorker(string name){
		hiredWorkers.Remove(name);	
	}
	
	public void addHiredPilot(string name){
		hiredPilots.AddLast(name);	
	}
	
	public void removeHiredPilot(string name){
		hiredPilots.Remove(name);	
	}
	
	public void setHiredSoldiersList(LinkedList<Soldier> newList){
		hiredSoldiers = newList;
	}
	
	#endregion
	
	#region Containment
	
	// type = 0, 1 or 2 as defined in Alien class
	// Doesn't check if there is containment space
	public void addAlien(int type) {
		aliensKilled[type]++;
	}
	
	// Returns the number of aliens killed of the specified type
	public int getNumAliens(int type) {
		return aliensKilled[type];
	}
	
	// Returns the total number of aliens killed in this base
	public int getAlienTotal() {
		return aliensKilled[0] + aliensKilled[1] + aliensKilled[2];
	}
	
	//Returns the amount of alien containment space
	public int getContainmentSpace() {
		return (this.facilityCount("Containment"))*AllFacilities.CONTAINMENT_CAPACITY;
	}
	
	#endregion
	
	#region Hospital
	public int getHospitalOccupied() {
		return occupiedHospital;
	}
	
	public int getHospitalSpace() {
		return (this.facilityCount ("Hospital")) * AllFacilities.HOSPITAL_CAPACITY;
	}
	
	// Doesn't check if space is available
	public void addToHospital(Soldier s) {
		occupiedHospital++;
		s.InHospital = true;
	}
	
	public void healWithTime(double time) {
		timeToHeal += time;
		if (timeToHeal >= 24)
		{
			// a day has passed, so heal soldiers.
			timeToHeal = 0;
			foreach (Soldier s in this.getHiredSoldiers())
			{
				if (s.InHospital) {
					s.updateHealth(s.getHealth() + 1);
					if (s.getHealth() >= 100) {
						// Soldier is healed
						s.InHospital = false;
						occupiedHospital--;
					}
				}
			}
		}
	}
	#endregion
	
	#region Equip Soldiers
	public void equip(Weapon w) {
		if (equippedItems.ContainsKey(w))
			equippedItems[w]++;
		else
			equippedItems.Add(w, 1);
	}
	
	public void dequip(Weapon w) {
		if (w == null) return;
		equippedItems[w]--;
		if (equippedItems[w] == 0)
			equippedItems.Remove(w);
	}
	
	public int getEquipped(Weapon w) {
		if (equippedItems.ContainsKey (w))
			return equippedItems[w];
		else
			return 0;
	}
	#endregion
	
	public bool isSoldierInAircraft(Soldier soldier){
		foreach(Soldier sold in aircraft.getSoldiers()){
			if(sold.getId()==soldier.getId ())
				return true;
		}
		return false;
	}
	
	public bool isPilotInAircraft(string pilotName){
		if(aircraft.getPilot()==null)
			return false;
		if(aircraft.getPilot().Equals(pilotName))
			return true;
		return false;
	}
		
	public Ship getShip() {
		return ship;
	}
	
	/// <summary>
	/// Gets the total number of living space.
	/// </summary>
	/// <returns>
	/// The total living space.
	/// </returns>
	public int getLivingSpace()
	{
		// Count the number of living spaces
		int numSpaces = 0;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 5; y++)
			{
				if (facilities[x,y] != null && facilities[x,y].getName() == "Living Quarters")
					numSpaces++;
			}
		}
		
		return numSpaces * AllFacilities.LIVING_CAPACITY;
	}
	
	/// <summary>
	/// Adds one employee to the living space. Does nothing if full.
	/// </summary>
	public void occupyLivingSpace()
	{
		if (occupiedLivingSpace < this.getLivingSpace())
			occupiedLivingSpace++;
	}
	
	/// <summary>
	/// Frees up one space in living quarters. No effect if empty.
	/// </summary>
	public void freeLivingSpace()
	{
		if (occupiedLivingSpace > 0)
			occupiedLivingSpace--;
	}
	
	/// <summary>
	/// Gets the amount of living space that is occupied.
	/// </summary>
	/// <returns>
	/// The occupied living space.
	/// </returns>
	public int getOccupiedLivingSpace()
	{
		return occupiedLivingSpace;
	}
	
	public void removeSoldier(Soldier s){
		LinkedList<Soldier> newList = new LinkedList<Soldier>();
		foreach	(Soldier soldier in hiredSoldiers){
			if(!(soldier.getId()==s.getId ())){
				newList.AddFirst(soldier);
			}
		}
		
		hiredSoldiers=newList;
	}
	
}