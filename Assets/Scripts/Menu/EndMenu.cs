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
		
		int screwGotten = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevelScript>().boltCount;
		screw_gotcha.GetComponent<UILabel>().text = screwGotten + "/3";
		
		int levelnumber = Datas.sharedDatas().datas.selectedLevel + Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld;
		
		Datas.sharedDatas().datas.timeLevels[levelnumber] = Time.timeSinceLevelLoad;
		string timeString = BoltTimeDisplay.FormatTime(Datas.sharedDatas().datas.timeLevels[levelnumber]);
		time_level.GetComponent<UILabel>().text   = timeString;
		
		if(Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] < screwGotten)
			Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] = screwGotten;
		if(Datas.sharedDatas().datas.timeLevels[levelnumber] < Time.timeSinceLevelLoad)
			Datas.sharedDatas().datas.timeLevels[levelnumber] = Time.timeSinceLevelLoad;
		
		int nextLevelLevel = (Datas.sharedDatas().datas.currentLevel+1)%MyDefines.kLevelsByWorld;
		int nextLevelWorld = Datas.sharedDatas().datas.currentWorld + (nextLevelLevel == 0 ? 1 : 0);
		
		if((nextLevelWorld >= MyDefines.kNbWorlds && nextLevelLevel >= MyDefines.kNbLevels) || MyDefines.kNbLevelsAvailable <= levelnumber+1)
		{
			nextLevelWorld = 0;
			nextLevelLevel = 0;
			exitScreen     = true;
			screenToGo     = MenuGesture.ScreenMenu.MAIN;
			loadLevel      = false;
			Debug.Log("Quit");
		}
		else
		{
			// Unlock Level
			if(Datas.sharedDatas().datas.lastWorld == nextLevelWorld)
				Datas.sharedDatas().datas.lastLevel = Mathf.Max(nextLevelLevel, Datas.sharedDatas().datas.lastLevel);
			else if(Datas.sharedDatas().datas.lastWorld < nextLevelWorld)
				Datas.sharedDatas().datas.lastLevel = nextLevelWorld;
			Datas.sharedDatas().datas.lastWorld     = Mathf.Max(nextLevelWorld, Datas.sharedDatas().datas.lastWorld);
			// Set Level to Launch From Continue Button in the Main Menu
			Datas.sharedDatas().datas.currentLevel  = nextLevelLevel;
			Datas.sharedDatas().datas.currentWorld  = nextLevelWorld;
			
			exitScreen = false;
			loadLevel  = false;
		}
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
		Datas.sharedDatas().datas.selectedLevel = Datas.sharedDatas().datas.currentLevel;
		Datas.sharedDatas().datas.selectedWorld = Datas.sharedDatas().datas.currentWorld;
		loadLevel = true;
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
		Datas.sharedDatas().datas.selectedLevel = Datas.sharedDatas().datas.currentLevel;
		Datas.sharedDatas().datas.selectedWorld = Datas.sharedDatas().datas.currentWorld;
	}
}
