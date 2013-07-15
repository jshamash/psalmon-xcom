using UnityEngine;
using System.Collections;

public class Visibility : MonoBehaviour {
	
	//For making soldier/aliens visible
	private float current = 0;
	private float nextCheck = 1.5f;
	
	// Update is called once per frame
	void Update () {
		if(current>nextCheck){//Wait a few delay for checking if we can see aliens or soldiers
			if(gameObject.tag.Equals("Soldier")){
				if(MultiplayerState.Instance.Side==1){
					makeAliensAppear();
				}else{
					makeSoldierDissappear();
				}
			}else{
				if(MultiplayerState.Instance.Side==0){
					makeSoldiersAppear();
				}else{
					makeAlienDissappear();
				}
			}
			
			current=0;	
		}
		current+=Time.deltaTime;
	}
	
	//Make all soldiers appear (the one seen by this alien gameObject)
	private void makeSoldiersAppear(){
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Soldier")){
			Ray ray = new Ray(transform.position+transform.up*4f,(obj.transform.position)-(transform.position+transform.up*3f));
			RaycastHit rayHit = new RaycastHit();
			//Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				if(rayHit.collider.tag == "Soldier"){
					((SoldierState)rayHit.collider.GetComponent(typeof(SoldierState))).setVisible();
				}
			}
		}
	}
	
	//Make all aliens appear (the one seen by this soldier gameObject)
	private void makeAliensAppear(){
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Alien")){
			Ray ray = new Ray(transform.position+transform.up*4f,(obj.transform.position)-(transform.position+transform.up*3f));
			RaycastHit rayHit = new RaycastHit();
			//Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				if(rayHit.collider.tag == "Alien"){
					((AlienState)rayHit.collider.GetComponent(typeof(AlienState))).setVisible();					
				}
			}
		}
	}
	
	//Make alien (current gameObject) dissapear if he does not see any soldiers
	private void makeAlienDissappear(){
		bool seeSomething=false;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Soldier")){
			Ray ray = new Ray(transform.position+transform.up*4f,(obj.transform.position)-(transform.position+transform.up*3f));
			RaycastHit rayHit = new RaycastHit();
			//Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				if(rayHit.collider.tag == "Soldier"){
					seeSomething=true;					
				}
			}
		}
		if(!seeSomething)
			((AlienState)gameObject.GetComponent(typeof(AlienState))).setInvisible();	
	}
	
	//Make soldier(current gameObject) dissapear if he does not see any aliens
	private void makeSoldierDissappear(){
		bool seeSomething=false;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Alien")){
			Ray ray = new Ray(transform.position+transform.up*4f,(obj.transform.position)-(transform.position+transform.up*3f));
			RaycastHit rayHit = new RaycastHit();
			Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				if(rayHit.collider.tag == "Alien"){
					seeSomething=true;					
				}
			}
		}
		if(!seeSomething)
			((SoldierState)gameObject.GetComponent(typeof(SoldierState))).setInvisible();	
	}
}
