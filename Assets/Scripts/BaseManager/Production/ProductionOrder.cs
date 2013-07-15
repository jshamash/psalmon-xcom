using System;

[System.Serializable]
public class ProductionOrder
{
	Weapon weapon;
	int quantity;
	int workers;
	double timeToComplete;
	
	public ProductionOrder (Weapon w)
	{
		weapon = w;
		quantity = 0;
		workers = 0;
	}
	
	public Weapon getWeapon()
	{
		return weapon;
	}
	
	public int getQuantity()
	{
		return quantity;
	}
	
	public int getWorkers()
	{
		return workers;
	}
	
	public void increaseQuantity()
	{
		quantity++;
	}
	
	public void decreaseQuantity()
	{
		if (quantity > 0)
			quantity--;
	}
	
	public void increaseWorkers()
	{
		workers++;
	}
	
	public void decreaseWorkers()
	{
		if (workers > 0)
			workers--;
	}
	
	public void placeOrder()
	{
		timeToComplete = weapon.getProductionTime() * quantity;
	}
	
	public void advanceTime(double hours)
	{
		timeToComplete -= hours * workers;
	}
	
	public bool isComplete()
	{
		return timeToComplete <= 0;
	}
	
	public double getTimeRemaining()
	{
		return timeToComplete;
	}
	
	public int getDaysRemaining()
	{
		return (int) ((timeToComplete/workers)/24);
	}
}
