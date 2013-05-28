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
			
			if(levelIsUnlocked)
			{
				levels_buttons_go[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "Niveau " + (iii+1);
				int levelNumber = iii + Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld;
				string timeString = BoltTimeDisplay.FormatTime(Datas.sharedDatas().datas.timeLevels[levelNumber]);
				levels_buttons_go[iii].transform.FindChild("Time").GetComponent<UILabel>().text  = timeString;
				levels_buttons_go[iii].transform.FindChild("Screw").GetComponent<UILabel>().text = Datas.sharedDatas().datas.screwsGotchaByLevel[levelNumber] + "/3";
			}
			else
			{
				levels_buttons_off[iii].transform.FindChild("Label").GetComponent<UILabel>().text  = "Niveau " + (iii+1);
			}
		}
		
		exitScreen = false;
		loadLevel = false;
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
				loadLevel = true;
			}
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.WORLDS;
	}
}