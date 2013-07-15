using UnityEngine;
using System.Collections;

public class SingleVisibility : MonoBehaviour {

	
	//For making soldier/aliens visible
	private float current = 0;
	private float nextCheck = 1.5f;
	
	void Update(){
		if(gameObject.tag.Equals("Alien")){
			makeAlienDissappear();
			current=0;	
		}
		current+=Time.deltaTime;
	}
	
	//Make alien (current gameObject) dissapear if he does not see any soldiers
	private void makeAlienDissappear(){
		bool seeSomething=false;
		foreach(GameObject obj in GameObject.FindGameObjectsWithTag("Soldier")){
			Ray ray = new Ray(transform.position+transform.up*4f,(obj.transform.position)-(transform.position+transform.up*3f));
			RaycastHit rayHit = new RaycastHit();
			Debug.DrawRay(ray.origin,ray.direction*1000,Color.cyan);
			if(Physics.Raycast(ray,out rayHit)){
				if(rayHit.collider.tag == "Soldier"){
					seeSomething=true;					
				}
			}
		}
		if(!seeSomething){
			((AlienState)gameObject.GetComponent(typeof(AlienState))).setInvisible();	
		}else{
			((AlienState)gameObject.GetComponent(typeof(AlienState))).setVisible();
		}
	}
}
