using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlienState : MonoBehaviour {
	
	private bool dead = false;
	
	public GameObject crystal;
	public GameObject light1;
	public GameObject light2;
	public GameObject body;
	public GameObject body2;
	public GameObject body3;
	
	public GameObject weaponHandler;
	
	public GameObject alienExplosion;
	
	//Projectile prefab
	public GameObject projectile;
	
	private bool runAI = false;
	private bool aiFinished = false;
	private bool inMovement = false;
	private bool isShooting = false;
	private bool network = false;
	
	private Alien alien;
	
	private Vector3 initialPosition;
	
	private float current = 0;
	private float nextCheck = 1.5f;
	
	private float shootCurrent=0;
	private float shootDelay=1.5f;
	
	private int energy=0;
	
	private GameObject fireball=null;
	
	
	public void selectCharacter(){
		if(body.renderer.enabled){
			crystal.GetComponent<MeshRenderer>().enabled=true;
			light1.GetComponent<Light>().enabled=true;
			light2.GetComponent<Light>().enabled=true;
		}
	}
	
	public void deselectCharacter(){
		crystal.GetComponent<MeshRenderer>().enabled=false;
		light1.GetComponent<Light>().enabled=false;
		light2.GetComponent<Light>().enabled=false;
	}
	
	
	//Set alien to visible
	public void setVisible(){
		body.renderer.enabled = true;
		if(body2!=null){
			body2.renderer.enabled = true;
			body3.renderer.enabled = true;
		}
	}
	
	//Set alien to invisible
	public void setInvisible(){
		body.renderer.enabled = false;
		if(body2!=null){
			body2.renderer.enabled = false;
			body3.renderer.enabled = false;
		}
	}
	
	//Where the Alien Ai takes actions
	void Update(){
		
		
		if(!runAI) return;
		//If no more energy
		if(energy<=0){
			if(!isShooting){
				((Seeker) gameObject.GetComponent(typeof(Seeker))).setTargetPosition(transform.position+(new Vector3(0,2,0)));
				aiFinished=true;
				inMovement=false;
			}else{
				if(fireball==null)
					isShooting=false;
			}
		}else{
			if(runAI){
				selectCharacter();
				//If in movement
				if(inMovement){
					if(current>nextCheck){
						GameObject soldier = checkIfAlienSeesSoldier();
						if(soldier!=null){
							setVisible();
							//Turn alien to face closest soldier
							//transform.LookAt(soldier.transform.position,Vector3.up);
							//Check if soldier is in range
							if(Vector3.Distance(soldier.transform.position,transform.position)<=alien.getRange()&& energy >= 1){
								if(shootCurrent>shootDelay){
									((Seeker) gameObject.GetComponent(typeof(Seeker))).setTargetPosition(transform.position+(new Vector3(0,2,0)));
									//In range, SHOOT!!!!
									transform.LookAt((new Vector3(soldier.transform.position.x,24,soldier.transform.position.z))/*+soldier.transform.forward.normalized*0.5f*/,Vector3.up);
									isShooting=true;
									shoot();
									energy-=alien.getCost ();
									inMovement=false;	
								}
								shootCurrent+=Time.deltaTime;
							}else{//not in range
								((Seeker) gameObject.GetComponent(typeof(Seeker))).setTargetPosition(soldier.transform.position);
								inMovement=true;
							}					
						}
						
						current=0;					
					}
					//If alien is not still moving
					if(!((Seeker) gameObject.GetComponent(typeof(Seeker))).isStillMoving()){
						inMovement=false;
					}
					
					//Remove from energy
					float tempDistance = Vector3.Distance(transform.position,initialPosition);
					
					//Temp distance == 1
					if(tempDistance>=1){
						energy-=1;
						initialPosition=transform.position;
					}
					
					//Advance timer
					current+=Time.deltaTime;
				}else if(isShooting){//I alien is in process of shooting projectile
					if(fireball==null){
						isShooting=false;	
					}
				}else{//If alien is not moving
					GameObject soldier = checkIfAlienSeesSoldier();
					//If found something
					if(soldier!=null){
						setVisible();
						//Turn alien to face closest soldier
						transform.LookAt((new Vector3(soldier.transform.position.x,24,soldier.transform.position.z))/*+soldier.transform.forward.normalized*0.5f*/,Vector3.up);
						//Check if soldier is in range
						if(Vector3.Distance(soldier.transform.position,transform.position)<=alien.getRange()&& energy >= 1){
							if(shootCurrent>shootDelay){
								//In range, shoot!
								isShooting=true;
								shoot();
								energy-=alien.getCost ();
							}
							shootCurrent+=Time.deltaTime;
						}else{//not in range
							((Seeker) gameObject.GetComponent(typeof(Seeker))).setTargetPosition(soldier.transform.position);
							inMovement=true;
						}
					}else{
						setInvisible ();
						//Move to a different location
						((Seeker) gameObject.GetComponent(typeof(Seeker))).setTargetPosition(getRandomWaypoint());
						inMovement=true;
						current = 0;
					}
				}
			}
		}
	}
	
	//Go through the Waypoints on the map and obtain a random waypoint for the alien to move to
	public Vector3 getRandomWaypoint(){
		int random = Random.Range(0,GameObject.FindGameObjectsWithTag("Waypoint").Length-1);
		int index = 0;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Waypoint")){
			if(index==random){
				return obj.transform.position;
			}else{
				index++;	
			}
		}
		return(transform.position);
	}
	
	public void startAI(){
		aiFinished = false;
		runAI=true;
		energy = alien.getEnergy();
		initialPosition = transform.position;
		isShooting=false;
		inMovement=false;
		selectCharacter();
	}
	
	public bool isAiFinished(){
		if(aiFinished&&runAI){
			//print ("AI has finished running");
			runAI=false;
			deselectCharacter();
			return true;
		}
		return false;
	}
	
	//make alien shoot fireball forward
	public void shoot(){
		shootCurrent=0;
		//Attack animation
		//animation.Play("bitchslap");//Still not working, very normal you twat
		if(alien.getType()==0){
			fireball = Instantiate(projectile,transform.position+ transform.forward*2 + new Vector3(0,3,0), transform.rotation) as GameObject;
			//Setting velocity of projectile
			fireball.rigidbody.velocity=transform.forward.normalized*30;
			((ProjectileHandler) fireball.GetComponent(typeof(ProjectileHandler))).setRange (alien.getRange ());
		}else if(alien.getType ()==1){
			fireball = Instantiate(projectile,transform.position+ transform.forward*2 + new Vector3(0,3,0), transform.rotation) as GameObject;
			//Setting velocity of projectile
			fireball.rigidbody.velocity=transform.forward.normalized*30;
			((ProjectileHandler) fireball.GetComponent(typeof(ProjectileHandler))).setRange (alien.getRange ());
		}else if (alien.getType ()==2){
			Debug.Log("Creating fireball");
			fireball = Instantiate(projectile,transform.position+ transform.forward*5 + new Vector3(0,3,0), transform.rotation) as GameObject;
			//Setting velocity of projectile
			fireball.rigidbody.velocity=transform.forward.normalized*5;
			((ProjectileHandler) fireball.GetComponent(typeof(ProjectileHandler))).setRange (alien.getRange ());
			((Walking) gameObject.GetComponent(typeof(Walking))).activateAlienAttack();

			
			Debug.Log("Fireball should be created");
		}
	}
	
	public void shootFromWeaponHandler(){
		//shootCurrent=0;
		//Attack animation
		//animation.Play("bitchslap");//Still not working, very normal you twat
			fireball = Instantiate(projectile,weaponHandler.transform.position, weaponHandler.transform.rotation) as GameObject;
			//Setting velocity of projectile
			fireball.rigidbody.velocity=weaponHandler.transform.forward.normalized*30;
			((ProjectileHandler) fireball.GetComponent(typeof(ProjectileHandler))).setRange (50);
	}
	
	//Checks if the alien sees a soldier
	//If sees more that one soldier, return closest soldier
	//If sees no soldier, return null
	public GameObject checkIfAlienSeesSoldier(){
		GameObject soldier = null;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Soldier")){
			Ray ray = new Ray(transform.position+transform.up*2f,(obj.transform.position)-(transform.position+transform.up*2f));
			RaycastHit rayHit = new RaycastHit();
			Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				//print ("Tag was: "+rayHit.collider.tag);
				if(rayHit.collider.tag == "Soldier"){
					if(soldier==null){
						soldier = obj;
					}else{
						if(Vector3.Distance(obj.transform.position,transform.position)<Vector3.Distance(soldier.transform.position,transform.position))
							soldier=obj;
					}
					
				}
			}
		}
		
		return soldier;
	}
	
	
	//Set the Alien class object instance associated with the alien
	public void setAlien(Alien setAlien){
		alien = setAlien;
	}
	
	public void rotateWeaponHandler(Vector3 target){
		weaponHandler.transform.LookAt(target, Vector3.up);
	}
	
	public void setNetwork(){
		networkView.RPC ("setNetworkToTrue",RPCMode.AllBuffered);
	}
	
	public void networkShoot(Vector3 target){
		networkView.RPC ("makeAllShoot",RPCMode.AllBuffered,target);
	}
	
	void OnCollisionEnter(Collision collision){
		if(network){
			if(networkView.isMine){
				if(collision.collider.tag == "Bullet"){
					alien.decreaseHealth(30);
					if(alien.getHealth()<=0 && !dead){
						networkView.RPC ("die", RPCMode.AllBuffered);
					}
				}
			}
		}else{
			if(collision.collider.tag == "Fireball"){
				alien.decreaseHealth(10);
				if(alien.getHealth()<=0){
					((Walking)gameObject.GetComponent(typeof(Walking))).setDead();	
					Instantiate(alienExplosion,transform.position,transform.rotation);
				}
			}else if(collision.collider.tag == "Bullet"){
				alien.decreaseHealth(30);
				if(alien.getHealth()<=0 && !dead){
					((Walking)gameObject.GetComponent(typeof(Walking))).setDead();
					// added alien killed by one
					visual.currentShootSoldierHit = true;
					dead = true;
					print ("ALIEN STATE SET TRUE");
					Instantiate(alienExplosion,transform.position,transform.rotation);
				}
			}
		}
	}
	
	#region RPC functions
	[RPC]
	public void setNetworkToTrue(){
		network=true;	
	}
	
	[RPC]
	public void die(){
		dead = true;
		((Walking)gameObject.GetComponent(typeof(Walking))).setDead();	
		Instantiate(alienExplosion,transform.position,transform.rotation);
	}
	
	[RPC]
	public void makeAllShoot(Vector3 target){
		rotateWeaponHandler(target);
		shootFromWeaponHandler();
	}
	#endregion
}
