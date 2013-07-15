using System.Collections;
using System.Collections.Generic;

/*
 * Implements the transport ship that transport Soldiers
 * to missions
 * */
[System.Serializable]
public class Ship {
	private List<Soldier> soldiers = new List<Soldier>();

	public void addSoldier(Soldier newSoldier) {
		soldiers.Add(newSoldier);
	}
	
	public int getNumSoldiers() {
		return soldiers.Count;
	}
	
	public List<Soldier> getSoldiers() {
		return soldiers;
	}
}
