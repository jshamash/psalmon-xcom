using UnityEngine;
using System.Collections;

public class MultiplayerState : MonoBehaviour {
	
	public const int ALIENS = 0;
	public const int SOLDIERS = 1;
	
	private static MultiplayerState instance = null;
	
	private int side = -1;
	
	//Timer related variables
	private float startTime;
	private string textTime;
	private float pauseTime;
	private float turnTime;
	
	private bool soldierTurn = true;
	
	public static MultiplayerState Instance {
		get {
			if (instance == null){
				instance = new GameObject("MultiplayerState").AddComponent<MultiplayerState>();
				instance.startTime = Time.time;
				instance.turnTime = Time.time;
				instance.soldierTurn=true;
			}
			return instance;
		}
	}
	
	public int Side {
		get { return side; }
		set { side = value; }
	}
	
	void Awake () {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if(instance!=null){
			float guiTime = Time.time - startTime;
			int minutes = (int)guiTime / 60;
			int seconds = (int)guiTime % 60;
			int fraction = (int)(guiTime * 100) % 100;			
			textTime = string.Format ("{0:00}:{1:00}:{2:00}", minutes, seconds, fraction); 
		}
	}
	
	public void endTurn(){
		Debug.Log("Setting turn to: "+ (!soldierTurn));
		soldierTurn = !soldierTurn;
		turnTime=Time.time;
	}
	
	public string getTime(){
		return textTime;
	}
	
	public bool getTurn(){
		return soldierTurn;
	}
}
