using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Timers;

public class worldMapScript : MonoBehaviour {
	
	private static worldMapScript instance;
	
	private readonly static int BASEPRICE = 50000;
	private readonly static int PENDINGTIME = 20000;
	
	// GUI 
	private bool showInterception = false;
	private bool showInterceptionList = false;
	private bool showInterceptionOver = false;
	private bool showInstallations = false;
	private bool showStatistics = false;
	private bool showInitialBaseSelection = false;
	private bool showOptions = false;
	private bool showInsufficientFunds = false;
	private bool showYearlyPayment = false;
	private bool wasFast=false;
	private bool pickInterceptionBase = false;
	
	// For picking base before starting mission
	private Vector2 scrollPosition = new Vector2(0,0);
	
	RaycastHit mouseDown;
	RaycastHit mouseUp;
	
	// Misc. helper
	private bool wasPlaying = false;
	private bool enableMenuButtons = true;
	
	// For research events
	private bool researchSpawnEvent = false;
	private Weapon researchSpawnEventWeapon = null;
	private bool researchCompleteEvent = false;
	private Weapon researchCompleteEventWeapon = null;
	
	// For production events
	private bool productionCompleteEvent = false;
	private Weapon productionCompleteEventWeapon = null;
	private string productionCompleteEventBaseName = "";
	
	// For time
	private int currentMonth;
	private int currentYear;
	
	// For interception popup
	string interceptMessage = "";
	bool areYouSure = false;
	
	//Game object for base access
	public GameObject NorthAmerica;
	public GameObject NorthAmericaLight;
	public GameObject SouthAmerica;
	public GameObject SouthAmericaLight;
	public GameObject Africa;
	public GameObject AfricaLight;
	public GameObject Europe;
	public GameObject EuropeLight;
	public GameObject Asia;
	public GameObject AsiaLight;
	public GameObject Australia;
	public GameObject AustraliaLight;
	
	public GameObject cameraRotation;
	
	private Quaternion initialRotation;
	
	public GameObject[] continents = new GameObject[6];
	public GameObject[] continentLights = new GameObject[6];
	private string[] baseNames = {"Africa"
								, "Asia"
								, "Australia"
								, "Europe"
								, "North America"
								, "South America"};
	
	
	#region Base creation menu
	private int listEntry = 0;
	
	
	private List<string> legalBaseNames = new List<string>(new []{"North America"
																, "South America"
																, "Europe"
																, "Africa"
																, "Asia"
																, "Australia"});
	#endregion
	
	#region Time vars
	enum PlayState {Pause, Play, Fast};
	private PlayState timestate = PlayState.Pause;
	public GUISkin playSkin;
	public GUISkin fastSkin;
	public GUISkin pauseSkin;
	public GUISkin normalToggle;
	#endregion
	
	#region Interception
	
	private Timer interceptionTimer;
	private System.Random rand = new System.Random();
	private readonly static List<string> INTERCEPTIONS = new List<string>
	{ "China"
	, "United Kingdom"
	, "USA"
	, "Argentina"
	, "Sudan"
	, "Antartica"
	, "Australia"
	};
	private readonly static Dictionary<string, string> COUNTRY2CONTINENT = new Dictionary<string, string>{
		{"China", "Asia"},
		{"United Kingdom", "Europe"},
		{"USA", "North America"},
		{"Argentina", "South America"},
		{"Sudan", "Africa"},
		{"Antartica", "Antartica"},
		{"Australia", "Australia"}
	};
	private readonly static List<string> SPECIALREQUESTS = new List<string>
	{
		"China Requests 4 Snipers I",
		"UK Requests 10 Armor I",
		"Australia Requests 5 Shotgun I"
	};
	
	private List<Timer> pendingRequestsTimers = new List<Timer>();
	private string nextMissionName;	
	private string nextSpecialRequest;
	
	#endregion
	
	//Create an instance of a worldMap object if an instance does not already exist
	public static worldMapScript Instance{
		get{
			if(instance==null){
				instance = new GameObject("worldMapScript").AddComponent<worldMapScript>();	
			}
			return instance;
		}
	}
	
	// Use this for initializations
	void Start() {
		initialRotation = cameraRotation.transform.rotation;
		
		//Initialization
		interceptionTimer = new Timer();
		interceptionTimer.Elapsed += new ElapsedEventHandler(createInterception);
		interceptionTimer.Interval = 500;
		interceptionTimer.Enabled = false;
		
		//Create array of gameobjects
		continents[0]=Africa;continents[1]=Asia;continents[2]=Australia;continents[3]=Europe;continents[4]=NorthAmerica;continents[5]=SouthAmerica;
		continentLights[0]=AfricaLight;continentLights[1]=AsiaLight;continentLights[2]=AustraliaLight;continentLights[3]=EuropeLight;continentLights[4]=NorthAmericaLight;continentLights[5]=SouthAmericaLight;
		
		// If no base exist show selection screen to create base
		if(gameManager.Instance.numBases() == 0) {
			showInitialBaseSelection = true;
		}
		foreach(Base b in gameManager.Instance.getBases ()){
			for(int y = 0;y<baseNames.Length;y++){
				if(b.getName().Equals(baseNames[y])){
					continents[y].renderer.enabled=true;
					continentLights[y].light.enabled=true;
				}
			}
		}
		
		// Set Time
		currentMonth = gameManager.Instance.getTime().Month;
		currentYear = gameManager.Instance.getTime().Year;
		
		
		// Set pendingRequests Timers list
		int pendingTime = PENDINGTIME;
		while(gameManager.Instance.getPendingRequests().Count > pendingRequestsTimers.Count) {
			Timer interceptionOverTimer = new Timer();
			interceptionOverTimer.Elapsed += new ElapsedEventHandler(interceptionOver);
			interceptionOverTimer.Interval = pendingTime;
			interceptionOverTimer.Enabled = true;
			pendingRequestsTimers.Add(interceptionOverTimer);
			
			pendingTime += 1000;
		}
	}
	
	
	void OnGUI() {
		//Button Frame
		GUI.Box(new Rect(Screen.width*2/3,Screen.height*0.05f,Screen.width/3,Screen.height*0.9f), "Geoscape");
		GUI.skin = playSkin;
		
		// Money Label
		GUI.Label(new Rect(Screen.width*2/3 + Screen.width/48*6
						,Screen.height*0.05f + Screen.height*0.9f/24*2
						,Screen.width/10*0.9f
						,Screen.height*0.9f/12)
						, "Money " + gameManager.Instance.getMoney().ToString());
		
		//Time Label
		GUI.Label(new Rect(Screen.width*2/3 + Screen.width/48*6
						,Screen.height*0.05f + Screen.height*0.9f/24*3
						,Screen.width/10*0.9f
						,Screen.height*0.9f/12)
						,gameManager.Instance.getTime().ToString("dd MMM HH:mm:ss"));
		//Time Related Buttons
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/48*4)
							,Screen.height*0.05f+Screen.height*0.9f/24*5
							,Screen.width/24*0.9f,Screen.height*0.9f/12)
							,"")
			&& enableMenuButtons ){
			startTime();
			wasFast=false;
		}
		GUI.skin = fastSkin;
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/48*7)
							,Screen.height*0.05f+Screen.height*0.9f/24*5
							,Screen.width/24*0.9f,Screen.height*0.9f/12)
							,"")
			&& enableMenuButtons){
			//Advance time, set variable for update function
			timestate = PlayState.Fast;
			startInterceptTimer();
			setInterceptFastInterval();
			wasFast=true;
		}
		GUI.skin = pauseSkin;
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/48*10)
							,Screen.height*0.05f+Screen.height*0.9f/24*5
							,Screen.width/24*0.9f,Screen.height*0.9f/12)
							,"")
			&& enableMenuButtons ){
			stopTime();
		}
		
		// You can change or remove the skin for some Controls but not others
		GUI.skin = null;
		//Other Buttons
		
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f)
							,Screen.height*0.05f+Screen.height*0.9f/24*8
							,Screen.width/3*0.9f,Screen.height*0.9f/12)
							,"Interceptions")
			&& enableMenuButtons ){
			if(timestate == PlayState.Pause) {
				wasPlaying = false;
			} else {
				stopTime();
				wasPlaying = true;
			}
			showInterceptionList = true;
			enableMenuButtons = false;
			
		}
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f)
							,Screen.height*0.05f+Screen.height*0.9f/24*11
							,Screen.width/3*0.9f,Screen.height*0.9f/12)
							,"Base Manager")
			&& enableMenuButtons ){
			stopTime();
			//Access the game manager to get to Base Manager so that game manager is an entity in Base Manager
			gameManager.Instance.setLevel("BaseManager"); 
		}
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f)
							,Screen.height*0.05f+Screen.height*0.9f/24*14
							,Screen.width/3*0.9f,Screen.height*0.9f/12)
							,"Create Installation")
			&& enableMenuButtons){
			showInstallations = !showInstallations;
		}
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f)
							,Screen.height*0.05f+Screen.height*0.9f/24*17
							,Screen.width/3*0.9f
							,Screen.height*0.9f/12)
							,"Statistics")
			&& enableMenuButtons){
			showStatistics = !showStatistics;
		}
		if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f)
							,Screen.height*0.05f+Screen.height*0.9f/24*21
							,Screen.width/3*0.9f
							,Screen.height*0.9f/12)
							,"Options")
			&& enableMenuButtons){
			//Here we we need to create a small menu that pops up on top of the screen and prevent all other bottons from working. There the player can decide
			//if he wants to keep playing, quit the game, or save his game
			showOptions = !showOptions;
			stopTime();
			enableMenuButtons = false;
		}
		#region Popup Windows
		if (showInitialBaseSelection) {
			enableMenuButtons = false;
			foreach(Base iB in gameManager.Instance.getBases()) {
				legalBaseNames.Remove(iB.getName());
			}
			
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/2);
			windowRect = GUI.Window(0, windowRect, InstallationsWindow, "Create Initial Base");
		}
		
		if(showInstallations) {
			
			foreach(Base iB in gameManager.Instance.getBases()) {
				legalBaseNames.Remove(iB.getName());
			}
			
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/2);
			windowRect = GUI.Window(1, windowRect, InstallationsWindow, "Installations");
		}
				
		if(showInterception) {
			Rect windowRect = new Rect(Screen.width/3
								,Screen.height/3
								,Screen.width/3
								,Screen.height/3);
			windowRect = GUI.Window(2, windowRect, InterceptionWindow, "Interception");	
		}
		
		if (showInterceptionList) {
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/2);
			windowRect = GUI.Window(4, windowRect, InterceptionListWindow, "Current Interceptions");
		}
		
		if (showInterceptionOver){
			Rect windowRect = new Rect(Screen.width/3
								,Screen.height/3
								,Screen.width/3
								,Screen.height/3);
			windowRect = GUI.Window(5, windowRect, InterceptionOverWindow, "Interception Over");			
		}

		// Research spawn event. Stop timer and show popup.
		if (researchSpawnEvent)
		{
			Rect windowRect = new Rect(50
								,50
								,Screen.width/4
								,Screen.height/4);
			windowRect = GUI.Window(6, windowRect, ResearchEventWindow, "New Research Available: " + researchSpawnEventWeapon.getName());
			stopTime();
		}
		// Research complete event. Stop timer and show popup.
		if (researchCompleteEvent)
		{
			Rect windowRect = new Rect(50
								,50
								,Screen.width/4
								,Screen.height/4);
			windowRect = GUI.Window(7, windowRect, ResearchEventWindow, "Research Complete: " + researchCompleteEventWeapon.getName());
			stopTime();
		}
		
		// Production complete event. Stop timer and show popup
		if (productionCompleteEvent)
		{
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,50
								,Screen.width/4
								,Screen.height/4);
			windowRect = GUI.Window(8, windowRect, ProductionEventWindow, "Production order complete: " + productionCompleteEventWeapon.getName());
			stopTime();
		}
		
		if(showStatistics) {
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/3
								,Screen.height/2);
			windowRect = GUI.Window(9, windowRect, StatisticsWindow, "");	
		}
		
		if(showInsufficientFunds) {
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/2);
			windowRect = GUI.Window(10, windowRect, InsufficientFundsWindow, "InsufficientFunds");
			enableMenuButtons = false;
		}
		
		if(showOptions) {
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/2);
			windowRect = GUI.Window(11, windowRect, OptionsWindow, "Options Window");
		}
		
		if (pickInterceptionBase) {
			Rect windowRect = new Rect(Screen.width/3
								,Screen.height/3
								,Screen.width/3
								,Screen.height/3);
			windowRect = GUI.Window(12, windowRect, InterceptionBaseWindow, "");
		}
		
		if(showYearlyPayment) {
			Rect windowRect = new Rect(Screen.width/2-Screen.width/6
								,Screen.height/2-Screen.height/6
								,Screen.width/4
								,Screen.height/3);
			windowRect = GUI.Window(13, windowRect, YearlyPaymentWindow, "Year End Report");
			enableMenuButtons = false;
		}
		
		#endregion
	}
	
	#region Windows
	
	void OptionsWindow(int windowID) {
		GUILayout.BeginArea(new Rect(0,0, Screen.width/4, Screen.height/2));
		float buttonLeft = Screen.width/8 - 50;
		float buttonHeight = 20;
		if(GUI.Button(new Rect(buttonLeft
								,buttonHeight
								,100, 20), "Save to file 1")){
			SaveState.save("xcomsavefile1.save");
		}
		buttonHeight += 30;
		if(GUI.Button(new Rect(buttonLeft, buttonHeight, 100,20),"Save to file 2")){
			SaveState.save("xcomsavefile2.save");
		}
		buttonHeight += 30;
		if(GUI.Button(new Rect(buttonLeft, buttonHeight,100,20),"Save to file 3")){
			SaveState.save("xcomsavefile3.save");
		}
		buttonHeight += 30;
		if(GUI.Button(new Rect(buttonLeft, buttonHeight,100,20),"Main Menu")){
			PersistentSounds.Instance.Stop();
			Application.LoadLevel("Menu");
		}
		buttonHeight += 30;
		if(GUI.Button(new Rect(buttonLeft, buttonHeight,100,20),"Quit Game")){
			Application.Quit();
		}
		buttonHeight += 30;
		if(GUI.Button(new Rect(buttonLeft, buttonHeight,100,20),"Close")) {
			enableMenuButtons = true;
			showOptions = false;
		}
		
		GUILayout.EndArea();
	}
	
	void StatisticsWindow(int windowID) {
		GUIStyle centeredHeading = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredHeading.alignment = TextAnchor.MiddleCenter;
		centeredHeading.fontSize = 15;
		
		GUILayout.BeginArea(new Rect(0
								,15
								,Screen.width/3
								,Screen.height/2 - 15
							));
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		GUILayout.BeginVertical();
		GUILayout.Label ("Statistics", centeredHeading);
		
		GUILayout.Label("Money: " + gameManager.Instance.getMoney());
		GUILayout.Label("Bases: " + gameManager.Instance.numBases());
		GUILayout.Label("Missions won: " + gameManager.Instance.getMissionSuccess());
		GUILayout.Label("Missions failed: " + gameManager.Instance.getMissionFailed());
		GUILayout.Label("Aliens captured: " + gameManager.Instance.getNumAliens());
		GUILayout.Label("Civilians saved: " + gameManager.Instance.CiviliansSaved);
		GUILayout.Label("Civilians killed: " + gameManager.Instance.CiviliansKilled);
		GUILayout.Label("Panic levels", centeredHeading);
		foreach(string iCon in Funding.continents){
			GUILayout.Label(iCon + " : " + gameManager.Instance.getPanicLevel(iCon));
		}
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Close", GUILayout.Width(70), GUILayout.Height(50))) {
			showStatistics = false;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical ();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
	}
	
	void InstallationsWindow(int windowID) {
		GUILayout.BeginArea(new Rect(0,20,Screen.width/4
								,Screen.height/2));
		float buttonHeight = 20;
		for(int i = 0; i < legalBaseNames.Count; i++) {

			if (GUI.Button(new Rect(Screen.width/8-50,buttonHeight, 100, 20),legalBaseNames[i])) {
				listEntry = i;
				
				showInstallations = false;
				string newBaseName = legalBaseNames[listEntry];
				
				// Initial Base Creation
				if(showInitialBaseSelection) {
					enableMenuButtons = true;
					gameManager.Instance.addBase(newBaseName);
					gameManager.Instance.createInitialBase();
					showInitialBaseSelection = false;
					for(int y = 0;y<baseNames.Length;y++){
						if(newBaseName.Equals(baseNames[y])){
							continents[y].renderer.enabled=true;
							continentLights[y].light.enabled=true;
						}
					}
				}
				
				if(gameManager.Instance.baseExist(newBaseName)) {
					print ("Base Already Exists! Do nothing");
				} else {
					print ("Base Picked: " + newBaseName);
					if(gameManager.Instance.getMoney() < BASEPRICE) {
						showInsufficientFunds = true;
					} else {
						gameManager.Instance.spendMoney(BASEPRICE);
						gameManager.Instance.addBase(newBaseName);
					
						for(int y = 0;y<baseNames.Length;y++){
							if(newBaseName.Equals(baseNames[y])){
								continents[y].renderer.enabled=true;
							}
						}
					}
				}
				
				
			}
			buttonHeight+=30;
		}
		
		GUILayout.EndArea();
	}
	
	void InterceptionListWindow(int windowID) {
		float areaLeft = 0;//Screen.width/8 - 40;
		float areaHeight = Screen.height/2-20;
		float areaWidth = Screen.width/4 - areaLeft;
		GUILayout.BeginArea(new Rect(areaLeft,20,areaWidth,areaHeight));
		
		float buttonLeft = areaWidth/2 - 50;
		float buttonHeight = 20;
		
		foreach (string inter in gameManager.Instance.getPendingRequests()) {
			if(GUI.Button(new Rect(buttonLeft, buttonHeight,100, 20),inter)) {
				if(isSpecialRequest(inter)) {
					showInterceptionList = false;
				} else {
					Debug.Log ("Starting Mission from Interception List");
					showInterceptionList = false;
					
					pickInterceptionBase = true;
					nextMissionName = inter;					
				}
			} 
			buttonHeight += 30;
		}
		
		
		if (GUI.Button(new Rect(areaWidth-100,areaHeight-40,60,20),"Close")){
			showInterceptionList = false;
			if (wasPlaying) {
				startTime();
			}
			enableMenuButtons = true;
		}
		
		GUILayout.EndArea();
	}
	
	void InterceptionOverWindow(int windowID) {
		GUILayout.BeginArea(new Rect(0,20,Screen.width/4
											,Screen.height/3));
		
		GUI.Label(new Rect(Screen.width/8-50, 10, 100, 20),"The Aliens have captured ");
		GUI.Label(new Rect(Screen.width/8-50, 35,100,20 ),gameManager.Instance.getFirstPendingRequest());
		GUI.Label(new Rect(Screen.width/8-50, 60, 100,50), "You can no longer go to this mission!");
		
		if(GUI.Button(new Rect(Screen.width/8-50, 115,100,20),"Resume")) {
			showInterceptionOver = false;
			gameManager.Instance.addPanicLevel(COUNTRY2CONTINENT[gameManager.Instance.getFirstPendingRequest()]);
			gameManager.Instance.removeFirstPendingRequest();
			
			startTime();
		}
		
		GUILayout.EndArea();
		
		
	}
	
	// Enters this window in a paused state
	void ResearchEventWindow(int windowID)
	{
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		// Switch to BM
		if (GUILayout.Button("Go to Base Manager"))
		{
			gameManager.Instance.setLevel("BaseManager"); 
		}
		// Resume game
		if (GUILayout.Button ("Resume"))
		{
			researchSpawnEvent = false;
			researchCompleteEvent = false;
			startTime();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
	
	// Enters this window in a paused state
	void ProductionEventWindow(int windowID)
	{
		GUILayout.BeginVertical();
		GUILayout.Label("Base: " + productionCompleteEventBaseName);
		GUILayout.FlexibleSpace();
		// Switch to BM
		if (GUILayout.Button("Go to Base Manager"))
		{
			gameManager.Instance.setLevel("BaseManager"); 
		}
		// Resume game
		if (GUILayout.Button ("Resume"))
		{
			productionCompleteEvent = false;
			startTime();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
	
	void InterceptionWindow(int windowID) {
		
		GUILayout.BeginArea(new Rect(0,20,Screen.width/4
								,Screen.height/4));
		
		
		GUI.Label(new Rect(Screen.width/8-75, 0, 150, 20),nextMissionName + " was Invaded!");
		
		if (GUI.Button(new Rect(Screen.width/8-50, 20, 100, 20), "Accept")) {
			showInterception = false;
			pickInterceptionBase = true;
			gameManager.Instance.addPendingRequest(nextMissionName);
		}
		if (GUI.Button(new Rect(Screen.width/8-50, 45, 100, 20), "Cancel")) {
			
			
			Timer interceptionOverTimer = new Timer();
			interceptionOverTimer.Elapsed += new ElapsedEventHandler(interceptionOver);
			interceptionOverTimer.Interval = PENDINGTIME;
			interceptionOverTimer.Enabled = true;
			pendingRequestsTimers.Add(interceptionOverTimer);
			
			gameManager.Instance.addPendingRequest(nextMissionName);
			
			showInterception = false;
			enableMenuButtons = true;
			startTime();
		}
		
		GUILayout.EndArea();
	}
	
	void InsufficientFundsWindow(int windowID) {
		GUILayout.BeginArea(new Rect(120,12	,100,100));
		
		GUILayout.Label("Insufficient Funds");
		
		if(GUILayout.Button("Resume")) {
			showInsufficientFunds = false;
			enableMenuButtons = true;
			startTime();
		}
		
		GUILayout.EndArea();
	}
	
	void InterceptionBaseWindow(int windowID) {
		enableMenuButtons = false;
		
		if (interceptMessage == "") {
			interceptMessage = "Send aircraft from which base?";
		}
		
		GUILayout.BeginVertical();
		
		if (!areYouSure)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label(interceptMessage);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			
			scrollPosition = GUILayout.BeginScrollView (scrollPosition);
			foreach (Base b in gameManager.Instance.getBases())
			{
				if (b.hasAircraft())
				{
					GUILayout.BeginHorizontal ();
					GUILayout.FlexibleSpace();
					if (GUILayout.Button(b.getName(), GUILayout.Width (125), GUILayout.Height(25)))
					{
						// Check if a soldier is missing his weapon
						bool uselessFuck = false;
						foreach (Soldier s in b.getAircraft().getSoldiers()) {
							if (s.getWeapon() == null) {
								uselessFuck = true;
							}
						}
						
						// Check if aircraft has soldiers and a pilot
						if (!b.getAircraft().hasPilot()) {
							interceptMessage = "There is no pilot on the aircraft!";
						}
						else if (b.getAircraft().numSoldiers() <= 0) {
							interceptMessage = "There are no soldiers on the aircraft!";
						}
						else if (uselessFuck) {
							interceptMessage = "One or more of your soldiers does not have a weapon!";
						}
						else {
							interceptMessage = "";
							
							// Sets current base to be the one selected
							gameManager.Instance.setCurrentBase (b.getName());
							
							areYouSure = true;
						}
					}
					GUILayout.FlexibleSpace();
					GUILayout.EndHorizontal ();
				}
			}
			GUILayout.EndScrollView();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Cancel", GUILayout.Width(75), GUILayout.Height (25)))
			{
				interceptMessage = "";
				pickInterceptionBase = false;
				enableMenuButtons = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
		}
		else
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Are you sure?");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			GUILayout.Space(15);
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("You will not be able to equip soldiers once you enter the mission.");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Yes", GUILayout.Width(75), GUILayout.Height (25)))
			{
				// Remove from interception list
				gameManager.Instance.getPendingRequests().Remove(nextMissionName);
				
				// Stop music
				PersistentSounds.Instance.Stop ();
				
				gameManager.Instance.setLevel("LoadingScreen");
			}
			GUILayout.Space(5);
			if (GUILayout.Button("No", GUILayout.Width(75), GUILayout.Height (25)))
			{
				areYouSure = false;
				pickInterceptionBase = false;
				enableMenuButtons = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
		}
		
		GUILayout.EndVertical ();
	}
	
	void YearlyPaymentWindow(int windowID) {
		GUILayout.BeginArea(new Rect(0, 20, Screen.width/4, Screen.height/3));
		
		GUILayout.BeginVertical();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("It is time to pay your Employees.");
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Employees cost: " + gameManager.Instance.costEmployees());
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Pay")) {
			gameManager.Instance.payEmployees();
			showYearlyPayment = false;
			enableMenuButtons = true;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if(GUILayout.Button("Go to Base Manager")) {
			stopTime();
			gameManager.Instance.advanceTime(-2);
			gameManager.Instance.setLevel("BaseManager");
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
	}
	
	#endregion
	
	#region Interception functions
	
	// Helper
	private bool isSpecialRequest(string sp) {
		foreach (string iS in SPECIALREQUESTS) {
			if(sp.Equals(iS)) {
				return true;
			}
		}
		return false;
	}
	
	private void createInterception(object source, ElapsedEventArgs e) {
		enableMenuButtons = false;
		stopTime();
		nextMissionName = INTERCEPTIONS[rand.Next(INTERCEPTIONS.Count)];
		showInterception = true;
		print("Interception!");
	}
	
	public void stopInterceptTimer() {
		interceptionTimer.Enabled = false;
	}
	public void startInterceptTimer() {
		interceptionTimer.Enabled = true;
	}
	
	public void setInterceptNormalInterval() {
		int frequency = gameManager.Instance.getPendingRequests().Count * 2000;
		interceptionTimer.Interval = rand.Next(57600) + frequency;
	}
	
	public void setInterceptFastInterval() {
		int frequency = gameManager.Instance.getPendingRequests().Count * 2000;
		interceptionTimer.Interval = rand.Next(2400) + frequency;
	}
	
	void interceptionOver(object source, ElapsedEventArgs e) {
		stopTime ();
		showInterceptionOver = true;
		pendingRequestsTimers[0].Enabled = false;
		pendingRequestsTimers.RemoveAt (0);
	}
	
	void stopInterceptOverTimer() {
		foreach(Timer iT in pendingRequestsTimers) {
			iT.Enabled = false;
		}
	}
	
	void startInterceptOverTimer() {
		foreach(Timer iT in pendingRequestsTimers) {
			iT.Enabled = true;
		}
	}
	
	void checkInterceptionOverTimers() {
		foreach(Timer iT in pendingRequestsTimers) {
			print ("Over Timer : " + iT.Enabled);
		}
	}
	
	#endregion
	
	#region Time Functions
	private void stopTime() {
		timestate = PlayState.Pause;
		stopInterceptTimer();
		stopInterceptOverTimer();
		//checkInterceptionOverTimers();

	}
	
	private void startTime() {
		// Debug
		if (gameManager.Instance.getPendingRequests().Count > pendingRequestsTimers.Count) {
			Debug.Log("WORLDMAP::Not enough timers for Pending Requests.");
		}
		
		timestate = PlayState.Play;
		startInterceptTimer();
		setInterceptNormalInterval();
		startInterceptOverTimer();
	}
	
	private void startTimeFast() {
		timestate = PlayState.Play;
		startInterceptTimer();
		setInterceptFastInterval();
		print ("Activating wasFast");
		wasFast=true;
	}
	#endregion
	
	
	// Update is called once per frame
	void Update () {
		
		if (timestate == PlayState.Pause){
			// To ensure all timers are turned off
			stopTime();
		}
		
		if (timestate != PlayState.Pause)
		{
			double time = 0;
			if (timestate == PlayState.Play)
			{
				time = gameManager.TIME_SLOW;
				cameraRotation.transform.Rotate(Vector3.up *10* Time.deltaTime);
			}
			if (timestate == PlayState.Fast)
			{
				time = gameManager.TIME_FAST;
				cameraRotation.transform.Rotate(Vector3.up *100* Time.deltaTime);
			}
			
			gameManager.Instance.advanceTime(time);
			
			// Heal soldiers in hospital
			foreach (Base b in gameManager.Instance.getBases())
			{
				b.healWithTime(time);
			}
			
			// Advance time for research projects
			foreach (Queue<Weapon> q in gameManager.Instance.getUnresearchedWeapons())
			{
				if (q.Count <= 0)
					continue;
				
				Weapon w = q.Peek();
				if (w.advanceTime(time))
				{
					// The weapon at the front of the queue is telling us to create an event.
					researchSpawnEvent = true;
					researchSpawnEventWeapon = w;
					break;
				}
				
				if (w.researchFinished())
				{
					researchCompleteEvent = true;
					researchCompleteEventWeapon = w;
					// We can remove this weapon from the queue and add it to the available weapon list.
					w = q.Dequeue();
					gameManager.Instance.addAvailableWeapon(w);
					
					// Free workers working on this weapon.
					foreach (Base b in gameManager.Instance.getBases())
					{
						b.cleanLab();
					}
				}
			}
			
			// Advance time for production projects
			foreach (Base b in gameManager.Instance.getBases())
			{
				foreach (ProductionOrder order in b.getActiveOrders())
				{
					order.advanceTime(time);
					if (order.isComplete())
					{
						productionCompleteEvent = true;
						productionCompleteEventWeapon = order.getWeapon();
						productionCompleteEventBaseName = b.getName();
						for (int i = 0; i < order.getWorkers(); i++)
							b.freeWorker();
						b.addManufacturedItem(order.getWeapon(), order.getQuantity());
						b.removeOrder(order);
						break;
					}
				}
			}
			
		}else{
			if(cameraRotation.transform.rotation.eulerAngles.y>20 &&cameraRotation.transform.rotation.eulerAngles.y<270){
				if(wasFast){
					cameraRotation.transform.Rotate(Vector3.up *100* Time.deltaTime);
				}else{
					cameraRotation.transform.rotation = Quaternion.Slerp(cameraRotation.transform.rotation, initialRotation, Time.deltaTime);
				}
			}else{
				cameraRotation.transform.rotation = Quaternion.Slerp(cameraRotation.transform.rotation, initialRotation, Time.deltaTime);
				wasFast=false;
			}
			
			//To check if only clicked on the base
	        if(Input.GetMouseButtonDown(0)){
				Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out mouseDown);
			}
			
			//To move the planet around
			if (Input.GetMouseButton(0))
	        {
				RaycastHit hit = new RaycastHit();
				Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out mouseDown);
				RaycastHit tempHit = mouseDown;
				tempHit.point = new Vector3(-1000000000000,-10000000000,-10000000);
	            if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
	            {
					
	                float x = -Input.GetAxis("Mouse X");
	                float y = -Input.GetAxis("Mouse Y");
	                
	
	                //Maybe need to adjust the speed here
					float speed = 150;
	                transform.Rotate(Vector3.up * x*speed* Time.deltaTime,Space.World);
					transform.Rotate(Vector3.right * y*speed* Time.deltaTime,Space.World);
	            }else{
					mouseDown=tempHit;
				}
	        }
			if(Input.GetMouseButtonUp(0)){
			        if (Physics.Raycast (Camera.main.ScreenPointToRay(Input.mousePosition), out mouseUp)){
						//Checks if mouseup and mousedown got the same item
						if(mouseUp.transform.position.x==mouseDown.transform.position.x && mouseUp.transform.position.z==mouseDown.transform.position.z
									&& mouseUp.transform.position.y==mouseDown.transform.position.y){
						
							//Checks if the clicked item is a base
							for(int i = 0;i<continents.Length;i++){
								if(continents[i].transform.position.x==mouseDown.transform.position.x && continents[i].transform.position.z==mouseDown.transform.position.z
											&& continents[i].transform.position.y==mouseDown.transform.position.y){
									
									print ("You have clicked on: "+baseNames[i]);
									foreach(Base e in gameManager.Instance.getBases ()){
										if(baseNames[i].Equals(e.getName ())){
											//Setting new base if is in bases
											gameManager.Instance.setCurrentBase(e.getName());
				
											if(true){
												//Turning off the time
												stopTime();
												//Going to Base Manager
												gameManager.Instance.setLevel ("BaseManager");
											}
											print ("Set new base name.");
										}
									}
								}
							}
						}else{
							mouseUp = new RaycastHit();
							mouseDown = new RaycastHit();
						}
					}
			}
		}
		
		#region Time Intervals interacting with GameManager
		// Monthly events
		if(currentMonth < gameManager.Instance.getTime().Month) {
			//set current month again
			currentMonth = gameManager.Instance.getTime().Month;

			gameManager.Instance.addMonthlyFunding();
		}
		
		// Yearly events
		if(currentYear < gameManager.Instance.getTime().Year) {
			//set current year again
			currentYear = gameManager.Instance.getTime().Year;
			
			//gameManager.Instance.payEmployees();
			showYearlyPayment = true;
			stopTime();
		}
		
		#endregion
	}
}
