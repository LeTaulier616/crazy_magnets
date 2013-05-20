using UnityEngine;
using System.Collections;

public class EndMenu : MenuScreen 
{
	public GameObject next_button_go;
	public GameObject restart_button_go;
	public GameObject levels_button_go;
	public GameObject quit_button_go;
	public GameObject endgame_button_go;
	public GameObject time_level;
	public GameObject screw_gotcha;
	
	void Start () 
	{
   		UIEventListener.Get(next_button_go).onClick    = nextlevel;
  	 	UIEventListener.Get(restart_button_go).onClick = restartlevel;
  	 	UIEventListener.Get(levels_button_go).onClick  = levels;
   		UIEventListener.Get(quit_button_go).onClick    = quitgame;
		UIEventListener.Get(endgame_button_go).onClick = endGame;
	}
	
	public override void activateMenu()
	{
		next_button_go.transform.parent.gameObject.SetActive(true);
		
		int levelnumber = Datas.sharedDatas().datas.selectedLevel + Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld;
		Datas.sharedDatas().datas.timeLevels[levelnumber] = Time.timeSinceLevelLoad;
		string timeString = BoltTimeDisplay.FormatTime(Datas.sharedDatas().datas.timeLevels[levelnumber]);
		time_level.GetComponent<UILabel>().text   = timeString;
		
		int screwGotten = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevelScript>().boltCount;
		Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] = screwGotten;
		screw_gotcha.GetComponent<UILabel>().text = Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] + "/3";
		
		if(Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] < screwGotten)
			Datas.sharedDatas().datas.screwsGotchaByLevel[levelnumber] = screwGotten;
		if(Datas.sharedDatas().datas.timeLevels[levelnumber] < Time.timeSinceLevelLoad)
			Datas.sharedDatas().datas.timeLevels[levelnumber] = Time.timeSinceLevelLoad;
		
		int nextLevelLevel = (Datas.sharedDatas().datas.currentLevel+1)%MyDefines.kLevelsByWorld;
		int nextLevelWorld = Datas.sharedDatas().datas.currentWorld + (nextLevelLevel == 0 ? 1 : 0);
		
		if(Application.loadedLevelName == "CM_Level_0")
		{
			nextLevelWorld = 0;
			nextLevelLevel = 0;
			Datas.sharedDatas().datas.currentWorld  = 0;
			Datas.sharedDatas().datas.currentLevel  = 0;
			Datas.sharedDatas().datas.selectedWorld = 0;
			Datas.sharedDatas().datas.selectedLevel = 0;
			Datas.sharedDatas().datas.isNewGame     = false;
			Datas.sharedDatas().datas.tutoDone      = true;
		}
		else if((nextLevelWorld >= MyDefines.kNbWorlds && nextLevelLevel >= MyDefines.kNbLevels) || MyDefines.kNbLevelsAvailable <= levelnumber)
		{
			nextLevelWorld = 0;
			nextLevelLevel = 0;
			Datas.sharedDatas().datas.currentWorld  = 0;
			Datas.sharedDatas().datas.currentLevel  = 0;
			Datas.sharedDatas().datas.selectedWorld = 0;
			Datas.sharedDatas().datas.selectedLevel = 0;
			Datas.sharedDatas().datas.isNewGame     = true;
			
			endgame_button_go.SetActive(true);
			next_button_go.SetActive(false);
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
			
			endgame_button_go.SetActive(false);
			next_button_go.SetActive(true);
		}
			
		exitScreen = false;
		loadLevel  = false;
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
	
	void endGame(GameObject go)
	{	
		exitScreen     = true;
		screenToGo     = MenuGesture.ScreenMenu.MAIN;
		loadLevel      = false;
		Debug.Log("Quit");
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
