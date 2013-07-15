using UnityEngine;
using System.Collections;

//Simple script to make the earth rotate during menu selection

public class earthRotation : MonoBehaviour {
	
	private float rotationSpeed = 10f;
	
	// Update is called once per frame
	void Update () {
		transform.Rotate (0,rotationSpeed * Time.deltaTime,0);
	}
}
