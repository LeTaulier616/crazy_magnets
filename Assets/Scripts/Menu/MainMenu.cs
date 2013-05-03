using UnityEngine;
using System.Collections;

public class MainMenu : MonoBehaviour {
	
	public GameObject TitleScreen;
	public GameObject LevelSelectScreen;
	public GameObject Background;
	
	
	// Use this for initialization
	void Start () 
	{
		TitleScreen.SetActiveRecursively(false);
		Background.SetActiveRecursively(false);
		LevelSelectScreen.SetActiveRecursively(false);
		
		TitleScreen.SetActiveRecursively(true);
		Background.SetActiveRecursively(true);
		LevelSelectScreen.SetActiveRecursively(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(Input.GetKeyDown(KeyCode.Escape))
		{
			Application.Quit();
		}
	}
	
	public void ToLevelSelectScreen()
	{
		TitleScreen.SetActiveRecursively(false);
		LevelSelectScreen.SetActiveRecursively(true);
	}
	
	public void BackToMainMenu()
	{
		TitleScreen.SetActiveRecursively(true);
		Background.SetActiveRecursively(true);
		LevelSelectScreen.SetActiveRecursively(false);
	}
	
	public void ToLevel1()
	{
		Application.LoadLevel(1);
	}
	
	public void ToLevel2()
	{
		Application.LoadLevel(2);
	}
	
	public void ToLevel3()
	{
		Application.LoadLevel(3);
	}
	
	public void ToLevel4()
	{
		Application.LoadLevel(4);
	}
	
	public void ToLevel5()
	{
		Application.LoadLevel(5);
	}
}
