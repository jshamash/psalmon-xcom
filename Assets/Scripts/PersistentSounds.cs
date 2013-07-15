using UnityEngine;
using System.Collections;

public class PersistentSounds : MonoBehaviour {
	
	private static PersistentSounds instance = null;	
	public AudioClip sound;
	public float volume;
	private AudioSource soundSource;
	
	public static PersistentSounds Instance {
		get { return instance; }
	}
	
	public void Stop() {
		Destroy(this.gameObject);
	}
	
	void Awake() {
		if (instance != null && instance != this) {
			Destroy(this.gameObject);
			return;
		}
		else {
			instance = this;
			DontDestroyOnLoad(gameObject);
			soundSource = gameObject.AddComponent<AudioSource>();
			soundSource.loop = true;
			soundSource.volume = volume;
			soundSource.clip = sound;
			soundSource.Play();
		}
	}
	
	// Use this for initialization
	void Start () {
		
	}
}
