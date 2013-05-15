using UnityEngine;
using System.Collections;

public class Interface : MenuScreen {
	
	public GameObject pause_button_go;
	
	void Start () 
	{
		UIEventListener.Get(pause_button_go).onClick  = pauseonoff;
	}
	
	public override void activateMenu()
	{
		pause_button_go.transform.parent.gameObject.SetActive(true);
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		pause_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void pauseonoff(GameObject go)
	{
		Debug.Log("Pause");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.PAUSE;
	}
}
