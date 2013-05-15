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
		ENDLEVEL
	};
	
	private ScreenMenu menuScreen = ScreenMenu.NONE;
	private ScreenMenu nextScreen = ScreenMenu.NONE;
	private ScreenMenu lastScreen = ScreenMenu.NONE;
	
	private float timer       = 0.0f;
	private bool  setVisible  = false;
	private bool  setHidden   = false;
	
	private MenuScreen screen = null;
	
	private float lerpMaxValue = 255.0f;
	private float lerpSpeed    = 1.2f;
	
	void Start()
	{
		screen = GameObject.Find("Menus").GetComponent<MainMenu>();
		menuScreen = ScreenMenu.MAIN;
		screen.activateMenu();
		setVisible = true;
		timer = 0.0f;
		
		object[] allObjects = FindObjectsOfTypeAll(typeof(GameObject)) ;
		foreach(object thisObject in allObjects)
			DontDestroyOnLoad((GameObject) thisObject);
	}
	
	void FixedUpdate()
	{
		timer += Time.deltaTime/(lerpMaxValue*lerpSpeed);
		float lerpValue = Mathf.Lerp(0,lerpMaxValue,timer);
		foreach(UIWidget widget in GameObject.Find("Anchor").GetComponentsInChildren<UIWidget>())
        {
			if(widget.name != "Menu_Background" || lastScreen == ScreenMenu.NONE || nextScreen == ScreenMenu.NONE)
			{
				if(setVisible)
				{
					widget.alpha   = lerpValue;
					widget.color   = new Color(lerpValue,lerpValue,lerpValue,lerpValue);
				}
				else if(setHidden)
				{
					widget.alpha   = 1.0f - lerpValue;
					widget.color   = new Color(1.0f - lerpValue,1.0f - lerpValue,1.0f - lerpValue,1.0f - lerpValue);
				}
				else
				{
					widget.alpha = 1.0f;
					widget.color   = new Color(1.0f,1.0f,1.0f,1.0f);
				}
				if(widget.GetComponent<TweenColor>())
					widget.GetComponent<TweenColor>().enabled = false;
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
			if(lerpValue >= 1.0f && setHidden)
			{
				switchScreen();
				lerpValue = 0.0f;
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
	}
	
	private void switchScreen()
	{
		Debug.Log("Screen Switch");
		bool loadLevel = false;
		screen.desactivateMenu();
		if(menuScreen == ScreenMenu.PAUSE && nextScreen == ScreenMenu.PAUSE)
			nextScreen = ScreenMenu.NONE;
		else if(nextScreen == ScreenMenu.NONE)
			loadLevel = true;
		menuScreen = nextScreen;
		switch(menuScreen)
		{
			case ScreenMenu.NONE :
				screen = null;
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
		}
		if(screen != null)
		{
			screen.activateMenu();
			foreach(UIWidget widget in GameObject.Find("Anchor").GetComponentsInChildren<UIWidget>())
	        {
				if(widget.name != "Menu_Background")
				{
					widget.alpha   = 0.0f;
					widget.color   = new Color(0.0f,0.0f,0.0f,0.0f);
				}
			}
			setHidden = false;
			setVisible = true;
			timer = 0.0f;
		}
		if(loadLevel)
			Application.LoadLevelAsync(Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld + Datas.sharedDatas().datas.selectedLevel + 1);
	}
}
