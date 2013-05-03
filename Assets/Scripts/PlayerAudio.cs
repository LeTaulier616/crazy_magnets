using UnityEngine;
using System.Collections;

public class PlayerAudio : MonoBehaviour {
	
	private AudioSource[] audioSources;
	private AudioClip[] stepSounds;
	private AudioClip[] steamSounds;
	private AudioClip[] mechSounds;
	
	private AudioSource stepSource;
	private AudioSource steamSource;
	private AudioSource mechSource;
	
	private AudioClip step;
	private AudioClip oldstep;
	
	private AudioClip steam;
	private AudioClip oldsteam;
	
	private AudioClip mech;
	private AudioClip oldmech;
	
	// Use this for initialization
	void Start () 
	{
		this.audioSources = this.GetComponents<AudioSource>();
		this.stepSounds = GlobalVarScript.instance.WalkSounds;
		this.steamSounds = GlobalVarScript.instance.SteamSounds;
		this.mechSounds = GlobalVarScript.instance.MechSounds;
		
		this.stepSource = audioSources[0];
		this.steamSource = audioSources[1];
		this.mechSource = audioSources[2];
		
		
		this.step = null;
		this.steam = null;
		this.mech = null;
		
		this.oldstep = null;
		this.oldsteam = null;
		this.oldmech = null;	
	}
	
	// Update is called once per frame
	void Update () 
	{

	}
	
	void LateUpdate()
	{
		oldstep = step;
		oldsteam = steam;
		oldmech = mech;
	}
	
	public void PlaySteps()
	{
		int  stepindex = Random.Range(0, stepSounds.Length -1);
		step = stepSounds[stepindex];
		
		if(oldstep != step)
		{
			stepSource.clip = step;
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
			
			stepSource.clip = step;
		}
		
		if(!stepSource.isPlaying)
		{
			stepSource.Play();
			
			float effecttime = Random.Range(0.0f, 0.15f);
			Invoke("PlayEffects", effecttime);
		}
	}
	
	public void PlayEffects()
	{
		int effectmode = Random.Range(0, 2);
		
		if(effectmode == 0)
		{

		}
		
		else if(effectmode == 1)
		{
			int  steamindex = Random.Range(0, steamSounds.Length -1);
			steam = steamSounds[steamindex];
			
			if(oldsteam != steam)
			{
				steamSource.clip = steam;
			}
			
			else
			{
				if(steamindex == (steamSounds.Length - 1))
				{
					steamindex--;
					steam = steamSounds[steamindex];
				}
				
				else
				{
					steamindex++;
					steam = steamSounds[steamindex];
				}
				
				steamSource.clip = steam;
			}
			
			if(!steamSource.isPlaying)
			{
				steamSource.Play();
			}
		}
		
		else if(effectmode == 2)
		{
			int  mechindex = Random.Range(0, mechSounds.Length -1);
			steam = mechSounds[mechindex];
			
			if(oldmech != mech)
			{
				mechSource.clip = mech;
			}
			
			else
			{
				if(mechindex == (mechSounds.Length - 1))
				{
					mechindex--;
					mech = mechSounds[mechindex];
				}
				
				else
				{
					mechindex++;
					mech = mechSounds[mechindex];
				}
				
				mechSource.clip = mech;
			}
			
			if(!mechSource.isPlaying)
			{
				mechSource.Play();
			}
		}
	}
}
