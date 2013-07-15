using UnityEngine;
using System.Collections;

public class Movement : MonoBehaviour {

	public float speed = 4;
	public Vector3 targetPosition;
	public float height;
	
	private bool network=false;
	
	void start(){
		height = 3.1f;
	}
	
	void Awake()
	{
		targetPosition = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		/*if(network){
			if(!networkView.isMine){
				return;
			}
		}*/
		transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
	}
	
	/*public void setNetwork(){
		activateNetwork();
	}*/
	
	[RPC]
	public void activateNetwork(){
		network=true;
	}
}
