using System.Collections;

[System.Serializable]
public class AlienWeapon : Weapon {
	
	public int Alien0 { get; private set; }
	public int Alien1 { get; private set; }
	public int Alien2 { get; private set; }
	
	public AlienWeapon (int alien0, int alien1, int alien2, string name, string imgPath, bool isArmour, int daysToSpawn, int daysToResearch, int cost, int storageSpace,
					int productionLimit, int daysToProduce, int damage, int range, int maxAmmo, int actionPoints, int angle,
					double protection) : base(name, imgPath, isArmour, daysToSpawn, daysToResearch, cost, storageSpace,
					productionLimit, daysToProduce, damage, range, maxAmmo, actionPoints, angle, protection)
	{
		Alien0 = alien0;
		Alien1 = alien1;
		Alien2 = alien2;
	}
	
	// Returns true when this research should spawn. Makes sure we have enough aliens of each type first.
	public override bool advanceTime(double hours)
	{
		if (gameManager.Instance.getNumAliens(0) >= Alien0 &&
			gameManager.Instance.getNumAliens(1) >= Alien1 &&
			gameManager.Instance.getNumAliens(2) >= Alien2)
		{
			return base.advanceTime (hours);
		}
		return false;
	}
}
