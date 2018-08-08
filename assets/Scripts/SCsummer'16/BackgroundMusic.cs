using UnityEngine;
using System.Collections;

public class BackgroundMusic : MonoBehaviour {

	AudioSource source;
	public AudioClip song;
	TheInformationBridge infoBrg;

	// Use this for initialization
	void Start () {
		infoBrg = new TheInformationBridge ();
		source = GetComponent<AudioSource> ();
		source.clip = song;
		source.loop = true;
		source.playOnAwake = true;
		source.volume = 0.6f;
		source.Play ();
	}
	
	// Update is called once per frame
	void LateUpdate () 
	{
		if (!infoBrg.musicOn ()) {
			source.loop = false;
			source.mute = true;
		} else {
			if (!source.isPlaying) {
				source.clip = song;
				source.loop = true;
				source.mute = false;
				source.playOnAwake = true;
				source.volume = 0.6f;
				source.Play ();
			}
		}
	}
}
