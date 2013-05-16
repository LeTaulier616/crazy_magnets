using UnityEngine;
using System.Collections;

public class MainMenu : MenuScreen {
	
	public GameObject newgame_button_go;
	public GameObject continue_button_go;
	public GameObject options_button_go;
	public GameObject levels_button_go;
	public GameObject datas_button_go;

	void Start () 
	{
   		UIEventListener.Get(newgame_button_go).onClick  = newgame;
  	 	UIEventListener.Get(continue_button_go).onClick = continuegame;
   		UIEventListener.Get(options_button_go).onClick  = options;
  	 	UIEventListener.Get(levels_button_go).onClick   = levels;
  	 	UIEventListener.Get(datas_button_go).onClick    = resetDatas;
	}
	
	public override void activateMenu()
	{
		newgame_button_go.transform.parent.gameObject.SetActive(true);
		
		newgame_button_go.SetActive(Datas.sharedDatas().datas.isNewGame);
		continue_button_go.SetActive(!Datas.sharedDatas().datas.isNewGame);
		
		datas_button_go.SetActive(MyDefines.developmentMode);
		
		exitScreen = false;
		loadLevel = false;
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
		Datas.sharedDatas().datas.isNewGame = false;
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		loadLevel = true;
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
	}
}
