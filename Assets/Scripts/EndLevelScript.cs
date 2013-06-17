using UnityEngine;
using System.Collections;
using Fabric;

public class EndLevelScript : MonoBehaviour
{
	public int boltCount;
	
	private AudioClip winSound;
	
	private GameObject player;
	private GameObject audioManager;
	
	void Start()
	{
		boltCount = 0;
		player = GlobalVarScript.instance.player;
		audioManager = GlobalVarScript.instance.AudioManager;
		winSound = GlobalVarScript.instance.WinSound;
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			StartCoroutine(EndLevel());
		}
	}
	
	IEnumerator EndLevel()
	{
		player.GetComponent<PlayerScript>().hasWon = true;
		
		GlobalVarScript.instance.SetCameraTarget(player.GetComponent<PlayerScript>().transform, true);
		player.GetComponent<PlayerScript>().canMove = false;
		player.GetComponent<PlayerScript>().canJump = false;
		
		if (audioManager != null && audioManager.GetComponent<FabricManager>() != null)
			audioManager.GetComponent<FabricManager>().Stop();
		
		if(!audio.isPlaying && winSound != null)
		{
			audio.clip = winSound;
			audio.Play();
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