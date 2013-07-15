using UnityEngine;
using System.Collections;

public class Multiplayer : MonoBehaviour {
	
	#region variables
	
	//All public GameObjects
	public GameObject soldierPrefab;
	public GameObject alien1Prefab;
	public GameObject alien2Prefab;
	public GameObject alien3Prefab;
	public GameObject seekingSpherePrefab;
	
	//All public Textures
	public Texture soldierPortrait;
	public Texture2D healthBarFull;
	public Texture2D energyBarFull;
	public Texture2D emptyBar;
	public Texture2D alienActivity;
	public Texture2D humanActivity;
	public Texture2D alienPortrait1;
	public Texture2D alienPortrait2;
	public Texture2D alienPortrait3;
	public Texture optionTexture;
	public Texture2D crosshair;
	
	//All Skins
	public GUISkin middleAlignment;
	public GUISkin invisiBox;
	public GUISkin optionFrameSkin;
	public GUISkin timeRemainingGuiSkin;
	
	//Timer related variables
	private float startTime;
	private string textTime;
	private float pauseTime;
	private float startGameTime;
	private float waitTime = 20;
	
	//Weapon related variables
	private int ammoLeft;
	private int maxAmmo;
	private string weaponName;
	private bool containsRifle;
	private bool containsShotgun;
	private bool containsSniper;
	private float shootwait;//basically to prevent players to shoot before camera has switched position
	public AudioClip reloadSource;
	private Texture2D weaponImage; 
	
	//Important Lists
	private GameObject[] prefabs = new GameObject[8];
	private Soldier[] soldiers;
	private Alien [] aliens;
	
	
	//All boolean
	private bool isSoldier;
	private bool shootingMode = false;
	private bool playerTurn = true;
	private bool optionFrame = false;
	private bool areYouSureExit = false;
	private bool lose = false;
	private bool win = false;
	
	//All integers
	private int availableId=0;
	private int selected;
	private int numbCharacters;
	
	//Variables for camera
	private Vector3 targetPosition;
	private Quaternion targetRotation;
	private Vector3 previousTargetPosition;
	private Quaternion previousTargetRotation;
	private int vision;
	private float speed = 50.0f;
	private Vector3 velocity = Vector3.zero;
	public float smoothTime = 0.5F;
	
	//Other
	private string [] nameList;
	public float turnTime;
	
	
	#endregion
	
	// Use this for initialization
	void Start () {
		win=false;
		lose=false;
		
		startGameTime=Time.time;
		//Variables set for both types of players
		shootingMode = false;
		optionFrame=false;
		turnTime = Time.time;
		
		//Set camera position and variables 
		targetPosition = transform.position;
		targetRotation = transform.rotation;
		vision=0;
		
		shootwait=0;
		
		availableId=0;
		
		vision=0;
		
		BuildGraph.instance.Scan();
		
		//instantiateAliens();
		
		//Instantiate soldiers/aliens depending who is what
		int side = MultiplayerState.Instance.Side;
		if(side==0){
			instantiateAliens();
		}else if(side==1){
			instantiateSoldiers();
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(lose || win){
			return;	
		}
		checkGameState();
		
		transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
		transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, 2f* Time.deltaTime);
		
		if(shootingMode){
			//Update Timer
			shootwait+=Time.deltaTime;	
			
			//Rotate camera
			//Rotate Left
			if (Input.GetKeyDown("q")){
				prefabs[selected].transform.Rotate(0,-90,0);
				if(isSoldier){
					targetPosition = prefabs[selected].transform.position + (new Vector3(0,4,0)) + (prefabs[selected].transform.forward.normalized*-2)
						+ (prefabs[selected].transform.right.normalized*2);
					targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
				}else{
					targetPosition = prefabs[selected].transform.position + (new Vector3(0,2,0)) + (prefabs[selected].transform.forward.normalized*-2)
						+ (prefabs[selected].transform.right.normalized*2);
					targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
				}
				//targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y-90,targetRotation.eulerAngles.z);
			}
			//Rotate Right
			if (Input.GetKeyDown("e")){
				prefabs[selected].transform.Rotate(0,90,0);
				if(isSoldier){
					targetPosition = prefabs[selected].transform.position + (new Vector3(0,4,0)) + (prefabs[selected].transform.forward.normalized*-2)
						+ (prefabs[selected].transform.right.normalized*2);
					targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
				}else{
					targetPosition = prefabs[selected].transform.position + (new Vector3(0,2,0)) + (prefabs[selected].transform.forward.normalized*-2)
						+ (prefabs[selected].transform.right.normalized*2);
					targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
				}
				//targetRotation.eulerAngles = new Vector3(targetRotation.eulerAngles.x,targetRotation.eulerAngles.y+90,targetRotation.eulerAngles.z);
			}
			
			if(shootwait>1){
				if(!optionFrame){
					if(Input.GetMouseButtonUp(0)){
						if(!((Input.mousePosition.x>=(Screen.width*11/50)) && (Input.mousePosition.x<=(Screen.width*11/50+Screen.width/18)) && ((Screen.height-Input.mousePosition.y)>=(Screen.height*23/25)) && ((Screen.height-Input.mousePosition.y)<=(Screen.height*23/25+Screen.height/20)))){
							RaycastHit hit=new RaycastHit();
							Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
							Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
							if(Physics.Raycast (ray,out hit,10000000)){
								
								if(isSoldier){
									((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).rotateWeaponHandler(hit.point);
									
									// If the weapon is a type of sniper
									if(containsSniper){
										if(soldiers[selected].getEnergy() >= soldiers[selected].getWeapon().getActionPoints()){
											//fire!
											((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).fireWeapon(hit.point,2,soldiers[selected].getWeapon().getRange());
							
											soldiers[selected].setEnergy(soldiers[selected].getEnergy()-soldiers[selected].getWeapon().getActionPoints());
											//Decrease ammo
											//soldiers[selected].setAmmoLeft(soldiers[selected].getAmmoLeft()-1);
										}
									}
									// If the weapon is a type of shotgun
									if(containsShotgun){
										if(soldiers[selected].getEnergy() >= soldiers[selected].getWeapon().getActionPoints()){
											((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).fireWeapon(hit.point,1,soldiers[selected].getWeapon().getRange());
											//((Projectile) prefabs[selected].GetComponent(typeof(Projectile))).fireShotgun ();
											
											soldiers[selected].setEnergy(soldiers[selected].getEnergy()-soldiers[selected].getWeapon().getActionPoints());
											//Decrease ammo
											//soldiers[selected].setAmmoLeft(soldiers[selected].getAmmoLeft()-10);
										}
									}
									
									// If the weapon is a type of rifle
									if(containsRifle){
										if(soldiers[selected].getEnergy() >= soldiers[selected].getWeapon().getActionPoints()){
											((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).fireWeapon(hit.point,0,soldiers[selected].getWeapon().getRange());
											//((Projectile) prefabs[selected].GetComponent(typeof(Projectile))).fireRifle ();
											
											soldiers[selected].setEnergy(soldiers[selected].getEnergy()-soldiers[selected].getWeapon().getActionPoints());
											//Decrease ammo
											//soldiers[selected].setAmmoLeft(soldiers[selected].getAmmoLeft()-3);
										}
									}
								}else{//If alien
									((AlienState) prefabs[selected].GetComponent(typeof(AlienState))).rotateWeaponHandler(hit.point);
									if(aliens[selected].getEnergy() >= 10){
										((AlienState) prefabs[selected].GetComponent(typeof(AlienState))).networkShoot(hit.point);
							
										aliens[selected].setEnergy(aliens[selected].getEnergy()-10);
										//Decrease ammo
									}
								}
							}
						}
					}
				}
			}
		}else{
			
			if(isSoldier){
				setInGameWeapons();
			}
			
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
							if(isSoldier){
								if(hits[index].collider.tag=="Soldier"){ 
									for(int i = 0;i<numbCharacters;i++){
										if(prefabs[i]!=null){
											if(prefabs[i].transform.position.x==hits[index].transform.position.x && prefabs[i].transform.position.z==hits[index].transform.position.z){
												if(prefabs[selected]!=null)
													((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).deselectCharacter();
												selected=i;
												((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).selectCharacter();
												break;
											}
										}
									}
								}
							}else{
								if(hits[index].collider.tag=="Alien"){ 
									for(int i = 0;i<numbCharacters;i++){
										if(prefabs[i]!=null){
											if(prefabs[i].transform.position.x==hits[index].transform.position.x && prefabs[i].transform.position.z==hits[index].transform.position.z){
												if(prefabs[selected]!=null)
													((AlienState) prefabs[selected].GetComponent(typeof(AlienState))).deselectCharacter();
												selected=i;
												((AlienState) prefabs[selected].GetComponent(typeof(AlienState))).selectCharacter();
												break;
											}
										}
									}
								}
							}
							index++;
						}				
					}
					
					//Movement when a Soldier is selected
					if(Input.GetMouseButtonUp(1)){
						if(prefabs[selected]!=null){
							if(isSoldier){
								Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								RaycastHit[] hits;
								hits = Physics.RaycastAll (ray,1000);
								int index = 0;
								while(index < hits.Length){
									if(hits[index].collider.tag=="Terrain"){ 
										Vector3 tempVector=((GameObject.FindGameObjectWithTag("GridMapCube")).GetComponent<BuildGraph>()).getAvailablePosition(hits[index].point);
										((SeekerSoldier) prefabs[selected].GetComponent(typeof(SeekerSoldier))).setTargetPosition (tempVector, soldiers[selected]);
									}
									index++;
								}
							}else{//For alien movement
								Ray ray = Camera.main.ScreenPointToRay (Input.mousePosition);
								RaycastHit[] hits;
								hits = Physics.RaycastAll (ray,1000);
								int index = 0;
								while(index < hits.Length){
									if(hits[index].collider.tag=="Terrain"){ 
										Vector3 tempVector=((GameObject.FindGameObjectWithTag("GridMapCube")).GetComponent<BuildGraph>()).getAvailablePosition(hits[index].point);
										//GameObject newTarget = Instantiate(seekingSpherePrefab,tempVector,Quaternion.identity) as GameObject;
										((Seeker) prefabs[selected].GetComponent(typeof(Seeker))).setTargetPosition (tempVector);
									}
									index++;
								}
							}
						}
					}
				}
			}
		
			
		//EVERYTHING THAT NEEDS TO UPDATE AT ALL TIME--------------------	
			//Rotate camera
			//Rotate Left
			if(!shootingMode){
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
	}
	
	void OnGUI(){
		
		if(lose){
			Screen.showCursor = true;
			GUI.Box(new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height/2),"You Lose... Press OK to go back to the main menu");
			if(GUI.Button(new Rect(Screen.width/2-20,Screen.height/2-20,40,40),"OK")){
				Network.Disconnect();
				Application.LoadLevel("Menu");
			}
			return;
		}else if(win){
			Screen.showCursor = true;
			GUI.Box(new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height/2),"You WON!!! Press OK to go back to the main menu");
			if(GUI.Button(new Rect(Screen.width/2-20,Screen.height/2-20,40,40),"OK")){
				Network.Disconnect();
				Application.LoadLevel("Menu");
			}
			return;
		}
		
		//Box for Options
		if(GUI.Button (new Rect(Screen.width-40,0,40,30),optionTexture)){
			optionFrame=true;
			pauseTime = Time.time;			
						
		}
		//Show crosshair
		if(shootingMode){
			Screen.showCursor = false;
				GUI.DrawTexture(new Rect(Input.mousePosition.x-20,Screen.height-Input.mousePosition.y-20,40,40),crosshair);
		}
		
		if(playerTurn){
			if(!optionFrame){	
				
		//_======================CHARACTER BOX-===========================================		
				//Align all text to be in middle of rects
				GUI.skin = middleAlignment;
				
				//Box for selected person information
				float selectionWidth = Screen.width/3;
				if(selectionWidth>410)
					selectionWidth=410;
				GUI.Box (new Rect(0,0,selectionWidth,145),"");
				string tempName = "Select a character.";
				if(selected<numbCharacters){
					
					if(isSoldier){
						GUI.DrawTexture(new Rect(10,10,113,111), soldierPortrait);
						tempName = "Soldier "+ soldiers[selected].getName ();
					}else{
						int alienType = aliens[selected].getAlienType();
						switch (alienType){
						case (0):
							GUI.DrawTexture(new Rect(10,10,113,111), alienPortrait1);
							tempName = "Creeper";
							break;
						case (1):
							GUI.DrawTexture(new Rect(10,10,113,111), alienPortrait2);
							tempName = "Tormentor";
							break;
						case (2):
							GUI.DrawTexture(new Rect(10,10,113,111), alienPortrait3);
							tempName = "Crawler";
							break;
						}
					}
					
					GUI.Label (new Rect(125,40,60,20),"Health: ");
					GUI.Label (new Rect(125,70,60,20),"Energy: ");
					
					//Gui for health bars
					GUI.skin = invisiBox;
					
					float barSize = selectionWidth - 195;
					if(barSize>206)
						barSize=206;
					
					int health,energy;
					if(isSoldier){
						health = soldiers[selected].getHealth();
						energy = soldiers[selected].getEnergy();
					}else{
						health = aliens[selected].getHealth();
						energy = aliens[selected].getEnergy();
					}
					
					// draw the healthBar/energybar
					//Draw Health Bar
			        GUI.Box (new Rect (185,40, barSize, 30),emptyBar);
			        // draw the filled-in part:
			        GUI.BeginGroup (new Rect (185, 40, barSize * health/100+10, 30));
			            GUI.Box (new Rect (0,0, barSize, 30),healthBarFull);
			        GUI.EndGroup ();
					GUI.Label (new Rect (185,40, barSize, 30),""+health);
					
					//Draw Energy Bar
					GUI.Box (new Rect (185,70, barSize, 30),emptyBar);
			        // draw the filled-in part:
					float tempFloat = (float)energy;
					float otherTempFloat = tempFloat/100;
			        GUI.BeginGroup (new Rect (185, 70, barSize*otherTempFloat+10, 30));
			            GUI.Box (new Rect (0,0, barSize, 30),energyBarFull);
			        GUI.EndGroup ();
					GUI.Label (new Rect (185,70, barSize, 30),""+tempFloat);
					GUI.skin = middleAlignment;
				}
				GUI.Label(new Rect(130,10, selectionWidth-130,20),tempName);
			
		//===========================================================================================
		//===========================TIMER RELATED///=================================================
				
				//Time box for ending turn and see time remaining an time in game
				GUI.Box (new Rect(Screen.width*3/7,0,Screen.width/7,Screen.height/10),"Timer");
				//Time
				/*float guiTime = Time.time - startTime;
	 
	   			int minutes = (int)guiTime / 60;
	   			int seconds = (int)guiTime % 60;
	   			int fraction = (int)(guiTime * 100) % 100;*/
				
				textTime = MultiplayerState.Instance.getTime ();//string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction); 
	   			GUI.Label (new Rect (Screen.width/3, 10, Screen.width/3, Screen.height/10), textTime);
				
				if(isSoldier){
					if(!MultiplayerState.Instance.getTurn()){
						playerTurn=false;
						Screen.showCursor = false;
						if(shootingMode){
							targetPosition=previousTargetPosition;
							targetRotation= previousTargetRotation;
							shootingMode=false;
							turnTime=Time.time;
						}
					}
				}else{
					if(MultiplayerState.Instance.getTurn()){
						playerTurn=false;
						Screen.showCursor = false;
						if(shootingMode){
							targetPosition=previousTargetPosition;
							targetRotation= previousTargetRotation;
							shootingMode=false;
							turnTime=Time.time;
						}
					}
				}
				//Debug.Log("SoldierTurn: "+ MultiplayerState.Instance.getTurn());
				
				float turn = Time.time - turnTime;
				int turnMinutes =(int) turn / 60;
				if(turnMinutes>=1){
					((Manager)prefabs[selected].GetComponent(typeof (Manager))).changeState();
					turnTime=Time.time;
				}
				
				float guiTime = Time.time - turnTime;
				int minutes = 1-(int)guiTime / 60-1;
				int seconds = 60-(int)guiTime % 60;		
				if(minutes<0){
					seconds=0;
					minutes=0;
				}
				textTime = "Turn time remaining: ";
				textTime = textTime+ string.Format ("{0:00}:{1:00}", minutes, seconds); 
				GUI.skin = timeRemainingGuiSkin;
				GUI.Label (new Rect (Screen.width/2-300, Screen.height-50, 600, 50), textTime);
				GUI.skin = null;
				
				//END TURN BUTTON 
				Color oldCol = GUI.backgroundColor;
				GUI.backgroundColor = Color.red;
				if(GUI.Button(new Rect(Screen.width/2-Screen.width/28, Screen.height/10, Screen.width/14, Screen.height/32), "End Turn"))
				{
					((Manager)prefabs[selected].GetComponent(typeof (Manager))).changeState();
					turnTime=Time.time;
					//MultiplayerState.Instance.endTurn();
					
				}
				GUI.backgroundColor = oldCol;
		//================================================================================================================
		//=========================================SHOOT=================================================================
				
				float barSized = selectionWidth - 195;
				if(!shootingMode){
					//END TURN BUTTON 
					oldCol = GUI.backgroundColor;
					GUI.backgroundColor = Color.red;
					if(GUI.Button (new Rect(190,105,barSized*2/3,30),"Shoot")){
						GUI.backgroundColor = oldCol;
						//Start by stopping soldier from moving
						if(isSoldier)
							((SeekerSoldier)prefabs[selected].GetComponent(typeof(SeekerSoldier))).stopWalking();
						
						previousTargetPosition = targetPosition;
						previousTargetRotation = targetRotation;
						if(isSoldier){
							targetPosition = prefabs[selected].transform.position + (new Vector3(0,4,0)) + (prefabs[selected].transform.forward.normalized*-2)
								+ (prefabs[selected].transform.right.normalized*2);
							targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
						}else{
							targetPosition = prefabs[selected].transform.position + (new Vector3(0,2,0)) + (prefabs[selected].transform.forward.normalized*-2)
								+ (prefabs[selected].transform.right.normalized*2);
							targetRotation.eulerAngles = prefabs[selected].transform.eulerAngles;
						}
						
						//Now cursor will look like a rifle 
						Screen.showCursor = false;
						
						shootingMode=true;
						if(isSoldier)
							((Walking)prefabs[selected].GetComponent(typeof(Walking))).activateShootingMode();	
						
						shootwait = 0;
					}
					//Setting weapons
					if(isSoldier){
						weaponName = soldiers[selected].getWeapon().getName();
						containsRifle = weaponName.Contains("Rifle");
						containsShotgun = weaponName.Contains("Shotgun");
						containsSniper = weaponName.Contains("Sniper");
						if(weaponImage!=null)
							GUI.DrawTexture(new Rect(0,Screen.height*43/50,Screen.width*2/9,Screen.height/10), weaponImage);
					}
				}else{
					oldCol = GUI.backgroundColor;
					GUI.backgroundColor = Color.red;
					if(GUI.Button (new Rect(190,105,barSized*2/3,30),"Exit shooting mode")){
						GUI.backgroundColor = oldCol;
						targetPosition=previousTargetPosition;
						targetRotation= previousTargetRotation;
						
						//Now cursor will look like a rifle 
						Screen.showCursor = true;
						
						shootingMode=false;
						if(isSoldier)
							((Walking)prefabs[selected].GetComponent(typeof(Walking))).deactivateShootingMode();
					}
				}	
		//---------------------------------------------------------------------------------------------------
				
		//===============================================OPTION MENU===========================================
			}else{//Here we have the option menu that will pop up on the screen.
				GUI.Box (new Rect(Screen.width/3,Screen.height/3,Screen.width/3,Screen.height/3),"Option Menu");
				GUI.skin = optionFrameSkin;
				
				
				if(areYouSureExit){
					//GUI.Box(new Rect(Screen.width/3+Screen.width/9,Screen.height/3+Screen.height/9,Screen.width/9,Screen.height/9), "");
					GUI.Label(new Rect(Screen.width/3,Screen.height/3+20,Screen.width/3,Screen.height/3-Screen.height/27-20),"Are you certain you want to exit to the main menu? Quitting means you lose the game.");
					if(GUI.Button(new Rect(Screen.width/3+Screen.width/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"Yes")){
						//gameManager.Instance.deleteInstance();
						Network.Disconnect();
						Application.LoadLevel("Menu");
					}
					if(GUI.Button(new Rect(Screen.width/3+Screen.width*3/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"No")){
						areYouSureExit=false;
					}
				}
				if(!areYouSureExit){
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height/21,Screen.width*3/15,Screen.height/21),"Resume Gameplay")){
							optionFrame=false;//Revert back to gameplay
							pauseTime = Time.time-pauseTime;
							startTime+=pauseTime;
							//More actions to be done like resume time
							//
							//
							//
					}			
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height*5.5f/21,Screen.width*3/15,Screen.height/21),"Exit Gameplay")){
							//Make pop up window asking if that is really what the player wants
							areYouSureExit=true;
						
					}
				}
				GUI.skin = null;
			}
		//--------------------------------------------------------------------------------------------------------
			
		//====================================NOT PLAYER TURN======================================================
		}else{
			if(isSoldier){
				GUI.DrawTexture(new Rect(Screen.width/2-169,Screen.height/2-26,338,52),alienActivity);
				if(MultiplayerState.Instance.getTurn()){
					playerTurn=true;
					if(playerTurn)
						Screen.showCursor = true;
					turnTime = Time.time;
					foreach(Soldier s in soldiers){
						s.resetEnergy();
					}
				}
			}else{
				GUI.DrawTexture(new Rect(Screen.width/2-169,Screen.height/2-26,338,52),humanActivity);
				//Debug.Log("SoldierTurn: "+ MultiplayerState.Instance.getTurn());
				if(!MultiplayerState.Instance.getTurn()){
					playerTurn=true;
					if(playerTurn)
						Screen.showCursor = true;
					turnTime = Time.time;
					foreach(Alien a in aliens){
						a.resetEnergy();
					}
				}
			}
			float guiTime = Time.time - turnTime;
			int minutes = 1-(int)guiTime / 60-1;
			int seconds = 60-(int)guiTime % 60;		
			if(minutes<0){
				seconds=0;
				minutes=0;
			}
			textTime = string.Format ("{0:00}:{1:00}", minutes, seconds); 
			GUI.skin = timeRemainingGuiSkin;
			GUI.Label (new Rect (Screen.width/2-200, Screen.height/2+26, 400, 50), "Opponent's turn time remaining: ");
			GUI.Label (new Rect (Screen.width/2-200, Screen.height/2+76, 400, 50), textTime);
			
			if(optionFrame){
				GUI.Box (new Rect(Screen.width/3,Screen.height/3,Screen.width/3,Screen.height/3),"Option Menu");
				GUI.skin = optionFrameSkin;
				
				
				if(areYouSureExit){
					//GUI.Box(new Rect(Screen.width/3+Screen.width/9,Screen.height/3+Screen.height/9,Screen.width/9,Screen.height/9), "");
					GUI.Label(new Rect(Screen.width/3,Screen.height/3+20,Screen.width/3,Screen.height/3-Screen.height/27-20),"Are you certain you want to exit to the main menu? Quitting means you lose the game.");
					if(GUI.Button(new Rect(Screen.width/3+Screen.width/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"Yes")){
						//gameManager.Instance.deleteInstance();
						Network.Disconnect();
						Application.LoadLevel("Menu");
					}
					if(GUI.Button(new Rect(Screen.width/3+Screen.width*3/18,Screen.height*2/3-Screen.height/18,Screen.width/9,Screen.height/18),"No")){
						areYouSureExit=false;
					}
				}
				if(!areYouSureExit){
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height/21,Screen.width*3/15,Screen.height/21),"Resume Gameplay")){
							optionFrame=false;//Revert back to gameplay
							pauseTime = Time.time-pauseTime;
							startTime+=pauseTime;
							//More actions to be done like resume time
							//
							//
							//
					}			
					if(GUI.Button (new Rect(Screen.width/3+Screen.width*1/15,Screen.height/3+Screen.height*5.5f/21,Screen.width*3/15,Screen.height/21),"Exit Gameplay")){
							//Make pop up window asking if that is really what the player wants
							areYouSureExit=true;
						
					}
				}
				GUI.skin = null;
			}
		}
		//------------------------------------------------------------------------------------------------------------
		GUI.skin=null;
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
	
	//Instantiate aliens
	private void instantiateAliens(){
		//Set camera vision to alien start vision camera angles
		transform.RotateAround(transform.position,Vector3.up,90);
		targetRotation = transform.rotation;
		vision+=1;
		//Create aliens
		numbCharacters = 8;
		aliens = new Alien[numbCharacters];
		for(int i = 0;i<numbCharacters;i++){
			aliens[i]= new Alien(0);
			availableId++;
			
			Vector3 spawnPos = getRandomAlienSpawnPoint();
			GameObject tempAlien = (GameObject)Network.Instantiate(alien1Prefab as GameObject, spawnPos, Quaternion.identity,0);
			prefabs[i]=((GetSoldier) tempAlien.GetComponent(typeof(GetSoldier))).getSoldier();
			//GameObject tempSeekingSphere=(GameObject)Network.Instantiate(seekingSpherePrefab as GameObject, spawnPos, Quaternion.identity,2);
			
			//Get Seeker and set seeking sphere to soldier
			((Seeker) prefabs[i].GetComponent(typeof(Seeker))).activateNetwork();
			((Seeker) prefabs[i].GetComponent(typeof(Seeker))).setCurAlien(aliens[i]);
			
			//Sets soldier in soldier state of the soldier prefab
			((AlienState) prefabs[i].GetComponent(typeof(AlienState))).setAlien(aliens[i]);
			((AlienState) prefabs[i].GetComponent(typeof(AlienState))).setNetwork();
		}
		selected=0;
		((AlienState) prefabs[selected].GetComponent(typeof(AlienState))).selectCharacter();
		isSoldier=false;
	}
	
	//Instantiate Soldiers
	private void instantiateSoldiers(){
		numbCharacters = 4;
		soldiers = new Soldier[numbCharacters];
		for(int i = 0;i<numbCharacters;i++){
			soldiers[i]= new Soldier(availableId,getRandomName ());
			soldiers[i].setWeapon(AllWeapons.getWeaponByName("Rifle I"));
			availableId++;
			
			Vector3 spawnPos = getRandomSoldierSpawnPoint();
			GameObject tempSoldier =(GameObject)Network.Instantiate(soldierPrefab as GameObject, spawnPos, Quaternion.identity,1);
			prefabs[i]= ((GetSoldier) tempSoldier.GetComponent(typeof(GetSoldier))).getSoldier();
			//GameObject tempSeekingSphere=(GameObject)Network.Instantiate(seekingSpherePrefab as GameObject, spawnPos, Quaternion.identity,2);
			
			//Get Seeker and set seeking sphere to soldier
			((SeekerSoldier) prefabs[i].GetComponent(typeof(SeekerSoldier))).activateNetwork();
			
			((SeekerSoldier) prefabs[i].GetComponent(typeof(SeekerSoldier))).setCurSoldier(soldiers[i]);
			//Sets soldier in soldier state of the soldier prefab
			((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).setSoldier(soldiers[i]);
			((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).setNetwork();
		}
		selected = 0;
		((SoldierState) prefabs[selected].GetComponent(typeof(SoldierState))).selectCharacter();
		isSoldier=true;
	}
	
	#region Helper functions
	
	public void checkGameState(){
		if((Time.time-startGameTime)>waitTime){
			if(isSoldier){
				if(GameObject.FindGameObjectsWithTag("Soldier").Length==0){
					lose = true;	
				}
				if(GameObject.FindGameObjectsWithTag("Alien").Length==0){
					win = true;	
				}
			}else{
				if(GameObject.FindGameObjectsWithTag("Soldier").Length==0){
					win = true;	
				}
				if(GameObject.FindGameObjectsWithTag("Alien").Length==0){
					lose = true;	
				}
			}
		}else{
			return;	
		}
	}
	
	//Returns a random name from nameList
	public string getRandomName(){
		if(nameList==null){
			createNameList();
		}
		return nameList[Random.Range(0,nameList.Length-1)];	
	}
	
	//Creates an array of names
	public void createNameList(){
		string nameListString = "Staci Cesare,Marshall Longmore,Sanjuana Belue,Annice Commander,Liane Whitsett,Terisa Googe,Marcie Pietz,Danyell Capella,Norma Wiersma,Rosalyn Eastland,Hershel Radigan,Myrtice Mcevoy,Chasidy Navin,Fidelia Goodner,Annett Rudolph,Regine Champ,Johnna Mirza,Alicia Nealey,Clemmie Pool,Clorinda Moscato,Sharolyn Garza,Lala Priolo,Stephenie Ely,Eleanore Tamayo,Valencia Adorno,Carie Mcmurtrie,Yasuko Luce,Enola Hibner,Jacquelin Mcphatter,Marin Raglin,Clarissa Parnell,Alida Pokorny,Dot Golder,Roger Newsom,Lizzette Canary,Yuko Shiflet,Shane Taketa,Heide Randazzo,Silvia Debow,Lexie Giardina,Jaunita Amore,Yu Sereno,Alpha Gautier,Ramiro Brenes,Flora Quam,Wilfredo Mclachlan,Dawne Michel,Leena Kieffer,Crysta Perrotta,Stacy Bryd,Lynda Riche,Wen Mcmullen,Nicolle Riera,Loise Geary,Cara Johansson,Edgar Gunnell,Brain Bertucci,Herma Hager,Sanora Lablanc,Celine Boomhower,Robyn Vigue,Jinny Trask,Glayds Harbaugh,Eileen Ellenberger,Bernardine,Lewandowski,Elisa Darrington,Jillian Espinoza,Christal Goggans,Veda Thiel,Cyndy Summey,France Cournoyer,Franklyn Wulf,Fairy Cogar,Felton Croker,Tameka Sheriff,Ciara Esterly,Marhta Neel,Marcos Whisenhunt,Naomi Pascoe,Deloise Ammons,Cora Desir,Avis Depaul,Juliane Heyne,Tu Towery,Leontine Mccroy,Lizabeth Altieri,Tiffanie Lemond,Allie Sperber,Johnette Kostka,Johnny Lillie,Magdalene Maciel,Deja Minner,Trudi Macey,Concepcion Folden,Carrol Cather,Erick Osterman,Classie Crippen,Charley Schurman,Clifford Harnage,Jules Mateer";
		string[] words = nameListString.Split (',');
		nameList = words;
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
		for(int i =0;i<numbCharacters;i++){
			if(prefabs[i]!=null){
				int index = 0;
				//Check if haz weapon
				for(int j = 6;j<=20;j++){
					if(soldiers[i].getWeapon()!=null){
						if(weapons[j].Equals(soldiers[i].getWeapon ().getName ())){
							index = j;
							weaponImage = soldiers[i].getWeapon().getImage();
						}
					}
				}
				//Disable all weapons
				((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).disableAllWeapons();
				
				if(index>=6 && index <=10){//enable assault rifle
					((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).enableAssaultRifle();
				}else if(index>=11 && index <=15){//enable sniper rifle
					((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).enableSniper();
				}else if(index>=16 && index <=20){//enable shotgun
					((SoldierState) prefabs[i].GetComponent(typeof(SoldierState))).enableShotgun();
				}
			}
		}
	}
	
	#endregion
	
	#region Player Disconnects
	void OnPlayerDisconnected(NetworkPlayer player) {
		Application.LoadLevel("Menu");
	}
	
	void OnDisconnectedFromServer(NetworkDisconnection info) {
		Network.Disconnect();
		Application.LoadLevel("Menu");
	}
	#endregion
	
}

