using UnityEngine;
using System.Collections;

public class BigEnemyAudio : MonoBehaviour
{
	private AudioClip[] stepSounds;
	private AudioClip step;
	private AudioClip oldstep;
	
	private AudioClip[] chaseSounds;
	private AudioClip chase;
	private AudioClip oldchase;
	
		// Use this for initialization
	void Start ()
	{
		this.stepSounds = GlobalVarScript.instance.BigSounds;
		this.step = null;
		this.oldstep = null;
		
		this.chaseSounds = GlobalVarScript.instance.BigChaseSounds;
		this.chase = null;
		this.oldchase = null;
	}
	
		// Update is called once per frame
	void Update ()
	{
	
	}
	
	void LateUpdate()
	{
		oldstep = step;
		oldchase = chase;
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
	
	public void PlayChase()
	{
		int  chaseindex = Random.Range(0, stepSounds.Length -1);
		
		chase = chaseSounds[chaseindex];
		
		if(oldchase != chase)
		{
			audio.clip = chase;
		}
		
		else
		{
			if(chaseindex == (chaseSounds.Length - 1))
			{
				chaseindex--;
				chase = chaseSounds[chaseindex];
			}
			
			else
			{
				chaseindex++;
				chase = chaseSounds[chaseindex];
			}
			
			audio.clip = chase;
		}
		
		if(!audio.isPlaying)
		{
			audio.Play();
		}
	}
}
