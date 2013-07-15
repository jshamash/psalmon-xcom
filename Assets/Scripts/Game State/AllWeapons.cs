using System;

[System.Serializable]
public class AllWeapons
{
	/* (string name, string imgPath, bool isArmour,
	 * int daysToSpawn, int daysToResearch,
	 * int cost, int storageSpace,
		int productionLimit, int daysToProduce,
		int damage, int range, int maxAmmo,
		int actionPoints, int angle,
		double protection)
	*/
	public static Weapon[] initialWeapons =
	{
		new Weapon("Armour I", "Weapons/Armour1", true,
					0, 0, 100, 20, 60, 50,
					0, 0, 0, 0, 0, 0.05),
		new Weapon("Shotgun I", "Weapons/Shotgun1", false,
					0, 0, 500, 10, 50, 50,
					50, 10, 10, 10, 40, 0),
		new Weapon("Rifle I", "Weapons/Rifle1", false,
					0, 0, 250, 10, 60, 50,
					30, 30, 100, 10, 20, 0),
		new Weapon("Sniper I", "Weapons/Sniper1", false,
					0, 0, 1000, 17, 20, 50,
					100, 100, 5, 20, 5, 0)
	};
	
	public static Weapon[] armourUpgrades =
	{
		new Weapon("Armour II", "Weapons/Armour2", true,
					10, 500, 300, 20, 30, 100, 
					0, 0, 0, 0, 0, 0.1),
		new Weapon("Armour III", "Weapons/Armour3", true,
					50, 1000, 900, 20, 20, 200,
					0, 0, 0, 0, 0, 0.15),
		new Weapon("Armour IV", "Weapons/Armour4", true,
					200, 4000, 2700, 20, 15, 800,
					0, 0, 0, 0, 0, 0.25),
		new Weapon("Armour V", "Weapons/Armour5", true,
					720, 10000, 8100, 20, 10, 2000,
					0, 0, 0, 0, 0, 0.4)
	};
	
	public static Weapon[] shotgunUpgrades =
	{
		new Weapon("Shotgun II", "Weapons/Shotgun2", false,
					30, 500, 750, 10, 20, 100,
					75, 12, 10, 15, 35, 0),
		new Weapon("Shotgun III", "Weapons/Shotgun3", false,
					60, 1000, 1000, 10, 15, 200,
					100, 14, 10, 25, 15, 0),
		new Weapon("Shotgun IV", "Weapons/Shotgun4", false,
					120, 4000, 1500, 10, 10, 800,
					125, 16, 10, 35, 15, 0),
		new Weapon("Shotgun V", "Weapons/Shotgun5", false,
					360, 10000, 2500, 10, 5, 2000,
					150, 18, 10, 40, 10, 0)
	};
	
	public static Weapon[] rifleUpgrades =
	{
		new Weapon("Rifle II", "Weapons/Rifle2", false,
					20, 500, 350, 10, 30, 100,
					55, 35, 100, 15, 18, 0),
		new Weapon("Rifle III", "Weapons/Rifle3", false,
					50, 1000, 500, 10, 20, 200,
					80, 40, 100, 25, 16, 0),
		new Weapon("Rifle IV", "Weapons/Rifle4", false,
					110, 4000, 750, 10, 15, 800,
					105, 45, 100, 35, 14, 0),
		new Weapon("Rifle V", "Weapons/Rifle5", false,
					350, 10000, 1000, 10, 10, 2000,
					130, 50, 100, 40, 12, 0)
	};
	
	public static Weapon[] sniperUpgrades = 
	{
		new Weapon("Sniper II", "Weapons/Sniper2", false,
					60, 500, 1500, 17, 14, 100,
					150, 125, 5, 25, 4, 0),
		new Weapon("Sniper III", "Weapons/Sniper3", false,
					120, 1000, 3000, 17, 8, 200,
					200, 150, 5, 35, 3, 0),
		new Weapon("Sniper IV", "Weapons/Sniper4", false,
					240, 4000, 6000, 17, 4, 800,
					250, 200, 5, 45, 2, 0),
		new Weapon("Sniper V", "Weapons/Sniper5", false,
					720, 10000, 10000, 17, 2, 2000,
					9001, 9001, 5, 20, 1, 0)
	};
	
	// TODO -- alien weapons
	public static AlienWeapon[] alienWeapons =
	{
		new AlienWeapon(5, 5, 1,
						"Gallic Gun", "Weapons/alienWeapon1", false,
						0, 500, 1500, 5, 30, 50, 30, 75, 8, 10, 0, 0),
		new AlienWeapon(10, 20, 5,
						"Quantum Liquefier", "Weapons/alienWeapon2", false,
						0, 1000, 3000, 5, 25, 200, 75, 40, 20, 10, 0, 0),
		new AlienWeapon(25, 25, 15,
						"Devil's Chalice", "Weapons/alienWeapon3", false,
						0, 4000, 6000, 5, 20, 800, 100, 80, 20, 20, 0, 0),
		new AlienWeapon(50, 50, 50,
						"Star Destroyer", "Weapons/alienWeapon4", false,
						0, 10000, 10000, 5, 15, 2000, 400, 100, 10, 30, 0, 0),
		new AlienWeapon(150, 300, 350,
						"NERF Gun (ages 3+)", "Weapons/alienWeapon5", false,
						900, 20000, 20000, 20, 10000, 4000, 3, 3, 1, 100, 0, 0)
	};
	
	
	public static Weapon getWeaponByName(string name)
	{
		foreach (Weapon w in initialWeapons)
		{
			if (w.getName() == name)
				return w;
		}
		foreach (Weapon w in armourUpgrades)
		{
			if (w.getName() == name)
				return w;
		}
		foreach (Weapon w in shotgunUpgrades)
		{
			if (w.getName() == name)
				return w;
		}
		foreach (Weapon w in rifleUpgrades)
		{
			if (w.getName() == name)
				return w;
		}
		foreach (Weapon w in sniperUpgrades)
		{
			if (w.getName() == name)
				return w;
		}
		foreach (Weapon w in alienWeapons)
		{
			if (w.getName() == name)
				return w;
		}
		
		return null;
	}
}

