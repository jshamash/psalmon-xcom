using UnityEngine;
using System.Collections;

public class Civilian : MonoBehaviour{
	
	public bool alive { get; private set; }
	
	void Start(){
		alive=true;
	}
	
	public void setDead(){
		alive=false;	
	}
}
