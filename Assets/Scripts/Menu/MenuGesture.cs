using UnityEngine;
using System.Collections;

public class MenuGesture : MonoBehaviour {
	
	public enum ScreenMenu
	{
		NONE = 0,
		MAIN,
		WORLDS,
		LEVELS,
		OPTIONS,
		PAUSE,
		ENDLEVEL,
		CREDITS
	};
	
	public static ScreenMenu menuScreen;
	
	private static ScreenMenu nextScreen = ScreenMenu.NONE;
	private ScreenMenu lastScreen = ScreenMenu.NONE;
	
	private float timer       = 0.0f;
	private bool  setVisible  = false;
	private bool  setHidden   = false;
	
	private MenuScreen screen = null;
	
	private float lerpMaxValue = 255.0f;
	private float lerpSpeed    = 0.8f;
	
	private bool doNothing = false;
	private float alpha = 0;
	private float alphaDir = 0;
	
	private Fabric.GroupComponent ambiantComponent = null;
	private Fabric.GroupComponent musicComponent   = null;
	private bool firstUpdate = true;
	
	void Start()
	{
		if(Application.loadedLevelName == "MENU")
		{
			if(nextScreen == ScreenMenu.NONE)
				menuScreen = ScreenMenu.MAIN;
			else
				menuScreen = nextScreen;
		}
		else 
		{
			menuScreen = ScreenMenu.NONE;
			Datas.sharedDatas().datas.currentLevel  = Application.loadedLevel-1;
			Datas.sharedDatas().datas.currentWorld  = ((Application.loadedLevel-1) - (Application.loadedLevel-1)%MyDefines.kLevelsByWorld)/MyDefines.kLevelsByWorld;
			Datas.sharedDatas().datas.selectedLevel = Datas.sharedDatas().datas.currentLevel;
			Datas.sharedDatas().datas.selectedWorld = Datas.sharedDatas().datas.currentWorld;
		}
		
		ambiantComponent = GameObject.Find("AudioManager").transform.FindChild("AMB").GetComponent<Fabric.GroupComponent>();
		ambiantComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
		
		musicComponent = GameObject.Find("AudioManager").transform.FindChild("M").GetComponent<Fabric.GroupComponent>();
		musicComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
		
		if(Application.loadedLevelName == "MENU")
		{
			setScreen();
		}
		else
		{
			screen = this.GetComponent<Interface>();
			menuScreen = ScreenMenu.NONE;
		}
		
		screen.activateMenu();
		setVisible = true;
		doNothing  = false;
		timer = 0.0f;
		
		this.alpha = 1;
		this.alphaDir = -1f;
	}
	
	void LateUpdate()
	{
		if(doNothing)
			return;
		
		if (this.alpha != 0 && this.alpha != 1)
		{
			return;
		}
		
		timer += Time.deltaTime/(lerpMaxValue*lerpSpeed)/Time.timeScale;
		
		float lerpValue = Mathf.Lerp(0,lerpMaxValue,timer);
		
		if(screenIsMenuScreen(menuScreen) && screenIsMenuScreen(nextScreen))
		{
			lerpValue = 1.0f;
		}
			
		foreach(UIWidget widget in GameObject.Find("Anchor").GetComponentsInChildren<UIWidget>())
        {
			if(widget.name != "Menu_Background" || lastScreen == ScreenMenu.NONE || nextScreen == ScreenMenu.NONE)
			{
				if(widget.name == "PAUSE_BACKGROUND")
				{
					widget.alpha   = 0.5f;
					widget.color   = new Color(0.5f,0.5f,0.5f,0.5f);
				}
				else if(setVisible)
				{
					if(widget.name == "Back")
					{
						widget.alpha = lerpValue * 0.5f;
						widget.color = new Color(lerpValue*0.5f,lerpValue*0.5f,lerpValue*0.5f,lerpValue*0.5f);
					}
					else
					{
						widget.alpha = lerpValue;
						widget.color = new Color(lerpValue,lerpValue,lerpValue,lerpValue);
					}
				}
				else if(setHidden)
				{
					if(widget.name == "Back")
					{
						widget.alpha = (1.0f - lerpValue) * 0.5f;
						widget.color = new Color((1.0f - lerpValue)*0.5f,(1.0f - lerpValue)*0.5f,(1.0f - lerpValue)*0.5f,(1.0f - lerpValue)*0.5f);
					}
					else
					{
						widget.alpha = 1.0f - lerpValue;
						widget.color = new Color(1.0f - lerpValue,1.0f - lerpValue,1.0f - lerpValue,1.0f - lerpValue);
					}
				}
			}
			else
			{
				widget.alpha = 1.0f;
				widget.color   = new Color(1.0f,1.0f,1.0f,1.0f);
			}
        }
		
		if(screen != null && screen.exitScreen) 
		{
			if(!setHidden)
			{
				Debug.Log("Ask For Change Screen Received");
				nextScreen = screen.screenToGo;
				lastScreen = menuScreen;
				setVisible = false;
				setHidden  = true;
				timer      = 0.0f;
				lerpValue  = 0.0f;
			}
			
			if(screenIsMenuScreen(menuScreen) && screenIsMenuScreen(nextScreen))
			{
				lerpValue = 1.0f;
			}
			
			if ((Application.loadedLevelName == "Cutscene" || Application.loadedLevelName == "CM_Level_0_FINAL" || Application.loadedLevelName == "Cutscene2") && this.alphaDir == -1)
				this.alphaDir = 1;
			
			if(lerpValue >= 1.0f && setHidden)
			{
				switchScreen();
				lerpValue = 0.0f;
				if(screenIsMenuScreen(lastScreen) != screenIsMenuScreen(menuScreen))
					return;
			}
		}
		
		foreach(UIButton button in GameObject.Find("Anchor").GetComponentsInChildren<UIButton>())
        {
			button.isEnabled = !(setVisible || setHidden);
		}
		
		if(lerpValue >= 1.0f)
		{
			setVisible = false;
			setHidden  = false;
		}
		
		if(menuScreen == ScreenMenu.OPTIONS)
		{
			AudioSource[] audioLs = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
      	 	foreach (AudioSource audioL in audioLs) 
			{
      	    	audioL.volume = Datas.sharedDatas().datas.globalVolume * Datas.sharedDatas().datas.sfxVolume;
        	}
			ambiantComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
			musicComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
			musicComponent.gameObject.GetComponentInChildren<AudioSource>().volume = Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume;
		}
		
		if(firstUpdate)
		{
			Debug.Log("First Update");
			AudioSource[] audioLs = FindObjectsOfType(typeof(AudioSource)) as AudioSource[];
      	 	foreach (AudioSource audioL in audioLs)
			{
      	    	audioL.volume = Datas.sharedDatas().datas.globalVolume * Datas.sharedDatas().datas.sfxVolume;
        	}
			ambiantComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
			musicComponent.SetVolume(Datas.sharedDatas().datas.bgmVolume * Datas.sharedDatas().datas.globalVolume);
			firstUpdate = false;
		}
	}
	
	private void switchScreen()
	{
		Debug.Log("Screen Switch");
		
		Datas.sharedDatas().saveDatas();
		
		bool loadLevel = false;
		bool loadTuto  = false;
		bool loadMenus = false;
		bool switchHUD = false;
		
		screen.desactivateMenu();
		
		loadLevel = screen.loadLevel;
		loadTuto  = screen.loadTuto;
		if(!screenIsMenuScreen(nextScreen) && screenIsMenuScreen(menuScreen))
			loadMenus = true;
		if(screenIsMenuScreen(nextScreen) != screenIsMenuScreen(menuScreen))
			switchHUD = true;
		
		menuScreen = nextScreen;
		
		if(loadMenus)
		{
			doNothing = true;
			Application.LoadLevel("MENU");
		}
		else if(loadLevel)
		{
			doNothing = true;
			if(Application.loadedLevelName == "CM_Level_0_FINAL")
			{
				StartCoroutine(LoadEndCutsceneToLoad());
			}
			else
			{
				GameObject.Find("Anchor").transform.FindChild("LOADING_PANEL").gameObject.SetActive(true);
				StartCoroutine(LoadLevelToLoad());
			}
		}
		else if(loadTuto)
		{
			doNothing = true;
			GameObject.Find("Anchor").transform.FindChild("LOADING_PANEL").gameObject.SetActive(true);
			StartCoroutine(LoadCutsceneToLoad());
		}
		
		if(switchHUD || loadMenus || loadLevel)
		{
			doNothing = true;
			return;
		}

		setScreen ();
	}
		
	public void setScreen()
	{
		switch(menuScreen)
		{
			case ScreenMenu.NONE :
				screen = GameObject.Find("Menus").GetComponent<Interface>();
			break;
			case ScreenMenu.MAIN :
				screen = GameObject.Find("Menus").GetComponent<MainMenu>();
			break;
			case ScreenMenu.WORLDS :
				screen = GameObject.Find("Menus").GetComponent<WorldsMenu>();
			break;
			case ScreenMenu.LEVELS :
				screen = GameObject.Find("Menus").GetComponent<LevelsMenu>();
			break;
			case ScreenMenu.OPTIONS :
				screen = GameObject.Find("Menus").GetComponent<OptionsMenu>();
			break;
			case ScreenMenu.ENDLEVEL :
				screen = GameObject.Find("Menus").GetComponent<EndMenu>();
			break;
			case ScreenMenu.PAUSE :
				screen = GameObject.Find("Menus").GetComponent<PauseMenu>();
			break;
			case ScreenMenu.CREDITS :
				screen = GameObject.Find("Menus").GetComponent<Credits>();
			break;
		}
		
		Debug.Log("Menu Screen");
		
		if (screen != null)
			screen.activateMenu();
		setHidden = false;
		setVisible = true;
		timer = 0.0f;
	}
	
	public void startTuto()
	{
		menuScreen = MenuGesture.ScreenMenu.NONE;
		screen = GameObject.Find("Menus").GetComponent<Interface>();
	}
	
	public void endTuto()
	{
		screen.desactivateMenu();
		menuScreen = MenuGesture.ScreenMenu.ENDLEVEL;
		screen = GameObject.Find("Menus").GetComponent<EndMenu>();
		screen.activateMenu();
		setHidden = false;
		setVisible = true;
		timer = 0.0f;
	}
	
	private bool screenIsMenuScreen(ScreenMenu m_screen)
	{
		return (m_screen == ScreenMenu.NONE || m_screen == ScreenMenu.PAUSE || m_screen == ScreenMenu.ENDLEVEL);
	}
	
    IEnumerator LoadLevelToLoad() {
        AsyncOperation async = Application.LoadLevelAsync(Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld + Datas.sharedDatas().datas.selectedLevel + 1);
        yield return async;
    }
	
    IEnumerator LoadTutoToLoad() {
		AsyncOperation async = Application.LoadLevelAsync("CM_Level_0_FINAL");
		yield return async;
    }
	
    IEnumerator LoadCutsceneToLoad() {
		AsyncOperation async = Application.LoadLevelAsync("Cutscene");
		yield return async;
    }
	
	IEnumerator LoadEndCutsceneToLoad() {
		AsyncOperation async = Application.LoadLevelAsync("Cutscene2");
		yield return async;
    }
	
	public void fadeIn()
	{
		this.alphaDir = 1;
	}
	
	public void fadeOut()
	{
		this.alphaDir = -1;
	}
	
	void OnGUI()
	{
		if (this.alphaDir == 1)
		{
			this.alpha += Time.deltaTime / 2f;
			if (this.alpha > 1)
			{
				this.alpha = 1;
			}
		}
		else if (this.alphaDir == -1)
		{
			this.alpha -= Time.deltaTime / 2f;
			if (this.alpha < 0)
			{
				this.alpha = 0;
			}
		}
		Color color = new Color(0, 0, 0, this.alpha);
		Texture2D texture = new Texture2D(1, 1);
    	texture.SetPixel(0,0,color);
	    texture.Apply();
	    GUI.skin.box.normal.background = texture;
	    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);
	}
}

