using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Seeker : MonoBehaviour {
	
	//Target object
	public GameObject target;
	public GameObject seekingSpherePrefab;
	
	//Cache to know what our
	//last path was for
	Vector3 lastDestination;
	
	//Route to follow
	public List<Vector3> route = new List<Vector3>();
	
	//Check if has enough energy
	Vector3 energyDistance;
	
	//Position on the route
	public int routePos;
	
	private bool stillMoving = false;
	
	private bool network = false;
	
	//Something that knows how to move
	//our character
	Movement movement;
	
	Alien curAlien;
	
	void Start(){
		movement.speed = movement.speed*2;
	}
	
	void Awake()
	{
		//Something that can be told where to go
		movement = GetComponent<Movement>();
	}
		
	// Update is called once per frame
	void Update () {
		//Check for a target, if none, function doesn't do anything
		if(target == null) return;
		
		if(network){
			if(!networkView.isMine){
				///Debug.Log("NOT MINE!");
				return;
			}else{
				//Debug.Log("MINE!");
			}
		}else{
			//Debug.Log("Network not been set to true");
		}
		
		//Get the position from the target
		var targetPosition = target.transform.position;
		//Has it moved
		if(targetPosition != lastDestination)
		{
			//Update the cache
			lastDestination = targetPosition;
			
			route = new List<Vector3>();
			
			//Ask the grid to build a path
			route = BuildGraph.instance.SeekPath(transform.position, targetPosition);
			
			routePos=0;
			
			energyDistance=transform.position;
		}
		
		//Draws a blue line in scene mode to see where character is moving
		/*if(route.Count > 0)
		{
			var last = route[0];
			foreach(var step in route)
			{
				Debug.DrawLine(last, step, Color.blue);
				last = step;
			}
		}*/
		
		//Are we at the end of the route?
		if(routePos < route.Count)
		{
			//Set still moving to true
			stillMoving = true;
				
			if(network){
			
				if(curAlien.getEnergy()<=0){
						target.transform.position = transform.position;
				}else{
					//Set the target to the current route position
						if(network){
							networkView.RPC ("setTargetRPC", RPCMode.AllBuffered,route[routePos]);
						}else{
							movement.targetPosition = route[routePos];
						}
						//Check for the next route position
						var distance = Vector3.Distance(route[routePos], transform.position);	
						if(distance < movement.speed/2)
							routePos++;
						if(Vector3.Distance (energyDistance,transform.position)>1){
							curAlien.setEnergy(curAlien.getEnergy()-1);
							energyDistance=transform.position;
						}
				}
			}else{
				//Set the target to the current route position
				if(network){
					networkView.RPC ("setTargetRPC", RPCMode.AllBuffered,route[routePos]);
				}else{
					movement.targetPosition = route[routePos];
				}
				//Check for the next route position
				var distance = Vector3.Distance(route[routePos], transform.position);	
				if(distance < movement.speed/2)
					routePos++;
			}
		}else{
			stillMoving = false;
		}
	}
	
	public void setTarget(GameObject newTarget){
		target = newTarget;	
	}
	
	public void setTargetPosition(Vector3 newPosition){
		target.transform.position = newPosition;	
		stillMoving = true;
	}
	
	//Returns variable that tells the user if the alien is still moving
	public bool isStillMoving(){
		return stillMoving;	
	}
	
	public Vector3 getTargetPosition(){
		return target.transform.position;	
	}
	
	public void setCurAlien(Alien newAlien){
		curAlien = newAlien;	
	}
	
	//Create target and set target to new instantiated object
	public void activateNetwork(){
		networkView.RPC ("setNetwork",RPCMode.All);
		network = true;
	}
	
	#region RPC functions
	[RPC]
	public void setTargetRPC(Vector3 pos){
		movement.targetPosition = pos;
	}
	
	[RPC]
	public void setNetwork(){
		//Debug.Log("Setting to true");
		network=true;
		//Debug.Log("Network has been set to true: ");
	}
	
	#endregion
}
