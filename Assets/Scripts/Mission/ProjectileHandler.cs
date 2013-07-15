using UnityEngine;
using System.Collections;

public class ProjectileHandler : MonoBehaviour {
	
	private Vector3 initial;
	private int range;
	private float sqInitial;
	private Soldier iSoldier;
	
	public GameObject explosionPrefab;
	
	void Start(){
		initial = transform.position;
	}
	
	void Update(){
		/*if(range!=0){
			if(Vector3.Distance(initial, transform.position)>range){
				Destroy (gameObject);	
			}
		}
		*/
		sqInitial = (transform.position - initial).sqrMagnitude;
		
		if(sqInitial > (range * range))
		{
			Destroy (gameObject);
		}
		
	}
	
	void OnCollisionEnter( Collision collision){
		Debug.Log("Has hit: "+collision.transform.tag);
		if(gameObject.tag=="Fireball"){//If projectile is an explosion, make an explosion
			GameObject explosion = (GameObject)Instantiate(explosionPrefab, gameObject.transform.position,gameObject.transform.rotation);
			Destroy (explosion,2);
		}
		if(gameObject.tag=="MegaHit"){//If projectile is an explosion, make an explosion
			Debug.Log("Mega hit colliding with something");
			if(collision.transform.tag.Equals("Soldier")){
				Debug.Log("Starting explosion");
				Vector3 temp = gameObject.transform.position;
				temp.y = 24;
				GameObject explosion = (GameObject)Instantiate(explosionPrefab, temp,gameObject.transform.rotation);
				Destroy (explosion,2);
				Debug.Log("MegaHit to be destroyed");
			}
		}
		Destroy (gameObject);	
		return; 
		
	}
	
	//Range of the 
	public void setRange(int setRange){
		range = setRange;	
	}
	
	public void setSoldier(Soldier s)
	{
		iSoldier = s;
	}
	
	public Soldier getSoldier()
	{
		return iSoldier;
	}
}
