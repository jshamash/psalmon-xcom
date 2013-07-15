using UnityEngine;
using System.Collections;

public class Walking : MonoBehaviour {
	
	Vector3 lastPosition;
	AnimationState walk;
	private string idle;
	
	float current = 0;
	float duration  = 1.5f;
	
	private bool dead = false;
	private bool shootingMode = false;
	
	private bool alienAttack;
	
	// Use this for initialization
	void Start () {
		lastPosition = transform.position;
		if(gameObject.tag=="Soldier"){
			walk = animation["walkSoldier"];
			idle = "idleSoldier";
			//idle = "aiming";
		}else{
			walk = animation["run"];
			idle = "idle";
		}
		walk.enabled = true;
		walk.weight = 0;
		
		dead=false;
		shootingMode = false;
	}
	
	// Update is called once per frame
	void Update () {		
		if(shootingMode){
			animation.Play("aiming");
		}else if(dead){
			animation.Play("death");
			
			if(gameObject.tag=="Soldier"){
				Destroy(gameObject,4);
			}else{
				Destroy(gameObject,1);
			}
		}else{
			if(alienAttack){
				animation.Play("bitchslap");
				if(duration<current){
					alienAttack=false;
				}
				current+=Time.deltaTime;
			}else{
			  	//Call animation for characters
				var distanceMoved = Vector3.Distance(transform.position, lastPosition);
				
			 	//Postpone animation for now	
				if(distanceMoved > 0){
					walk.weight=50;
					animation.Blend(walk.name, 1);
				}
				else{//idle
					walk.weight=0;
					animation.Play (idle);
				}
				
				//walk.speed = (distanceMoved/Time.deltaTime);
				lastPosition = transform.position;
			}
		}
	}
	
	public void setDead(){
		dead=true;	
	}
	
	public void activateShootingMode(){
		shootingMode=true;
	}
	
	public void deactivateShootingMode(){
		shootingMode = false;
	}
	
	public void activateAlienAttack(){
		alienAttack=true;
		current=0;
	}
}
