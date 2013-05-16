using UnityEngine;
using System.Collections;

public class GlobalScript : MonoBehaviour
{	
	void Start ()
	{
		//Screen.showCursor = false;
		Screen.fullScreen = true;
	}
	
	void Update ()
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
}
