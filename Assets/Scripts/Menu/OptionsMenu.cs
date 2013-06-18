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
	}
	
	public override void activateMenu()
	{
		Debug.Log("Options Activated");
		back_button_go.transform.parent.gameObject.SetActive(true);
		
		global_button_go.GetComponent<UISlider>().sliderValue = Datas.sharedDatas().datas.globalVolume;
		sfx_button_go.GetComponent<UISlider>().sliderValue    = Datas.sharedDatas().datas.sfxVolume;
		bgm_button_go.GetComponent<UISlider>().sliderValue    = Datas.sharedDatas().datas.bgmVolume;
		
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
	
	private void goback(GameObject go)
	{
		Debug.Log("Go Back");
		exitScreen = true;
		screenToGo = MenuGesture.ScreenMenu.MAIN;
	}
	
	public void OnSliderChangeBGM()
	{
		Datas.sharedDatas().datas.bgmVolume = bgm_button_go.GetComponent<UISlider>().sliderValue;
	}
	
	public void OnSliderChangeSFX()
	{
		Datas.sharedDatas().datas.sfxVolume = sfx_button_go.GetComponent<UISlider>().sliderValue;
	}
	
	public void OnSliderChangeGlobal()
	{
		Datas.sharedDatas().datas.globalVolume = global_button_go.GetComponent<UISlider>().sliderValue;
	}
}
