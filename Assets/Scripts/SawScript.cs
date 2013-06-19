using UnityEngine;
using System.Collections;

public class SawScript : MonoBehaviour {
	
	private AudioClip sawSound;
	
	// Use this for initialization
	void Start () 
	{
		sawSound = GlobalVarScript.instance.SingleSawSound;
		
		if(audio != null && !audio.isPlaying)
		{
			audio.clip = sawSound;
			audio.Play();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(Vector3.forward, 15.0f);
	}
}
