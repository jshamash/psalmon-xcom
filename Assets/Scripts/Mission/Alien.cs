using UnityEngine;
using System.Collections;

public class Alien{
	
	private int health;
	private int type;
	private int armor;
	private int range;
	private int energy;
	private int maxEnergy;
	private int damage;
	private int cost;
	
	public Alien(int setType){
		type = setType;
		if(type==0){
			health = 100;
			armor = 10;
			range = 40;
			maxEnergy = 50;
			energy=maxEnergy;
			damage = 10;
			cost = 10;
		}else if(type==1){
			health = 500;
			armor = 10;
			range = 150;
			maxEnergy = 30;
			energy=maxEnergy;
			damage = 20;
			cost = 20;
		}else{//type==2
			health = 1000;
			armor = 10;
			range = 20;
			maxEnergy = 20;
			energy=maxEnergy;
			damage = 100;
			cost = 1;
		}
	}
	
	public int getHealth(){
		return health;	
	}
	
	public void setHealth(int newHealth){
		health=newHealth;	
	}
	
	public int getRange(){
		return range;
	}
	
	public int getEnergy(){
		return energy;
	}
	
	public int getType() {
		return type;
	}
	
	//Decrease health of a the alien 
	public void decreaseHealth(int ammount){
		health-= ammount;
		if(health<0)
			health=0;
	}
	
	public int getAlienType(){
		return type;	
	}
	
	public int getCost(){
		return cost;
	}
	
	public int getDamage(){
		return damage;
	}
	
	public void resetEnergy(){
		energy = maxEnergy;	
	}
	
	public void setEnergy(int newEnergy){
		energy=newEnergy;	
	}
}
