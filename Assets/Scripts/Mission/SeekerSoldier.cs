using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SeekerSoldier : MonoBehaviour {
	
	//Target object
	public GameObject target;
	
	//Cache to know what our
	//last path was for
	Vector3 lastDestination;
	
	//Check if has enough energy
	Vector3 energyDistance;
	
	//Route to follow
	public List<Vector3> route = new List<Vector3>();
	
	//Position on the route
	public int routePos;
	
	private bool network = false;
	
	Soldier curSoldier;
	
	//Something that knows how to move
	//our character
	Movement movement;
	
	void Start(){
		movement.speed *= 2;
		//Debug.Log("Starting function just finished");
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
				//Debug.Log("NOT MINE!");
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
		
		if(route==null)
			return;
		//Are we at the end of the route?
		if(routePos < route.Count)
		{
				if(curSoldier.getEnergy()<=0){
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
						curSoldier.setEnergy(curSoldier.getEnergy()-1);
						energyDistance=transform.position;
					}
				}
		}
		
		
	}
	
	public void setTarget(GameObject newTarget){
		target = newTarget;	
	}
	
	public void setTargetPosition(Vector3 newPosition, Soldier soldier){
		curSoldier=soldier;
		target.transform.position = newPosition;	
	}
	
	public void setCurSoldier(Soldier soldier){
		curSoldier=soldier;
	}
	
	//Stop soldier from walking
	public void stopWalking(){
		target.transform.position = transform.position;
	}
	
	//Create target and set target to new instantiated object
	public void activateNetwork(){
		networkView.RPC ("setNetworkSeeker",RPCMode.All);
		network = true;
	}
	
	#region RPC functions
	[RPC]
	public void setTargetRPC(Vector3 pos){
		movement.targetPosition = pos;
	}
	
	[RPC]
	public void setNetworkSeeker(){
		Debug.Log("Setting to true");
		network=true;
		Debug.Log("Network has been set to true: ");
	}
	
	#endregion
}
