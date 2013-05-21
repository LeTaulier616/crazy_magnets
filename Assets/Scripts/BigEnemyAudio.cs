using UnityEngine;
using System.Collections;

public class BigEnemyAudio : MonoBehaviour
{
	private AudioClip[] stepSounds;
	
	private AudioClip step;
	private AudioClip oldstep;
	
		// Use this for initialization
	void Start ()
	{
		this.stepSounds = GlobalVarScript.instance.BigSounds;
		this.step = null;
		this.oldstep = null;
	}
	
		// Update is called once per frame
	void Update ()
	{
	
	}
	
	void LateUpdate()
	{
		oldstep = step;
	}
	
	public void PlaySteps()
	{
		int  stepindex = Random.Range(0, stepSounds.Length -1);
		step = stepSounds[stepindex];
		
		if(oldstep != step)
		{
			audio.clip = step;
		}
		
		else
		{
			if(stepindex == (stepSounds.Length - 1))
			{
				stepindex--;
				step = stepSounds[stepindex];
			}
			
			else
			{
				stepindex++;
				step = stepSounds[stepindex];
			}
			
			audio.clip = step;
		}
		
		if(!audio.isPlaying)
		{
			audio.Play();
		}
	}
}
