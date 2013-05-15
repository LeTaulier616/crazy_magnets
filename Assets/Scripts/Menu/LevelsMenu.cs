using UnityEngine;
using System.Collections;

public class LevelsMenu : MenuScreen {

	public GameObject[] levels_buttons_go;
	public GameObject[] levels_buttons_off;
	public GameObject   back_button_go;
	
	void Start ()
	{
		for(int iii = 0; iii < levels_buttons_go.Length; ++iii)
   			UIEventListener.Get(levels_buttons_go[iii]).onClick  = levelClicked;
		UIEventListener.Get(back_button_go).onClick    = goback;
	}
	
	public override void activateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		for(int iii = 0; iii < levels_buttons_off.Length; ++iii)
		{
			bool levelIsUnlocked  = (iii <= Datas.sharedDatas().datas.lastLevel);
			levelIsUnlocked	     &= (Datas.sharedDatas().datas.selectedWorld == Datas.sharedDatas().datas.lastWorld);
			levelIsUnlocked      |= (Datas.sharedDatas().datas.selectedWorld < Datas.sharedDatas().datas.lastWorld);
			
			levels_buttons_off[iii].SetActive(!levelIsUnlocked);
			levels_buttons_go[iii].SetActive(levelIsUnlocked);
		}
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void levelClicked(GameObject go)
	{
		for(int iii = 0; iii < levels_buttons_go.Length; ++iii)
			if(levels_buttons_go[iii] == go)
			{
				Debug.Log("Go to Level : " + iii);
				exitScreen = true;
				screenToGo = MenuGesture.ScreenMenu.NONE;
				Datas.sharedDatas().datas.selectedLevel = iii;
			}
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.WORLDS;
	}
}