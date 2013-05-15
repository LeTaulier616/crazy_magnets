using UnityEngine;
using System.Collections;

public class WorldsMenu : MenuScreen {
	
	public GameObject[] world_buttons_go;
	public GameObject[] world_buttons_off;
	public GameObject   back_button_go;
	
	void Start () 
	{
		for(int iii = 0; iii < world_buttons_go.Length; ++iii)
   			UIEventListener.Get(world_buttons_go[iii]).onClick  = worldClicked;
		UIEventListener.Get(back_button_go).onClick  = goback;
	}
	
	public override void activateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		for(int iii = 0; iii < world_buttons_go.Length; ++iii)
		{
			world_buttons_go[iii].SetActive(iii <= Datas.sharedDatas().datas.lastWorld);
			if(iii <= Datas.sharedDatas().datas.lastWorld)
				world_buttons_go[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "World" + (iii+1);
		}
		for(int iii = 0; iii < world_buttons_off.Length; ++iii)
		{
			world_buttons_off[iii].SetActive(iii > Datas.sharedDatas().datas.lastWorld);
			if(iii > Datas.sharedDatas().datas.lastWorld)
				world_buttons_off[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "World" + (iii+1);
		}
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void worldClicked(GameObject go)
	{
		for(int iii = 0; iii < world_buttons_go.Length; ++iii)
		{
			if(world_buttons_go[iii] == go)
			{
				Debug.Log("Go to World : " + iii);
				exitScreen = true;
				screenToGo = MenuGesture.ScreenMenu.LEVELS;
				Datas.sharedDatas().datas.selectedWorld = iii;
			}
		}
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
}
