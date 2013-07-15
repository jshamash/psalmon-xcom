using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System;

[System.Serializable]
public class gameManager : MonoBehaviour {
	
	#region constants
	public const double TIME_SLOW = 0.1;
	public const double TIME_FAST = 1.0;
	public const int MISSION_PAYOFF = 50000;
	#endregion
	
	#region variables	
	//HERE WE WILL NEED TO STORE ALL VARIABLES THAT ARE IMPORTANT TO OUR GAME
	private int money;
	private Funding funding;
	private System.DateTime time;
	private List<string> pendingRequests = new List<string>();
	private int currentBase;
	private int availableId = 0;
	
	private int missionsSuccess = 0;
	private int missionsFailed = 0;
	public int CiviliansKilled {get; set;}
	public int CiviliansSaved {get; set;}
	
	// Weapons that are available to buy/produce
	List<Weapon> availableWeapons = new List<Weapon>(AllWeapons.initialWeapons);
	
	// Weapons that have yet to be researched
	Queue<Weapon>[] unresearchedWeapons =
	{
		new Queue<Weapon>(AllWeapons.shotgunUpgrades),
		new Queue<Weapon>(AllWeapons.armourUpgrades),
		new Queue<Weapon>(AllWeapons.rifleUpgrades),
		new Queue<Weapon>(AllWeapons.sniperUpgrades),
		new Queue<Weapon>(AllWeapons.alienWeapons)
	};
	
	//List of names initiation in startState() 
	private string[] nameList;
	//Randomizer
	private System.Random random= new System.Random();
	
	// Bases
	private List<Base> bases = new List<Base>();
	
	
	//Important variable to know which level we are currently at
	public string activeLevel;
	
	#endregion
	
	
	private static gameManager instance;
	
	//Create an instance of a gameManager object if an instance does not already exist
	public static gameManager Instance{
		get{
			if(instance==null){
				instance = new GameObject("gameManager").AddComponent<gameManager>();	
			}
			return instance;
		}
	}
	
	//Delete gameManagerInstance
	public void deleteInstance(){
		instance=null;	
	}
	
	//Sets the instances to null when the application quits
	public void OnApplicationQuit(){
		instance=null;
	}
	
	//This is where we instantiate every variables for a new game
	public void startState(){
		print ("Creating a new game state.");	
		//Set variables relevent to game manager
		money = 500000;
		funding = new Funding();
		time = System.DateTime.Now;
		
		// variable used to create new soldiers.
		availableId = 0;
		//Calling the new scene
		setLevel("WorldMap");
		nameList = createNameList();
		
		missionsSuccess = 0;
		missionsFailed = 0;
	}
	
	// Start from saved game
	public void setState(SavedGameState save){
		this.money = save.money;
		this.funding = save.funding;
		this.time = save.time;
		this.pendingRequests = save.pendingRequests;
		this.currentBase = save.currentBase;
		this.availableId = save.availableId;
		this.availableWeapons = save.availableWeapons;
		this.unresearchedWeapons = save.unresearchedWeapons;
		this.nameList = save.nameList;
		this.random = new System.Random();
		this.bases = save.bases;
		
		missionsSuccess = save.missionsSuccess;
		missionsFailed = save.missionsFailed;
		CiviliansKilled = save.CiviliansKilled;
		CiviliansSaved = save.CiviliansSaved;
		
		setLevel("WorldMap");

	}
	
	
	public void increaseId(){
		availableId++;	
	}
	
	#region Time Functions
	public void advanceTime(double hours){
		time = time.AddHours(hours);
	}
	#endregion
	

	#region Getters and Setters
	
	//Obtain the current date
	public DateTime getTime(){
		return time;
	}
	
	public int getAvailableId(){
		availableId++;
		return availableId;
	}
	
	public int getMoney(){
		return money;
	}
	
	public void updateMoney(int newAmount){
		money=newAmount;
	}
	
	public bool spendMoney(int amount) {
		if (amount > money)
			return false;
		
		money -= amount;
		return true;
	}
	
	public void earnMoney(int amount) {
		money += amount;
	}
	
	public string getLevel(){
		return activeLevel;	
	}
	
	public void addBase(string b) {
		Base nb = new Base(b);
		bases.Add(nb);
	}
	
	public void createInitialBase() {
		currentBase = 0;
		bases[currentBase].createInitialBase();
	}
	
	public int numBases() {
		return bases.Count;
	}
	
	public List<Base> getBases() {
		return bases;
	}
	
	public Base getBaseAfter(Base b) {
		return bases[(bases.IndexOf(b) + 1) % bases.Count];
	}
	
	public Base getBaseBefore(Base b) {
		int index = bases.IndexOf(b) - 1;
		if (index < 0) return bases[bases.Count - 1];
		else return bases[index];
	}
	
	public bool baseExist(string baseName) {
		foreach(Base b in bases) {
			if(b.getName().Equals(baseName)) {
				return true;
			}
		}
		return false;
	}

	public Base getCurrentBase() {
		return bases[currentBase];
	}
	
	public void setCurrentBase(Base newBase){
		bases[currentBase]=newBase;	
	}
	
	public void setCurrentBase(string baseName) {
		for(int i = 0; i < bases.Count; i++) {
			if(bases[i].getName() == baseName) {
				currentBase = i;
			}
		}	
	}
	
	public Base nextBase() {
		currentBase = (currentBase + 1) % bases.Count;
		return bases[currentBase];
	}
	
	public Base previousBase() {
		currentBase = currentBase - 1;
		if (currentBase < 0) {
			currentBase = bases.Count - 1;
		}
		return bases[currentBase];
	}
	
	public Queue<Weapon>[] getUnresearchedWeapons()
	{
		return unresearchedWeapons;
	}
	
	public void addAvailableWeapon(Weapon w)
	{
		availableWeapons.Add(w);
		availableWeapons.Sort(
			delegate(Weapon w1, Weapon w2)
			{
				return w1.getName().CompareTo(w2.getName());
			}
		);
	}
	
	public List<Weapon> getAvailableWeapons()
	{
		return availableWeapons;
	}
	
	public void addPendingRequest(string req)
	{
		pendingRequests.Add(req);
	}
	
	public List<string> getPendingRequests()
	{
		return pendingRequests;
	}
	
	public string getFirstPendingRequest()
	{
		return pendingRequests[0];
	}
	
	public void removeFirstPendingRequest()
	{
		pendingRequests.RemoveAt(0);
	}
	
	public Funding getFunding() 
	{
		return funding;
	}
	
	public string[] getNameList()
	{
		return nameList;
	}
	
	public int getCurrentBaseNum() 
	{
		return currentBase;
	}
	
	public int getPanicLevel(string continent)
	{
		return funding.getPanicLevel(continent);
	}
	
	
	
	#endregion
	
	#region Funding
	public void addMonthlyFunding() {
		money += funding.getMonthlyFund(numBases());
	}
	
	public void addPanicLevel(string continent) {
		funding.addPanicLevel(continent);
	}
	
	public void removePanicLevel(string continent) {
		funding.removePanicLevel(continent);
	}
	
	// Pay Employees Yearly
	public int costEmployees() {
		int payment = 0;
		foreach(Base b in bases) {
			payment += b.getNumbPilots() * EmployeeScript.EMPLOYEEWAGE;
			payment += b.getNumbScientists() * EmployeeScript.EMPLOYEEWAGE;
			payment += b.getNumbWorkers() * EmployeeScript.EMPLOYEEWAGE;
			foreach(Soldier s in b.getHiredSoldiers()) {
				payment += EmployeeScript.SALARIES[s.getRank()];
			}
		}
		
		return payment;
	}
	
	public void payEmployees() {
		money -= costEmployees();
	}
		
	#endregion
	
	#region aliens
	// Returns the total number of aliens of this type
	public int getNumAliens(int type) {
		int total = 0;
		foreach (Base b in bases) {
			total += b.getNumAliens(type);
		}
		return total;		
	}
	
	// Returns the total number of aliens
	public int getNumAliens() {
		return getNumAliens(0) + getNumAliens (1) + getNumAliens (2);
	}
	#endregion
	
	//Returns a random name from nameList
	public string getRandomName(){
		if(nameList==null){
			nameList = createNameList();
		}
		return nameList[(int)(random.NextDouble() * (double)nameList.Length)];	
	}
	
	//Creates an array of names
	public string[] createNameList(){
		string nameListString = "Staci Cesare,Marshall Longmore,Sanjuana Belue,Annice Commander,Liane Whitsett,Terisa Googe,Marcie Pietz,Danyell Capella,Norma Wiersma,Rosalyn Eastland,Hershel Radigan,Myrtice Mcevoy,Chasidy Navin,Fidelia Goodner,Annett Rudolph,Regine Champ,Johnna Mirza,Alicia Nealey,Clemmie Pool,Clorinda Moscato,Sharolyn Garza,Lala Priolo,Stephenie Ely,Eleanore Tamayo,Valencia Adorno,Carie Mcmurtrie,Yasuko Luce,Enola Hibner,Jacquelin Mcphatter,Marin Raglin,Clarissa Parnell,Alida Pokorny,Dot Golder,Roger Newsom,Lizzette Canary,Yuko Shiflet,Shane Taketa,Heide Randazzo,Silvia Debow,Lexie Giardina,Jaunita Amore,Yu Sereno,Alpha Gautier,Ramiro Brenes,Flora Quam,Wilfredo Mclachlan,Dawne Michel,Leena Kieffer,Crysta Perrotta,Stacy Bryd,Lynda Riche,Wen Mcmullen,Nicolle Riera,Loise Geary,Cara Johansson,Edgar Gunnell,Brain Bertucci,Herma Hager,Sanora Lablanc,Celine Boomhower,Robyn Vigue,Jinny Trask,Glayds Harbaugh,Eileen Ellenberger,Bernardine,Lewandowski,Elisa Darrington,Jillian Espinoza,Christal Goggans,Veda Thiel,Cyndy Summey,France Cournoyer,Franklyn Wulf,Fairy Cogar,Felton Croker,Tameka Sheriff,Ciara Esterly,Marhta Neel,Marcos Whisenhunt,Naomi Pascoe,Deloise Ammons,Cora Desir,Avis Depaul,Juliane Heyne,Tu Towery,Leontine Mccroy,Lizabeth Altieri,Tiffanie Lemond,Allie Sperber,Johnette Kostka,Johnny Lillie,Magdalene Maciel,Deja Minner,Trudi Macey,Concepcion Folden,Carrol Cather,Erick Osterman,Classie Crippen,Charley Schurman,Clifford Harnage,Jules Mateer";
		string[] words = nameListString.Split (',');
		return words;
	}
	
	
	public void setLevel(string newLevel){
		activeLevel = newLevel;	
		Application.LoadLevel(newLevel);
		print("Level "+ getLevel() +" should load now.");
	}
	
	public string getActiveLevel(){
		return activeLevel;
	}
	
	public void increaseMissionSuccess(){
		missionsSuccess++;	
	}
	
	public void increaseMissionFailed(){
		missionsFailed++;
	}
	
	public int getMissionSuccess() {
		return missionsSuccess;
	}
	
	public int getMissionFailed() {
		return missionsFailed;
	}
}
