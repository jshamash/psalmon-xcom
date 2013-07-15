using UnityEngine;
using System.Collections;

public class SpawnPoint : MonoBehaviour {
	
	private bool isUsed = false;
	
	// Use this for initialization
	void Start () {
		isUsed = false;
	}
	
	public bool isSpawnInUse(){
		return isUsed;	
	}
	
	public void setToUsed(){
		isUsed=true;
	}
}
