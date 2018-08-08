using UnityEngine;
using System.Collections;

public class DoorSound : MonoBehaviour {

	public AudioClip doorSound;
	AudioSource source;
	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource> ();
	}

	void AvatarNearBy()
	{
		source.PlayOneShot (doorSound, 1f);
	}
		
	// Update is called once per frame
	void Update () {
	
	}
}
