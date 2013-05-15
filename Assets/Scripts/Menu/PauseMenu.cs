using UnityEngine;
using System.Collections;

public class PauseMenu : MenuScreen {
	
	public GameObject   back_button_go;
	public GameObject   pause_button_go;
	public GameObject   resume_button_go;
	public GameObject   level_button_go;
	public GameObject   quit_button_go;
	
	void Start () 
	{
		UIEventListener.Get(back_button_go).onClick   = goback;
		UIEventListener.Get(pause_button_go).onClick  = pauseonoff;
		UIEventListener.Get(resume_button_go).onClick = resume;
		UIEventListener.Get(level_button_go).onClick  = levels;
		UIEventListener.Get(quit_button_go).onClick   = quitgame;
	}
	
	public override void activateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void pauseonoff(GameObject go)
	{
		Debug.Log("Pause");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
	}
	
	private void resume(GameObject go)
	{
		Debug.Log("Resume");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
	}
	
	private void levels(GameObject go)
	{
		Debug.Log("Levels");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.WORLDS;
	}
	
	private void quitgame(GameObject go)
	{
		Debug.Log("Quit Game");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
	}
}
