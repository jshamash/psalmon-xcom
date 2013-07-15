using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public class EmployeeScript : MonoBehaviour {
	
	#region Variables
	
	#region Constants
	public const int EMPLOYEEWAGE=40000;//Salary for normal Employee such as scientists, workers and pilots
	public static readonly int[] SALARIES = {30000,40000,50000,70000,100000};//Salaries of soldiers, depending on their ranks
	#endregion
	
	Base curBase;
	
	//Selected employee variable(s)
	private string nameSelectedEmployee="";
	private int selectedSoldier = 0;
	private bool selectionSoldierExist = false;
	private bool selectionNameExist = false;
	private string[] ranks = {"Private","Corporal", "Sergeant", "Lieutnant", "General"};
 	
	//Vector for available employees scrollbar menu
	Vector2 scrollPosition = Vector2.zero;
	
	//Booleans for pop-up windows
		bool popUpWindow=false;
		bool moreLivingQuarters=false;
		bool notEnoughMoney=false;
		bool scientistsWorking=false;
		bool workersWorking=false;
		bool equipMenu = true;
		bool soldierInAircraft= false;
		bool pilotInAircraft=false;
	
	//Boolean for toggle buttons
		private bool soldierToggle = true;
		private bool scientistToggle = false;
		private bool workerToggle = false;
		private bool pilotToggle = false;
	//Skin for toggle buttons
		public GUISkin toggleSkin;
		public GUISkin employeeSkin;
		public GUISkin selectedEmployeeSkin;
		public GUISkin selectedEmployee;
		public GUISkin unselectedEmployee;
		public Texture2D myTexture;
	
	//List of available for hire employees
		//AFH stands for available for hires
			private LinkedList<Soldier> afhSoldiers = new LinkedList<Soldier>();
			private LinkedList<string> afhScientists = new LinkedList<string>();
			private LinkedList<string> afhWorkers = new LinkedList<string>();
			private LinkedList<string> afhPilots =  new LinkedList<string>();
	
	//List of names initiation in startState() 
		private string[] nameList;
	
	//Randomizer
		private System.Random random= new System.Random();
	
	//Size of Scrollbar
		private int scrollbarSize=Screen.height;
	
	#endregion
	
	
    void OnGUI() {
		//Try to create a pop-up window
		if(popUpWindow){
			GUI.Window (1,new Rect(Screen.width/3,Screen.height/3,Screen.width/3,Screen.height/3),createPopUpWindow,"");
		}
		if(equipMenu){
			//GUI.Window (2,new Rect(Screen.width/4,Screen.height/4,Screen.width/2,Screen.height/2),createEquipWindow,"");
		}
		
		curBase = gameManager.Instance.getCurrentBase();
		//Left portion of screen
		GUI.skin = toggleSkin;
		if(GUI.Toggle (new Rect(0,0,Screen.width/8,Screen.height*0.1f),soldierToggle,"Soldier")){
			soldierToggle=true;
			scientistToggle=false;
			workerToggle=false;
			pilotToggle=false;
		}
		if(GUI.Toggle(new Rect(Screen.width*1/8,0,Screen.width/8,Screen.height*0.1f),scientistToggle,"Scientist")){
			soldierToggle=false;
			scientistToggle=true;
			workerToggle=false;
			pilotToggle=false;
		}
		if(GUI.Toggle(new Rect(Screen.width*2/8,0,Screen.width/8,Screen.height*0.1f),workerToggle,"Worker")){
			soldierToggle=false;
			scientistToggle=false;
			workerToggle=true;
			pilotToggle=false;
		}
		if(GUI.Toggle(new Rect(Screen.width*3/8,0,Screen.width/8,Screen.height*0.1f),pilotToggle,"Pilot")){
			soldierToggle=false;
			scientistToggle=false;
			workerToggle=false;
			pilotToggle=true;
		}		
	//GUI for the Right Side of the Screen
		GUI.Label (new Rect(Screen.width/2+Screen.width*1/10,0,Screen.width*2/10,Screen.height*0.1f),"Money: "+gameManager.Instance.getMoney ());
		GUI.skin = null;
		
		if(GUI.Button (new Rect(Screen.width/2+Screen.width*4/10,0,Screen.width/10,Screen.height*0.1f),"Back")){
			//Changing state to get back to base manager
			gameManager.Instance.setLevel("BaseManager");	
		}
		GUI.Box (new Rect(Screen.width/2,Screen.height*0.1f+2,Screen.width/2,Screen.height*0.1f),"");
		//GUI.Box(new Rect(Screen.width/2,Screen.height*0.3f+2,Screen.width/2,Screen.height*0.7f-2),"Employee Information");
		if(selectionSoldierExist){
			if(soldierToggle){
				GUI.Label (new Rect(Screen.width/2*1.1f,Screen.height*0.2f,200,20),"Employee Name:");
				string tempName = getSoldier(selectedSoldier).getName();
				string newName = tempName;
				//GUI.TextField(new Rect(Screen.width/2*1.1f,Screen.height*0.35f+25,Screen.width/2*0.8f,20),newName);
				newName = GUI.TextField(new Rect(Screen.width/2*1.1f,Screen.height*0.2f+20,Screen.width/2*0.8f,20),newName,40);
				if(tempName!=newName){
					changeSoldierName(selectedSoldier,newName);
				}
			}
		}
		if(selectionNameExist&&!soldierToggle){
			GUI.Label (new Rect(Screen.width/2*1.1f,Screen.height*0.35f,200,20),"Employee Name:");
			if(scientistToggle){
				string check = nameSelectedEmployee.Substring (0,4);
				if(check.Equals ("Dr. ")){
					string tempName = nameSelectedEmployee;
					nameSelectedEmployee = nameSelectedEmployee.Substring(4, nameSelectedEmployee.Length-4);
					nameSelectedEmployee = GUI.TextField(new Rect(Screen.width/2*1.1f,Screen.height*0.35f+25,Screen.width/2*0.8f,20),nameSelectedEmployee,40);
					nameSelectedEmployee = "Dr. "+nameSelectedEmployee;
					if (tempName!=nameSelectedEmployee){
						if(afhScientists.Remove(tempName)){
							afhScientists.AddLast (nameSelectedEmployee);	
						}else{
							curBase.removeHiredScientist(tempName);
							curBase.addHiredScientist(nameSelectedEmployee);
						}
					}
					GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*10,Screen.width/2*0.8f,20),"Yearly Salary: $"+EMPLOYEEWAGE);
					DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
					TimeSpan span= temp2.Subtract(temp1);
					int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
					GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*11,Screen.width/2*0.8f,20),"Cost of hiring till end of current year: $"+salary);
				}
			}else if(workerToggle){
				if(nameSelectedEmployee.Length>=6){
					string check = nameSelectedEmployee.Substring (0,7);
					if(check.Equals ("Worker ")){
						string tempName = nameSelectedEmployee;
						nameSelectedEmployee = nameSelectedEmployee.Substring(7, nameSelectedEmployee.Length-7);
						nameSelectedEmployee = GUI.TextField(new Rect(Screen.width/2*1.1f,Screen.height*0.35f+25,Screen.width/2*0.8f,20),nameSelectedEmployee,40);
						nameSelectedEmployee = "Worker "+nameSelectedEmployee;
						if (tempName!=nameSelectedEmployee){
							if(afhWorkers.Remove(tempName)){
								afhWorkers.AddLast (nameSelectedEmployee);	
							}else{
								curBase.removeHiredWorker(tempName);
								curBase.addHiredWorker(nameSelectedEmployee);
							}
						}
					}
					GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*10,Screen.width/2*0.8f,20),"Yearly Salary: $"+EMPLOYEEWAGE);
					DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
					TimeSpan span= temp2.Subtract(temp1);
					int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
					GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*11,Screen.width/2*0.8f,20),"Cost of hiring till end of current year: $"+salary);
				}
			}else{//Pilot Toggle\
				if(nameSelectedEmployee.Length>=7){
					string check = nameSelectedEmployee.Substring (0,8);
					if(check.Equals ("Captain ")){
						string tempName = nameSelectedEmployee;
						nameSelectedEmployee = nameSelectedEmployee.Substring(8, nameSelectedEmployee.Length-8);
						nameSelectedEmployee = GUI.TextField(new Rect(Screen.width/2*1.1f,Screen.height*0.35f+25,Screen.width/2*0.8f,20),nameSelectedEmployee,40);
						nameSelectedEmployee = "Captain "+nameSelectedEmployee;
						if (tempName!=nameSelectedEmployee){
							if(afhPilots.Remove(tempName)){
								afhPilots.AddLast (nameSelectedEmployee);	
							}else{
								curBase.removeHiredPilot(tempName);
								curBase.addHiredPilot(nameSelectedEmployee);
							}
						}
					}
				}
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*10,Screen.width/2*0.8f,20),"Yearly Salary: $"+EMPLOYEEWAGE);
				DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
				TimeSpan span= temp2.Subtract(temp1);
				int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*11,Screen.width/2*0.8f,20),"Cost of hiring till end of current year: $"+salary);
			}
		}
		if(soldierToggle){
			//GUI for right side of screen
			
			if(selectionSoldierExist){
				Soldier tempSoldier = getSoldier (selectedSoldier);	
				/*
				if(GUI.Button (new Rect(Screen.width/2*1.5f,Screen.height*0.3f+(25)*12,Screen.width/4,Screen.height*0.1f),"Equip Soldier")){
					//OPEN MENU FOR EQUIPPING SOLDIERS
					equipMenu = true;
				}
				*/
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f,Screen.width/2*0.8f,20),"Health: "+tempSoldier.getHealth());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25),Screen.width/2*0.8f,20),"Accuracy: "+tempSoldier.getAccuracy());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*2,Screen.width/2*0.8f,20),"Mind state: "+tempSoldier.getMind());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*3,Screen.width/2*0.8f,20),"Sniping Skills: "+tempSoldier.getSnipePower());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*4,Screen.width/2*0.8f,20),"Assault Skills: "+tempSoldier.getAssaultPower());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*5,Screen.width/2*0.8f,20),"Close Combat Skills: "+tempSoldier.getCloseCombatPower());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*6,Screen.width/2*0.8f,20),"Number of Aliens Killed: "+tempSoldier.getAliens());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*7,Screen.width/2*0.8f,20),"Number of Missions done: "+tempSoldier.getMissions());
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*8,Screen.width/2*0.8f,20),"Rank: "+ranks[tempSoldier.getRank ()]);
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.3f+(25)*10,Screen.width/2*0.8f,20),"Yearly Salary: $"+SALARIES[tempSoldier.getRank ()]);
				DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
				TimeSpan span= temp2.Subtract(temp1);
				int salary = (int)((Double)SALARIES[tempSoldier.getRank ()] * span.TotalDays/365.242199);
				GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*11,Screen.width/2*0.8f,20),"Cost of hiring till end of current year: $"+salary);
				//GUI.Label (new Rect(Screen.width/2*1.2f,Screen.height*0.5f+(25)*10,Screen.width/2*0.8f,20),"Date: "+salary);
			}	
		}
		
	//Show # of employees person can hire in current base and # of soldiers/cientists/workers/pilots hired 
	GUI.skin = toggleSkin;
		GUI.Label (new Rect(Screen.width/2,Screen.height*0.1f,Screen.width/10,Screen.height*0.1f),"Occupation: "
			+curBase.getOccupiedLivingSpace()+"/"+curBase.getLivingSpace());
		GUI.Label (new Rect(Screen.width/2+Screen.width/10,Screen.height*0.1f,Screen.width/10,Screen.height*0.1f),"Soldiers Hired: "
			+curBase.getNumbSoldiers());
		GUI.Label (new Rect(Screen.width/2+Screen.width*2/10,Screen.height*0.1f,Screen.width/10,Screen.height*0.1f),"Scientists Hired: "
			+curBase.getNumbScientists());
		GUI.Label (new Rect(Screen.width/2+Screen.width*3/10,Screen.height*0.1f,Screen.width/10,Screen.height*0.1f),"Workers Hired: "
			+curBase.getNumbWorkers());
		GUI.Label (new Rect(Screen.width/2+Screen.width*4/10,Screen.height*0.1f,Screen.width/10,Screen.height*0.1f),"Pilots Hired: "
			+curBase.getNumbPilots());
	GUI.skin = null;
    //An absolute-positioned example: We make a scrollview that has a really large client
	// rect and put it in a small rect on the screen.
	    scrollPosition = GUI.BeginScrollView (new Rect (0,Screen.height*0.1f,Screen.width/2,Screen.height*0.9f),scrollPosition, new Rect (0, 0, Screen.width/2, scrollbarSize));
		
		GUI.skin = employeeSkin;
	    // Everything under here will be inside the ScollGui View
	    if(soldierToggle){
		//GUI for left side of screen
			LinkedList<Soldier> hired = curBase.getHiredSoldiers();
			Soldier[] tempArray1=new Soldier[hired.Count];
			hired.CopyTo (tempArray1,0);
			for(int i = 0;i<hired.Count;i++){
				if(tempArray1[i].getId()==selectedSoldier){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*i),Screen.width/4*0.90f,50), tempArray1[i].getName ())){
					selectedSoldier=tempArray1[i].getId ();	
					selectionSoldierExist = true;
				}
				GUI.skin = selectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*i),30,30),"")){
					if(curBase.isSoldierInAircraft(tempArray1[i])){
						popUpWindow=true;
						soldierInAircraft=true;
					}else{
						// dequip soldier's weapon and armor
						if (tempArray1[i].getWeapon() != null) {
							gameManager.Instance.getCurrentBase().dequip(tempArray1[i].getWeapon());
							tempArray1[i].setWeapon (null);
						}
						if (tempArray1[i].getArmor() != null) {
							gameManager.Instance.getCurrentBase().dequip(tempArray1[i].getArmor());
							tempArray1[i].setArmor(null);
						}
						afhSoldiers.AddLast (tempArray1[i]);
						curBase.removeHiredSoldier (tempArray1[i]);
						curBase.freeLivingSpace();
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)SALARIES[tempArray1[i].getRank ()] * span.TotalDays/365.242199);
						gameManager.Instance.updateMoney (gameManager.Instance.getMoney () + salary);
					}
				}
				GUI.skin=employeeSkin;
			}
			createAfhSoldiers();
			Soldier[] tempArray2=new Soldier[afhSoldiers.Count];
			afhSoldiers.CopyTo (tempArray2,0);
			for(int i = 0;i<tempArray2.Length;i++){
				if(tempArray2[i].getId()==selectedSoldier){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*hired.Count)+(60*i),Screen.width/4*0.90f,50), tempArray2[i].getName())){
					selectedSoldier=tempArray2[i].getId();
					selectionSoldierExist = true;
				}
				GUI.skin = unselectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*hired.Count)+(60*i),30,30),"")){
					if((curBase.getLivingSpace()-curBase.getOccupiedLivingSpace())<=0){
						popUpWindow=true;
						moreLivingQuarters=true;
					}else{
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)SALARIES[tempArray2[i].getRank ()] * span.TotalDays/365.242199);
						if((gameManager.Instance.getMoney () - salary) <0){
							popUpWindow=true;
							notEnoughMoney=true;
						}else{
							curBase.addHiredSoldier (tempArray2[i]);
							removeAfhSoldier(tempArray2[i]);
							curBase.occupyLivingSpace();
							gameManager.Instance.updateMoney (gameManager.Instance.getMoney () - salary);
						}
					}
				}
				
				GUI.skin=employeeSkin;
			}
			scrollbarSize = (afhSoldiers.Count+hired.Count)*70;
		}else if(scientistToggle){
			LinkedList<string> hired = curBase.getHiredScientists();
			string[] tempArray1=new string[hired.Count];
			hired.CopyTo (tempArray1,0);
			for(int i = 0;i<hired.Count;i++){
				if(tempArray1[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*i),Screen.width/4*0.90f,50), tempArray1[i])){
					nameSelectedEmployee=tempArray1[i];	
					selectionNameExist = true;
				}
				GUI.skin = selectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*i),30,30),"")){
					if(tempArray1.Length-1<curBase.getOccupiedScientists()){
						popUpWindow=true;
						scientistsWorking=true;
					}else{
						afhScientists.AddLast (tempArray1[i]);
						curBase.removeHiredScientist (tempArray1[i]);
						curBase.freeLivingSpace();
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						gameManager.Instance.updateMoney (gameManager.Instance.getMoney () + salary);
					}					
				}
				GUI.skin=employeeSkin;
			}
			createAfhScientists();
			string[] tempArray2=new string[afhScientists.Count];
			afhScientists.CopyTo (tempArray2,0);
			for(int i = 0;i<tempArray2.Length;i++){
				if(tempArray2[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*hired.Count)+(60*i),Screen.width/4*0.90f,50), tempArray2[i])){
					nameSelectedEmployee=tempArray2[i];
					selectionNameExist = true;
				}
				GUI.skin = unselectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*hired.Count)+(60*i),30,30),"")){
					if((curBase.getLivingSpace()-curBase.getOccupiedLivingSpace())<=0){
						popUpWindow=true;
						moreLivingQuarters=true;
					}else{
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						if((gameManager.Instance.getMoney () - salary) <0){
							popUpWindow=true;
							notEnoughMoney=true;
						}else{
							curBase.addHiredScientist (tempArray2[i]);
							afhScientists.Remove(tempArray2[i]);
							curBase.occupyLivingSpace();
							gameManager.Instance.updateMoney (gameManager.Instance.getMoney () - salary);
						}
					}
				}
				
				GUI.skin=employeeSkin;
			}
			scrollbarSize = (afhScientists.Count+hired.Count)*70;
		}else if(workerToggle){
			LinkedList<string> hired = curBase.getHiredWorkers();
			string[] tempArray1=new string[hired.Count];
			hired.CopyTo (tempArray1,0);
			for(int i = 0;i<hired.Count;i++){
				if(tempArray1[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*i),Screen.width/4*0.90f,50), tempArray1[i])){
					nameSelectedEmployee=tempArray1[i];	
					selectionNameExist = true;
				}
				GUI.skin = selectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*i),30,30),"")){
					if(tempArray1.Length-1<curBase.getOccupiedWorkers()){
						popUpWindow=true;
						workersWorking=true;
					}else{
						afhWorkers.AddLast (tempArray1[i]);
						curBase.removeHiredWorker (tempArray1[i]);
						curBase.freeLivingSpace();
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						gameManager.Instance.updateMoney (gameManager.Instance.getMoney () + salary);
					}
				}
				GUI.skin=employeeSkin;
			}
			createAfhWorkers ();
			string[] tempArray2=new string[afhWorkers.Count];
			afhWorkers.CopyTo (tempArray2,0);
			for(int i = 0;i<tempArray2.Length;i++){
				if(tempArray2[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*hired.Count)+(60*i),Screen.width/4*0.90f,50), tempArray2[i])){
					nameSelectedEmployee=tempArray2[i];
					selectionNameExist = true;
				}
				GUI.skin = unselectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*hired.Count)+(60*i),30,30),"")){
					if((curBase.getLivingSpace()-curBase.getOccupiedLivingSpace())<=0){
						popUpWindow=true;
						moreLivingQuarters=true;
					}else{
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						if((gameManager.Instance.getMoney () - salary) <0){
							popUpWindow=true;
							notEnoughMoney=true;
						}else{
							curBase.addHiredWorker (tempArray2[i]);
							afhWorkers.Remove(tempArray2[i]);
							curBase.occupyLivingSpace();
							gameManager.Instance.updateMoney (gameManager.Instance.getMoney () - salary);
						}
					}
				}
				
				GUI.skin=employeeSkin;
			}
			scrollbarSize = (afhWorkers.Count+hired.Count)*70;
		}else if(pilotToggle){
			LinkedList<string> hired = curBase.getHiredPilots();
			string[] tempArray1=new string[hired.Count];
			hired.CopyTo (tempArray1,0);
			for(int i = 0;i<hired.Count;i++){
				if(tempArray1[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*i),Screen.width/4*0.90f,50), tempArray1[i])){
					nameSelectedEmployee=tempArray1[i];
					selectionNameExist = true;
				}
				GUI.skin = selectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*i),30,30),"")){
					if(curBase.isPilotInAircraft(tempArray1[i])){
						popUpWindow=true;
						pilotInAircraft=true;
					}else{
						afhPilots.AddLast (tempArray1[i]);
						curBase.removeHiredPilot(tempArray1[i]);
						curBase.freeLivingSpace();
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						gameManager.Instance.updateMoney (gameManager.Instance.getMoney () + salary);
					}
				}
				GUI.skin=employeeSkin;
			}
			createAfhPilots();
			string[] tempArray2=new string[afhPilots.Count];
			afhPilots.CopyTo (tempArray2,0);
			for(int i = 0;i<tempArray2.Length;i++){
				if(tempArray2[i]==nameSelectedEmployee){
					GUI.skin = selectedEmployeeSkin;
				}else{
					GUI.skin = employeeSkin;
				}
				if(GUI.Button (new Rect(Screen.width/2*0.1f,Screen.height*0.10f+(60*hired.Count)+(60*i),Screen.width/4*0.90f,50), tempArray2[i])){
					nameSelectedEmployee=tempArray2[i];	
					selectionNameExist = true;
				}
				GUI.skin = unselectedEmployee;
				if(GUI.Button(new Rect(Screen.width/2*0.60f,Screen.height*0.10f+(60*hired.Count)+(60*i),30,30),"")){
					if((curBase.getLivingSpace()-curBase.getOccupiedLivingSpace())<=0){
						popUpWindow=true;
						moreLivingQuarters=true;
					}else{
						DateTime temp1 = gameManager.Instance.getTime (),temp2 = new DateTime(temp1.Year+1,1,1);
						TimeSpan span= temp2.Subtract(temp1);
						int salary = (int)((Double)EMPLOYEEWAGE * span.TotalDays/365.242199);
						if((gameManager.Instance.getMoney () - salary) <0){
							popUpWindow=true;
							notEnoughMoney=true;
						}else{
							curBase.addHiredPilot (tempArray2[i]);
							afhPilots.Remove(tempArray2[i]);
							curBase.occupyLivingSpace();
							gameManager.Instance.updateMoney (gameManager.Instance.getMoney () - salary);
						}
					}
				}
				
				GUI.skin=employeeSkin;
			}
			scrollbarSize = (afhPilots.Count+hired.Count)*70;
		}
		GUI.skin = null;
		// End the scroll view that we began above.
    	GUI.EndScrollView ();
    }
	
	public void createAfhSoldiers(){
		int tempInt = afhSoldiers.Count;
		
		if(tempInt<5){
			for(;tempInt<5;tempInt++){
				gameManager.Instance.increaseId();
				afhSoldiers.AddLast (new Soldier(gameManager.Instance.getAvailableId(),getRandomName ()));
			}
		}
		
	}
	
	public void createAfhScientists(){
		int tempInt = afhScientists.Count;
		if(tempInt<5){
			for(;tempInt<5;tempInt++){
				string tempName = "Dr. "+ getRandomName();
				if(afhScientists.Remove(tempName)){
					afhScientists.AddLast(tempName);
					while(afhScientists.Remove(tempName)){
						afhScientists.AddLast(tempName);
							tempName=tempName+" Jr.";
					}
					afhScientists.AddLast(tempName);
				}else{
					afhScientists.AddLast(tempName);
				}
			}
		}else{
			while(tempInt>5){
				afhScientists.RemoveLast();	
				tempInt = afhScientists.Count;
			}
		}
		
	}
	
	public void createAfhWorkers(){
		int tempInt = afhWorkers.Count;
		if(tempInt<5){
			for(;tempInt<5;tempInt++){
				string tempName = "Worker "+getRandomName();
				if(afhWorkers.Remove(tempName)){
					afhWorkers.AddLast(tempName);
					while(afhWorkers.Remove(tempName)){
						afhWorkers.AddLast(tempName);
							tempName=tempName+" Jr.";
					}
					afhWorkers.AddLast(tempName);
				}else{
					afhWorkers.AddLast(tempName);
				}
			}
		}else{
			while(tempInt>5){
				afhWorkers.RemoveLast();	
				tempInt = afhWorkers.Count;
			}
		}		
	}
	
	public void createAfhPilots(){
		int tempInt = afhPilots.Count;
		if(tempInt<2){
			for(;tempInt<2;tempInt++){
				string tempName ="Captain "+ getRandomName();
				if(afhPilots.Remove(tempName)){
					afhPilots.AddLast(tempName);
					while(afhPilots.Remove(tempName)){
						afhPilots.AddLast(tempName);
							tempName=tempName+" Jr.";
					}
					afhPilots.AddLast(tempName);
				}else{
					afhPilots.AddLast(tempName);
				}
			}
		}else{
			while(tempInt>2){
				afhPilots.RemoveLast();	
				tempInt = afhPilots.Count;
			}
		}
	}
	
	public void removeAfhSoldier(Soldier removedSoldier){
		LinkedList<Soldier> newList = new LinkedList<Soldier>();
		int id = removedSoldier.getId ();
		Soldier[] tempList = new Soldier[afhSoldiers.Count];
		afhSoldiers.CopyTo(tempList,0);
		for(int i = 0;i<afhSoldiers.Count;i++){
			if(tempList[i].getId () != id){
				newList.AddLast (tempList[i]);	
			}
		}
		
		afhSoldiers=newList;
	}
	
	//Returns a random name from nameList
	public string getRandomName(){
		return nameList[(int)(random.NextDouble() * (double)nameList.Length)];	
	}
	
	//Creates an array of names
	public string[] createNameList(){
		string nameListString = "Staci Cesare,Marshall Longmore,Sanjuana Belue,Annice Commander,Liane Whitsett,Terisa Googe,Marcie Pietz,Danyell Capella,Norma Wiersma,Rosalyn Eastland,Hershel Radigan,Myrtice Mcevoy,Chasidy Navin,Fidelia Goodner,Annett Rudolph,Regine Champ,Johnna Mirza,Alicia Nealey,Clemmie Pool,Clorinda Moscato,Sharolyn Garza,Lala Priolo,Stephenie Ely,Eleanore Tamayo,Valencia Adorno,Carie Mcmurtrie,Yasuko Luce,Enola Hibner,Jacquelin Mcphatter,Marin Raglin,Clarissa Parnell,Alida Pokorny,Dot Golder,Roger Newsom,Lizzette Canary,Yuko Shiflet,Shane Taketa,Heide Randazzo,Silvia Debow,Lexie Giardina,Jaunita Amore,Yu Sereno,Alpha Gautier,Ramiro Brenes,Flora Quam,Wilfredo Mclachlan,Dawne Michel,Leena Kieffer,Crysta Perrotta,Stacy Bryd,Lynda Riche,Wen Mcmullen,Nicolle Riera,Loise Geary,Cara Johansson,Edgar Gunnell,Brain Bertucci,Herma Hager,Sanora Lablanc,Celine Boomhower,Robyn Vigue,Jinny Trask,Glayds Harbaugh,Eileen Ellenberger,Bernardine,Lewandowski,Elisa Darrington,Jillian Espinoza,Christal Goggans,Veda Thiel,Cyndy Summey,France Cournoyer,Franklyn Wulf,Fairy Cogar,Felton Croker,Tameka Sheriff,Ciara Esterly,Marhta Neel,Marcos Whisenhunt,Naomi Pascoe,Deloise Ammons,Cora Desir,Avis Depaul,Juliane Heyne,Tu Towery,Leontine Mccroy,Lizabeth Altieri,Tiffanie Lemond,Allie Sperber,Johnette Kostka,Johnny Lillie,Magdalene Maciel,Deja Minner,Trudi Macey,Concepcion Folden,Carrol Cather,Erick Osterman,Classie Crippen,Charley Schurman,Clifford Harnage,Jules Mateer";
		string[] words = nameListString.Split (',');
		return words;
	}
	
	//Obtain the soldier associated to the id 
	public Soldier getSoldier(int id){
		LinkedList<Soldier> tempList = curBase.getHiredSoldiers ();
		Soldier[] tempArray= new Soldier[tempList.Count+afhSoldiers.Count];
		tempList.CopyTo (tempArray,0);
		afhSoldiers.CopyTo(tempArray,tempList.Count);
		for(int i = 0;i<tempArray.Length;i++){
			if(tempArray[i].getId ()==id){
				return 	tempArray[i];
			}
		}
		return null;
		
	}
	
	//Change name of soldier
	public void changeSoldierName(int id, string newName){
		LinkedList<Soldier> tempList = new LinkedList<Soldier>();
		Soldier[] tempArray = new Soldier[afhSoldiers.Count];
		afhSoldiers.CopyTo(tempArray,0);
		bool changed = false;
		for(int i=0; i<tempArray.Length;i++){
			if(tempArray[i].getId ()==id){
				tempArray[i].changeName(newName);
				changed=true;
			}
			tempList.AddLast (tempArray[i]);
		}
		if(!changed){
			tempList = new LinkedList<Soldier>();
			tempArray = new Soldier[curBase.getHiredSoldiers().Count];
			curBase.getHiredSoldiers().CopyTo(tempArray,0);
			for(int i=0; i<tempArray.Length;i++){
				if(tempArray[i].getId ()==id){
					tempArray[i].changeName(newName);
				}
				tempList.AddLast (tempArray[i]);	
			}
			curBase.setHiredSoldiersList(tempList);
		}else{
			afhSoldiers=tempList;	
		}
	}
	
	
	public void createPopUpWindow(int windowInt){
		if(notEnoughMoney){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"You do not posess sufficient funds to hire this employee.");
		}else if(moreLivingQuarters){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"There is no more space in your base to hire more employee.  Add more living quarters to your base to complete this action");
		}else if(scientistsWorking){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"There are "+curBase.getOccupiedScientists()+" scientists working on projects.  You need to decrease the numbers of scientists working on your projects to fire more scientists.");
		}else if(workersWorking){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"There are "+curBase.getOccupiedWorkers()+" workers producing items.  You need to decrease the numbers of workers producing items to fire more workers.");
		}else if(soldierInAircraft){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"This soldier is currently in the base aircraft.  Proceed by removing this soldier from the aircraft before you can fire him/her.");
		}else if(pilotInAircraft){
			GUI.Label (new Rect(10,10,Screen.width/3-10,80),"This pilot is currently in the base aircraft.  Proceed by removing the pilot from the aircraft before you can fire him/her.");			
		}
		
		if (GUI.Button(new Rect(Screen.width/9, Screen.height*4/18, Screen.width/9, Screen.height/18), "Ok")){
			moreLivingQuarters=false;
			notEnoughMoney=false;
			popUpWindow=false;
			workersWorking=false;
			scientistsWorking=false;
            print("Got a click");
		}
	}
	
	
	// Use this for initialization 
	void Start () {
		//Creates the list of random names for new employee creation
		nameList = createNameList();
		
		curBase = gameManager.Instance.getCurrentBase();
	}
	
	#region Are These Useful?
	
	// Update is called once per frame
	void Update () {
	
	}
	#endregion
}
