using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WeaponTransfer {

	private Dictionary<Weapon, int> weapons;
	
	public WeaponTransfer()
	{
		weapons = new Dictionary<Weapon, int>();
	}
	
	public void addWeapon(Weapon w)
	{
		if (weapons.ContainsKey(w)) weapons[w]++;
		else weapons.Add(w, 1);
	}
	
	public void removeWeapon(Weapon w)
	{
		int amount;
		if (weapons.TryGetValue (w, out amount) && amount > 0)
		{
			weapons[w]--;
			if (weapons[w] == 0)
				weapons.Remove(w);
		}
	}
	
	public int getQuantity(Weapon w)
	{
		int amount;
		if (weapons.TryGetValue(w, out amount)) return amount;
		else return 0;
	}
	
	public List<Weapon> getWeapons()
	{
		return new List<Weapon>(weapons.Keys);
	}
	
	public int getStorageSpace()
	{
		int space = 0;
		foreach (Weapon w in weapons.Keys)
			space += w.getStorageSpace();
		return space;
	}
}
