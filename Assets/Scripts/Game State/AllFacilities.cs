using UnityEngine;
using System.Collections;

/// <summary>
/// Class to store all the facilities that are possible to build as well as constants related to these facilities.
/// </summary>
[System.Serializable]
public class AllFacilities {
	
	// Array of facilities
	public static Facility[] facilities = 
	{
		new Facility("Storage Facility", 10000, STORAGE_CAPACITY, "Store weapons and armor", "Facilities/Storage"),
		new Facility("Living Quarters", 20000, LIVING_CAPACITY, "Space for employees to live", "Facilities/Living_Quarters", "Facilities/Living_Quarters2"),
		new Facility("Laboratory", 15000, LAB_CAPACITY, "A lab for scientists to research items as they become available", "Facilities/Laboratory"),
		new Facility("Workshop", 15000, WORKSHOP_CAPACITY, "A workshop for workers to manufacture new items", "Facilities/Workshop"),
		new Facility("Hangar", 50000, 1, "Holds an aircraft for deploying soldiers to missions. One per base.", "Facilities/Hangar1", "Facilities/Hangar2"),
		new Facility("Hospital", 50000, HOSPITAL_CAPACITY, "Wounded soldiers will be healed between missions", "Facilities/Hospital"),
		new Facility("Containment", 20000, CONTAINMENT_CAPACITY, "Captured alien bodies will be studied here in order to better understand their technology", "Facilities/Containment")
	
	};
	
	public static Facility getFacilityByName(string name)
	{
		foreach (Facility f in facilities)
		{
			if (f.getName() == name)
				return f;
		}
		return null;
	}
				
	public const int LAB_CAPACITY = 10;
	public const int STORAGE_CAPACITY = 10000;
	public const int WORKSHOP_CAPACITY = 10;
	public const int LIVING_CAPACITY = 30;
	public const int HOSPITAL_CAPACITY = 10;
	public const int CONTAINMENT_CAPACITY = 50;
}
