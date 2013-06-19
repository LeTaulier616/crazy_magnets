using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour
{	
	void Start ()
	{
		//Screen.showCursor = false;
		Screen.fullScreen = true;
		
		//Application.targetFrameRate = 30;
		Application.targetFrameRate = -1;
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape) && Application.loadedLevelName == "MENU")
		{
			Application.Quit();
		}
	}
}
