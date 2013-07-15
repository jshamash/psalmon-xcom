using UnityEngine;
using System.Collections;

public class Nuke : MonoBehaviour {

	public GameObject explosion;
	
	//On collision destroy itself and create atomic explosion
	void OnCollisionEnter(Collision collision){
		Instantiate (explosion,transform.position,transform.rotation);
		Destroy(gameObject);
	}
}
