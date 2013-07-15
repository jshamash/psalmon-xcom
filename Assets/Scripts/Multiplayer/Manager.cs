using UnityEngine;
using System.Collections;

public class Manager : MonoBehaviour {
	
	public void changeState(){
		networkView.RPC ("accessMultiplayerState",RPCMode.AllBuffered);	
	}
	
	[RPC]
	public void accessMultiplayerState(){
		MultiplayerState.Instance.endTurn();
		Debug.Log("In manager, setting turn to false");
	}
}
