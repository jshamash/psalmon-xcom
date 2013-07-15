using UnityEngine;
using System.Collections;

public class EquipScript : MonoBehaviour {
	
	//Selected soldier in the mission
	private int selectedSoldier=9;
	//Number of soldiers in the mission
	private int numbSoldier;
	
	//Skins
	public GUISkin basicSkin;
	
	//Textures
	private Texture2D soldierTexture;
	private Texture2D grid;
	private Texture2D quit;
	private Texture2D empty;
	private Texture2D takenTexture;
	
	//Mouse Variables
	private int mousex =100000;
	private int mousey =1000000;
	private int mouseLetGox = -100000;
	private int mouseLetGoy = -100000;
	private int mouseClickx = 100000;
	private int mouseClicky = 100000;
	private int takenFrom = 0;
	private int selectedObject;

	//Arrays
	private int[] gridPics;
	private Texture2D[] textures;
	Soldier[] soldiers = new Soldier[8];//max of 8 soldiers per aircrafts and game
	string[] weapons;
	
	//Lists
	
	//Booleans
	bool foundANiche = false;
	bool stillDown = false;
	public bool equipMenu = true;
	
	
	void Start(){
		
		//Set variables and load textures required
		foundANiche=false;
		stillDown=false;
		
		quit = (Texture2D)Resources.Load("Mission/quit");
		grid = (Texture2D)Resources.Load("Mission/grid");
		soldierTexture = (Texture2D)Resources.Load("Mission/tempSoldier");
		
		//Create texture array and load the textures in the array
		textures=new Texture2D[50];
		textures[0]=empty;
		textures[1]=(Texture2D)Resources.Load("Weapons/Armour1");
		textures[2]=(Texture2D)Resources.Load("Weapons/Armour2");
		textures[3]=(Texture2D)Resources.Load("Weapons/Armour3");
		textures[4]=(Texture2D)Resources.Load("Weapons/Armour4");
		textures[5]=(Texture2D)Resources.Load("Weapons/Armour5");
		textures[6]=(Texture2D)Resources.Load("Weapons/Rifle1");
		textures[7]=(Texture2D)Resources.Load("Weapons/Rifle2");
		textures[8]=(Texture2D)Resources.Load("Weapons/Rifle3");
		textures[9]=(Texture2D)Resources.Load("Weapons/Rifle4");
		textures[10]=(Texture2D)Resources.Load("Weapons/Rifle5");
		textures[11]=(Texture2D)Resources.Load("Weapons/Sniper1");
		textures[12]=(Texture2D)Resources.Load("Weapons/Sniper2");
		textures[13]=(Texture2D)Resources.Load("Weapons/Sniper3");
		textures[14]=(Texture2D)Resources.Load("Weapons/Sniper4");
		textures[15]=(Texture2D)Resources.Load("Weapons/Sniper5");
		textures[16]=(Texture2D)Resources.Load("Weapons/Shotgun1");
		textures[17]=(Texture2D)Resources.Load("Weapons/Shotgun2");
		textures[18]=(Texture2D)Resources.Load("Weapons/Shotgun3");
		textures[19]=(Texture2D)Resources.Load("Weapons/Shotgun4");
		textures[20]=(Texture2D)Resources.Load("Weapons/Shotgun5");
		
		weapons = new string[50];
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
		weapons[13]= "Sniper II";
		weapons[14]= "Sniper IV";
		weapons[15]= "Sniper V";
		weapons[16]= "Shotgun I";
		weapons[17]= "Shotgun II";
		weapons[18]= "Shotgun III";
		weapons[19]= "Shotgun IV";
		weapons[20]= "Shotgun V";
		
		//Set all gridPics to 0 initially
		gridPics = new int[50];
		for(int i = 0;i<30;i++){
			gridPics[i]=0;
		}
		selectedObject = 0;//nothing selected at the beginning
		
		/*gameManager.Instance.createNameList();
		for(int i = 0;i<8;i++){
			soldiers[i] = new Soldier(i, gameManager.Instance.getRandomName());
		}
		numbSoldier=8;*/
		
		//Load soldiers 
		numbSoldier = gameManager.Instance.getCurrentBase().getAircraft().getSoldiers().Count;
		soldiers = new Soldier[numbSoldier];
		gameManager.Instance.getCurrentBase().getAircraft().getSoldiers().CopyTo(soldiers,0);
		selectedSoldier=0;
		
		//Get weapon of selected soldier
		gridPics[8] = getWeapon ();
		gridPics[9] = getArmor();
		
	}
	
	//Obtain the texture for the weapon
	private int getWeapon(){
		int index = 0;
		//Check if haz weapon
		for(int j = 1;j<=20;j++){
			if(soldiers[selectedSoldier].getWeapon()!=null){
				if(weapons[j].Equals(soldiers[selectedSoldier].getWeapon ().getName ())){
					index = j;
				}
			}
		}
		return index;
	}
	
	//Obtain the texture for the armor
	private int getArmor(){
		int index = 0;
		//Check if haz weapon
		for(int j = 1;j<=20;j++){
			if(soldiers[selectedSoldier].getArmor()!=null){
				if(weapons[j].Equals(soldiers[selectedSoldier].getArmor ().getName ())){
					index = j;
				}
			}
		}
		return index;
	}
	
	//Now insert into new location
	public void insertWeapon(int target, Weapon weapon){
		print ("getting here");
		if(target==8){
			soldiers[selectedSoldier].setWeapon(weapon);
		}else if (target==9){
			soldiers[selectedSoldier].setArmor(weapon);
			print ("Armor should be inserted");
			if(soldiers[selectedSoldier].getArmor()==null){
				print ("Armor is null? -- yes");
			}else{
				print ("Armor is null? -- no");
			}
		}else if(target>9){
			gameManager.Instance.getCurrentBase().getAircraft().addWeapon(weapon);	
			if(gameManager.Instance.getCurrentBase().getAircraft().getWeapons().Count==0){
				print("Insertion did not work");//if insertion didnt work
			}else{
				print("Insertion should have worked: "+gameManager.Instance.getCurrentBase().getAircraft().getWeapons().Count);//if it worked
			}
		}
	}
	
	public void makeTheSwitch(int target, int selection){
		//Start by removing weapon instance in previous location
		if(selection==8){
			insertWeapon(target,soldiers[selectedSoldier].getWeapon());
			soldiers[selectedSoldier].setWeapon(null);
		}else if(selection==9){
			insertWeapon(target,soldiers[selectedSoldier].getArmor());
			soldiers[selectedSoldier].setArmor(null);
			print("After being inserted");
		}else if(selection>9){
			print ("Before");
			insertWeapon(target,gameManager.Instance.getCurrentBase().getAircraft().removeWeapon(weapons[selectedObject]));
			print ("after");
		}
	}
	
	public void OnGUI(){
		GUI.Window (0, new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height/2), createWindow, "My Window");	
	}
	
	public void createWindow(int window){
		//Previous and Next soldier buttons
		//Previous
		if(GUI.Button(new Rect(0,0,Screen.width/20, Screen.height/14),"Previous Soldier")){
			if(selectedSoldier==0){
				selectedSoldier=numbSoldier-1;
			}else{
				selectedSoldier--;
			}
			gridPics[8] = getWeapon ();
			gridPics[9] = getArmor();
		}
		//Next
		if(GUI.Button(new Rect(Screen.width*5/20,0,Screen.width/20, Screen.height/14),"Next Soldier")){
			if(selectedSoldier==numbSoldier-1){
				selectedSoldier=0;
			}else{
				selectedSoldier++;
			}
			gridPics[8] = getWeapon ();
			gridPics[9] = getArmor();
		}
		//Exit window button
		if(GUI.Button(new Rect(Screen.width/2-20,0,20, 20),quit)){
			Destroy (gameObject);
			//equipMenu=false;
		}
		
		GUI.Label(new Rect(Screen.width/20,0,Screen.width*4/20, Screen.height/14),soldiers[selectedSoldier].getRankString()+" "+soldiers[selectedSoldier].getName ());
		
		GUI.DrawTexture(new Rect(Screen.width*6/20,Screen.height/28,Screen.width*4/20, Screen.height*6/14), soldierTexture);
		
		//Draw all grid squares
		int currentGridIndex = 0;
		
		//INVENTORY GRID SQUARES
		for(int i=0;i<4;i++){
			if(gridPics[currentGridIndex]!=0)
				GUI.DrawTexture(new Rect(Screen.width*2/20+Screen.height*i/14,Screen.height*1.25f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
			GUI.DrawTexture(new Rect(Screen.width*2/20+Screen.height*i/14,Screen.height*1.25f/14,Screen.height/14, Screen.height/14),grid);
			
			//Select object
			if(mouseClickx>Screen.width*2/20+Screen.height*i/14 &&mouseClickx<Screen.width*2/20+Screen.height*i/14+Screen.height/14&&mouseClicky>Screen.height*1.25f/14 &&mouseClicky<Screen.height*1.25f/14+Screen.height/14){
				if(selectedObject==0)
					selectedObject=gridPics[currentGridIndex];
				
				gridPics[currentGridIndex] = 0;
				takenFrom=currentGridIndex;
			}
			if(mouseLetGox>Screen.width*2/20+Screen.height*i/14 &&mouseLetGox<Screen.width*2/20+Screen.height*i/14+Screen.height/14&&mouseLetGoy>Screen.height*1.25f/14 &&mouseLetGoy<Screen.height*1.25f/14+Screen.height/14){
				if(selectedObject>21 && gridPics[currentGridIndex]==0 && currentGridIndex!=takenFrom){
					mouseLetGoy=-100000;
					mouseLetGoy=-100000;
					makeTheSwitch(currentGridIndex,takenFrom);
					gridPics[currentGridIndex]=selectedObject;
					
					foundANiche=true;
				}
			}
			currentGridIndex++;
		}
		for(int i=0;i<4;i++){
			if(gridPics[currentGridIndex]!=0)
				GUI.DrawTexture(new Rect(Screen.width*2/20+Screen.height*i/14,Screen.height*2.25f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
			GUI.DrawTexture(new Rect(Screen.width*2 /20+Screen.height*i/14,Screen.height*2.25f/14,Screen.height/14, Screen.height/14),grid);
			//Select object
			if(mouseClickx>Screen.width*2/20+Screen.height*i/14 &&mouseClickx<Screen.width*2/20+Screen.height*i/14+Screen.height/14&&mouseClicky>Screen.height*2.25f/14 &&mouseClicky<Screen.height*2.25f/14+Screen.height/14){
				if(selectedObject==0)
					selectedObject=gridPics[currentGridIndex];
				gridPics[currentGridIndex] = 0;
				takenFrom=currentGridIndex;
			}
			if(mouseLetGox>Screen.width*2/20+Screen.height*i/14 &&mouseLetGox<Screen.width*2/20+Screen.height*i/14+Screen.height/14&&mouseLetGoy>Screen.height*2.25f/14 &&mouseLetGoy<Screen.height*2.25f/14+Screen.height/14){
				if(selectedObject>21&& gridPics[currentGridIndex]==0&& currentGridIndex!=takenFrom){
					mouseLetGoy=-100000;
					mouseLetGoy=-100000;
					makeTheSwitch(currentGridIndex,takenFrom);
					gridPics[currentGridIndex]=selectedObject;
					
					foundANiche=true;
				}
			}
			currentGridIndex++;
		}
		
		//ARM GRID SQUARE
		if(gridPics[currentGridIndex]!=0)
			GUI.DrawTexture(new Rect(Screen.width*8.5f/20,Screen.height*1.5f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
		GUI.DrawTexture(new Rect(Screen.width*8.5f/20,Screen.height*1.5f/14,Screen.height/14, Screen.height/14),grid);
		if(mouseClickx>Screen.width*8.5f/20 &&mouseClickx<Screen.width*8.5f/20+Screen.height/14&&mouseClicky>Screen.height*1.5f/14 &&mouseClicky<Screen.height*1.5f/14+Screen.height/14){
			if(selectedObject==0)
				selectedObject=gridPics[currentGridIndex];
			gridPics[currentGridIndex] = 0;
			takenFrom=currentGridIndex;
		}
		if(mouseLetGox>Screen.width*8.5f/20 && mouseLetGox<Screen.width*8.5f/20+Screen.height/14&&mouseLetGoy>Screen.height*1.5f/14 &&mouseLetGoy<Screen.height*1.5f/14+Screen.height/14){
			if(selectedObject>5&&selectedObject<21 && gridPics[currentGridIndex]==0&&currentGridIndex!=takenFrom){
				
				mouseLetGoy=-100000;
				mouseLetGoy=-100000;
				makeTheSwitch(currentGridIndex,takenFrom);
				gridPics[currentGridIndex]=selectedObject;
				
				foundANiche=true;//for fixing the most weird annoying bug ever
			}
		}
		currentGridIndex++;
		
		//BODY GRID SQUARE
		if(gridPics[currentGridIndex]!=0)
			GUI.DrawTexture(new Rect(Screen.width*7.3f/20,Screen.height*2.25f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
		GUI.DrawTexture(new Rect(Screen.width*7.3f/20,Screen.height*2.25f/14,Screen.height/14, Screen.height/14),grid);
		if(mouseClickx>Screen.width*7.3f/20 &&mouseClickx<Screen.width*7.3f/20+Screen.height/14&&mouseClicky>Screen.height*2.25f/14 &&mouseClicky<Screen.height*2.25f/14+Screen.height/14){
			if(selectedObject==0)
				selectedObject=gridPics[currentGridIndex];
			gridPics[currentGridIndex] = 0;
			takenFrom=currentGridIndex;
		}
		if(mouseLetGox>Screen.width*7.3f/20 && mouseLetGox<Screen.width*7.3f/20+Screen.height/14&&mouseLetGoy>Screen.height*2.25f/14 &&mouseLetGoy<Screen.height*2.25f/14+Screen.height/14){
			if(selectedObject>0&&selectedObject<6&& gridPics[currentGridIndex]==0&& currentGridIndex!=takenFrom){
				mouseLetGoy=-100000;
				mouseLetGoy=-100000;
				makeTheSwitch(currentGridIndex,takenFrom);
				print ("SOMEFUCKINGTHING");
				gridPics[currentGridIndex]=selectedObject;
				
				foundANiche=true;
			}
		}
		currentGridIndex++;
		
		
		//SPACESHIP GRIDSQUARES
		for(int i=0;i<9;i++){
			if(gridPics[currentGridIndex]!=0)
				GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*3.75f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
			GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*3.75f/14,Screen.height/14, Screen.height/14),grid);
			if(mouseClickx>Screen.width/40+Screen.height*i/14 &&mouseClickx<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseClicky>Screen.height*3.75f/14 &&mouseClicky<Screen.height*3.75f/14+Screen.height/14){
				if(selectedObject==0)
					selectedObject=gridPics[currentGridIndex];
				gridPics[currentGridIndex] = 0;
				takenFrom=currentGridIndex;
			}
			if(mouseLetGox>Screen.width/40+Screen.height*i/14 &&mouseLetGox<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseLetGoy>Screen.height*3.75f/14 &&mouseLetGoy<Screen.height*3.75f/14+Screen.height/14){
				if(gridPics[currentGridIndex]==0&& currentGridIndex!=takenFrom){
					mouseLetGoy=-100000;
					mouseLetGoy=-100000;
					makeTheSwitch(currentGridIndex,takenFrom);
					gridPics[currentGridIndex]=selectedObject;
					
					foundANiche=true;
				}
			}
			currentGridIndex++;
		}
		for(int i=0;i<9;i++){
			if(gridPics[currentGridIndex]!=0)
				GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*4.75f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
			GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*4.75f/14,Screen.height/14, Screen.height/14),grid);
			if(mouseClickx>Screen.width/40+Screen.height*i/14 &&mouseClickx<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseClicky>Screen.height*4.75f/14 &&mouseClicky<Screen.height*4.75f/14+Screen.height/14){
				if(selectedObject==0)
					selectedObject=gridPics[currentGridIndex];
				gridPics[currentGridIndex] = 0;
				takenFrom=currentGridIndex;
			}
			if(mouseLetGox>Screen.width/40+Screen.height*i/14 &&mouseLetGox<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseLetGoy>Screen.height*4.75f/14 &&mouseLetGoy<Screen.height*4.75f/14+Screen.height/14){
				if(gridPics[currentGridIndex]==0&& currentGridIndex!=takenFrom){
					mouseLetGoy=-100000;
					mouseLetGoy=-100000;
					makeTheSwitch(currentGridIndex,takenFrom);
					gridPics[currentGridIndex]=selectedObject;
					
					foundANiche=true;
				}
			}
			currentGridIndex++;
		}
		for(int i=0;i<9;i++){
			if(gridPics[currentGridIndex]!=0)
				GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*5.75f/14,Screen.height/14, Screen.height/14),textures[gridPics[currentGridIndex]]);
			GUI.DrawTexture(new Rect(Screen.width/40+Screen.height*i/14,Screen.height*5.75f/14,Screen.height/14, Screen.height/14),grid);
			if(mouseClickx>Screen.width/40+Screen.height*i/14 &&mouseClickx<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseClicky>Screen.height*5.75f/14 &&mouseClicky<Screen.height*5.75f/14+Screen.height/14){
				if(selectedObject==0)
					selectedObject=gridPics[currentGridIndex];
				gridPics[currentGridIndex] = 0;
				takenFrom=currentGridIndex;
			}
			if(mouseLetGox>Screen.width/40+Screen.height*i/14 &&mouseLetGox<Screen.width/40+Screen.height*i/14+Screen.height/14&&mouseLetGoy>Screen.height*5.75f/14 &&mouseLetGoy<Screen.height*5.75f/14+Screen.height/14){
				if(gridPics[currentGridIndex]==0&& currentGridIndex!=takenFrom){
					mouseLetGoy=-100000;
					mouseLetGoy=-100000;
					makeTheSwitch(currentGridIndex,takenFrom);
					gridPics[currentGridIndex]=selectedObject;
					
					foundANiche=true;
				}
			}
			currentGridIndex++;
		}
		
		if(mouseLetGox>0||mouseLetGoy>0){
			if(foundANiche){
				gridPics[takenFrom]=0;
			}else{
				gridPics[takenFrom]=selectedObject;
				mouseLetGox=-100;
				mouseLetGoy=-100;
			}
		}
		
		
		if(Input.GetMouseButtonDown (0)){
			if(selectedObject!=0){
				if(!stillDown){
					selectedObject=0;
				}
			}
			takenFrom=0;
			stillDown=true;
			foundANiche=false;
			mouseClickx = (int)Input.mousePosition.x - (int)Screen.width/4;
			mouseClicky =(-1)*(int)Input.mousePosition.y +Screen.height -(int)Screen.height/4;
		}
		
		if(Input.GetMouseButton (0)){
			mousex = (int)Input.mousePosition.x - (int)Screen.width/4;
			mousey = (-1)*(int)Input.mousePosition.y +Screen.height -(int)Screen.height/4;
			if(selectedObject!=0)
				GUI.DrawTexture (new Rect(mousex-Screen.height/28,mousey-Screen.height/28,Screen.height/14, Screen.height/14),textures[selectedObject]);	
			print ("Selected Object"+selectedObject);
		}
		if(Input.GetMouseButtonUp (0)){
			mouseClickx = 10000000;
			mouseClicky =10000000;
			mouseLetGox = (int)Input.mousePosition.x - (int)Screen.width/4;
			mouseLetGoy = (-1)*(int)Input.mousePosition.y +Screen.height -(int)Screen.height/4;
			stillDown=false;
		}
		
	}
}
