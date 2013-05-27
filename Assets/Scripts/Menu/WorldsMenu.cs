using UnityEngine;
using System.Collections;

public class WorldsMenu : MenuScreen {
	
	public GameObject[] world_buttons_go;
	public GameObject[] world_buttons_off;
	public GameObject   back_button_go;
	public GameObject   tuto_button_go;
	
	void Start () 
	{
		for(int iii = 0; iii < world_buttons_go.Length; ++iii)
   			UIEventListener.Get(world_buttons_go[iii]).onClick  = worldClicked;
		UIEventListener.Get(back_button_go).onClick  = goback;
		UIEventListener.Get(tuto_button_go).onClick  = gotuto;
	}
	
	public override void activateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		for(int iii = 0; iii < world_buttons_go.Length; ++iii)
		{
			world_buttons_go[iii].SetActive(iii <= Datas.sharedDatas().datas.lastWorld && Datas.sharedDatas().datas.tutoDone);
			if(iii <= Datas.sharedDatas().datas.lastWorld && Datas.sharedDatas().datas.tutoDone)
				world_buttons_go[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "Monde " + (iii+1);
		}
		for(int iii = 0; iii < world_buttons_off.Length; ++iii)
		{
			world_buttons_off[iii].SetActive(iii > Datas.sharedDatas().datas.lastWorld || !Datas.sharedDatas().datas.tutoDone);
			if(iii > Datas.sharedDatas().datas.lastWorld || !Datas.sharedDatas().datas.tutoDone)
				world_buttons_off[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "Monde " + (iii+1);
		}
		
		exitScreen = false;
		loadTuto   = false;
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
	
	private void gotuto(GameObject go)
	{
		exitScreen = true;
		loadTuto   = true;
		screenToGo = MenuGesture.ScreenMenu.NONE;
		Datas.sharedDatas().datas.selectedLevel = 0;
		Datas.sharedDatas().datas.selectedWorld = 0;
	}
}
