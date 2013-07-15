using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// GUI for the base manager menu.
/// </summary>
public class baseMenu : MonoBehaviour {
	private const int BASE_MAIN = 0;
	private const int FACILITIES = 1;
	private const int RESEARCH = 2;
	private const int PRODUCTION = 3;
	private const int BUY_SELL = 4;
	private const int CONTAINMENT = 5;
	private const int HOSPITAL = 6;
	private const int TRANSFER = 7;
	private const int AIRCRAFT = 8;
	private const int SUMMARY = 9;
	private const int EQUIP = 10;
	
	private int menuShown;
	public float topMenuHeight = 75;
	public float menuItemHeight = 40;
	
	// Button textures
	public Texture increase;
	public Texture decrease;
	public GUISkin increaseSkin;
	public GUISkin decreaseSkin;
	
	public Texture2D researchWindowBackground;
	
	int researchItemSelected = 0;
	
	// Production stuff
	private Vector2 scrollPosition = new Vector2(0,0);
	private Vector2 scrollPosition2 = new Vector2(0,0);
	private int productionSelected = 0;
	private ProductionOrder order;
	bool showWeaponInfo;
	
	// Buy/Sell stuff
	private int storeItemSelected = 0;
	
	private string facilitiesMessage = "";
	private string researchMessage = "";
	private string productionMessage = "";
	private string storeMessage = "";
	private string transferMessage = "";
	private string aircraftMessage = "";
	
	private Facility selectedFacility;
	
	// Transfer stuff
	private int category;
	private Vector2 scrollPositionT = new Vector2(0,0);
	private WeaponTransfer wTransfer = new WeaponTransfer();
	private EmployeeTransfer eTransfer = new EmployeeTransfer();
	private Base destBase = null;
	
	//Aircraft stuff
	int soldierSelected = 0;
	int pilotSelected = 0;
	private Vector2 scrollPositionA = new Vector2(0,0);
	
	public Base currentBase;
	
	private gameManager manager;
	
	
	// Use this for initialization
	void Start () {
		manager = gameManager.Instance;
		
		// THIS SECTION IS FOR TESTING PURPOSES
		/*
		if (!manager.baseExist("Base 1")) {
			manager.startState();
			manager.addBase("Base 1");
			manager.addBase("Base 2");
			manager.addBase("Base 3");
			manager.createInitialBase ();
			DontDestroyOnLoad(manager);
		}
		*/
		//END OF SECTION

		
		currentBase = manager.getCurrentBase();
		menuShown = BASE_MAIN;
		
		order = new ProductionOrder(manager.getAvailableWeapons()[0]);
		
		destBase = manager.getBaseAfter(currentBase);
	}
	
	void OnGUI() {		
		if (menuShown == BASE_MAIN) {
			showBaseMenu();
		}
		if (menuShown == FACILITIES) {
			showFacilitiesMenu();
		}
		if (menuShown == RESEARCH) {
			showResearchMenu();
		}
		if (menuShown == PRODUCTION) {
			showProductionMenu();
		}
		if (menuShown == BUY_SELL) {
			showBuySellMenu();
		}
		if (menuShown == CONTAINMENT) {
			showContainmentMenu();
		}
		if (menuShown == HOSPITAL) {
			showHospitalMenu();
		}
		if (menuShown == TRANSFER) {
			showTransferMenu();
		}
		if (menuShown == AIRCRAFT) {
			showAircraftMenu();
		}
		if (menuShown == SUMMARY) {
			showSummaryMenu();
		}
		if (menuShown == EQUIP) {
			showEquipMenu();
		}
	}
	
	void showTopMenu() {
		
		TextAnchor old = GUI.skin.box.alignment;
		GUI.skin.box.alignment = TextAnchor.MiddleCenter;
		// Top Menu:
		
		GUILayout.BeginArea(new Rect(0, 0, Screen.width, topMenuHeight));
		GUILayout.BeginHorizontal();
		if (GUILayout.Button("Prev", GUILayout.ExpandHeight(true), GUILayout.Width(topMenuHeight))) {
			currentBase = manager.previousBase();
			eTransfer = new EmployeeTransfer();
			wTransfer = new WeaponTransfer();
			soldierSelected = 0;
			pilotSelected = 0;
			facilitiesMessage = "";
			researchMessage = "";
			productionMessage = "";
			storeMessage = "";
			transferMessage = "";
			aircraftMessage = "";
		}
		GUILayout.Box("Base Name: " + currentBase.getName(), GUILayout.ExpandHeight(true), GUILayout.Width(Screen.width/2));
		if (GUILayout.Button("Next", GUILayout.ExpandHeight(true), GUILayout.Width(topMenuHeight))) {
			currentBase = manager.nextBase();
			eTransfer = new EmployeeTransfer();
			wTransfer = new WeaponTransfer();
			soldierSelected = 0;
			pilotSelected = 0;
			facilitiesMessage = "";
			researchMessage = "";
			productionMessage = "";
			storeMessage = "";
			transferMessage = "";
			aircraftMessage = "";
		}
		GUILayout.Box("Money: "+gameManager.Instance.getMoney(), GUILayout.ExpandHeight(true));
		if (GUILayout.Button("Back", GUILayout.Width(topMenuHeight), GUILayout.ExpandHeight (true))) {
			if (menuShown == BASE_MAIN) {
				GUI.skin.box.alignment = old;
				manager.setLevel("WorldMap"); 
			}
			else if (menuShown == EQUIP) {
				menuShown = AIRCRAFT;
			}
			else {
				menuShown = BASE_MAIN;
			}
		}
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		GUI.skin.box.alignment = old;
	}
		
	void showBaseMenu() {
		
		showTopMenu();
		
		// Layout for bottom of menu.
		GUILayout.BeginArea(new Rect(0, topMenuHeight, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		
		// Display facilities:
		facilitiesMessage = "";
		selectedFacility = null;
		GUILayout.Label(facilitiesMessage, GUILayout.Width(2*Screen.width/3), GUILayout.ExpandHeight(true));
		
		// Menu Buttons
		GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
		
		if (GUILayout.Button ("Build Facilities", GUILayout.Height(menuItemHeight))) {
			menuShown = FACILITIES;
		}
		
		if (GUILayout.Button ("Buy/Sell", GUILayout.Height(menuItemHeight))) {
			menuShown = BUY_SELL;
		}
		if (GUILayout.Button ("Transfer", GUILayout.Height(menuItemHeight))) {
			menuShown = TRANSFER;
		}
		if (GUILayout.Button ("Research", GUILayout.Height(menuItemHeight))) {
			menuShown = RESEARCH;
		}
		if (GUILayout.Button ("Production", GUILayout.Height(menuItemHeight))) {
			menuShown = PRODUCTION;
		}
		if (GUILayout.Button ("Employees", GUILayout.Height(menuItemHeight))){
			manager.setLevel ("Employee");
		}
		if (GUILayout.Button ("Aircraft", GUILayout.Height(menuItemHeight))) {
			menuShown = AIRCRAFT;
		}
		if (currentBase.facilityCount("Containment") > 0) {
			if (GUILayout.Button ("Containment", GUILayout.Height(menuItemHeight))){
				menuShown = CONTAINMENT;
			}
		}
		if (currentBase.facilityCount("Hospital") > 0) {
			if (GUILayout.Button ("Hospital", GUILayout.Height(menuItemHeight))){
				menuShown = HOSPITAL;
			}
		}
		if (GUILayout.Button ("Base Summary", GUILayout.Height(menuItemHeight))) {
			menuShown = SUMMARY;
		}
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void showFacilitiesMenu() {
		showTopMenu();
		
		// Layout for bottom of menu.
		GUILayout.BeginArea(new Rect(0, topMenuHeight, Screen.width, Screen.height));
		GUILayout.BeginHorizontal();
		
		// Display facilities:
		GUILayout.Label(facilitiesMessage, GUILayout.Width(2*Screen.width/3), GUILayout.ExpandHeight(true));
		
		// Menu Buttons
		GUILayout.BeginVertical("box", GUILayout.ExpandHeight(true));
		
		foreach (Facility f in AllFacilities.facilities)
		{
			if (GUILayout.Button(f.getName(), GUILayout.Height(menuItemHeight)))
			{
				if (f.getName () == "Hangar" && currentBase.facilityCount("Hangar") > 0)
				{
					cantBuildHangar();
				}
				else
				{
					facilitiesMessage = "Place " + f.getName() + "\n";
					facilitiesMessage += f.getDescription() + "\n";
					facilitiesMessage += "Cost: " + f.getCost() + " -- Capacity: " + f.getCapacity ();
					selectedFacility = f;
				}
			}
		}
		GUILayout.EndVertical();
		
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
	}
	
	void showResearchMenu()
	{
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(0, windowRect, researchWindow, "", windowStyle);
	}
	
	void researchWindow(int id)
	{		
		float margin = 15;
		int space = 10;
		float selectionAreaWidth = 7*Screen.width/12;
		float weaponInfoHeight = 2*(Screen.height-topMenuHeight)/3;
		float itemColWidth = 250;
		float scientistsColWidth = 50;		
		float buttonSize = 15;
		float itemHeight = 30;
		
		List<Weapon> weapons = new List<Weapon>();
		foreach (Queue<Weapon> q in manager.getUnresearchedWeapons())
		{
			if (q.Count <= 0)
			{
				continue;
			}
			Weapon w = q.Peek();
			if (w.isResearchable())
			{
				// Add to list
				weapons.Add(w);
			}			
		}
		
		string[] names = new string[weapons.Count];
		for (int i = 0; i < names.Length; i++)
			names[i] = weapons[i].getName();
		
		GUIStyle centeredLabelStyle = new GUIStyle();
		centeredLabelStyle.normal = GUI.skin.label.normal;
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		centeredLabelStyle.margin = GUI.skin.label.margin;
		
		showTopMenu();
		
		#region Research Selection
		GUILayout.BeginArea(new Rect(margin
									,topMenuHeight+margin
									,selectionAreaWidth
									,Screen.height-topMenuHeight-2*margin)
							,GUI.skin.box);
		
		GUILayout.BeginVertical ();
		
		// First row
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label("Item Name", centeredLabelStyle, GUILayout.Width(itemColWidth));
		GUILayout.Space(space);
		GUILayout.Label ("Assigned Scientists", centeredLabelStyle, GUILayout.Width(scientistsColWidth+buttonSize+increaseSkin.button.margin.horizontal));
		GUILayout.EndHorizontal ();
		
		GUILayout.Space(margin);
		
		if (weapons != null && weapons.Count > 0)
		{
			// Scrollable section
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width (selectionAreaWidth));
			GUILayout.BeginHorizontal();
			GUILayout.Space(margin);
			// First column -- item name
			researchItemSelected = GUILayout.SelectionGrid(researchItemSelected, names, 1, GUILayout.Width (itemColWidth),
														GUILayout.Height(weapons.Count*(space+itemHeight)));
			
			GUILayout.Space(space);
			
			// Next two columns -- Scientists, Buttons
			GUILayout.BeginVertical();
			// Changing the alignment -- workaround
			TextAnchor oldLabel = GUI.skin.label.alignment;
			TextAnchor oldButton = GUI.skin.button.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			foreach (Weapon w in weapons) {
				GUILayout.BeginHorizontal(GUILayout.Height(itemHeight));
				//Purchased
				GUILayout.Label ("" + w.getAssignedScientists(), GUILayout.Width (scientistsColWidth));
				GUILayout.BeginVertical();
				// Buttons
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					researchMessage = "";
					// Make sure a scientist is available and there is space
					if (currentBase.getOccupiedScientists() < currentBase.getNumbScientists() && currentBase.getAvailLabSpace() > 0)
					{						
						// Occupy scientist in base
						if (currentBase.occupyScientist(w))
						{
							// If it worked, assign scientist to project
							w.assignScientist();
						}
					}
					else
					{
						researchMessage = "Not enough lab space and/or scientists.\nBuild a lab or hire some scientists!";
					}
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					researchMessage = "";
					// Free scientist in base
					if (currentBase.freeScientist(w))
					{
						// If it worked, remove scientist from project
						w.removeScientist();
					}
					else researchMessage = "No scientists from this base were assigned to that project.";
				}
				
				GUILayout.EndVertical();
				GUILayout.Space(space);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(space);
			}
			GUI.skin.button.alignment = oldButton;
			GUI.skin.label.alignment = oldLabel;
			GUILayout.EndVertical();
			GUILayout.Space(margin);
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label ("No Items available for research at this time.", centeredLabelStyle);
		}
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
		#endregion
		
		#region Lab/Scientists Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,topMenuHeight+margin
									,Screen.width-selectionAreaWidth-3*margin
									,Screen.height-weaponInfoHeight-topMenuHeight-3*margin)
							,GUI.skin.box);
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Lab Space\n(used/all)", centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Label (currentBase.getTotalLabSpace() - currentBase.getAvailLabSpace() + "/" + currentBase.getTotalLabSpace(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Scientists\n(available/all)", centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Label (currentBase.getNumbScientists() - currentBase.getOccupiedScientists() + "/" + currentBase.getNumbScientists(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label (researchMessage, centeredLabelStyle);
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		#endregion
		
		#region Weapon Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,Screen.height-margin-weaponInfoHeight
									,Screen.width-selectionAreaWidth-3*margin
									,weaponInfoHeight)
							,GUI.skin.box);
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		if (weapons != null && weapons.Count > 0)
		{
			Weapon w = weapons[researchItemSelected];
			scrollPosition2 = GUILayout.BeginScrollView (scrollPosition2);
			GUILayout.BeginVertical();
			GUILayout.Space(margin);
			GUILayout.Label(names[researchItemSelected], centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			GUILayout.Label (w.getImage (), centeredLabelStyle, GUILayout.Height (weaponInfoHeight/3));
			GUILayout.Space(space);
			GUILayout.Label ("Research days left (one man): " + w.getDaysToResearch(), centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			GUILayout.Label ("Storage: " + w.getStorageSpace()
							,centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			GUILayout.Label ("Cost: " + w.getCost(), centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			
			if (w.isArmourType())
			{
				GUILayout.Label ("Protection: " + w.getProtection()*100 + "%", centeredLabelStyle, GUILayout.ExpandHeight(true));
			}
			else
			{
				GUILayout.Label ("Damage: " + w.getDamage(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Range: " + w.getRange(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Max Ammo: " + w.getMaxAmmo(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Action Points: " + w.getActionPoints(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				//TODO -- accuracy = (angle/10)*100?
			}
			
			GUILayout.Space(margin);
			GUILayout.EndVertical();
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label ("No Items Available", centeredLabelStyle, GUILayout.ExpandHeight(true));
		}
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		#endregion
	}
	
	void showProductionMenu()
	{
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(1, windowRect, productionWindow, "", windowStyle);
	}
	
	void productionWindow(int windowID)
	{	
		float margin = 15;
		int space = 10;
		float selectionAreaWidth = 7*Screen.width/12;
		float weaponInfoHeight = 2*(Screen.height-topMenuHeight)/3;
		float itemColWidth = 250;
		float producedColWidth = 100;
		float limitColWidth = 100;
		float buttonSize = 15;
		float itemHeight = 30;
		
		List<Weapon> weapons = manager.getAvailableWeapons();

		string[] names = new string[weapons.Count];
		for (int i = 0; i < weapons.Count; i++) {
			names[i] = weapons[i].getName();
		}
		
		GUIStyle centeredLabelStyle = new GUIStyle();
		centeredLabelStyle.normal = GUI.skin.label.normal;
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		centeredLabelStyle.margin = GUI.skin.label.margin;
		
		showTopMenu();
		
		#region Weapon Selection
		GUILayout.BeginArea(new Rect(margin
									,topMenuHeight+margin
									,selectionAreaWidth
									,Screen.height-topMenuHeight-2*margin)
							,GUI.skin.box);
		
		GUILayout.BeginVertical ();
		
		// First row
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label("Item Name", centeredLabelStyle, GUILayout.Width(itemColWidth));
		GUILayout.Space(space);
		GUILayout.Label ("Produced", centeredLabelStyle, GUILayout.Width(producedColWidth));
		GUILayout.Space(space);
		GUILayout.Label ("Item Limit", centeredLabelStyle, GUILayout.Width(limitColWidth));
		GUILayout.EndHorizontal ();
		
		GUILayout.Space(margin);
		
		if (weapons != null && weapons.Count > 0)
		{
			// Scrollable section
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width (selectionAreaWidth));
			
			GUILayout.BeginHorizontal();
			GUILayout.Space(margin);
			// First column -- item name
			int previousItem = productionSelected;
			productionSelected = GUILayout.SelectionGrid(productionSelected, names, 1, GUILayout.Width (itemColWidth),
														GUILayout.Height(weapons.Count*(space+itemHeight)));
			if (previousItem != productionSelected)
			{
				// On change, instantiate new order
				order = new ProductionOrder(weapons[productionSelected]);
				if (currentBase.orderInProgress(order.getWeapon()))
					productionMessage = "Order in progress";
				else
					productionMessage = "";
			}
			GUILayout.Space(space);
			
			// Next two columns -- Produced, Production Limit
			GUILayout.BeginVertical();
			// Changing the alignment -- workaround
			TextAnchor oldLabel = GUI.skin.label.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			foreach (Weapon w in weapons) {
				GUILayout.BeginHorizontal(GUILayout.Height(itemHeight));
				//Produced
				GUILayout.Label ("" + currentBase.getAmountManufactured(w), GUILayout.Width (producedColWidth));
				GUILayout.Space(space);
				// Limit
				GUILayout.Label ("" + w.getProductionLimit(), GUILayout.Width(limitColWidth));
				GUILayout.Space(space);
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(space);
			}
			//GUI.skin.button.alignment = oldButton;
			GUI.skin.label.alignment = oldLabel;
			GUILayout.EndVertical();
			GUILayout.Space(margin);
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label ("No Items available for production!\nResearch some first.", centeredLabelStyle);
		}
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
		#endregion
		
		#region Workshop Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,topMenuHeight+margin
									,Screen.width-selectionAreaWidth-3*margin
									,Screen.height-weaponInfoHeight-topMenuHeight-3*margin)
							,GUI.skin.box);
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Workers\n(Ready/all)", centeredLabelStyle, GUILayout.ExpandHeight (true), GUILayout.Width(itemColWidth));
		GUILayout.Label ((currentBase.getNumbWorkers()-currentBase.getOccupiedWorkers())
							+ "/" + currentBase.getNumbWorkers(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Workshop Capacity\n(Used/total)", centeredLabelStyle, GUILayout.ExpandHeight (true), GUILayout.Width(itemColWidth));
		GUILayout.Label (currentBase.getOccupiedWorkers() + "/" + currentBase.getTotalWorkshopSpace(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Storage Capacity\n(Used/total)", centeredLabelStyle, GUILayout.ExpandHeight (true), GUILayout.Width(itemColWidth));
		GUILayout.Label (currentBase.getUsedStorage() + "/" + currentBase.getTotalStorageSpace(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		#endregion
		
		#region Weapon Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,Screen.height-margin-weaponInfoHeight
									,Screen.width-selectionAreaWidth-3*margin
									,weaponInfoHeight)
							,GUI.skin.box);
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		if (weapons != null && weapons.Count > 0)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(margin);
			GUILayout.Label(names[productionSelected], centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			if (!showWeaponInfo)
			{
				GUILayout.Label ("Production Days: " + weapons[productionSelected].getProductionTime()/24, centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				
				//Choose Quantity
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("Quantity: " + order.getQuantity(), centeredLabelStyle, GUILayout.Height(2*buttonSize));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				// Buttons
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					order.increaseQuantity();
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					order.decreaseQuantity();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				
				GUILayout.Space(margin);
				
				// Choose workers
				GUILayout.BeginHorizontal();
				GUILayout.FlexibleSpace();
				GUILayout.Label ("Workers: " + order.getWorkers(), centeredLabelStyle, GUILayout.Height(2*buttonSize));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				// Buttons
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					if (order.getWorkers() < currentBase.getNumbWorkers() - currentBase.getOccupiedWorkers())
						order.increaseWorkers();
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					order.decreaseWorkers();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				
				GUILayout.Space(margin);
				
				if (GUILayout.Button("Place Order"))
				{
					// Check weapon isn't already in an order
					if (currentBase.orderInProgress(order.getWeapon()))
					{
						productionMessage = "Error: Order is already in progress";
					}
					// Check quantity/workers > 0
					else if (order.getQuantity() <= 0 || order.getWorkers() <= 0)
					{
						productionMessage = "Error: Must assign quantity and workers";
					}
					// Check production limit
					else if (currentBase.getAmountManufactured(order.getWeapon()) + order.getQuantity() > order.getWeapon().getProductionLimit())
					{
						productionMessage = "Error: exceeds item's production limit";
					}			
					// Check that there are enough free workers
					else if (currentBase.getNumbWorkers() - currentBase.getOccupiedWorkers() < order.getWorkers())
					{
						productionMessage = "Error: not enough free workers to process request";
					}
					// Check that there is enough workshop space
					else if (order.getWorkers() > currentBase.getTotalWorkshopSpace() - currentBase.getOccupiedWorkers())
					{
						productionMessage = "Error: not enough workshop space";
					}
					else
					{
						order.placeOrder();
						
						productionMessage = "Order placed!\nDays until completion: " + (double)(order.getTimeRemaining()/order.getWorkers())/24.0;
						
						// Occupy workers
						for (int i = 0; i < order.getWorkers(); i++)
							currentBase.occupyWorker();
						
						// Add to list of active orders
						currentBase.placeOrder(order);
						
						order = new ProductionOrder(weapons[productionSelected]);
					}
				}
				
				GUILayout.Space(margin);
				
				if (GUILayout.Button("Additional Info"))
				{
					showWeaponInfo = true;
				}
				
				GUILayout.Space(margin);
				
				GUILayout.Label (productionMessage, centeredLabelStyle);
			}
			else
			{
				space = 7;
				Weapon w = order.getWeapon();
				// Show weapon details
				GUILayout.Label (w.getImage (), centeredLabelStyle, GUILayout.Height (weaponInfoHeight/4));
				GUILayout.Space(space);
				GUILayout.Label ("Storage: " + w.getStorageSpace()
								,centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Cost: " + w.getCost(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				
				if (w.isArmourType())
				{
					GUILayout.Label ("Protection: " + w.getProtection()*100 + "%", centeredLabelStyle, GUILayout.ExpandHeight(true));
				}
				else
				{
					GUILayout.Label ("Damage: " + w.getDamage(), centeredLabelStyle, GUILayout.ExpandHeight(true));
					GUILayout.Space(space);
					GUILayout.Label ("Range: " + w.getRange(), centeredLabelStyle, GUILayout.ExpandHeight(true));
					GUILayout.Space(space);
					GUILayout.Label ("Max Ammo: " + w.getMaxAmmo(), centeredLabelStyle, GUILayout.ExpandHeight(true));
					GUILayout.Space(space);
					GUILayout.Label ("Action Points: " + w.getActionPoints(), centeredLabelStyle, GUILayout.ExpandHeight(true));
					//TODO -- accuracy = (angle/10)*100?
				}
				

				if (GUILayout.Button("Back"))
				{
					showWeaponInfo = false;
				}
			}
			
			GUILayout.Space(margin);
			GUILayout.EndVertical();
			
		}
		else
		{
			GUILayout.Label ("No Weapons Available", centeredLabelStyle, GUILayout.ExpandHeight(true));
		}
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		#endregion
	}
	
	#region Buy/Sell Menu
	void showBuySellMenu()
	{
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(2, windowRect, buySellWindow, "", windowStyle);		
	}
	
	void buySellWindow(int windowID)
	{
		float margin = 15;
		int space = 10;
		float selectionAreaWidth = 7*Screen.width/12;
		float weaponInfoHeight = 2*(Screen.height-topMenuHeight)/3;
		float itemColWidth = 250;
		float purchasedColWidth = 50;
		float stockColWidth = 100;
		float costColWidth = 100;
		float itemHeight = 30;
		float buttonSize = itemHeight/2;
		
		List<Weapon> weapons = currentBase.getManufacturedItems();
		string[] names = new string[weapons.Count];
		for (int i = 0; i < names.Length; i++)
			names[i] = weapons[i].getName();
		
		GUIStyle centeredLabelStyle = new GUIStyle();
		centeredLabelStyle.normal = GUI.skin.label.normal;
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		centeredLabelStyle.margin = GUI.skin.label.margin;
		
		showTopMenu();
		
		#region Weapon Selection
		GUILayout.BeginArea(new Rect(margin
									,topMenuHeight+margin
									,selectionAreaWidth
									,Screen.height-topMenuHeight-2*margin)
							,GUI.skin.box);
		
		GUILayout.BeginVertical ();
		
		// First row
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label("Item Name", centeredLabelStyle, GUILayout.Width(itemColWidth));
		GUILayout.Space(space);
		GUILayout.Label ("Purchased", centeredLabelStyle, GUILayout.Width(purchasedColWidth+buttonSize));
		GUILayout.Space(space);
		GUILayout.Label ("In Stock", centeredLabelStyle, GUILayout.Width(stockColWidth));
		GUILayout.Space(space);
		GUILayout.Label ("Cost", centeredLabelStyle, GUILayout.Width(costColWidth));
		GUILayout.EndHorizontal ();
		
		GUILayout.Space(margin);
		
		if (weapons != null && weapons.Count > 0)
		{
			// Scrollable section
			scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true), GUILayout.Width (selectionAreaWidth));
			GUILayout.BeginHorizontal();
			GUILayout.Space(margin);
			// First column -- item name
			storeItemSelected = GUILayout.SelectionGrid(storeItemSelected, names, 1, GUILayout.Width (itemColWidth),
														GUILayout.Height(weapons.Count*(itemHeight+space)));
			
			GUILayout.Space(space);
			GUILayout.Space(margin);
			// Next four columns -- Purchased, Buttons, In Stock, Cost
			GUILayout.BeginVertical(GUILayout.Height(weapons.Count*(space+itemHeight)));
			// Changing the alignment -- workaround
			TextAnchor oldLabel = GUI.skin.label.alignment;
			TextAnchor oldButton = GUI.skin.button.alignment;
			GUI.skin.label.alignment = TextAnchor.MiddleCenter;
			GUI.skin.button.alignment = TextAnchor.MiddleCenter;
			foreach (Weapon w in weapons) {
				int purchased = currentBase.getAmountPurchased(w);
				int manufactured = currentBase.getAmountManufactured(w);
				GUILayout.BeginHorizontal(GUILayout.Height(itemHeight));
				//Purchased
				GUILayout.Label ("" + purchased, GUILayout.Width (purchasedColWidth), GUILayout.Height (itemHeight));
				GUILayout.BeginVertical(GUILayout.Height(itemHeight));
				// Buttons
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					if (purchased >= manufactured)
					{
						// None in stock
						storeMessage = "Out of stock";
					}
					else if (w.getCost() > manager.getMoney())
					{
						// Can't afford
						storeMessage = "You cannot afford this item";
					}
					else if (w.getStorageSpace() > currentBase.getTotalStorageSpace()-currentBase.getUsedStorage())
					{
						// Not enough space
						storeMessage = "Not enough storage space";
					}
					else
					{
						storeMessage = "";
						currentBase.purchaseItem(w);
					}
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					if (currentBase.getAmountPurchased(w) <= 0)
					{
						// Nothing to return
						storeMessage = "Nothing to return";
					}
					else if (currentBase.getAmountPurchased(w) - 1 < currentBase.getEquipped(w))
					{
						// All weapons w are in use by soldiers
						storeMessage = "Item is in use by a soldier. You can free it in the equip soldiers menu.";
					}
					else
					{
						storeMessage = "";
						currentBase.returnItem(w);
					}
				}
				GUILayout.EndVertical();
				GUILayout.Space(space);
				// Stock
				GUILayout.Label ("" + (manufactured-purchased), GUILayout.Width(stockColWidth), GUILayout.Height (itemHeight));
				GUILayout.Space(space);
				// Cost
				GUILayout.Label ("" + w.getCost(), GUILayout.Width(costColWidth), GUILayout.Height (itemHeight));
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
				GUILayout.Space(space);
			}
			GUI.skin.button.alignment = oldButton;
			GUI.skin.label.alignment = oldLabel;
			GUILayout.EndVertical();
			GUILayout.Space(margin);
			GUILayout.EndHorizontal();
			GUILayout.EndScrollView();
		}
		else
		{
			GUILayout.Label ("No Items available for purchase!\nManufacture some first.", centeredLabelStyle);
		}
		GUILayout.EndVertical();
		
		GUILayout.EndArea();
		#endregion
		
		#region Storage Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,topMenuHeight+margin
									,Screen.width-selectionAreaWidth-3*margin
									,Screen.height-weaponInfoHeight-topMenuHeight-3*margin)
							,GUI.skin.box);
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label ("Storage Space\n(Used/all)", centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Label (":", centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Label (currentBase.getUsedStorage() + "/" + currentBase.getTotalStorageSpace(), centeredLabelStyle, GUILayout.ExpandHeight (true));
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		GUILayout.Label (storeMessage, centeredLabelStyle);
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		#endregion
		
		#region Weapon Info
		GUILayout.BeginArea(new Rect(selectionAreaWidth+2*margin
									,Screen.height-margin-weaponInfoHeight
									,Screen.width-selectionAreaWidth-3*margin
									,weaponInfoHeight)
							,GUI.skin.box);
		GUILayout.BeginHorizontal();
		GUILayout.Space(margin);
		if (weapons != null && weapons.Count > 0)
		{
			Weapon w = weapons[storeItemSelected];
			
			GUILayout.BeginVertical();
			GUILayout.Space(margin);
			GUILayout.Label(w.getName(), centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			GUILayout.Label (w.getImage (), centeredLabelStyle, GUILayout.Height (weaponInfoHeight/3));
			GUILayout.Space(space);
			GUILayout.Label ("Storage: " + w.getStorageSpace()
							,centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			GUILayout.Label ("Cost: " + w.getCost(), centeredLabelStyle, GUILayout.ExpandHeight(true));
			GUILayout.Space(space);
			
			if (w.isArmourType())
			{
				GUILayout.Label ("Protection: " + w.getProtection()*100 + "%", centeredLabelStyle, GUILayout.ExpandHeight(true));
			}
			else
			{
				GUILayout.Label ("Damage: " + w.getDamage(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Range: " + w.getRange(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Max Ammo: " + w.getMaxAmmo(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				GUILayout.Space(space);
				GUILayout.Label ("Action Points: " + w.getActionPoints(), centeredLabelStyle, GUILayout.ExpandHeight(true));
				//TODO -- accuracy = (angle/10)*100?
			}
			GUILayout.Space(margin);
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.Label ("No Weapons Available", centeredLabelStyle, GUILayout.ExpandHeight(true));
		}
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		GUILayout.EndArea();
		#endregion
	}
	#endregion
	
	void showContainmentMenu()
	{
		float margin = 15;
		
		GUIStyle titleLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		titleLabelStyle.fontSize = 30;
		
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		showTopMenu();
		
		GUILayout.BeginArea(new Rect(2*Screen.width/3, topMenuHeight, Screen.width/3, Screen.height-topMenuHeight), GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.ExpandHeight (true));
		GUILayout.BeginHorizontal();
		GUILayout.Space(2*margin);
		GUILayout.BeginVertical();
		
		GUILayout.Space(margin);
		GUILayout.Label("Aliens Captured", titleLabelStyle);
		GUILayout.Space(margin);
		
		GUILayout.Label("Space (occupied/total): " + currentBase.getAlienTotal () + "/" + currentBase.getContainmentSpace(), centeredLabelStyle);
		GUILayout.Space(margin);
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label(Resources.Load("Alien1") as Texture, centeredLabelStyle, GUILayout.Width (150), GUILayout.Height (150));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();
		GUILayout.Label("Type: Creeper", centeredLabelStyle);
		GUILayout.Label("Captured: " + currentBase.getNumAliens(0), centeredLabelStyle);
		GUILayout.Space(margin);
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label(Resources.Load("Alien2") as Texture, centeredLabelStyle, GUILayout.Width (150), GUILayout.Height (150));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();
		GUILayout.Label("Type: Tormentor", centeredLabelStyle);
		GUILayout.Label("Captured: " + currentBase.getNumAliens(1), centeredLabelStyle);
		GUILayout.Space (margin);
		
		GUILayout.BeginHorizontal ();
		GUILayout.FlexibleSpace();
		GUILayout.Label(Resources.Load("Alien3") as Texture, centeredLabelStyle, GUILayout.Width (150), GUILayout.Height (150));
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();
		GUILayout.Label("Type: Crawler", centeredLabelStyle);
		GUILayout.Label("Captured: " + currentBase.getNumAliens(2), centeredLabelStyle);
		GUILayout.Space(margin);
		
		GUILayout.EndVertical();
		GUILayout.Space(2*margin);
		GUILayout.EndHorizontal();
		GUILayout.Space(margin);
		GUILayout.EndScrollView ();
		GUILayout.EndArea();
	}
	
	void showHospitalMenu()
	{
		float margin = 15;
		float col1 = 150;
		float col2 = 50;
		float space = 10;
		
		GUIStyle titleLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		titleLabelStyle.fontSize = 30;
		
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle headingLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		headingLabelStyle.alignment = TextAnchor.MiddleLeft;
		headingLabelStyle.fontSize = 15;
		
		showTopMenu();
		
		GUILayout.BeginArea(new Rect(2*Screen.width/3, topMenuHeight, Screen.width/3, Screen.height-topMenuHeight), GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.ExpandHeight (true));
		GUILayout.BeginHorizontal();
		GUILayout.Space(2*margin);
		GUILayout.BeginVertical();
		
		GUILayout.Space(margin);
		GUILayout.Label("Hospital", titleLabelStyle);
		GUILayout.Space(margin);
		
		GUILayout.Label("Space (occupied/total): " + currentBase.getHospitalOccupied () + "/" + currentBase.getHospitalSpace(), centeredLabelStyle);
		GUILayout.Space(margin);
		
		//Headings
		GUILayout.BeginHorizontal();
		GUILayout.Label("Soldier Name", headingLabelStyle, GUILayout.Width(col1));
		GUILayout.Label("Health", headingLabelStyle, GUILayout.Width (col2));		
		GUILayout.EndHorizontal();
		GUILayout.Space(space);
		
		// Entries
		foreach (Soldier s in currentBase.getHiredSoldiers()) {
			if (s.InHospital) {
				GUILayout.BeginHorizontal();
				GUILayout.Label(s.getName(), GUILayout.Width(col1));
				GUILayout.Label("" + s.getHealth(), GUILayout.Width(col2));
				GUILayout.EndHorizontal ();
				GUILayout.Space (space);
			}
		}
		
		GUILayout.EndVertical();
		GUILayout.Space(2*margin);
		GUILayout.EndHorizontal();
		GUILayout.Space(margin);
		GUILayout.EndScrollView ();
		GUILayout.EndArea();
	}
	
	#region Transfer
	
	void showTransferMenu()
	{
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(2, windowRect, transferWindow, "", windowStyle);
	}
	
	void transferWindow(int windowID)
	{
		float margin = 15;
		int space = 10;
		float toolbarHeight = 40;
		float selectionAreaWidth = 7*Screen.width/12 - 2*margin;
		float transferAreaHeight = (Screen.height-topMenuHeight-3*margin)/2;
		float selectionItemWidth = selectionAreaWidth/2;
		float buttonSize = 15;
		float numberWidth = 30;
		float transferItemWidth = 240;
		
		GUIStyle centeredLabelStyle = new GUIStyle();
		centeredLabelStyle.normal = GUI.skin.label.normal;
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		centeredLabelStyle.margin = GUI.skin.label.margin;
		
		GUIStyle titleLabelStyle = new GUIStyle();
		titleLabelStyle.normal = GUI.skin.label.normal;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		titleLabelStyle.margin = GUI.skin.label.margin;
		titleLabelStyle.fontSize = 30;
		
		
		string[] categories = {"Equipment", "Employees"};
		
		showTopMenu();
		
		// Toolbar to select category
		GUILayout.BeginArea(new Rect(margin, topMenuHeight+margin, selectionAreaWidth, toolbarHeight));
		int oldCategory = category;
		category = GUILayout.Toolbar(category, categories, GUILayout.ExpandHeight(true));
		if (oldCategory != category)
		{
			// Switching categories
			scrollPosition = new Vector2(0,0);
			scrollPositionT = new Vector2(0,0);
		}
		GUILayout.EndArea();
		
		// Selection Area
		GUILayout.BeginArea(new Rect(margin,
									topMenuHeight+margin+toolbarHeight+margin,
									selectionAreaWidth,
									Screen.height-topMenuHeight-toolbarHeight-3*margin),
							GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView(scrollPosition, GUILayout.ExpandHeight(true));
		
		GUILayout.BeginVertical ();
		
		// Column titles
		GUILayout.BeginHorizontal ();
		if (category == 0)
			GUILayout.Label("Items", titleLabelStyle, GUILayout.Width(selectionItemWidth));
		if (category == 1)
			GUILayout.Label("Employees", titleLabelStyle, GUILayout.Width(selectionItemWidth));
		GUILayout.Label("Available/Transfer", titleLabelStyle);
		GUILayout.EndHorizontal();
		
		// Column entries
		if (category == 0)
		{
			// Item Transfer
			List<Weapon> baseWeapons = currentBase.getPurchasedItems();
			
			foreach (Weapon w in baseWeapons)
			{
				GUILayout.Space(space);
				GUILayout.BeginHorizontal();
				GUILayout.Label(w.getName(), centeredLabelStyle, GUILayout.Width (selectionItemWidth));
				GUILayout.FlexibleSpace();
				GUILayout.Label(""+(currentBase.getAmountPurchased(w) - wTransfer.getQuantity(w))
								, centeredLabelStyle, GUILayout.Width (numberWidth));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (wTransfer.getQuantity(w) + currentBase.getEquipped(w) == currentBase.getAmountPurchased(w)) {
						// Trying to transfer a weapon that a soldier is equipped with.
						transferMessage = "All remaining items of this type are currently in use by soldiers.";
					}
					else if (wTransfer.getQuantity(w) < currentBase.getAmountPurchased(w)) {
						wTransfer.addWeapon(w);
					}
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (wTransfer.getQuantity(w) > 0)
						wTransfer.removeWeapon(w);
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}
		else if (category == 1)
		{
			// Employee Transfer
			LinkedList<Soldier> soldiers = currentBase.getHiredSoldiers();
			
			// Soldiers
			foreach (Soldier s in soldiers)
			{
				GUILayout.Space(space);
				GUILayout.BeginHorizontal();
				GUILayout.Label(s.getName(), centeredLabelStyle, GUILayout.Width (selectionItemWidth));
				GUILayout.FlexibleSpace();
				if (eTransfer.containsSoldier(s)) {
					GUILayout.Label("0", centeredLabelStyle, GUILayout.Width (numberWidth));
				}
				else {
					GUILayout.Label("1", centeredLabelStyle, GUILayout.Width (numberWidth));
				}
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (!eTransfer.containsSoldier(s)) {
						if (s.getWeapon() != null || s.getArmor() != null)
						{
							transferMessage = "This soldier is equipped with ";
							if (s.getWeapon() != null)
								transferMessage += s.getWeapon().getName();
							if (s.getWeapon() != null && s.getArmor() != null)
								transferMessage += " and ";
							if (s.getArmor() != null)
								transferMessage += s.getArmor().getName();
							transferMessage += ". These will not be transferred.";
						}
						if (currentBase.getAircraft() != null && currentBase.getAircraft().contains(s))
						{
							// Soldier will be removed from aircraft when transfer is processed
							transferMessage += "\nThis soldier will be removed from the aircraft";
						}
						eTransfer.addSoldier(s);
					}
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					eTransfer.removeSoldier (s);
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			
			// Scientists
			if (currentBase.getNumbScientists() > 0)
			{
				GUILayout.Space(space);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Scientists", centeredLabelStyle, GUILayout.Width (selectionItemWidth));
				GUILayout.FlexibleSpace();
				GUILayout.Label("" + (currentBase.getNumbScientists() - eTransfer.getScientists())
								, centeredLabelStyle, GUILayout.Width (numberWidth));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getScientists() < currentBase.getNumbScientists())
						eTransfer.addScientist();
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getScientists() > 0)
						eTransfer.removeScientist ();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			// Workers
			if (currentBase.getNumbWorkers() > 0)
			{
				GUILayout.Space(space);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Workers", centeredLabelStyle, GUILayout.Width (selectionItemWidth));
				GUILayout.FlexibleSpace();
				GUILayout.Label("" + (currentBase.getNumbWorkers() - eTransfer.getWorkers())
								, centeredLabelStyle, GUILayout.Width (numberWidth));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				int busyWorkers = 0;
				foreach (ProductionOrder p in currentBase.getActiveOrders()) {
					busyWorkers += p.getWorkers();
				}
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getWorkers() < currentBase.getNumbWorkers() - busyWorkers)
						eTransfer.addWorker();
					else if (busyWorkers > 0)
						transferMessage = "All remaining workers are busy";
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getWorkers() > 0)
						eTransfer.removeWorker ();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
			
			// Pilots
			if (currentBase.getNumbPilots() > 0)
			{
				GUILayout.Space(space);
				GUILayout.BeginHorizontal();
				GUILayout.Label("Pilots", centeredLabelStyle, GUILayout.Width (selectionItemWidth));
				GUILayout.FlexibleSpace();
				GUILayout.Label("" + (currentBase.getNumbPilots() - eTransfer.getPilots())
								, centeredLabelStyle, GUILayout.Width (numberWidth));
				GUILayout.Space(space);
				GUILayout.BeginVertical();
				if (GUILayout.Button ("", increaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getPilots() < currentBase.getNumbPilots())
					{
						// If there is only one pilot left in base and aircraft has a pilot, the pilot will be removed from aircraft.
						if (currentBase.getNumbPilots() - eTransfer.getPilots() == 1 && currentBase.getAircraft() != null
							&& currentBase.getAircraft().hasPilot())
						{
							transferMessage = "Pilot will be removed from the aircraft";
						}						
						eTransfer.addPilot();
					}
				}
				if (GUILayout.Button ("", decreaseSkin.button, GUILayout.Width (buttonSize), GUILayout.Height (buttonSize)))
				{
					transferMessage = "";
					if (eTransfer.getPilots() > 0)
						eTransfer.removePilot ();
				}
				GUILayout.EndVertical();
				GUILayout.FlexibleSpace();
				GUILayout.EndHorizontal();
			}
		}
		else
		{
			GUILayout.Label("No corresponding category!!");
		}
		
		
		GUILayout.EndVertical ();
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		
		// Destination Base Area
		GUILayout.BeginArea(new Rect(margin+selectionAreaWidth+margin,
									topMenuHeight+margin,
									Screen.width-selectionAreaWidth-3*margin,
									Screen.height-topMenuHeight-transferAreaHeight-3*margin),
							GUI.skin.box);
		
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		GUILayout.Label("Destination Base", titleLabelStyle);
		GUILayout.Space(space);
		
		float baseNameHeight = 50;
		GUILayout.FlexibleSpace();
		GUILayout.BeginHorizontal(GUILayout.Height(baseNameHeight));
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Prev", GUILayout.Width(baseNameHeight), GUILayout.ExpandHeight (true)))
		{
			destBase = manager.getBaseBefore(destBase);
			transferMessage = "";
		}
		GUILayout.Label(destBase.getName(), centeredLabelStyle, GUILayout.ExpandHeight (true), GUILayout.Width(200));
		if (GUILayout.Button("Next", GUILayout.Width(baseNameHeight), GUILayout.ExpandHeight (true)))
		{
			destBase = manager.getBaseAfter(destBase);
			transferMessage = "";
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal ();
		GUILayout.FlexibleSpace();
		
		GUILayout.Label ("Storage available: " + (destBase.getTotalStorageSpace() - destBase.getUsedStorage()),
						centeredLabelStyle);
		GUILayout.Space(space);
		
		GUILayout.Label ("Living space available: " + (destBase.getLivingSpace() - destBase.getOccupiedLivingSpace()),
						centeredLabelStyle);
		GUILayout.Space(space);
		
		GUILayout.Label(transferMessage, centeredLabelStyle);
		
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		GUILayout.EndArea();
		
		// Current Transfer Area
		GUILayout.BeginArea(new Rect(margin+selectionAreaWidth+margin,
									Screen.height-margin-transferAreaHeight,
									Screen.width-selectionAreaWidth-3*margin,
									transferAreaHeight),
							GUI.skin.box);
		GUILayout.BeginHorizontal();
		GUILayout.Space (margin);
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		scrollPositionT = GUILayout.BeginScrollView(scrollPositionT, GUILayout.ExpandHeight(true));
		
		// Items
		foreach (Weapon w in wTransfer.getWeapons())
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label (w.getName(), GUILayout.Width(transferItemWidth));
			GUILayout.Label ("" + wTransfer.getQuantity(w), centeredLabelStyle, GUILayout.Width(numberWidth));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
		}
		foreach (Soldier s in eTransfer.getSoldiers())
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label (s.getName(), GUILayout.Width(transferItemWidth));
			GUILayout.Label ("1", centeredLabelStyle, GUILayout.Width(numberWidth));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
		}
		if (eTransfer.getScientists() > 0)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Scientists", GUILayout.Width(transferItemWidth));
			GUILayout.Label ("" + eTransfer.getScientists(), centeredLabelStyle, GUILayout.Width(numberWidth));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
		}
		if (eTransfer.getWorkers() > 0)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Workers", GUILayout.Width(transferItemWidth));
			GUILayout.Label ("" + eTransfer.getWorkers(), centeredLabelStyle, GUILayout.Width(numberWidth));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
		}
		if (eTransfer.getPilots() > 0)
		{
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			GUILayout.Label ("Pilots", GUILayout.Width(transferItemWidth));
			GUILayout.Label ("" + eTransfer.getPilots(), centeredLabelStyle, GUILayout.Width(numberWidth));
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
		}
		GUILayout.EndScrollView ();
		
		GUILayout.BeginHorizontal();
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Transfer", GUILayout.Width (200)))
		{
			if (wTransfer.getStorageSpace() > (destBase.getTotalStorageSpace() - destBase.getUsedStorage()))
			{
				transferMessage = "Not enough storage space in destination base";
			}
			else if (eTransfer.getLivingSpace() > (destBase.getLivingSpace() - destBase.getOccupiedLivingSpace()))
			{
				transferMessage = "Not enough living space in destination base";
			}
			else if (currentBase.Equals(destBase))
			{
				transferMessage = "Current base and destination base are the same";
			}
			else
			{
				transferMessage = "";
				currentBase.removeForTransfer(wTransfer);
				currentBase.removeSoldiersForTransfer(eTransfer);
				List<string> scientists = currentBase.removeScientistsForTransfer(eTransfer);
				List<string> workers = currentBase.removeWorkersForTransfer(eTransfer);
				List<string> pilots = currentBase.removePilotsForTransfer(eTransfer);
				destBase.addForTransfer(wTransfer);
				destBase.addSoldiersForTransfer(eTransfer);
				destBase.addScientistsForTransfer(eTransfer, scientists);
				destBase.addWorkersForTransfer(eTransfer, workers);
				destBase.addPilotsForTransfer(eTransfer, pilots);
				eTransfer = new EmployeeTransfer();
				wTransfer = new WeaponTransfer();
			}
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		GUILayout.Space(margin);
		GUILayout.EndHorizontal();
		GUILayout.EndArea ();
	}
	
	#endregion
	
	#region Aircraft Menu
	public void showAircraftMenu() {
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(2, windowRect, aircraftWindow, "", windowStyle);
	}
	
	public void aircraftWindow(int windowID)
	{
		float margin = 15;
		float space = 10;
		float topAreaHeight = (Screen.height - topMenuHeight - 3*margin)/3;
		float bottomAreaHeight = Screen.height - topMenuHeight - topAreaHeight - 3*margin;
		float toolbarHeight = 40;
		float halfScreen = (Screen.width - 3*margin)/2;
		float itemHeight = 30;
		float itemWidth = 300;
		
		GUIStyle centeredLabelStyle = new GUIStyle();
		centeredLabelStyle.normal = GUI.skin.label.normal;
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		centeredLabelStyle.margin = GUI.skin.label.margin;
		
		GUIStyle titleLabelStyle = new GUIStyle();
		titleLabelStyle.normal = GUI.skin.label.normal;
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		titleLabelStyle.margin = GUI.skin.label.margin;
		titleLabelStyle.fontSize = 30;
		
		GUIStyle toggleStyle = new GUIStyle(GUI.skin.GetStyle("toggle"));
		toggleStyle.fixedHeight = itemHeight;
		
		GUIStyle buttonStyle = new GUIStyle(GUI.skin.GetStyle("button"));
		buttonStyle.fixedHeight = itemHeight;
		
		Aircraft aircraft = currentBase.getAircraft();
		
		LinkedList<string> pilots = currentBase.getHiredPilots();
		LinkedList<Soldier> soldiersLinked = currentBase.getHiredSoldiers();
		// Copy soldiers to array because linked lists are shit
		Soldier[] soldiers = new Soldier[soldiersLinked.Count];
		soldiersLinked.CopyTo(soldiers, 0);
		
		string[] pilotNames = new string[pilots.Count];
		pilots.CopyTo(pilotNames, 0);
		string[] soldierNames = new string[soldiers.Length];
		for (int i = 0; i < soldiers.Length; i++)
		{
			soldierNames[i] = soldiers[i].getName();
		}
		
		showTopMenu();
		
		// Picture
		if (aircraft != null)
			GUILayout.BeginArea(new Rect(margin, topMenuHeight+margin, halfScreen, topAreaHeight),
								Resources.Load("Aircraft") as Texture, GUI.skin.box);
		else
			GUILayout.BeginArea(new Rect(margin, topMenuHeight+margin, halfScreen, topAreaHeight), GUI.skin.box);
		GUILayout.EndArea();
		
		// Aircraft info
		GUILayout.BeginArea(new Rect(2*margin+halfScreen, topMenuHeight+margin, halfScreen, topAreaHeight), GUI.skin.box);
		if (aircraft != null)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(margin);
			GUILayout.Label("Galacticon XI", titleLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.Label("Soldiers: " + aircraft.numSoldiers() + " of " + Aircraft.MAX_SOLDIERS, centeredLabelStyle);
			GUILayout.Space(space);
			string pilotName = (aircraft.hasPilot() ? aircraft.getPilot() : "None");
			GUILayout.Label ("Pilot: " + pilotName, centeredLabelStyle);
			GUILayout.Space(space);
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Equip Soldiers", GUILayout.Width(200))) {
				menuShown = EQUIP;
				EquipSoldiers.initialize();
			}
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space(space);
			GUILayout.Label(aircraftMessage, centeredLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical();
		}
		else
		{
			GUILayout.BeginHorizontal();
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Buy an aircraft: $" + Aircraft.PRICE, GUILayout.Width(itemWidth), GUILayout.Height (itemHeight)))
			{
				if (currentBase.facilityCount("Hangar") < 1) {
					aircraftMessage = "Build a hangar first";
				}
				else if (manager.getMoney() < Aircraft.PRICE) {
					aircraftMessage = "Not enough money!";
				}
				else {
					manager.spendMoney(Aircraft.PRICE);
					currentBase.addAircraft();
					aircraftMessage = "";
				}
			}
			GUILayout.Space(space);
			GUILayout.Label(aircraftMessage, centeredLabelStyle);
			GUILayout.FlexibleSpace();
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
		}
		GUILayout.EndArea();
		
		// Toolbar
		GUILayout.BeginArea(new Rect(margin, Screen.height-margin-bottomAreaHeight, halfScreen, toolbarHeight));
		string[] categories = {"Soldiers", "Pilots"};
		category = GUILayout.Toolbar (category, categories, GUILayout.ExpandHeight(true));
		GUILayout.EndArea();
		
		// Soldier/pilot selection
		GUILayout.BeginArea(new Rect(margin, Screen.height-bottomAreaHeight+toolbarHeight, halfScreen, bottomAreaHeight-toolbarHeight-margin), GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition);
		GUILayout.BeginVertical ();
		GUILayout.Space(margin);
		GUILayout.BeginHorizontal ();
		GUILayout.Space(margin);
		if (!currentBase.hasAircraft())
		{
			GUILayout.Label("Buy an aircraft first", centeredLabelStyle);
		}
		else if (category == 0)
		{
			if (soldiers.Length <= 0)
			{
				GUILayout.Label("Hire some soldiers first", centeredLabelStyle);
			}
			else
			{
				// Soldiers
				soldierSelected = GUILayout.SelectionGrid(soldierSelected, soldierNames, 1, buttonStyle,
														GUILayout.Height ((itemHeight) * soldierNames.Length),
														GUILayout.Width (itemWidth));
				GUILayout.Space(100);
				GUILayout.BeginVertical(GUILayout.Height((itemHeight) * soldierNames.Length));
				foreach (Soldier s in soldiers)
				{
					bool toggleNew;
					toggleNew = GUILayout.Toggle(aircraft.contains(s), "", toggleStyle, GUILayout.Height(itemHeight), GUILayout.Width (itemHeight));
					if (toggleNew != aircraft.contains(s))
					{
						if (toggleNew)
						{
							if (aircraft.numSoldiers() < Aircraft.MAX_SOLDIERS)
							{
								aircraft.addSoldier(s);
								aircraftMessage = "";
							}
							else
							{
								aircraftMessage = "No room for more soldiers";
							}
						}
						else
						{
							aircraft.removeSoldier(s);
							aircraftMessage = "";
						}
					}
				}
				GUILayout.EndVertical();
			}
		}
		else if (category == 1)
		{
			if (pilots.Count <= 0)
			{
				GUILayout.Label("Hire a pilot first", centeredLabelStyle);
			}
			else
			{
				// Pilots
				pilotSelected = GUILayout.SelectionGrid(pilotSelected, pilotNames, 1, buttonStyle,
														GUILayout.Height ((itemHeight) * pilotNames.Length),
														GUILayout.Width (itemWidth));
				GUILayout.Space(100);
				GUILayout.BeginVertical(GUILayout.Height((itemHeight) * pilotNames.Length));
				foreach (string p in pilots)
				{
					bool toggleNew = GUILayout.Toggle(aircraft.contains(p), "", toggleStyle, GUILayout.Height(itemHeight), GUILayout.Width (itemHeight));
					if (toggleNew != aircraft.contains(p))
					{
						if (toggleNew)
						{
							if (aircraft.hasPilot())
							{
								aircraftMessage = "This aircraft already has a pilot";
							}
							else
							{
								aircraft.addPilot(p);
								aircraftMessage = "";
							}
						}
						else
						{
							aircraft.removePilot();
							aircraftMessage = "";
						}
					}			
				}
				GUILayout.EndVertical();
			}
		}
		
		GUILayout.Space(margin);
		GUILayout.EndHorizontal ();
		GUILayout.Space(margin);
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
		GUILayout.EndArea ();
		
		//Soldier/pilot info
		GUILayout.BeginArea(new Rect(Screen.width-margin-halfScreen, Screen.height-margin-bottomAreaHeight,halfScreen, bottomAreaHeight), GUI.skin.box);
		
		if (soldiers.Length > 0 && currentBase.hasAircraft()) {
			GUILayout.BeginArea (new Rect(0,15,halfScreen/2 - 50,bottomAreaHeight), Resources.Load("Mission/tempSoldier2") as Texture);
			GUILayout.EndArea();
		}
		
		GUILayout.BeginArea(new Rect(halfScreen/2 - 50, 0, halfScreen/2 + 50, bottomAreaHeight));
		scrollPositionA = GUILayout.BeginScrollView(scrollPositionA, GUILayout.ExpandHeight(true));
		GUILayout.BeginVertical ();
		GUILayout.Space(margin);
		if (category == 0)
		{
			if (soldiers.Length > 0 && currentBase.hasAircraft())
			{
				Soldier s = soldiers[soldierSelected];
				Weapon wep = s.getWeapon();
				Weapon arm = s.getArmor();
				
				GUILayout.BeginHorizontal();
				if (wep != null) {
					GUILayout.BeginVertical ();
					GUILayout.Label(wep.getImage(), GUI.skin.box, GUILayout.Width ((halfScreen/2) / 2), GUILayout.Height (100));
					GUILayout.Label(wep.getName (), centeredLabelStyle);
					GUILayout.EndVertical ();
				}
				else {
					GUILayout.BeginVertical ();
					GUILayout.Label("", GUI.skin.box, GUILayout.Width ((halfScreen/2) / 2), GUILayout.Height (100));
					GUILayout.Label("No weapon", centeredLabelStyle);
					GUILayout.EndVertical ();
				}
				if (arm != null) {
					GUILayout.BeginVertical ();
					GUILayout.Label(arm.getImage(), GUI.skin.box, GUILayout.Width ((halfScreen/2) / 2), GUILayout.Height(100));
					GUILayout.Label(arm.getName (), centeredLabelStyle);
					GUILayout.EndVertical ();
				}
				else {
					GUILayout.BeginVertical ();
					GUILayout.Label("", GUI.skin.box, GUILayout.Width ((halfScreen/2) / 2), GUILayout.Height (100));
					GUILayout.Label("No armor", centeredLabelStyle);
					GUILayout.EndVertical ();
				}
				GUILayout.EndHorizontal ();
				
				GUILayout.Space(space);
				GUILayout.Label(s.getName(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Health: " + s.getHealth(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Accuracy: " + s.getAccuracy(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Mind State: " + s.getMind(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Sniping Skills: " + s.getSnipePower(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Assault Skills: " + s.getAssaultPower(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Close Combat Skills: " + s.getCloseCombatPower(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Aliens Killed: " + s.getAliens(), centeredLabelStyle);
				GUILayout.Space(space);
				GUILayout.Label("Rank: " + s.getRank(), centeredLabelStyle);
			}
		}
		else if (category == 1)
		{
			if (pilots.Count > 0 && currentBase.hasAircraft())
			{
				GUILayout.FlexibleSpace();
				GUILayout.Label("<image>", centeredLabelStyle, GUILayout.Height(100));
				GUILayout.Space(space);
				GUILayout.Label(pilotNames[pilotSelected], centeredLabelStyle);
				GUILayout.FlexibleSpace();
			}
		}
		GUILayout.Space(margin);
		GUILayout.EndVertical();
		
		//Right column
		//GUILayout.Label (Resources.Load("Mission/tempSoldier") as Texture, GUILayout.Width(halfScreen/2 - 50));
		GUILayout.EndScrollView();
		GUILayout.EndArea();
		GUILayout.EndArea();
	}
	#endregion
	
	#region Base Summary
	public void showSummaryMenu()
	{		
		float margin = 15;
		// For tables:
		float col1 = 125;
		float col2 = 100;
		float col3 = 125;
		
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle titleLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		titleLabelStyle.alignment = TextAnchor.MiddleCenter;
		titleLabelStyle.fontSize = 30;
		
		GUIStyle headingLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		headingLabelStyle.alignment = TextAnchor.MiddleLeft;
		headingLabelStyle.fontSize = 15;
		
		GUIStyle centeredHeading = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredHeading.alignment = TextAnchor.MiddleCenter;
		centeredHeading.fontSize = 15;
		
		showTopMenu();
		
		GUILayout.BeginArea(new Rect(2*Screen.width/3, topMenuHeight, Screen.width/3, Screen.height-topMenuHeight), GUI.skin.box);
		scrollPosition = GUILayout.BeginScrollView (scrollPosition, GUILayout.ExpandHeight (true));
		GUILayout.BeginHorizontal();
		GUILayout.Space(2*margin);
		GUILayout.BeginVertical();
		
		GUILayout.Space(margin);
		GUILayout.Label("Employees", titleLabelStyle);
		GUILayout.Label("Soldiers: " + currentBase.getNumbSoldiers());
		GUILayout.Label("Scientists: " + currentBase.getNumbScientists());
		GUILayout.Label("Workers: " + currentBase.getNumbWorkers());
		GUILayout.Label("Pilots: " + currentBase.getNumbPilots());
		
		GUILayout.Label("Buildings", titleLabelStyle);
		GUILayout.BeginHorizontal ();
		GUILayout.BeginVertical();
		GUILayout.Label("Name", headingLabelStyle);
		GUILayout.Label("Alien Containment: ");
		GUILayout.Label("Living Quarters: ");
		GUILayout.Label("Storage: ");
		GUILayout.Label("Hangar: ");
		GUILayout.Label("Workshop: ");
		GUILayout.Label("Laboratory: ");
		GUILayout.Label("Hospital: ");
		GUILayout.EndVertical();
		
		GUILayout.BeginVertical ();
		GUILayout.Label ("Capacity", centeredHeading);
		GUILayout.Label(currentBase.getAlienTotal() + "/" + currentBase.getContainmentSpace(), centeredLabelStyle);
		GUILayout.Label(currentBase.getOccupiedLivingSpace() + "/" + currentBase.getLivingSpace(), centeredLabelStyle);
		GUILayout.Label(currentBase.getUsedStorage() + "/" + currentBase.getTotalStorageSpace(), centeredLabelStyle);
		int craft = (currentBase.hasAircraft() ? 1 : 0);
		GUILayout.Label(craft + "/" + currentBase.facilityCount("Hangar"), centeredLabelStyle);
		GUILayout.Label(currentBase.getOccupiedWorkers() + "/" + currentBase.getTotalWorkshopSpace(), centeredLabelStyle);
		GUILayout.Label((currentBase.getTotalLabSpace()-currentBase.getAvailLabSpace()) + "/" + currentBase.getTotalLabSpace(), centeredLabelStyle);
		GUILayout.Label(currentBase.getHospitalOccupied() + "/" + currentBase.getHospitalSpace(), centeredLabelStyle);
		GUILayout.EndVertical ();
		
		GUILayout.EndHorizontal();
		
		GUILayout.Label("Production", titleLabelStyle);
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Item", headingLabelStyle, GUILayout.Width(col1));
		GUILayout.Label ("Quantity", centeredHeading, GUILayout.Width(col2));
		GUILayout.Label ("Days Remaining", centeredHeading, GUILayout.Width(col3));
		GUILayout.EndHorizontal();
		if (currentBase.getActiveOrders().Count > 0)
		{
			foreach (ProductionOrder po in currentBase.getActiveOrders ())
			{
				GUILayout.BeginHorizontal ();
				GUILayout.Label(po.getWeapon().getName(), GUILayout.Width(col1));
				GUILayout.Label("" + po.getQuantity(), centeredLabelStyle, GUILayout.Width(col2));
				GUILayout.Label("" + po.getDaysRemaining(), centeredLabelStyle, GUILayout.Width(col3));
				GUILayout.EndHorizontal ();			
			}
		}
		else
		{
			GUILayout.Label("Nothing");
		}
		
		GUILayout.Label("Research", titleLabelStyle);
		GUILayout.BeginHorizontal ();
		GUILayout.Label("Item", headingLabelStyle, GUILayout.Width(col1));
		GUILayout.Label ("Scientists", centeredHeading, GUILayout.Width(col2));
		GUILayout.Label ("Days Remaining", centeredHeading, GUILayout.Width(col3));
		GUILayout.EndHorizontal();
		bool researching = false;
		foreach (Queue<Weapon> q in gameManager.Instance.getUnresearchedWeapons())
		{
			if (q.Count <= 0)
				continue;
			
			Weapon w = q.Peek();
			if (w.researchInProgress())
			{
				researching = true;
				GUILayout.BeginHorizontal ();
				GUILayout.Label(w.getName(), GUILayout.Width(col1));
				GUILayout.Label("" + w.getAssignedScientists(), centeredLabelStyle, GUILayout.Width(col2));
				GUILayout.Label("" + (int)(w.getDaysToResearch()/w.getAssignedScientists()), centeredLabelStyle, GUILayout.Width(col3));
				GUILayout.EndHorizontal ();	
			}
		}
		if (!researching)
		{
			// Nothing in progress
			GUILayout.Label("Nothing");
		}
		
		GUILayout.EndVertical();
		GUILayout.Space(2*margin);
		GUILayout.EndHorizontal();
		GUILayout.Space(margin);
		GUILayout.EndScrollView ();
		GUILayout.EndArea();		
	}
	#endregion
	
	#region Equip Soldiers
	void showEquipMenu()
	{
		Rect windowRect = new Rect (0,0,Screen.width, Screen.height);
		GUIStyle windowStyle = new GUIStyle();
		windowStyle.normal.background = researchWindowBackground;
		windowRect = GUI.Window(98, windowRect, equipWindow, "", windowStyle);		
	}
	
	void equipWindow(int windowID) {
		showTopMenu();
		EquipSoldiers.show(topMenuHeight);
	}
	
	#endregion
	
	public bool isFacilitiesMode() {
		return menuShown == FACILITIES;
	}
	
	public Facility getSelectedFacility() {
		return selectedFacility;
	}
	
	public void cantAffordFacility() {
		facilitiesMessage = "Not enough money!";
		selectedFacility = null;
	}
	
	public void cantBuildHangar() {
		facilitiesMessage = "A base can only contain one hangar";
		selectedFacility = null;
	}
	
	
}
