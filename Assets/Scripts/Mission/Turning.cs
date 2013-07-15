using UnityEngine;
using System.Collections;

public class Turning : MonoBehaviour {
	
	Vector3 lastPosition;
	
	// Use this for initialization
	void Start () {
		lastPosition = transform.position;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		
		var direction = (transform.position - lastPosition);
		if(direction == Vector3.zero)
			return;
		var targetRotation = Quaternion.LookRotation(direction, Vector3.up).eulerAngles;
		targetRotation.x = 0;
		targetRotation.z = 0;
		transform.rotation = Quaternion.Euler(Vector3.Lerp(transform.rotation.eulerAngles, targetRotation, Time.deltaTime * 4));
		lastPosition = transform.position;
	}
}
