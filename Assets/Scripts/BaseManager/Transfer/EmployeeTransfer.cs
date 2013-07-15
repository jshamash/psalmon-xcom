using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

public class EmployeeTransfer {
	
	private int scientists;
	private int workers;
	private int pilots;
	private List<Soldier> soldiers;
	
	public EmployeeTransfer() {
		scientists = 0;
		workers = 0;
		pilots = 0;
		soldiers = new List<Soldier>();
	}
	
	public void addSoldier(Soldier s)
	{
		soldiers.Add(s);
	}
	
	public void removeSoldier(Soldier s)
	{
		soldiers.Remove(s);
	}
	
	public bool containsSoldier(Soldier s)
	{
		foreach (Soldier s2 in soldiers)
		{
			if (s2.getId() == s.getId())
				return true;
		}
		return false;
	}
	
	public int soldierAmount(Soldier s)
	{
		if (soldiers.Contains(s)) return 1;
		else return 0;
	}
	
	public ReadOnlyCollection<Soldier> getSoldiers()
	{
		return soldiers.AsReadOnly();
	}
	
	public void addScientist()
	{
		scientists++;
	}
	
	public void removeScientist()
	{
		scientists--;
	}
	
	public int getScientists()
	{
		return scientists;
	}
	
	public void addWorker()
	{
		workers++;
	}
	
	public void removeWorker()
	{
		workers--;
	}
	
	public int getWorkers()
	{
		return workers;
	}
	
	public void addPilot()
	{
		pilots++;
	}
	
	public void removePilot()
	{
		pilots--;
	}
	
	public int getPilots()
	{
		return pilots;
	}
	
	public int getLivingSpace()
	{
		return scientists + workers + pilots + soldiers.Count;
	}
}
