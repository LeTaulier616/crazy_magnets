using UnityEngine;
using System.Collections;

public class EndMenu : MenuScreen 
{
	public GameObject next_button_go;
	public GameObject restart_button_go;
	public GameObject levels_button_go;
	public GameObject quit_button_go;
	public GameObject time_level;
	public GameObject screw_gotcha;
	
	void Start () 
	{
   		UIEventListener.Get(next_button_go).onClick    = nextlevel;
  	 	UIEventListener.Get(restart_button_go).onClick = restartlevel;
  	 	UIEventListener.Get(levels_button_go).onClick  = levels;
   		UIEventListener.Get(quit_button_go).onClick    = quitgame;
	}
	
	public override void activateMenu()
	{
		next_button_go.transform.parent.gameObject.SetActive(true);
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		next_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	void nextlevel(GameObject go)
	{
		Debug.Log("Next Level");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		if(Datas.sharedDatas().datas.selectedLevel < MyDefines.kLevelsByWorld-1)
			Datas.sharedDatas().datas.selectedLevel++;
		else
		{
			Datas.sharedDatas().datas.selectedLevel = 0;
			Datas.sharedDatas().datas.selectedWorld++;
		}
	}
	
	void restartlevel(GameObject go)
	{
		Debug.Log("Restart");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
	}
	
	void levels(GameObject go)
	{
		Debug.Log("Levels");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.LEVELS;
	}
	
	void quitgame(GameObject go)
	{
		Debug.Log("Quit");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
}
