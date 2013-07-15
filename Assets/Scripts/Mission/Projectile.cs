using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {
	
	public GameObject projectile;
	public Transform weaponHandler;
	private float spread = 60.0f;
	float currentTime;
	
	private float nextFire = 0.1f;
	private float current = 0.0f;
	
	private int numberOfBulletsRifle=0;
	private int numberOfBulletsShotgun=0;
	private float spreadRange = 0;
	
	private int strayRange = 10;
	
	public AudioClip rifleSource;
	public AudioClip shotgunSource;
	public AudioClip sniperSource;
	
	private int rifleRange;
	private int shotgunRange;
	private int sniperRange;
	private Soldier iSoldier;

	void Start(){
		numberOfBulletsRifle=0;
		numberOfBulletsShotgun=0;
		spreadRange = 0;
	}
	
	// Update is called once per frame
	void Update () {

		if(numberOfBulletsRifle != 0){
			if(current >= nextFire){
				current=0;
				GameObject bulletClone = Instantiate (projectile, weaponHandler.position+ weaponHandler.forward*2, weaponHandler.rotation) as GameObject;
				((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setRange(rifleRange);
				((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setSoldier(iSoldier);
				/*Debug.Log("Firing rifle");
				Debug.Log(weaponHandler.forward.normalized*10*Time.deltaTime);*/
				bulletClone.rigidbody.velocity=weaponHandler.forward.normalized*50;
				//bulletClone.rigidbody.AddForce(weaponHandler.forward.normalized*50);
				audio.PlayOneShot(rifleSource);
				numberOfBulletsRifle--;
				Debug.Log("Fire bullet");
			}
			current+=Time.deltaTime;			
		}
		if(numberOfBulletsShotgun!=0){
			/*spreadRange = spread / 2.0f;
			float variance = Random.Range(-spreadRange, spreadRange);
			Quaternion rotation = Quaternion.AngleAxis(variance, transform.up);
			GameObject bulletClone = Instantiate (projectile, weaponHandler.position+ weaponHandler.forward*2, rotation * weaponHandler.rotation) as GameObject;
			bulletClone.rigidbody.velocity=weaponHandler.forward.normalized*50;
			numberOfBulletsShotgun--;
			*/
			int randomNumberX = Random.Range(-strayRange, strayRange);
			int randomNumberY = Random.Range(-strayRange, strayRange);
			int randomNumberZ = Random.Range(-strayRange, strayRange);
			GameObject bulletClone = Instantiate (projectile, weaponHandler.position+ weaponHandler.forward*2, weaponHandler.rotation) as GameObject;
			((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setRange(shotgunRange);
			((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setSoldier(iSoldier);
    		weaponHandler.transform.Rotate(randomNumberX, randomNumberY, randomNumberZ);
       		//bulletClone.rigidbody.velocity=weaponHandler.forward.normalized*50;	
			bulletClone.rigidbody.velocity=weaponHandler.forward.normalized*50;
			numberOfBulletsShotgun--;
			
		}
	}
	
	//Fire sniper bullet
	public void fireSniper(){
			GameObject bulletClone = Instantiate (projectile, weaponHandler.position+ weaponHandler.forward*2, weaponHandler.rotation) as GameObject;;
			//send object Forward
		((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setRange(sniperRange);
		((ProjectileHandler) bulletClone.GetComponent(typeof (ProjectileHandler))).setSoldier(iSoldier);
		bulletClone.rigidbody.velocity=weaponHandler.forward.normalized*100;
		audio.PlayOneShot(sniperSource);
		
	}
	
	//Fire shotgun bullet
	public void fireShotgun(){
		numberOfBulletsShotgun=10;
		audio.PlayOneShot(shotgunSource);
	}
	
	//Fire rifle bullets
	public void fireRifle(){
		current = 0;
		numberOfBulletsRifle = 3;
	}
	
	public void setRifleRange(int newRange){
		rifleRange = newRange;
	}
	
	public void setSniperRange(int newRange){
		sniperRange = newRange;
	}
	
	public void setShotgunRange(int newRange){
		shotgunRange = newRange;
	}
	
	public void setSoldier(Soldier s)
	{
		iSoldier = s;
	}
	
	public Soldier getSoldier()
	{
		return iSoldier;
	}
}