using UnityEngine;
using System.Collections;

public class PauseMenu : MenuScreen {
	
	public GameObject   pause_button_go;
	public GameObject   resume_button_go;
	public GameObject   next_button_go;
	public GameObject   quit_button_go;
	public GameObject   restart_button_go;
	
	void Start () 
	{
		UIEventListener.Get(pause_button_go).onClick   = pauseonoff;
		UIEventListener.Get(resume_button_go).onClick  = resume;
		UIEventListener.Get(next_button_go).onClick    = nextCheckPoint;
		UIEventListener.Get(quit_button_go).onClick    = quitgame;
		UIEventListener.Get(restart_button_go).onClick = restartLevel;
	}
	
	public override void activateMenu()
	{
		resume_button_go.transform.parent.gameObject.SetActive(true);
		
		foreach(UIWidget widget in GameObject.Find("Anchor").GetComponentsInChildren<UIWidget>())
        {
			if(widget.name != "Menu_Background")
			{
				widget.alpha   = 0.0f;
				widget.color   = new Color(0.0f,0.0f,0.0f,0.0f);
			}
		}
		
		exitScreen = false;
		loadLevel  = false;
	}
	
	public override void desactivateMenu()
	{
		resume_button_go.transform.parent.gameObject.SetActive(false);
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
	
	private void nextCheckPoint(GameObject go)
	{
		Debug.Log("Next CheckPoint");
	}
	
	private void restartLevel(GameObject go)
	{
		Debug.Log("Restart Level");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		loadLevel  = true;
	}
	
	private void quitgame(GameObject go)
	{
		Debug.Log("Quit Game");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
}
