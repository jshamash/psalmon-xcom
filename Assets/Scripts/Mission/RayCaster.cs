using UnityEngine;
using System.Collections;

public class RayCaster : MonoBehaviour {
	
	//hold some of the properties of the object that is selected
	RaycastHit hit;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
		//report if there is something 1 meter ahead
	if (Physics.Raycast (transform.position, transform.forward, out hit, 10))
		{
			print ("There is a " + hit.transform.name + " in front of the object");
		}
		else print("There is nothing in front.");
		
	}
}
