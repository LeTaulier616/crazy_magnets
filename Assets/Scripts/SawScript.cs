using UnityEngine;
using System.Collections;

public class SawScript : MonoBehaviour {
	
	public float Speed;
	
	public bool Inverse;
	
	private AudioClip sawSound;	
	
	// Use this for initialization
	void Start () 
	{
		
		if(audio != null && !audio.isPlaying)
		{
			sawSound = GlobalVarScript.instance.SingleSawSound;
			audio.clip = sawSound;
			audio.Play();
		}
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(!Inverse)
			transform.Rotate(0.0f, Speed, 0.0f);
		
		else
			transform.Rotate(0.0f, -Speed, 0.0f);
	}
}
