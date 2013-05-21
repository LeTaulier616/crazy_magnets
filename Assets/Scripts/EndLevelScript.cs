using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour
{
	public int boltCount;
	
	private GameObject player;
	void Start()
	{
		boltCount = 0;
		player = GlobalVarScript.instance.player;
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
		
		GlobalVarScript.instance.SetCameraTarget(player.GetComponent<PlayerScript>().transform);
		player.GetComponent<PlayerScript>().canMove = false;
		player.GetComponent<PlayerScript>().canJump = false;
				
		yield return new WaitForSeconds(player.GetComponent<PlayerScript>().playerMesh.animation["win"].length / 4.0f);
				
		yield return new WaitForSeconds(2.0f);
		
		if(MenuGesture.menuScreen != MenuGesture.ScreenMenu.ENDLEVEL)
		{
			GameObject.Find("Menus").GetComponent<Interface>().exitScreen = true;
			GameObject.Find("Menus").GetComponent<Interface>().screenToGo = MenuGesture.ScreenMenu.ENDLEVEL;
		}
	}
}