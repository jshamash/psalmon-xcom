using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class EquipSoldiers {
	
	static readonly int WEAPONS = 0;
	static readonly int ARMOR = 1;
	static int menu = 0;
	static Vector2 scr1 = new Vector2(0,0);
	static Texture2D soldierImage = Resources.Load("Mission/tempSoldier") as Texture2D;
	static Base currentBase = gameManager.Instance.getCurrentBase();
	static Soldier[] soldiers;
	static Soldier currentSoldier;
	static int soldierIndex;
	static List<Weapon> weapons;
	
	static EquipSoldiers() {
		initialize();
	}
	
	public static void initialize() {
		LinkedList<Soldier> soldiersLinked = currentBase.getHiredSoldiers();
		// Copy soldiers to array because linked lists are shit
		soldiers = new Soldier[soldiersLinked.Count];
		soldiersLinked.CopyTo(soldiers, 0);
		
		if (soldiers.Length <= 0) {
			currentSoldier = null;
			soldierIndex = -1;
		}
		else {
			currentSoldier = soldiers[0];
			soldierIndex = 0;
		}
		
		weapons = currentBase.getPurchasedItems();
	}
	
	public static void show(float topMenuHeight) {
		
		// Update vars on base change
		if (gameManager.Instance.getCurrentBase() != currentBase) {
			currentBase = gameManager.Instance.getCurrentBase();
			initialize();			
		}
		
		GUIStyle centeredLabelStyle = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;
		
		GUIStyle centeredHeading = new GUIStyle(GUI.skin.GetStyle ("label"));
		centeredHeading.alignment = TextAnchor.MiddleCenter;
		centeredHeading.fontSize = 15;
		
		float margin = 15;
		float space = 7;
		float toolbarHeight = 40;
		float weaponAreaHeight = Screen.height - topMenuHeight - toolbarHeight - 3*margin;
		float soldierAreaHeight = Screen.height - topMenuHeight - 2*margin;
		float areaWidth = (Screen.width - 3*margin)/2;
		
		// Inside weapon area
		float weaponHeight = 100;
		float weaponWidth = 100;
		
		// Inside soldier area
		float topSectionHeight = 50;
		float bottomSectionHeight = weaponHeight + 50;
		float imageHeight = soldierAreaHeight-topSectionHeight-bottomSectionHeight-3*margin;
		
		// GAME RELATED VARIABLES
		List<Weapon> weaponsShown = new List<Weapon>();
		foreach (Weapon w in weapons) {
			int available = (currentBase.getAmountPurchased(w) - currentBase.getEquipped(w));
			if (w.isArmourType() && menu == ARMOR && available > 0)
				weaponsShown.Add(w);
			else if (!w.isArmourType() && menu == WEAPONS && available > 0)
				weaponsShown.Add(w);
		}
		
		// Toolbar - menu selection
		GUILayout.BeginArea(new Rect(margin, topMenuHeight+margin, areaWidth, toolbarHeight));
		menu = GUILayout.Toolbar(menu, new string[] {"Weapons", "Armor"}, GUILayout.ExpandHeight(true));
		GUILayout.EndArea();
		
		// Weapon selection area
		GUILayout.BeginArea(new Rect(margin, topMenuHeight+toolbarHeight+2*margin, areaWidth, weaponAreaHeight), GUI.skin.box);
		scr1 = GUILayout.BeginScrollView (scr1);
		GUILayout.BeginVertical();
		GUILayout.Space(margin);
		
		string type;
		if (menu == WEAPONS) {
			type = "weapons";
		}
		else {
			type = "armor";
		}
		if (weaponsShown == null || weaponsShown.Count <= 0) {
			GUILayout.Label("You don't have any " + type + " available", centeredLabelStyle);
		}
		else {
			GUILayout.Label("Available " + type, centeredHeading);
			GUILayout.Space(space);
		}
		foreach (Weapon w in weaponsShown) {
			GUILayout.BeginHorizontal ();
			GUILayout.Space(2*margin);
			if (GUILayout.Button (w.getImage(), GUILayout.Height (weaponHeight), GUILayout.Width(weaponWidth)))
			{
				// First check it's available
				if (currentBase.getAmountPurchased(w) - currentBase.getEquipped(w) > 0) {
					// Remove soldier's current weapon, assign weapon to soldier
					if (menu == ARMOR) {
						if (currentSoldier.getArmor() != null)
							currentBase.dequip (currentSoldier.getArmor());
						currentSoldier.setArmor(w);
					}
					else {
						if (currentSoldier.getWeapon() != null)
							currentBase.dequip (currentSoldier.getWeapon());
						currentSoldier.setWeapon(w);
					}
					currentBase.equip (w);
				}
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label("Name: " + w.getName());
			GUILayout.Label("Available: " + (currentBase.getAmountPurchased(w) - currentBase.getEquipped(w)));
			GUILayout.EndVertical();
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.Space (space);
		}
		
		GUILayout.Space(margin);
		GUILayout.EndVertical ();
		GUILayout.EndScrollView ();
		GUILayout.EndArea();
		
		//Soldier section
		GUILayout.BeginArea(new Rect(Screen.width-margin-areaWidth, topMenuHeight+margin, areaWidth, soldierAreaHeight), GUI.skin.box);
		if (currentSoldier == null)
		{
			GUILayout.BeginVertical();
			GUILayout.Space(margin);
			GUILayout.Label("Hire some soldiers first", centeredLabelStyle);
			GUILayout.EndVertical ();
		}
		else
		{
			// Top section: soldier selection
			GUILayout.BeginArea(new Rect(margin, margin, areaWidth-2*margin, topSectionHeight));
			GUILayout.BeginHorizontal ();
			
			if (GUILayout.Button("Previous", GUILayout.ExpandHeight(true), GUILayout.Width(100))) {
				soldierIndex -= 1;
				if (soldierIndex < 0)
					soldierIndex = soldiers.Length-1;
				currentSoldier = soldiers[soldierIndex];
			}
			GUILayout.FlexibleSpace();
			GUILayout.BeginVertical();
			GUILayout.Label(currentSoldier.getName());
			if (currentBase.getAircraft() != null & currentBase.getAircraft().contains(currentSoldier)) {
				GUILayout.Label("In aircraft");
			}
			else {
				GUILayout.Label("Not in aircraft");
			}
			GUILayout.EndVertical ();
			GUILayout.FlexibleSpace();
			if (GUILayout.Button("Next", GUILayout.ExpandHeight(true), GUILayout.Width(100))) {
				soldierIndex += 1;
				if (soldierIndex >= soldiers.Length)
					soldierIndex = 0;
				currentSoldier = soldiers[soldierIndex];
			}
			
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
			
			// Soldier Image
			GUILayout.BeginArea(new Rect(margin, margin+topSectionHeight, areaWidth-2*margin, imageHeight), soldierImage, centeredLabelStyle);
			GUILayout.EndArea();
			
			// Bottom section: weapon & armor
			GUILayout.BeginArea(new Rect(margin, soldierAreaHeight-margin-bottomSectionHeight, areaWidth-2*margin, bottomSectionHeight));
			GUILayout.BeginHorizontal ();
			GUILayout.FlexibleSpace();
			
			// Weapon
			GUILayout.BeginVertical();
			Weapon wep = currentSoldier.getWeapon();
			// Display "No weapon" if no weapon instead of weapon name
			string wepName;
			if (wep == null)
				wepName = "No weapon";
			else
				wepName = wep.getName();
			GUILayout.Label(wepName, centeredLabelStyle);
			// Display nothing if no weapon instead of weapon image
			GUIContent wepImage;
			if (wep == null || wep.getImage() == null)
				wepImage = new GUIContent("");
			else
				wepImage = new GUIContent(wep.getImage());
			if (GUILayout.Button(wepImage, GUILayout.Height (weaponHeight), GUILayout.Width(weaponWidth)))
			{
				// Remove weapon from soldier and dequip in base
				currentSoldier.setWeapon(null);
				currentBase.dequip(wep);
			}
			GUILayout.EndVertical();
			
			GUILayout.Space(25);
			
			GUILayout.BeginVertical ();
			Weapon armor = currentSoldier.getArmor();
			// Like with weapons
			string armName;
			if (armor == null)
				armName = "No armor";
			else
				armName = armor.getName();
			GUILayout.Label(armName, centeredLabelStyle);
			GUIContent armImage;
			if (armor == null || armor.getImage() == null)
				armImage = new GUIContent("");
			else
				armImage = new GUIContent(armor.getImage());
			if (GUILayout.Button(armImage, GUILayout.Height (weaponHeight), GUILayout.Width(weaponWidth)))
			{
				// Remove armor from soldier and make available.
				currentSoldier.setArmor(null);
				currentBase.dequip(armor);
			}
			GUILayout.EndVertical ();
			
			GUILayout.FlexibleSpace();
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		GUILayout.EndArea();
		
	}
}
