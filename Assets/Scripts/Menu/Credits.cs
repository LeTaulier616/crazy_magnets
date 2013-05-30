using UnityEngine;
using System.Collections;

public class Credits : MenuScreen {

	public GameObject   back_button_go;
	
	void Start ()
	{
		UIEventListener.Get(back_button_go).onClick    = goback;
	}
	
	public override void activateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		exitScreen = false;
		loadLevel  = false;
	}
	
	public override void desactivateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
}