using UnityEngine;
using System.Collections;

public class SoldierState : MonoBehaviour {
	
	public GameObject soldierBody;
	public GameObject crystal;
	public GameObject selectionLight;
	public GameObject light;
	public GameObject shotgun;
	public GameObject sniper1;
	public GameObject sniper2;
	public GameObject sniper3;
	public GameObject sniper4;
	public GameObject sniper5;
	public GameObject sniper6;
	public GameObject sniper7;
	public GameObject sniper8;
	public GameObject sniper9;
	public GameObject sniper10;
	public GameObject sniper11;
	public GameObject sniper12;
	public GameObject sniper13;
	public GameObject sniper14;
	public GameObject sniper15;
	public GameObject sniper16;
	public GameObject sniper17;
	public GameObject sniper18;
	public GameObject sniper19;
	public GameObject sniper20;
	public GameObject sniper21;
	public GameObject sniper22;
	public GameObject sniper23;	
	public GameObject assaultRifle;
	public GameObject alienGun1;
	
	public GameObject weaponHandler;
	
	
	private int currentWeapon;
	
	
	private Soldier soldier;
	
	private bool network;
	
	//Make Soldier visible
	public void setVisible(){
		soldierBody.renderer.enabled=true;
		switch (currentWeapon){
		case (0):
			enableAssaultRifle();
			break;
		case(1):
			enableShotgun();
			break;
		case(2):
			enableSniper();
			break;
		case(3):
			enableAlienGun();
			break;
		}
	}
	
	//Make Soldier Invisible
	public void setInvisible(){
		soldierBody.renderer.enabled=false;
		disableAllWeapons();
	}
	
	public void selectCharacter(){
		crystal.GetComponent<MeshRenderer>().enabled=true;
		light.GetComponent<Light>().enabled=true;
		selectionLight.GetComponent<Light>().enabled=true;
	}
	
	public void deselectCharacter(){
		crystal.GetComponent<MeshRenderer>().enabled=false;
		light.GetComponent<Light>().enabled=false;
		selectionLight.GetComponent<Light>().enabled=false;
	}
	
	public void disableAllWeapons(){
		shotgun.renderer.enabled=false;
		assaultRifle.renderer.enabled=false;
		sniper1.renderer.enabled=false;
		sniper2.renderer.enabled=false;
		sniper3.renderer.enabled=false;
		sniper4.renderer.enabled=false;
		sniper5.renderer.enabled=false;
		sniper6.renderer.enabled=false;
		sniper7.renderer.enabled=false;
		sniper8.renderer.enabled=false;
		sniper9.renderer.enabled=false;
		sniper10.renderer.enabled=false;
		sniper11.renderer.enabled=false;
		sniper12.renderer.enabled=false;
		sniper13.renderer.enabled=false;
		sniper14.renderer.enabled=false;
		sniper15.renderer.enabled=false;
		sniper16.renderer.enabled=false;
		sniper17.renderer.enabled=false;
		sniper18.renderer.enabled=false;
		sniper19.renderer.enabled=false;
		sniper20.renderer.enabled=false;
		sniper21.renderer.enabled=false;
		sniper22.renderer.enabled=false;
		sniper23.renderer.enabled=false;
		if(alienGun1!=null){
			alienGun1.renderer.enabled=false;
		}
	}
	
	public void enableSniper(){
		currentWeapon=2;
		sniper1.renderer.enabled=true;
		sniper2.renderer.enabled=true;
		sniper3.renderer.enabled=true;
		sniper4.renderer.enabled=true;
		sniper5.renderer.enabled=true;
		sniper6.renderer.enabled=true;
		sniper7.renderer.enabled=true;
		sniper8.renderer.enabled=true;
		sniper9.renderer.enabled=true;
		sniper10.renderer.enabled=true;
		sniper11.renderer.enabled=true;
		sniper12.renderer.enabled=true;
		sniper13.renderer.enabled=true;
		sniper14.renderer.enabled=true;
		sniper15.renderer.enabled=true;
		sniper16.renderer.enabled=true;
		sniper17.renderer.enabled=true;
		sniper18.renderer.enabled=true;
		sniper19.renderer.enabled=true;
		sniper20.renderer.enabled=true;
		sniper21.renderer.enabled=true;
		sniper22.renderer.enabled=true;
		sniper23.renderer.enabled=true;
	}
	
	public void enableShotgun(){
		currentWeapon=1;
		shotgun.renderer.enabled=true;
	}
	
	public void enableAssaultRifle(){
		currentWeapon=0;
		assaultRifle.renderer.enabled=true;
	}
	
	public void enableAlienGun(){
		currentWeapon=3;
		alienGun1.renderer.enabled=true;
	}
	
	public void setSoldier(Soldier setSoldier){
		soldier = setSoldier;	
	}
	
	void OnCollisionEnter(Collision collision){
		
		if(network){
			if(networkView.isMine){
				//Decreasing soldier health
				Debug.Log("Decreasing soldier health");
				soldier.decreaseHealth(10);
				if(soldier.getHealth()<=0){
					networkView.RPC ("die", RPCMode.AllBuffered);
				}
			}
		}else{
			if(collision.collider.tag == "Fireball"){
				soldier.decreaseHealth(10);
				if(soldier.getHealth()<=0){
					((Walking)gameObject.GetComponent(typeof(Walking))).setDead();	
				}
			}else if(collision.collider.tag == "MegaHit"){
				soldier.decreaseHealth(100);
				if(soldier.getHealth()<=0){
					((Walking)gameObject.GetComponent(typeof(Walking))).setDead();	
				}
			}
		}
	}
	
	public void rotateWeaponHandler(Vector3 target){
		weaponHandler.transform.LookAt(target, Vector3.up);
	}
	
	public GameObject getWeaponHandler(){
		return weaponHandler;
	}
	
	public void fireWeapon(Vector3 target, int weaponType, int range){
		networkView.RPC ("fire", RPCMode.AllBuffered,target, weaponType,range);
	}
	
	public void setNetwork(){
		networkView.RPC ("setNetworkToTrue",RPCMode.AllBuffered);
	}
	
	#region RPC functions
	[RPC]
	public void fire(Vector3 target, int weaponType,int range){
		rotateWeaponHandler(target);
		switch(weaponType){
		case(0):
			((Projectile) gameObject.GetComponent(typeof(Projectile))).setRifleRange(range);
			((Projectile) gameObject.GetComponent(typeof(Projectile))).fireRifle  ();
			
			break;
		case(1):
			((Projectile) gameObject.GetComponent(typeof(Projectile))).setShotgunRange(range);
			((Projectile) gameObject.GetComponent(typeof(Projectile))).fireShotgun();
			break;
		case(2):
			((Projectile) gameObject.GetComponent(typeof(Projectile))).setSniperRange(range);
			((Projectile) gameObject.GetComponent(typeof(Projectile))).fireSniper ();
			break;
		}
	}
	
	[RPC]
	public void setNetworkToTrue(){
		network=true;	
	}
	
	[RPC]
	public void die(){
		((Walking)gameObject.GetComponent(typeof(Walking))).setDead();	
	}
	#endregion
}
