using UnityEngine;
using System.Collections;

public class Spaceship : MonoBehaviour {

	private float orbitSpeed = -2;
	
	// Update is called once per frame
	void Update () {
		// planet to travel along a path that rotates around the sun
    	transform.RotateAround (Vector3.zero, Vector3.up, orbitSpeed * Time.deltaTime);
	}
}
