using UnityEngine;
using System.Collections;
using Fabric;

public class EndLevelScript : MonoBehaviour
{
	public int boltCount;
	
	private AudioClip winSound;
	
	private GameObject player;
	private GameObject audioManager;
	
	private bool soundFade;
	
	private float time;
	private float startVolumeBGM;
	
	private bool endMusicPlayed;
	
	private ParticleSystem[] endParticles;
	
	void Start()
	{
		boltCount = 0;
		player = GlobalVarScript.instance.player;
		audioManager = GlobalVarScript.instance.AudioManager;
		winSound = GlobalVarScript.instance.WinSound;
		soundFade = false;
		endMusicPlayed = false;
		
		startVolumeBGM = audioManager.GetComponent<FabricManager>().GetComponentInChildren<GroupComponent>().Volume;
		
		this.endParticles = this.GetComponentsInChildren<ParticleSystem>();
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			StartCoroutine(EndLevel());
		}
	}
	
	void Update()
	{	
		if(soundFade)
		{
			if (audioManager != null && audioManager.GetComponent<FabricManager>() != null)
			{
				audioManager.GetComponent<FabricManager>().GetComponentInChildren<GroupComponent>().Volume -= Time.deltaTime;
						
				if(audioManager.GetComponent<FabricManager>().GetComponentInChildren<GroupComponent>().Volume <= 0.0f)
					audioManager.GetComponent<FabricManager>().Stop();
			}
				
			if(winSound != null && audio != null)
			{
				if(audio.clip != winSound)
					audio.clip = winSound;
				
				if(audio.volume < 1.0f)
					audio.volume += Time.deltaTime;
				
				if(!audio.isPlaying && !endMusicPlayed)
				{
					audio.Play();
					endMusicPlayed = true;
				}
			}
		}
		
		time += Time.deltaTime;
	}
	
	IEnumerator EndLevel()
	{
		player.GetComponent<PlayerScript>().hasWon = true;
		
		GlobalVarScript.instance.SetCameraTarget(player.GetComponent<PlayerScript>().transform, true);
		player.GetComponent<PlayerScript>().canMove = false;
		player.GetComponent<PlayerScript>().canJump = false;
		
		soundFade = true;
		
		foreach(ParticleSystem particle in endParticles)
		{
			particle.Play();
		}
				
		yield return new WaitForSeconds(player.GetComponent<PlayerScript>().playerMesh.animation["win"].length / 4.0f);
				
		yield return new WaitForSeconds(2.0f);
		
		if(MenuGesture.menuScreen != MenuGesture.ScreenMenu.ENDLEVEL)
		{
			GameObject.Find("Menus").GetComponent<Interface>().exitScreen = true;
			GameObject.Find("Menus").GetComponent<Interface>().screenToGo = MenuGesture.ScreenMenu.ENDLEVEL;
		}
		
		if (Application.loadedLevelName == "CM_Level_0")
		{
			GameObject.Find("CAMERA").BroadcastMessage("endTuto", SendMessageOptions.DontRequireReceiver);
		}
	}
}