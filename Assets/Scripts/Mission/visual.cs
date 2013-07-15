using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class visual : MonoBehaviour {
	
	// Turn based variables -- to be used later
	private bool playerTurn = true;
	
	//weapon related variables
	private int ammoLeft;
	private int maxAmmo;
	private string weaponName;
	private bool containsRifle;
	private bool containsShotgun;
	private bool containsSniper;
	private bool shootingMode;
	private float shootwait;
	private int range;
	public int currentShootSoldierIndex;
	public static bool currentShootSoldierHit = false;
	private bool instanceCurrentShootSoldierHit = false;

	
	//Audio
	//public AudioSource audioSource;
	public AudioClip reloadSource;
	//public AudioClip alienActivitySource;
	
	//Variables for initializing game
		private Soldier[] soldiers = new Soldier[8];
		private GameObject[] soldierPrefabs = new GameObject[8];
		private GameObject[] seekingSpherePrefabs=new GameObject[8];
		private GameObject[] alienSeekingSpherePrefabs;
		private GameObject[] alienPrefabs;
		private Alien[] aliens;
		
		//Used to create instances ob objects
		public GameObject soldierPrefab;
		public GameObject seekingSpherePrefab;
		public GameObject alien1;
		public GameObject alien2;
		public GameObject alien3;
		public GameObject nuke;
	
		//Used to set all
		private int selectedSoldier = 9;
		private int numbSoldier;
		private int numbAliens;
	
	//Gui Skins
	public GUISkin middleAlignment;
	public GUISkin invisiBox;
	public Texture soldierPortrait;
	public Texture2D healthBarFull;
	public Texture2D energyBarFull;
	public Texture2D emptyBar;
	public Texture2D weaponImage; 
	public Texture2D alienActivity;
	public Texture2D crosshair;
	
	//Timer related variables
	private float startTime;
	private string textTime;
	private float pauseTime;
	
	//GUI Skins + textures
	public GUISkin optionFrameSkin;
	public Texture optionTexture;
	
	private Vector2 scrollPosition = new Vector2(0,0);
	
	//Variables for pop up windows
	bool optionFrame = false;
	public GameObject equipMenu;
	private GameObject equip;
	private bool areYouSureAbort = false;
	private bool areYouSureExit = false;
	private bool victory = false;
	private bool failed = false;
	private bool aborted = false;
	
	//For smoothing camera movement
		public float smoothTime = 0.5F;
		private Vector3 velocity = Vector3.zero;
		Vector3 targetPosition;
		Quaternion targetRotation;
		Vector3 previousTargetPosition;
		Quaternion previousTargetRotation;
		int vision;
		private float speed = 50.0f;
	
	//Variables for AI
		private int currentAlienAI;
	
	void OnGUI(){
		
		//VICTORY SCREEN!
		if(victory){
			// Calculate aliens & soldiers killed
			int aliensKilled = 0, soldiersKilled = 0, aliensKept = 0;
			foreach (Soldier s in soldiers) {
				//TODO dead soldier count isn't working?
				if (s != null && s.getHealth () <= 0)
					soldiersKilled++;
			}
			foreach (Alien a in aliens) {
				if (a.getHealth() <= 0)
					aliensKilled++;
			}
			
			// Calculate aliens being taken back to base
			Base curBase = gameManager.Instance.getCurrentBase();
			int space = curBase.getContainmentSpace() - curBase.getAlienTotal();
			if (aliensKilled <= space)
				aliensKept = aliensKilled;
			else
				aliensKept = space;
			
			
			GUILayout.BeginArea(new Rect(Screen.width/3,Screen.height/3 - 15,Screen.width/3,Screen.height/3 + 30), GUI.skin.box);
			scrollPosition = GUILayout.BeginScrollView(scrollPosition);
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Victory!");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Aliens Killed: " + aliensKilled);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Soldiers Killed: " + soldiersKilled);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Alien bodies kept: " + aliensKept);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			int saved = 0;
			int civTotal = 0;
			//TODO civilians are all showing up as null
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Civilian")) {
				Civilian c = (Civilian) go.GetComponent(typeof(Civilian));
				civTotal++;
				if (c != null && c.alive)
					saved++;
			}
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Civilians killed: " + (civTotal-saved));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Civilians saved: " + saved);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Money earned: $" + gameManager.MISSION_PAYOFF);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Continue", GUILayout.Width (70), GUILayout.Height(50)))
			{
				loadWorldMap();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical ();
			GUILayout.EndScrollView();
			GUILayout.EndArea();
			
			return;
		}
		
		//LOSING SCREEN!
		if(failed){
			// Calculate aliens & soldiers killed
			int aliensKilled = 0, soldiersKilled = 0;
			foreach (Soldier s in soldiers) {
				// TODO same as victory screen comments
				if (s != null && s.getHealth () <= 0)
					soldiersKilled++;
			}
			foreach (Alien a in aliens) {
				if (a.getHealth() <= 0)
					aliensKilled++;
			}
			
			GUILayout.BeginArea(new Rect(Screen.width/3,Screen.height/3,Screen.width/3,Screen.height/3), GUI.skin.box);	
			GUILayout.BeginVertical();
			GUILayout.FlexibleSpace();
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Mission Failed");
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Aliens Killed: " + aliensKilled);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Soldiers Killed: " + soldiersKilled);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			int saved = 0;
			int civTotal = 0;
			//TODO same as victory screen comments
			foreach (GameObject go in GameObject.FindGameObjectsWithTag("Civilian")) {
				Civilian c = (Civilian) go.GetComponent(typeof(Civilian));
				civTotal++;
				if (c != null && c.alive)
					saved++;
			}
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Civilians killed: " + (civTotal-saved));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label("Civilians saved: " + saved);
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			
			if (GUILayout.Button("Continue", GUILayout.Width (70), GUILayout.Height(50)))
			{
				loadWorldMap();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical ();
			GUILayout.EndArea();
			
			return;
		}
		
		//MISSION ABORTED SCREEN
		if(aborted){
			GUI.Box (new Rect(Screen.width/2-100,Screen.height/2-50,200,100),"Mission aborted.  Nuke on its way.");
			return;
		}
		
		if(shootingMode){
			Screen.showCursor = false;
				GUI.DrawTexture(new Rect(Input.mousePosition.x-20,Screen.height-Input.mousePosition.y-20,40,40),crosshair);
		}
		
		//Box for Options
		if(GUI.Button (new Rect(Screen.width-40,0,40,30),optionTexture)){
			optionFrame=true;
			pauseTime = Time.time;			
						
		}
		
		if(playerTurn){
			if(!optionFrame){	
				//Align all text to be in middle of rects
				GUI.skin = middleAlignment;
				
				//Box for selected person information
				float selectionWidth = Screen.width/3;
				if(selectionWidth>410)
					selectionWidth=410;
				GUI.Box (new Rect(0,0,selectionWidth,145),"");
				string tempName = "Select a character.";
				if(selectedSoldier<numbSoldier){
					
					tempName = "Soldier "+ soldiers[selectedSoldier].getName ();	
					GUI.DrawTexture(new Rect(10,10,113,111), soldierPortrait);
					
					GUI.Label (new Rect(125,40,60,20),"Health: ");
					GUI.Label (new Rect(125,70,60,20),"Energy: ");
					
					//Gui for health bars
					GUI.skin = invisiBox;
					
					float barSize = selectionWidth - 195;
					if(barSize>206)
						barSize=206;
					
					// draw the healthBar/energybar
					//Draw Health Bar
			        GUI.Box (new Rect (185,40, barSize, 30),emptyBar);
			        // draw the filled-in part:
			        GUI.BeginGroup (new Rect (185, 40, barSize * soldiers[selectedSoldier].getHealth ()/100+10, 30));
			            GUI.Box (new Rect (0,0, barSize, 30),healthBarFull);
			        GUI.EndGroup ();
					GUI.Label (new Rect (185,40, barSize, 30),""+soldiers[selectedSoldier].getHealth());
					
					//Draw Energy Bar
					GUI.Box (new Rect (185,70, barSize, 30),emptyBar);
			        // draw the filled-in part:
					float tempFloat = (float)soldiers[selectedSoldier].getEnergy();
					float otherTempFloat = tempFloat/100;
			        GUI.BeginGroup (new Rect (185, 70, barSize*otherTempFloat+10, 30));
			            GUI.Box (new Rect (0,0, barSize, 30),energyBarFull);
			        GUI.EndGroup ();
					GUI.Label (new Rect (185,70, barSize, 30),""+tempFloat);
					GUI.skin = middleAlignment;
					
					/*
					Color prevCol = GUI.backgroundColor;
					GUI.backgroundColor = Color.green;
					if(GUI.Button (new Rect(190,105,barSize*2/3,30),"Equip Soldier")){
						equip = (GameObject)Instantiate (equipMenu);
						playerTurn=false;
						optionFrame=true;
					}
					GUI.backgroundColor = prevCol;
					*/
				}
				GUI.Label(new Rect(130,10, selectionWidth-130,20),tempName);
			
				//Time box for ending turn and see time remaining an time in game
				GUI.Box (new Rect(Screen.width*3/7,0,Screen.width/7,Screen.height/10),"Timer");
				//Time
				float guiTime = Time.time - startTime;
	 
	   			int minutes = (int)guiTime / 60;
	   			int seconds = (int)guiTime % 60;
	   			int fraction = (int)(guiTime * 100) % 100;
				
				textTime = string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction); 
	   			GUI.Label (new Rect (Screen.width/3, 10, Screen.width/3, Screen.height/10), textTime);
				
				
				//END TURN BUTTON 
				Color oldCol = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;
				if(GUI.Button(new Rect(Screen.width/2-Screen.width/28, Screen.height/10, Screen.width/14, Screen.height/32), "End Turn"))
				{
					if(shootingMode){
						targetPosition=previousTargetPosition;
						targetRotation= previousTargetRotation;
						
						//Now cursor will look like a rifle 
						Screen.showCursor = true;
						
						shootingMode=false;
					}
					
					playerTurn = false;
					Screen.showCursor = false;
					//pauseTime = Time.time;	
					AlienTurn();
					
					//Deactivate shooting mode animation
					((Walking)soldierPrefabs[selectedSoldier].GetComponent(typeof(Walking))).deactivateShootingMode();	
				}
				
				GUI.backgroundColor = oldCol;
				
				//Box for left hand weapon
				if(soldiers[selectedSoldier].getWeapon()!=null){
					GUI.Box (new Rect(0,Screen.height*5/6,Screen.width*3/10,Screen.height/6),"Weapon: " + weaponName);
					if(selectedSoldier<numbSoldier){
						
						maxAmmo = soldiers[selectedSoldier].getWeapon().getMaxAmmo();
						ammoLeft = soldiers[selectedSoldier].getAmmoLeft();
						weaponImage = soldiers[selectedSoldier].getWeapon().getImage ();
						weaponName = soldiers[selectedSoldier].getWeapon().getName();
						
						//string comparison
						containsRifle = weaponName.Contains("Rifle");
						containsShotgun = weaponName.Contains("Shotgun");
						containsSniper = weaponName.Contains("Sniper");
						GUI.DrawTexture(new Rect(0,Screen.height*43/50,Screen.width*2/9,Screen.height/10), weaponImage);
						GUI.Label (new Rect(0,Screen.height*23/25,Screen.width/5,Screen.height/8),"Ammo:  " + ammoLeft + " / " + maxAmmo);
						
						if(shootingMode){
							if( GUI.Button(new Rect(Screen.width*11/50,Screen.height*43/50,Screen.width/18,Screen.height/20), "Exit")){
								targetPosition=previousTargetPosition;
								targetRotation= previousTargetRotation;
								
								//Now cursor will look like a rifle 
								Screen.showCursor = true;
								
								shootingMode=false;
								((Walking)soldierPrefabs[selectedSoldier].GetComponent(typeof(Walking))).deactivateShootingMode();	
							}
						}else{
							if( GUI.Button(new Rect(Screen.width*11/50,Screen.height*43/50,Screen.width/18,Screen.height/20), "Shoot")){
	
								//Start by stopping soldier from moving
								((SeekerSoldier)soldierPrefabs[selectedSoldier].GetComponent(typeof(SeekerSoldier))).stopWalking();
								
								previousTargetPosition = targetPosition;
								previousTargetRotation = targetRotation;
								
								targetPosition = soldierPrefabs[selectedSoldier].transform.position + (new Vector3(0,4,0)) + (soldierPrefabs[selectedSoldier].transform.forward.normalized*-2)
									+ (soldierPrefabs[selectedSoldier].transform.right.normalized*2);
								targetRotation.eulerAngles = soldierPrefabs[selectedSoldier].transform.eulerAngles;
								
								//Now cursor will look like a rifle 
								Screen.showCursor = false;
								
								shootingMode=true;
								((Walking)soldierPrefabs[selectedSoldier].GetComponent(typeof(Walking))).activateShootingMode();	
								
								shootwait = 0;
							}
						}
						
						if(GUI.Button(new Rect(Screen.width*11/50,Screen.height*23/25,Screen.width/18,Screen.height/20), "Reload")){
							
							if(selectedSoldier<numbSoldier){
								maxAmmo = soldiers[selectedSoldier].getWeapon().getMaxAmmo();
								ammoLeft = soldiers[selectedSoldier].getAmmoLeft();
								
								if(ammoLeft < 10 ) {
									soldiers[selectedSoldier].setAmmoLeft(maxAmmo);
									audio.PlayOneShot(reloadSource);
								}
							}
							
						}
						
					}
				}
				
				//Time box for ending turn and see time remaining an time in game
				GUI.Box (new Rect(Screen.width*2/3,Screen.height*4/5 +60,Screen.width/3,Screen.height/10),"Additionnal options");
				
				
			}else{//Here we have the option menu that will pop up on the screen.
				GUI.Box (new Rect(Screen.width/3,Screen.height/3,Screen.width/3,Screen.height/3),"Option Menu");
				GUI.skin = optionFrameSkin;
				
				
				if(areYouSureExit){
					//GUI.Box(new Rect(Screen.width/3+Screen.width/9,Screen.height/3+Screen.height/9,Screen.width/9,Screen.height/9), "");
					GUI.Label(new Rect(Screen.width/3,Screen.height/3+20,Screen.width/3,Screen.height/3-Screen.height/27-20),"Are you certain you want to exit to the main menu? Quitting now will lose all unsaved progress.");
					if(GUI.Button(new Rect(Screen.width/3+Screen.width/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"Yes")){
						gameManager.Instance.deleteInstance();
						Application.LoadLevel("Menu");
					}
					if(GUI.Button(new Rect(Screen.width/3+Screen.width*3/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"No")){
						areYouSureExit=false;
					}
				}
				
				if(areYouSureAbort){
					//GUI.Box(new Rect(Screen.width/3+Screen.width/9,Screen.height/3+Screen.height/9,Screen.width/9,Screen.height/9), "");
					GUI.Label(new Rect(Screen.width/3,Screen.height/3+20,Screen.width/3,Screen.height/3-Screen.height/27-20),"Are you certain you want to exit to the world map menu? Aborting the mission means letting your troops die. All civilians will most likely be abducted.");
					if(GUI.Button(new Rect(Screen.width/3+Screen.width/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"Yes")){
						//Start by killing off all soldier in the aircraft and in the base
						foreach (Soldier s in soldiers) {
							if (s != null) {
								gameManager.Instance.getCurrentBase().getAircraft().removeSoldier(s);
								gameManager.Instance.getCurrentBase().removeSoldier (s);
								gameManager.Instance.getCurrentBase().dequip(s.getWeapon());
								gameManager.Instance.getCurrentBase().dequip(s.getArmor());
							}
						}
						//create nuke
						Instantiate(nuke,soldierPrefabs[selectedSoldier].transform.position+new Vector3(0,80,0),nuke.transform.rotation);
						Invoke ("loadWorldMap",7);
						aborted=true;
					}
					if(GUI.Button(new Rect(Screen.width/3+Screen.width*3/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"No")){
						areYouSureAbort=false;
					}
				}
				
				if(!areYouSureAbort && !areYouSureExit){
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height/21,Screen.width*3/15,Screen.height/21),"Resume Gameplay")){
							optionFrame=false;//Revert back to gameplay
							pauseTime = Time.time-pauseTime;
							startTime+=pauseTime;
							//More actions to be done like resume time
							//
							//
							//
					}
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height*2.5f/21,Screen.width*3/15,Screen.height/21),"Abort Mission")){
							//Make pop up window asking if that is really what the player wants
						areYouSureAbort=true;
					}				
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height*5.5f/21,Screen.width*3/15,Screen.height/21),"Exit Gameplay")){
							//Make pop up window asking if that is really what the player wants
							areYouSureExit=true;
						
					}
				}
				GUI.skin = null;
			}
		}else{
			if(optionFrame){
				if(equip==null){
					playerTurn=true;
					optionFrame=false;
					setInGameWeapons();
				}
			}else{
				GUI.DrawTexture(new Rect(Screen.width/2-169,Screen.height/2-26,338,52),alienActivity);
				//audio.PlayOneShot(alienActivitySource);
			}
		}
		GUI.skin=null;
	}
	
	
	
	//Initialize all objects and variables for the game
	void Start(){
		victory= false;
		failed = false;
		aborted= false;
		
		areYouSureAbort = false;
		areYouSureExit = false;
		
		shootingMode=false;
		
		targetPosition = transform.position;
		targetRotation = transform.rotation;
		vision=0;
		
		Destroy(loadingScript.Instance);
		
		//AudioSource.PlayClipAtPoint(audioSource.clip, Camera.main.transform.position, 0.1f);
		//audio.Play();
		
		
		BuildGraph.instance.Scan();
		
		startTime = Time.time;	
		
		List<Base> bases = gameManager.Instance.getBases();
		Base tempBase=null;
		foreach (Base b in bases){
			if(b.getHiredSoldiers().Count>0){
				tempBase=b;
				break;
			}
		}
		if(tempBase!=null){
			//TEMPORARY DECLARATION OF NUMBER OF SOLDIERS
			numbSoldier = tempBase.getHiredSoldiers().Count;
			if(numbSoldier>8){
				numbSoldier=8;	
			}
			
			// TODO shouldn't this be using aircraft soldiers?
			Soldier[] baseSoldiers = new Soldier[tempBase.getHiredSoldiers().Count];
			tempBase.getHiredSoldiers().CopyTo(baseSoldiers,0);
			
			int tempX = 30,tempZ=120;
			int tempId=0;
			SeekerSoldier tempSeeker;
			selectedSoldier=0;
			
			if(gameManager.Instance.getActiveLevel().Equals("Terrain")){
				while(tempId<numbSoldier){
					soldiers[tempId]=baseSoldiers[tempId];
					soldiers[tempId].setAmmoLeft(soldiers[tempId].getWeapon().getMaxAmmo());
					soldiers[tempId].setWeaponRange(soldiers[tempId].getWeapon().getRange());
					
					Vector3 spawnPos = new Vector3(tempX, 25.5f, tempZ);
					soldierPrefabs[tempId]=(GameObject)Instantiate(soldierPrefab as GameObject, spawnPos, Quaternion.identity);
					//soldierPrefabs[tempId].name = "Soldier"+tempId;
					seekingSpherePrefabs[tempId]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, spawnPos, Quaternion.identity);
					
					//Get Seeker and set seeking sphere to soldier
					tempSeeker = (SeekerSoldier) soldierPrefabs[tempId].GetComponent(typeof(SeekerSoldier));
					tempSeeker.setTarget(seekingSpherePrefabs[tempId]);
					
					((SeekerSoldier) soldierPrefabs[tempId].GetComponent(typeof(SeekerSoldier))).setCurSoldier(soldiers[tempId]);
					//Sets soldier in soldier state of the soldier prefab
					((SoldierState) soldierPrefabs[tempId].GetComponent(typeof(SoldierState))).setSoldier(soldiers[tempId]);
					
					//Get next position for next soldier
					switch (tempX){
						case 30:
							tempX=38;
							break;
						case 38:
							if(tempZ==120){
								tempX=30;
								tempZ=112;
							}else{
								tempX=34;
								tempZ=116;
							}
							break;
						case 34:
							tempX=42;
							break;
						case 42:
							tempX=34;
							tempZ=108;
						break;
					}
					tempId++;
				}
			}else{
				soldiers = new Soldier[numbSoldier];
				for(int i = 0;i<numbSoldier;i++){
					soldiers[i]= baseSoldiers[i];
					soldiers[tempId].setAmmoLeft(soldiers[tempId].getWeapon().getMaxAmmo());
					soldiers[tempId].setWeaponRange(soldiers[tempId].getWeapon().getRange());
					
					Vector3 spawnPos = getRandomSoldierSpawnPoint();
					soldierPrefabs[i] =(GameObject)Instantiate(soldierPrefab as GameObject, spawnPos, Quaternion.identity);
					//GameObject tempSeekingSphere=(GameObject)Network.Instantiate(seekingSpherePrefab as GameObject, spawnPos, Quaternion.identity,2);
					
					//Get Seeker and set seeking sphere to soldier
					seekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, spawnPos, Quaternion.identity);
					
					//Get Seeker and set seeking sphere to soldier
					tempSeeker = (SeekerSoldier) soldierPrefabs[i].GetComponent(typeof(SeekerSoldier));
					tempSeeker.setTarget(seekingSpherePrefabs[i]);
					
					((SeekerSoldier) soldierPrefabs[i].GetComponent(typeof(SeekerSoldier))).setCurSoldier(soldiers[i]);
					//Sets soldier in soldier state of the soldier prefab
					((SoldierState) soldierPrefabs[i].GetComponent(typeof(SoldierState))).setSoldier(soldiers[i]);
				}
			}
		}
		selectedSoldier = 0;
		((SoldierState) soldierPrefabs[selectedSoldier].GetComponent(typeof(SoldierState))).selectCharacter();
		
		setInGameWeapons();
		
		//Create all aliens on field
		Vector3 [] positions = new Vector3[28];
		positions[0] = new Vector3(105,25.5f,10);
		positions[1] = new Vector3(100,25.5f,10);
		positions[2] =  new Vector3(95,25.5f,10);
		positions[3] = new Vector3(102.5f,25.5f,15);
		positions[4] =  new Vector3(97.5f,25.5f,15);
		positions[5] = new Vector3(105,25.5f,20);
		positions[6] = new Vector3(100,25.5f,20);
		positions[7] =  new Vector3(95,25.5f,20);
		positions[8] = new Vector3(102.5f,25.5f,25);
		positions[9] =  new Vector3(97.5f,25.5f,25);
		positions[10] = new Vector3(105,25.5f,30);
		positions[11] = new Vector3(100,25.5f,30);
		positions[12] =  new Vector3(95,25.5f,30);
		positions[13] = new Vector3(102.5f,25.5f,35);
		positions[14] =  new Vector3(97.5f,25.5f,35);
		
		//Randomize nubmer of aliens on map
		
		int numbAlien1 = Random.Range(3,6);
		int numbAlien2 = Random.Range (0,5);
		int numbAlien3 = Random.Range (1,3);
		numbAliens = numbAlien1+numbAlien2+numbAlien3;
		
		int index = 0;
		aliens = new Alien[numbAliens];
		
		alienPrefabs= new GameObject[numbAliens];
		alienSeekingSpherePrefabs = new GameObject[numbAliens];
		for(int i = 0;i<numbAliens;i++,index++){
			//Set alien type
			if(i<numbAlien1){
				aliens[i] = new Alien(0);
				if(gameManager.Instance.getActiveLevel().Equals("Terrain")){
					alienPrefabs[i] = (GameObject)Instantiate(alien1, positions[index], Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, positions[index], Quaternion.identity);
				}else{
					alienPrefabs[i] = (GameObject)Instantiate(alien1, getRandomAlienSpawnPoint()+(new Vector3(0,2,0)), Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, alienPrefabs[i].transform.position, Quaternion.identity);
				}
			}else if(i<numbAlien1+numbAlien2){
				aliens[i] = new Alien(1);
				if(gameManager.Instance.getActiveLevel().Equals("Terrain")){
					alienPrefabs[i] = (GameObject)Instantiate(alien2, positions[index], Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, positions[index], Quaternion.identity);
				}else{
					alienPrefabs[i] = (GameObject)Instantiate(alien2, getRandomAlienSpawnPoint()+(new Vector3(0,2,0)), Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, alienPrefabs[i].transform.position, Quaternion.identity);
				}
			}else{
				aliens[i] = new Alien(2);
				if(gameManager.Instance.getActiveLevel().Equals("Terrain")){
					alienPrefabs[i] = (GameObject)Instantiate(alien3, positions[index], Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, positions[index], Quaternion.identity);
				}else{
					alienPrefabs[i] = (GameObject)Instantiate(alien3, getRandomAlienSpawnPoint()+(new Vector3(0,2,0)), Quaternion.identity);
					alienSeekingSpherePrefabs[i]=(GameObject)Instantiate(seekingSpherePrefab as GameObject, alienPrefabs[i].transform.position, Quaternion.identity);
				}
			}
			
			//Get Seeker and set seeking sphere to alien
			((Seeker) alienPrefabs[i].GetComponent(typeof(Seeker))).setTarget(alienSeekingSpherePrefabs[i]);
			((AlienState) alienPrefabs[i].GetComponent(typeof(AlienState))).setInvisible();
			((AlienState) alienPrefabs[i].GetComponent(typeof(AlienState))).setAlien (aliens[i]);
		}
	}

 
	//Mainly for moving camera around screen
	void Update()
	{	
		if(currentShootSoldierHit){
			instanceCurrentShootSoldierHit = true;
		}
		
		if(instanceCurrentShootSoldierHit)
		{
			soldiers[currentShootSoldierIndex].addAlienKilled(1);
			print ("update ALIEN KILLED");
			currentShootSoldierHit = false;
			instanceCurrentShootSoldierHit = false;
		}
		
		
		//Check if win or lose
		checkIfWin();
		checkIfLose();
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f* Time.deltaTime);
		
		if(shootingMode){
			
			//Update Timer
			shootwait+=Time.deltaTime;	
			
			//Rotate camera
			//Rotate Left
			if (Input.GetKeyDown("q")){
				soldierPrefabs[selectedSoldier].transform.Rotate(0,-90,0);
				targetPosition = soldierPrefabs[selectedSoldier].transform.position + (new Vector3(0,4,0)) + (soldierPrefabs[selectedSoldier].transform.forward.normalized*-2)
					+ (soldierPrefabs[selectedSoldier].transform.right.normalized*2);
				targetRotation.eulerAngles = soldierPrefabs[selectedSoldier].transform.eulerAngles;
				//targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y-90,targetRotation.eulerAngles.z);
			}
			//Rotate Right
			if (Input.GetKeyDown("e")){
				soldierPrefabs[selectedSoldier].transform.Rotate(0,90,0);
				targetPosition = soldierPrefabs[selectedSoldier].transform.position + (new Vector3(0,4,0)) + (soldierPrefabs[selectedSoldier].transform.forward.normalized*-2)
					+ (soldierPrefabs[selectedSoldier].transform.right.normalized*2);
				targetRotation.eulerAngles = soldierPrefabs[selectedSoldier].transform.eulerAngles;
				//targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y+90,targetRotation.eulerAngles.z);
			}
			
			if(shootwait>1){
				if(Input.GetMouseButtonUp(0)){
					if(!((Input.mousePosition.x>=(Screen.width*11/50)) && (Input.mousePosition.x<=(Screen.width*11/50+Screen.width/18)) && ((Screen.height-Input.mousePosition.y)>=(Screen.height*23/25)) && ((Screen.height-Input.mousePosition.y)<=(Screen.height*23/25+Screen.height/20)))){
						RaycastHit hit=new RaycastHit();
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
						if(Physics.Raycast (ray,out hit,10000000)){
							if(!hit.collider.tag.Equals("Soldier")){
								((SoldierState) soldierPrefabs[selectedSoldier].GetComponent(typeof(SoldierState))).rotateWeaponHandler(hit.point);
								
								range = soldiers[selectedSoldier].getWeaponRange();
								
								// If the weapon is a type of sniper
								if(containsSniper){
									//Set Range
									((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).setSniperRange(range);
									
									
									if(ammoLeft >= 1 && soldiers[selectedSoldier].getEnergy() >= soldiers[selectedSoldier].getWeapon().getActionPoints()){
										currentShootSoldierIndex = selectedSoldier;
										//fire!
										((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).fireSniper ();
		
											if(selectedSoldier<numbSoldier){
												// Decrease energy
												soldiers[selectedSoldier].setEnergy(soldiers[selectedSoldier].getEnergy()-soldiers[selectedSoldier].getWeapon().getActionPoints());
												//Decrease ammo
												soldiers[selectedSoldier].setAmmoLeft(soldiers[selectedSoldier].getAmmoLeft()-1);
											}
									}
								}
								// If the weapon is a type of shotgun
								if(containsShotgun){
									//Set Range
										((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).setShotgunRange(range);
									
									
									if(ammoLeft >= 1 && soldiers[selectedSoldier].getEnergy() >= soldiers[selectedSoldier].getWeapon().getActionPoints()){
										currentShootSoldierIndex = selectedSoldier;
										((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).fireShotgun ();
										if(selectedSoldier<numbSoldier){
											soldiers[selectedSoldier].setEnergy(soldiers[selectedSoldier].getEnergy()-soldiers[selectedSoldier].getWeapon().getActionPoints());
											//Decrease ammo
											soldiers[selectedSoldier].setAmmoLeft(soldiers[selectedSoldier].getAmmoLeft()-1);
										}
									}
								}
								
								// If the weapon is a type of rifle
								if(containsRifle){
										//Set Range
										((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).setRifleRange(range);
										
									if(ammoLeft >= 3 && soldiers[selectedSoldier].getEnergy() >= soldiers[selectedSoldier].getWeapon().getActionPoints()){
										currentShootSoldierIndex = selectedSoldier;
										((Projectile) soldierPrefabs[selectedSoldier].GetComponent(typeof(Projectile))).fireRifle ();
										if(selectedSoldier<numbSoldier){
											soldiers[selectedSoldier].setEnergy(soldiers[selectedSoldier].getEnergy()-soldiers[selectedSoldier].getWeapon().getActionPoints());
											//Decrease ammo
											soldiers[selectedSoldier].setAmmoLeft(soldiers[selectedSoldier].getAmmoLeft()-3);
										}
									}
								}
							}
						}
					}
				}
			}
			
			if(Input.GetMouseButtonUp(1)){
				//ZOOM
				//
				//
				//
			}
		}else{
			setInGameWeapons();
			if(playerTurn){
				//In mission we have an option frame that can popup, checks wether this fram is showing
				if(!optionFrame){
					
					//Selecting soldier(and maybe further implementations to select aliens)
					if(Input.GetMouseButtonUp(0)){
						Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
						RaycastHit[] hits;
						hits = Physics.RaycastAll (ray,1000);
						int index = 0;
						while(index < hits.Length){
							if(hits[index].collider.tag=="Soldier"){ 
								for(int i = 0;i<numbSoldier;i++){
									if(soldierPrefabs[i]!=null){
										if(soldierPrefabs[i].transform.position.x==hits[index].transform.position.x && soldierPrefabs[i].transform.position.z==hits[index].transform.position.z){
											if(selectedSoldier!=9){
												if(soldierPrefabs[selectedSoldier]!=null)
													((SoldierState) soldierPrefabs[selectedSoldier].GetComponent(typeof(SoldierState))).deselectCharacter();
											}
											selectedSoldier=i;
											((SoldierState) soldierPrefabs[selectedSoldier].GetComponent(typeof(SoldierState))).selectCharacter();
											break;
										}
									}
								}
							}
							index++;
						}				
					}
					
					//Movement when a Soldier is selected
					if(Input.GetMouseButtonUp(1)){
						Debug.Log("Trying to move");
						if(selectedSoldier<numbSoldier){
							if(soldierPrefabs[selectedSoldier]!=null){
								Debug.Log("Selected Soldier is not null");
								Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								RaycastHit[] hits;
								hits = Physics.RaycastAll (ray,1000);
								int index = 0;
								while(index < hits.Length){
									if(hits[index].collider.tag=="Terrain"){
										Debug.Log("Found terrain");
										Vector3 tempVector=((GameObject.FindGameObjectWithTag("GridMapCube")).GetComponent<BuildGraph>()).getAvailablePosition(hits[index].point);
										//Vector3 tempVector = hits[index].point;
										Debug.Log("Setting movement");
										((SeekerSoldier) soldierPrefabs[selectedSoldier].GetComponent(typeof(SeekerSoldier))).setTargetPosition (tempVector, soldiers[selectedSoldier]);
										break;
									}
									index++;
								}
							}
						}
					}
				}
			}else{
				//If not option frame
				if(!optionFrame){
					//Check if current alien is dead
					if(aliens[currentAlienAI].getHealth()<=0){
						currentAlienAI++;//if dead, get next alien
						if(currentAlienAI==numbAliens){
							endAITurn ();
						}else{
							((AlienState) alienPrefabs[currentAlienAI].GetComponent(typeof(AlienState))).startAI();
						}
					}else{
						//Check if AI is done running, if it is, look at next alien.  if not, let algorithm continu
						if(((AlienState) alienPrefabs[currentAlienAI].GetComponent(typeof(AlienState))).isAiFinished()){
							print ("Alien AI "+currentAlienAI+ " has finised running");
							currentAlienAI++;//if dead, get next alien
							if(currentAlienAI==numbAliens){
								endAITurn();
							}else{
								((AlienState) alienPrefabs[currentAlienAI].GetComponent(typeof(AlienState))).startAI();
							}
						}
					}
				}
			}
			//Rotate camera
			//Rotate Left
			if (Input.GetKeyDown("q")){
				targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y-90,targetRotation.eulerAngles.z);
				vision-=1;
			}
			//Rotate Right
			if (Input.GetKeyDown("e")){
				targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y+90,targetRotation.eulerAngles.z);
				vision+=1;
			}
			int tempZ = (int)transform.position.z;
			int tempX = (int)transform.position.x;
			int tempY = (int)transform.position.y;
			
			//Mouse scrolls
			if(Input.GetAxis ("Mouse ScrollWheel")>0){
				if(tempY>40){
					targetPosition += transform.forward*4*speed * Time.deltaTime;
					//transform.position += transform.forward*speed * 2*Time.deltaTime;
				}
			}
			if(Input.GetAxis ("Mouse ScrollWheel")<0){
				if(tempY<110){
					targetPosition -= transform.forward*4*speed * Time.deltaTime;
					//transform.position -= transform.forward*speed * 2*Time.deltaTime;
				}
			}
			
			switch (vision){
			case (-1):
				vision=3;
				break;
			case (0):
				if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey("w")){
					if(tempZ<110){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z+speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.DownArrow)||Input.GetKey("s")){
					if(tempZ>-20){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z-speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.LeftArrow)||Input.GetKey("a")){
					if(tempX>0){
						targetPosition = new Vector3(targetPosition.x-speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				if (Input.GetKey(KeyCode.RightArrow)||Input.GetKey("d")){
					if(tempX<90){
						targetPosition = new Vector3(targetPosition.x+speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				break;
			case(1):
				if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey("w")){
					if(tempX<90){
						targetPosition = new Vector3(targetPosition.x+speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
					
				}
				if (Input.GetKey(KeyCode.DownArrow)||Input.GetKey("s")){
					if(tempX>0){
						targetPosition = new Vector3(targetPosition.x-speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				if (Input.GetKey(KeyCode.LeftArrow)||Input.GetKey("a")){
					if(tempZ<110){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z+speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.RightArrow)||Input.GetKey("d")){
					if(tempZ>-20){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z-speed * Time.deltaTime);
					}
				}
				break;
			case(2):
				if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey("w")){
					if(tempZ>-20){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z-speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.DownArrow)||Input.GetKey("s")){
					if(tempZ<110){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z+speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.LeftArrow)||Input.GetKey("a")){
					if(tempX<90){
						targetPosition = new Vector3(targetPosition.x+speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				if (Input.GetKey(KeyCode.RightArrow)||Input.GetKey("d")){
					if(tempX>0){
						targetPosition = new Vector3(targetPosition.x-speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				break;
			case(3):
				if (Input.GetKey(KeyCode.UpArrow)||Input.GetKey("w")){
					if(tempX>0){
						targetPosition = new Vector3(targetPosition.x-speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				if (Input.GetKey(KeyCode.DownArrow)||Input.GetKey("s")){
					if(tempX<90){
						targetPosition = new Vector3(targetPosition.x+speed * Time.deltaTime,targetPosition.y, targetPosition.z);
					}
				}
				if (Input.GetKey(KeyCode.LeftArrow)||Input.GetKey("a")){
					if(tempZ>-20){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z-speed * Time.deltaTime);
					}
				}
				if (Input.GetKey(KeyCode.RightArrow)||Input.GetKey("d")){
					if(tempZ<110){
						targetPosition = new Vector3(targetPosition.x,targetPosition.y, targetPosition.z+speed * Time.deltaTime);
					}
				}
				break;
			case(4):
				vision=0;
				break;
			}
			
			if(targetPosition.y<30){
				targetPosition.y=30;
			}
			
			if(targetPosition.z<-15){
				targetPosition.z=-15;	
			}
			
			if(targetPosition.z>115){
				targetPosition.z=115;	
			}
			
			if(targetPosition.x<-5){
				targetPosition.x=-5;	
			}
			
			if(targetPosition.x>95){
				targetPosition.x=95;	
			}
		}
	}
	
	//End AI turn
	void endAITurn(){
		playerTurn=true;
		Screen.showCursor = true;
		for(int i = 0;i<numbSoldier;i++){
			soldiers[i].setEnergy(100);	
		}
	}
	
	//Alien AI
	void AlienTurn(){
		currentAlienAI=0;
		//Start first alien AI if alien is not dead
		if(aliens[currentAlienAI].getHealth()>0){
			((AlienState) alienPrefabs[currentAlienAI].GetComponent(typeof(AlienState))).startAI();
		}
		//Set player to not be able to do anything during ai turn
		playerTurn = false;
		
	}
	
	//checks type of weapon
	bool haveSniper(){
		return true;
	}
	
	//Set meshes for weapons of in game soldiers
	void setInGameWeapons(){
		string[] weapons = new string[21];
		weapons[0] = null;
		weapons[1]= "Armour I";
		weapons[2]= "Armour II";
		weapons[3]= "Armour III";
		weapons[4]= "Armour IV";
		weapons[5]= "Armour V";
		weapons[6]= "Rifle I";
		weapons[7]= "Rifle II";
		weapons[8]= "Rifle III";
		weapons[9]= "Rifle IV";
		weapons[10]= "Rifle V";
		weapons[11]= "Sniper I";
		weapons[12]= "Sniper II";
		weapons[13]= "Sniper III";
		weapons[14]= "Sniper IV";
		weapons[15]= "Sniper V";
		weapons[16]= "Shotgun I";
		weapons[17]= "Shotgun II";
		weapons[18]= "Shotgun III";
		weapons[19]= "Shotgun IV";
		weapons[20]= "Shotgun V";
		
		//For each soldiers
		for(int i =0;i<numbSoldier;i++){
			if(soldierPrefabs[i]!=null){
				int index = 0;
				//Check if haz weapon
				for(int j = 6;j<=20;j++){
					if(soldiers[i].getWeapon()!=null){
						if(weapons[j].Equals(soldiers[i].getWeapon ().getName ())){
							index = j;
						}
					}
				}
				//Disable all weapons
				((SoldierState) soldierPrefabs[i].GetComponent(typeof(SoldierState))).disableAllWeapons();
				
				if(index>=6 && index <=10){//enable assault rifle
					((SoldierState) soldierPrefabs[i].GetComponent(typeof(SoldierState))).enableAssaultRifle();
				}else if(index>=11 && index <=15){//enable sniper rifle
					((SoldierState) soldierPrefabs[i].GetComponent(typeof(SoldierState))).enableSniper();
				}else if(index>=16 && index <=20){//enable shotgun
					((SoldierState) soldierPrefabs[i].GetComponent(typeof(SoldierState))).enableShotgun();
				}
			}
		}
	}
	
	// Updates game state with whatever changes took place in the mission and returns to geoscape.
	private void loadWorldMap(){
		
		Base curBase = gameManager.Instance.getCurrentBase();
		Aircraft curAircraft = curBase.getAircraft();
		
		foreach (Soldier s in soldiers) {
			// Remove soldiers killed from aircraft & base
			if (s != null && s.getHealth() <= 0) {
				curAircraft.removeSoldier(s);
				curBase.removeSoldier(s);
				curBase.dequip(s.getWeapon());
				curBase.dequip(s.getArmor());
			}
			// Send wounded soldiers to hospital
			else if (s != null && s.getHealth() < 100 && curBase.getHospitalOccupied() < curBase.getHospitalSpace()) {
				curBase.addToHospital(s);
			}
		}
		
		// Tell game manager how many civilians were killed / saved
		int saved = 0;
		int civTotal = 0;
		foreach (GameObject go in GameObject.FindGameObjectsWithTag("Civilian")) {
			Civilian c = (Civilian) go.GetComponent(typeof(Civilian));
			civTotal++;
			if (c != null && c.alive)
				saved++;
		}
		gameManager.Instance.CiviliansKilled += (civTotal - saved);
		gameManager.Instance.CiviliansSaved += saved;
		
		// On a victory, add aliens killed to base if there's enough containment space
		if (victory)
		{
			foreach (Alien a in aliens) {
				if (a.getHealth() <= 0 && curBase.getAlienTotal() < curBase.getContainmentSpace()) {
					curBase.addAlien(a.getType());
				}
			}
			
			// Payday, bitch!!
			gameManager.Instance.earnMoney(gameManager.MISSION_PAYOFF);
		}
		
		// Return to geoscape
		Screen.showCursor = true;
		gameManager.Instance.setLevel("WorldMap");	
	}
	
	//Check if player wins
	private void checkIfWin(){
		if(GameObject.FindGameObjectsWithTag("Alien").Length==0){
			gameManager.Instance.increaseMissionSuccess();
			victory=true;
			Screen.showCursor = true;
			playerTurn = true;	// So that "ALIEN ACTIVITY" goes away
		}
	}
	
	//Check if player lost
	private void checkIfLose(){
		if(GameObject.FindGameObjectsWithTag("Soldier").Length==0){
			gameManager.Instance.increaseMissionFailed();
			failed=true;
			Screen.showCursor = true;
			playerTurn = true;	// So that "ALIEN ACTIVITY" goes away
		}
	}
	
	//Gets a spawn point to initialize soldier
	private Vector3 getRandomSoldierSpawnPoint(){
		GameObject[] spawnList =  GameObject.FindGameObjectsWithTag("SoldierSpawnPoint");
		GameObject spawn=null;
		bool notFound = true;
		while(notFound){
			spawn = spawnList[Random.Range(0, spawnList.Length-1)];
			if(!((SpawnPoint)spawn.GetComponent(typeof(SpawnPoint))).isSpawnInUse()){
				notFound=false;
			}
		}
		
		//Store wanted position
		if(spawn!=null){
			Vector3 returnValue = spawn.transform.position;
			//Destroy spawn point to ensure that no other soldiers get instantiated at this position
			((SpawnPoint)spawn.GetComponent(typeof(SpawnPoint))).setToUsed();
			return returnValue;
		}
		
		return new Vector3(0,0,0);		
	}
	
	//Gets a spawn point to initialize soldier
	private Vector3 getRandomAlienSpawnPoint(){
		GameObject [] spawnList =  GameObject.FindGameObjectsWithTag("AlienSpawnPoint");
		GameObject spawn=null;
		bool notFound = true;
		while(notFound){
			spawn = spawnList[Random.Range(0, spawnList.Length-1)];
			if(!((SpawnPoint)spawn.GetComponent(typeof(SpawnPoint))).isSpawnInUse()){
				notFound=false;
			}
		}
		
		//Store wanted position
		if(spawn!=null){
			Vector3 returnValue = spawn.transform.position;
			//Destroy spawn point to ensure that no other soldiers get instantiated at this position
			((SpawnPoint)spawn.GetComponent(typeof(SpawnPoint))).setToUsed();
			return returnValue;
		}
		
		return new Vector3(0,0,0);	
	}
}
