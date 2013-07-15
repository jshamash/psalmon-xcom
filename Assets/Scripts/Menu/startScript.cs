using UnityEngine;
using System.Collections;

public class startScript : MonoBehaviour {
	
	private const string UNIQUE_NAME = "PsalmonXCOM";
	private bool isRefreshing = false;
	private HostData[] hostData = null;
	private bool waitForPlayer = false;
	private string createGameMessage = "";
	private string joinGameMessage = "";
	
	//Multiplayer GUI
	Vector2 scrollPosition = new Vector2(0,0);
	int gameSelected = -1;
	int sideSelected = -1;
	
	//These variables are used as flags to check which menu to display
		private bool showMain = false;
		private bool showSingle = false;
		private bool showMultiplayer = false;
		private bool showCreateGame = false;
		private bool showLoad = false;
		private bool showCredits = false;
	//Fields for creating new MULTIPLAYER GAME INSTANCE
		private string gameName = "";
	
	//GUISkins
		public GUISkin menuSkin;
	
	//Textures
		public Texture2D XCOMLogo;
	
	
	public void Start(){
		showMain = true;
	}
	
	private void Update(){
		if (isRefreshing) {
			hostData = null;
			if (MasterServer.PollHostList().Length > 0) {
				Debug.Log("Refresh completed");
				hostData = MasterServer.PollHostList();
				isRefreshing = false;
				MasterServer.ClearHostList();
			}
		}
	}
	
	//Function holding all the GUI interfaces
	void OnGUI(){
		//The followings represent the different menuds, encapsulated by an if-statement checking which menu is to be shown
		//Main Menu
		GUI.skin = menuSkin;
		if(showMain){
			
			GUI.DrawTexture(new Rect(Screen.width-420,Screen.height-120,400,100),XCOMLogo);
			
			GUI.Box(new Rect(Screen.width*2/3,Screen.height/5,Screen.width/4*0.95f,Screen.height*3/5), "");
			GUI.skin = menuSkin;
			if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f),Screen.height/5+(Screen.height*3/5*0.15f),Screen.width/4*0.85f,Screen.height/15),"Singleplayer Campain")){
				showMain=false;
				showSingle=true;
			}
			if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f),Screen.height/5+(Screen.height*3/5*0.3f),Screen.width/4*0.85f,Screen.height/15),"Multiplayer")){
				showMain=false;
				showMultiplayer=true;
			}
			if (GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f),Screen.height/5+(Screen.height*3/5*0.65f),Screen.width/4*0.85f,Screen.height/15),"Credits")) {
				showMain = false;
				showCredits = true;
			}
			if(GUI.Button(new Rect(Screen.width*2/3 + (Screen.width/3*0.05f),Screen.height/5+(Screen.height*3/5*0.8f),Screen.width/4*0.85f,Screen.height/15),"Quit Game")){
				Application.Quit();
				print("Currently quitting the game");
			}
			GUI.skin=null;
		}
		//Singleplayer menu
		if(showSingle){
			// Left side
			GUI.Box(new Rect(Screen.width*0.05f,Screen.height*0.15f,Screen.width*0.25f,Screen.height*0.7f),"");
			if(GUI.Button(new Rect(Screen.width*0.075f,Screen.height*0.15f+(Screen.height*0.7f/10),Screen.width*0.2f,(Screen.height*0.7f/5)),"Start new Campain")){
				//Call game manager to create a new instance of game manager, and incidently start the game
				startGame(true, null);
				print("Attempting to create new game");
			}
			if(GUI.Button(new Rect(Screen.width*0.075f,Screen.height*0.15f+(4*Screen.height*0.7f/10),Screen.width*0.2f,(Screen.height*0.7f/5)),"Load existing game")){
				//Load a past saved game
				showLoad = true;
				//SavedGameState save = SaveState.load();
				//startGame(false, save);
			}
			if(GUI.Button(new Rect(Screen.width*0.075f,Screen.height*0.15f+(7*Screen.height*0.7f/10),Screen.width*0.2f,(Screen.height*0.7f/5)),"Back")){
				showSingle=false;
				showLoad = false;
				showMain=true;
			}
			
			// Right side
			GUIStyle titleLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
			titleLabelStyle.alignment = TextAnchor.MiddleCenter;
			titleLabelStyle.fontSize = 30;
			
			GUILayout.BeginArea(new Rect(Screen.width*2/3,Screen.height*0.1f,Screen.width/3*0.95f,Screen.height*0.8f), GUI.skin.box);
			GUILayout.BeginVertical();
			scrollPosition = GUILayout.BeginScrollView (scrollPosition);
			GUILayout.Label("X-COM Origin:", titleLabelStyle);
			GUILayout.Space(20);
			int year = System.DateTime.Now.Year;
			GUILayout.Label ("It is the year "+year+". Unidentified Flying Objects (UFOs) have started appearing with disturbing regularity in the night skies. Reports of violent human abductions and horrific experimentation has struck terror into the hearts of millions. Mass public hysteria has only served to expose Earth's impotence against a vastly superior technology.Many countries have attempted to deal independently with the aliens. \n\nIn August "+(year-1)+", Japan established an anti-alien combat force; the Kiryu-Kai. Equipped with Japanese-made fighter aircraft, the Kiryu-Kai certainly looked like a powerful force. However, after 5 months of expensive operations they had yet to intercept their first UFO. The lesson was clear: this was a worldwide problem which could not be dealt with by individual countries.\n\nOn December 11, "+(year-1)+", representatives from the world's most economically powerful countries gathered secretly in Geneva. After much debate, the decision was made to establish a covert independent body to combat, investigate and defeat the alien threat. This organization would be equipped with the world's finest pilots, soldiers, scientists and engineers, working together as one multi-national force.");
			GUILayout.EndScrollView();
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		
		if (showCredits) {
			GUILayout.BeginArea(new Rect(Screen.width*2/3,Screen.height*0.1f,Screen.width/3*0.95f,Screen.height*0.8f), GUI.skin.box);
			GUILayout.BeginVertical ();
			
			GUIStyle titleLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
			titleLabelStyle.alignment = TextAnchor.MiddleCenter;
			titleLabelStyle.fontSize = 30;
			
			float colWidth = Screen.width/3*0.95f / 2;
			GUILayout.Space (15);
			GUILayout.Label ("Credits", titleLabelStyle);
			GUILayout.Space(15);
			GUILayout.BeginHorizontal ();
			GUILayout.Label("Development Team", GUILayout.Width(colWidth));
			GUILayout.BeginVertical ();
			GUILayout.Label("Jeremie Bedard", GUILayout.Width(colWidth));
			GUILayout.Label("Jake Shamash", GUILayout.Width(colWidth));
			GUILayout.Label("Jonathan Fokkan", GUILayout.Width(colWidth));
			GUILayout.Label("Christina Tran", GUILayout.Width(colWidth));
			GUILayout.EndVertical();
			GUILayout.EndHorizontal ();
			
			GUILayout.Space(15);
			GUILayout.BeginHorizontal();
			GUILayout.Label("Original music", GUILayout.Width(colWidth));
			GUILayout.Label("Costa Damoulianos", GUILayout.Width(colWidth));
			GUILayout.EndHorizontal ();
			
			GUILayout.FlexibleSpace();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Back", GUILayout.Width(70), GUILayout.Height(50))) {
				showCredits = false;
				showMain = true;
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			GUILayout.Space(15);
			GUILayout.EndVertical();
			GUILayout.EndArea();
		}
		
		if(showLoad){
			GUI.Box(new Rect(Screen.width*0.05f+Screen.width*0.25f,Screen.height*0.15f,Screen.width*0.25f,Screen.height*0.7f),"Load A Previous Game");
			/*
			FileInfo[] savePaths = DirectoryInfo.GetFiles(SaveState.savePath, "*.save");
			string[] saveFiles;
			if(savePaths.Length == 0){
				GUI.Label(new Rect(Screen.width*0.05f+Screen.width*0.25f+Screen.width*0.25f,Screen.height*0.15f+20,Screen.width*0.25f,Screen.height*0.7f),"No Save File");
			} else {
				saveFiles = new string[savePaths.Length];
				for(int i = 0; i < savePaths.Length; i++){
					saveFiles[i] = Path.GetFileName(savePaths[i]);
				}
			}
			float buttonHeight = Screen.height*0.15f+(7*Screen.height*0.7f/10);
			foreach(string iS in saveFiles){
				if(GUI.Button(new Rect(Screen.width*0.075f,buttonHeight,Screen.width*0.2f,(Screen.height*0.7f/5)), iS)) {
					SavedGameState save = SaveState.load(iS);
					startGame(false, save);	
				}
				buttonHeight += (Screen.height*0.7f/5);
			}
			*/
			float buttonHeight = Screen.height*0.15f+(Screen.height*0.7f/10);
			float buttonleft = Screen.width*0.05f+Screen.width*0.25f + Screen.width*0.05f/2;
			if(GUI.Button(new Rect(buttonleft,buttonHeight,Screen.width*0.2f,(Screen.height*0.7f/5)), "Load File 1")){
				SavedGameState save = SaveState.load("xcomsavefile1.save");
				startGame(false, save);
			}
			buttonHeight += (Screen.height*0.9f/5);
			if(GUI.Button(new Rect(buttonleft,buttonHeight,Screen.width*0.2f,(Screen.height*0.7f/5)), "Load File 2")){
				SavedGameState save = SaveState.load("xcomsavefile2.save");
				startGame(false, save);	
			}
			buttonHeight += (Screen.height*0.9f/5);
			if(GUI.Button(new Rect(buttonleft,buttonHeight,Screen.width*0.2f,(Screen.height*0.7f/5)), "Load File 3")){
				SavedGameState save = SaveState.load("xcomsavefile3.save");
				startGame(false, save);	
			}
			
		}
		
		
		//Multiplayer Menu
		if(showMultiplayer){
			
			// ag = available games, gd = game description
			float left = Screen.width/2+Screen.width/2*0.05f;
			float agTop = Screen.height*0.10f;
			float boxWidth = Screen.width/2*0.85f;
			float agHeight = Screen.height*0.45f;
			float margin = 30;
			float gdHeight = Screen.height-2*agTop-agHeight-margin;
			float buttonWidth = 100;
			float buttonHeight = 50;
			float contentWidth = 250;
			
			GUILayout.BeginArea(new Rect(left, agTop, boxWidth, agHeight), GUI.skin.box);
			GUILayout.BeginVertical();
			
			GUILayout.Label("Available Games");			
			
			
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			if (hostData == null || hostData.Length <= 0) {
				GUILayout.Label("No active hosts");
			}
			else {
				// Copy game names to string array
				string[] names = new string[hostData.Length];
				for (int i = 0; i < hostData.Length; i++) {
					names[i] = hostData[i].gameName;
				}
					
				// Selection grid showing game names
				GUILayout.FlexibleSpace();
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				gameSelected = GUILayout.SelectionGrid(gameSelected, names, 1, GUILayout.MinWidth(contentWidth), GUILayout.ExpandWidth(false));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.FlexibleSpace();
			}
			GUILayout.EndScrollView();
			
			// Refresh button
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if(GUILayout.Button("Refresh", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))){
				//Refresh available game instances
				refreshHostList();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
						
			
			GUILayout.BeginArea(new Rect(left, agTop+agHeight+margin, boxWidth, gdHeight), GUI.skin.box);
			GUILayout.BeginVertical();
			
			if (gameSelected < 0 || hostData == null || hostData.Length <= 0)
			{
				GUILayout.Label("Select a game");				
			}
			else if (hostData[gameSelected].connectedPlayers >= 2)
			{
				GUILayout.Label("This game is full");
			}
			else
			{
				GUILayout.Label(hostData[gameSelected].gameName);
				GUILayout.FlexibleSpace();
				if (hostData[gameSelected].comment == "0") {
					GUILayout.Label ("Your side: Soldiers");
				}
				else if (hostData[gameSelected].comment == "1") {
					GUILayout.Label ("Your side: Aliens");
				}
				else {
					Debug.Log("Got a weird comment: " + hostData[gameSelected].comment);
				}
				GUILayout.FlexibleSpace();
				
				GUILayout.BeginHorizontal ();
				GUILayout.FlexibleSpace();
				if (GUILayout.Button ("Join", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight)))
				{
					Debug.Log ("Start game " + hostData[gameSelected].gameName);
					showMultiplayer = false;
					Network.Connect(hostData[gameSelected]);
				}
				GUILayout.Space(5);
				GUILayout.Label(joinGameMessage);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal ();
			}
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			
			// Left panel
			GUI.Box(new Rect(Screen.width*0.05f,Screen.height*0.15f,Screen.width*0.25f,Screen.height*0.7f),"");
			if(GUI.Button(new Rect(Screen.width*0.075f,Screen.height*0.15f+(Screen.height*0.7f/10),Screen.width*0.2f,(Screen.height*0.7f/5)),"Create game")){
				//Create a multiplayer instance
				showMultiplayer=false;
				showCreateGame=true;
				print("Creating new game");
			}
			if(GUI.Button(new Rect(Screen.width*0.075f,Screen.height*0.15f+(7*Screen.height*0.7f/10),Screen.width*0.2f,(Screen.height*0.7f/5)),"Back")){
				// Go back
				showMultiplayer=false;
				showMain=true;
			}
		}
		
		
		//Create Game menu (Multiplayer)
		if(showCreateGame){
			float buttonWidth = 100;
			float buttonHeight = 50;			
			
			GUILayout.BeginArea(new Rect(Screen.width/2,Screen.height*0.05f,Screen.width/2*0.95f,Screen.height*0.9f), GUI.skin.box);
			GUILayout.BeginVertical();
			
			GUILayout.FlexibleSpace();
			
			GUILayout.Label("Host a game");
			
			GUILayout.FlexibleSpace();
			
			//Text field for writing name of the game
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Game name: ");
			GUILayout.Space (100);
			gameName = GUILayout.TextField(gameName, 50, GUILayout.MinWidth(200));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			
			GUILayout.FlexibleSpace();
			//GUILayout.Space(vSpace);
			
			// Toolbar for selecting game type
			GUILayout.Label("Pick your side: ");
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			sideSelected = GUILayout.Toolbar(sideSelected, new string[] {"Aliens", "Soldiers"}, GUILayout.Height(50), GUILayout.Width(200));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal ();
			
			GUILayout.FlexibleSpace();
			//GUILayout.Space(vSpace);
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Back", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))) {
				//Get back to Multiplayer menu
				createGameMessage = "";
				showCreateGame=false;
				showMultiplayer=true;
			}
			GUILayout.Space (60);
			if(GUILayout.Button("Create", GUILayout.Width(buttonWidth), GUILayout.Height(buttonHeight))){
				//Create new game instance.  Make certain all fields are used
				if(gameName == "") {
					//Print message saying information are missing	
					createGameMessage = "ERROR: Enter name";
				} else if (sideSelected < 0) {
					createGameMessage = "ERROR: Pick a side";
				} else {
					//All conditions are met to create a new game instance, create game instance
					createGameMessage = "";
					startServer(gameName);
				}
			}			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.Space(10);
			GUILayout.Label(createGameMessage);
			GUILayout.FlexibleSpace();
			
			GUILayout.EndVertical();
			GUILayout.EndArea();
			
			//GUI.skin=null;
		}
		if (waitForPlayer) {
			showCreateGame = false;
			Rect windowRect = new Rect(
				Screen.width/2 - Screen.width/6,
				Screen.height/2 - Screen.height/6,
				Screen.width/3,
				Screen.height/3
				);
			GUI.Window(0, windowRect, waitWindow, "");
		}
	}
	
	/*
	 * isNew : true if starting new game else false
	 * sav: null if isNew else comes from SaveState.load()
	 */
	private void startGame(bool isNew, SavedGameState sav){
		print("Starting Game");
		//Tell Unity not to destroy this instance when we load up a new scene
		DontDestroyOnLoad(gameManager.Instance);
		if(isNew) {
			//Calling a method in our state manager to start a new game 
			gameManager.Instance.startState();
		} else {
			gameManager.Instance.setState(sav);
		}
	}
	
	private void startServer(string name) {
		Debug.Log("Starting Server");
		createGameMessage = "";
		// Start waiting
		waitForPlayer = true;
		Network.InitializeServer(2, 25001, !Network.HavePublicAddress());
		// Advertises the side selected (0=alien, 1=soldier)
		MasterServer.RegisterHost(UNIQUE_NAME, name, "" + sideSelected);
	}
	
	void OnServerInitialized() {
		Debug.Log ("Server initialized");
	}
	
	void OnMasterServerEvent(MasterServerEvent mse) {
		if (mse == MasterServerEvent.RegistrationSucceeded)
		{
			Debug.Log("Registered server");
		}
		else if (mse == MasterServerEvent.RegistrationFailedGameName)
		{
			Debug.Log("Failed Game Name");
			createGameMessage = "ERROR: try a different game name";
			waitForPlayer = false;
		}
		else if (mse == MasterServerEvent.RegistrationFailedGameType)
		{
			Debug.Log("Failed Game Type");
			createGameMessage = "ERROR: game type";
			waitForPlayer = false;
		}
		else if (mse == MasterServerEvent.RegistrationFailedNoServer)
		{
			Debug.Log("Failed No Server");
			createGameMessage = "ERROR: Couldn't initialize server";
			waitForPlayer = false;
		}
	}
	
	void refreshHostList() {
		MasterServer.RequestHostList(UNIQUE_NAME);
		isRefreshing = true;
	}
	
	void waitWindow(int windowID) {
		GUILayout.BeginVertical();
		GUILayout.FlexibleSpace();
		GUILayout.Label("Waiting for another player to connect...");
		GUILayout.Space (15);
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Cancel", GUILayout.Width(100), GUILayout.Height(50)))
		{
			// Disconnect
			Network.Disconnect();
			MasterServer.UnregisterHost();
			waitForPlayer = false;
			showCreateGame = true;
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.FlexibleSpace();
		GUILayout.EndVertical();
	}
	
	void OnPlayerConnected() {
		Debug.Log ("Client connected");
		waitForPlayer = false;
		// Don't allow any more connections
		Network.maxConnections = -1;
		StartMultiplayerGame ();
	}
	
	#region Client Status
	void OnConnectedToServer() {
		Debug.Log ("Connected to server");
		joinGameMessage = "";
		StartMultiplayerGame();
	}
	
	void OnFailedToConnect(NetworkConnectionError error) {
		Debug.Log ("failed to connect " + error);
		showMultiplayer = true;
		joinGameMessage = "Failed to connect: " + error;		
	}
	
	 void OnFailedToConnectToMasterServer(NetworkConnectionError info) {
        Debug.Log("Could not connect to master server: " + info);
    }
	
	#endregion
	
	void StartMultiplayerGame() {
		// Create MultiplayerState and set side.
		Debug.Log("Starting multiplayer game");
		DontDestroyOnLoad(MultiplayerState.Instance);
		
		if (Network.isServer) {
			MultiplayerState.Instance.Side = sideSelected;
		}
		if (Network.isClient) {
			if (hostData[gameSelected].comment == "0") {
				MultiplayerState.Instance.Side = MultiplayerState.SOLDIERS;
			}
			else if (hostData[gameSelected].comment == "1") {
				MultiplayerState.Instance.Side = MultiplayerState.ALIENS;
			}
			else {
				Debug.Log("Invalid comment: " + hostData[gameSelected].comment);
			}
		}
		
		Debug.Log("Loading level, side = " + MultiplayerState.Instance.Side);
		Application.LoadLevel("MultiplayerMission");
	}
}