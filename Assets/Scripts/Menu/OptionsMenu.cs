using UnityEngine;
using System.Collections;

public class OptionsMenu : MenuScreen 
{
	public GameObject back_button_go;
	public GameObject reinit_button_go;
	public GameObject global_button_go;
	public GameObject sfx_button_go;
	public GameObject bgm_button_go;
	
	void Start ()
	{
		UIEventListener.Get(back_button_go).onClick   = goback;
		UIEventListener.Get(reinit_button_go).onClick = reinit;
		UIEventListener.Get(global_button_go).onClick = changeVolume;
		UIEventListener.Get(sfx_button_go).onClick    = changeSFX;
		UIEventListener.Get(bgm_button_go).onClick    = changeBGM;
	}
	
	public override void activateMenu()
	{
		Debug.Log("Options Activated");
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		exitScreen = false;
	}
	
	public override void desactivateMenu()
	{
		back_button_go.transform.parent.gameObject.SetActive(false);
	}
	
	private void reinit(GameObject go)
	{
		Debug.Log("Reinit");
	}
	
	private void changeVolume(GameObject go)
	{
		Debug.Log("Change Volume");
		Datas.sharedDatas().datas.globalVolume = (int)(go.GetComponent<UISlider>().sliderValue * 100.0f);
	}
	
	private void changeSFX(GameObject go)
	{
		Debug.Log("Change SFX");
		Datas.sharedDatas().datas.sfxVolume = (int)(go.GetComponent<UISlider>().sliderValue * 100.0f);
	}
	
	private void changeBGM(GameObject go)
	{
		Debug.Log("Change BGM");
		Datas.sharedDatas().datas.bgmVolume = (int)(go.GetComponent<UISlider>().sliderValue * 100.0f);
	}
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
}
