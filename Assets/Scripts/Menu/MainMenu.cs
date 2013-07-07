using UnityEngine;
using System.Collections;

public class MainMenu : MenuScreen {
	
	public GameObject newgame_button_go;
	public GameObject continue_button_go;
	public GameObject options_button_go;
	public GameObject levels_button_go;
	public GameObject datas_button_go;
	public GameObject demo_button_go;
	public GameObject credits_button_go;

	void Start () 
	{
   		UIEventListener.Get(newgame_button_go).onClick  = newgame;
  	 	UIEventListener.Get(continue_button_go).onClick = continuegame;
   		UIEventListener.Get(options_button_go).onClick  = options;
  	 	UIEventListener.Get(levels_button_go).onClick   = levels;
  	 	UIEventListener.Get(datas_button_go).onClick    = resetDatas;
  	 	UIEventListener.Get(demo_button_go).onClick     = unlockDemo;
  	 	UIEventListener.Get(credits_button_go).onClick  = credits;
		
		if(!Debug.isDebugBuild)
		{
			demo_button_go.SetActive(false);
			datas_button_go.SetActive(false);
		}
	}
	
	public override void activateMenu()
	{
		newgame_button_go.transform.parent.gameObject.SetActive(true);
		
		newgame_button_go.SetActive(Datas.sharedDatas().datas.isNewGame);
		continue_button_go.SetActive(!Datas.sharedDatas().datas.isNewGame);
		
		credits_button_go.SetActive(true);
		
		if(Debug.isDebugBuild)
		{
			datas_button_go.SetActive(MyDefines.developmentMode);
		}
		
		exitScreen = false;
		loadLevel  = false;
		loadTuto   = false;
	}
	
	public override void desactivateMenu()
	{
		newgame_button_go.transform.parent.gameObject.SetActive(false);
	}

	void newgame(GameObject go)
	{
		Debug.Log("New Game");
		Datas.sharedDatas().datas.selectedLevel = 0;
		Datas.sharedDatas().datas.selectedWorld = 0;
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		if(Datas.sharedDatas().datas.isNewGame)
		{
			Debug.Log("Launch Tuto");
			loadTuto  = true;
		}
		else
		{
			Debug.Log("Launch Level");
			loadLevel = true;
		}
		Datas.sharedDatas().datas.isNewGame = false;
	}

	void continuegame(GameObject go)
	{
		Debug.Log("Continue");
		Datas.sharedDatas().datas.selectedLevel = Datas.sharedDatas().datas.currentLevel;
		Datas.sharedDatas().datas.selectedWorld = Datas.sharedDatas().datas.currentWorld;
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		loadLevel = true;
	}

	void options(GameObject go)
	{
		Debug.Log("Options");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.OPTIONS;
	}

	void levels(GameObject go)
	{
		Debug.Log("Levels Selection");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.WORLDS;
	}
	
	void resetDatas(GameObject go)
	{
		Debug.Log("Reset Datas");
		Datas.sharedDatas().reinitDatas();
		Datas.sharedDatas().saveDatas();
	}
	
	private void gotuto(GameObject go)
	{
		exitScreen = true;
		loadTuto   = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		Datas.sharedDatas().datas.selectedLevel = 0;
		Datas.sharedDatas().datas.selectedWorld = 0;
	}
	
	private void unlockDemo(GameObject go)
	{
		Datas.sharedDatas().datas.tutoDone  = true;
		Datas.sharedDatas().datas.lastLevel = 4;
	}
	
	private void credits(GameObject go)
	{
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.CREDITS;
	}
}
