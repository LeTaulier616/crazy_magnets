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
		
		time_level.GetComponent<UILabel>().text   = "" + Time.timeSinceLevelLoad;
		
		int screwGotten = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevelScript>().boltCount;
		screw_gotcha.GetComponent<UILabel>().text = screwGotten + "/3";
		
		int levelnumber = Datas.sharedDatas().datas.selectedLevel + Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld;
		if(Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] < screwGotten)
			Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] = screwGotten;
		if(Datas.sharedDatas().datas.timeLevels < Time.timeSinceLevelLoad)
			Datas.sharedDatas().datas.timeLevels = Time.timeSinceLevelLoad;
		
		exitScreen = false;
<<<<<<< HEAD
		loadLevel  = false;
=======
		loadLevel = false;
>>>>>>> ef8088e3be74715c827bdf78ee76c720a2d5f30b
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
		loadLevel = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		loadLevel = true;
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
