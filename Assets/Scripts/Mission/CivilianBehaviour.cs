using UnityEngine;
using System.Collections;

public class CivilianBehaviour : MonoBehaviour {
	
	public GameObject container;
	public MeshCollider collider;
	
	private bool isDead = false;
	
	void Update(){
		if(!isDead)
			container.animation.Play("idleSoldier");
	}
	
	void OnCollisionEnter(Collision collision){
		Debug.Log("Something Hit Me: "+collision.transform.tag);
		if(collision.transform.tag.Equals("Fireball")||collision.transform.tag.Equals("Bullet")){
			isDead = true;
			((CapsuleCollider)gameObject.GetComponent(typeof(CapsuleCollider))).enabled=false;
			container.animation.Play("death");
		}
	}
	
	//Get state of civilian
	public bool isCivilianDead(){
		return isDead;	
	}
	
	
}
