using UnityEngine;

public class loadingScript : MonoBehaviour
{
    public Texture2D texture;
	
	
	
	private float progress = 0;
	private int percentage= 0;
	private float speed = 0;
	private Vector2 pos = new Vector2(Screen.width/4,Screen.height - 100);
	private Vector2 size = new Vector2(Screen.width/2,40);
	private Texture2D progressBarEmpty;
	private Texture2D progressBarFull;
	
	
	private static loadingScript instance;

	//Create an instance of a gameManager object if an instance does not already exist
	public static loadingScript Instance{
		get{
			if(instance==null){
				instance = new GameObject("loadingScript").AddComponent<loadingScript>();	
			}
			return instance;
		}
	}
	
	//Sets the instances to null when the application quits
	public void OnApplicationQuit(){
		instance=null;	
	}
	
	//Start the new terrain
	public void startState(){
		//gameManager.Instance.setLevel ("Terrain");
		texture = (Texture2D)Resources.Load("Mission/background");
		progressBarFull = (Texture2D)Resources.Load("Mission/loadingBarFull");
		progressBarEmpty = (Texture2D)Resources.Load("Mission/loadingBarEmpty");
		progress=Time.time;
		percentage=0;
		speed = 0.05f;
		
	}
	
	public void Start(){
		texture = (Texture2D)Resources.Load("Mission/background");
		progressBarFull = (Texture2D)Resources.Load("Mission/loadingBarFull");
		progressBarEmpty = (Texture2D)Resources.Load("Mission/loadingBarEmpty");
	}
	
	void OnGUI(){
		
		GUI.DrawTexture (new Rect(0,0,Screen.width,Screen.height),texture);
		
		if(percentage>20){
			if(percentage>40){
				if(percentage>45){
					if(percentage>90){
						speed = 0.1f;
					}else{
						speed = 0.01f;
					}
				}else{
					speed=0.03f;
				}
			}else{
				speed = 0.01f;
			}
		}
		
		if(percentage==108){
			int temp = Random.Range(1,3);
			if(temp==1){
				//gameManager.Instance.setLevel ("Terrain");
				gameManager.Instance.setLevel ("Terrain");
			}else{
				gameManager.Instance.setLevel ("Terrain");
			}
		}
		
		if(Time.time-progress >speed){
			progress=Time.time;
			percentage++;
		}
		
		GUI.DrawTexture(new Rect(pos.x, pos.y, size.x, size.y), progressBarEmpty);
		GUI.BeginGroup(new Rect (pos.x, pos.y, size.x * ((float)percentage)/100, size.y));
			GUI.DrawTexture(new Rect(0, 0, size.x, size.y), progressBarFull);
		GUI.EndGroup();
		if(percentage>99){
			GUI.Label (new Rect(Screen.width/2 -2, pos.y+size.y, size.x, size.y), "99%");
		}else
			GUI.Label (new Rect(Screen.width/2 -2, pos.y+size.y, size.x, size.y), (int)percentage + "%");
	}
}