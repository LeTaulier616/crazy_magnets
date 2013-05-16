using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour
{
	public int boltCount;
	
	void Start()
	{
		boltCount = 0;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			if(MenuGesture.menuScreen != MenuGesture.ScreenMenu.ENDLEVEL)
			{
				GameObject.Find("Menus").GetComponent<Interface>().exitScreen = true;
				GameObject.Find("Menus").GetComponent<Interface>().screenToGo = MenuGesture.ScreenMenu.ENDLEVEL;
			}
		}
	}
}