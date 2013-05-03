using UnityEngine;
using System.Collections;

public class MenuManager : MonoBehaviour {
	
	public GameObject HUDScreen;
	public GameObject PauseScreen;
	public GameObject ResultsScreen;
	
	private EndLevelScript endLevel;
	private float time;
	
	private GameObject player;
	
	// Use this for initialization
	void Start () 
	{
		Time.timeScale = 1.0f;
		HUDScreen.SetActiveRecursively(true);
		PauseScreen.SetActiveRecursively(false);
		ResultsScreen.SetActiveRecursively(false);
		
		endLevel = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevelScript>();
		time = 0.0f;
		
		player = GlobalVarScript.instance.player;
	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public void Pause()
	{
		if(Time.timeScale == 1.0f)
		{
			Time.timeScale = 0.0f;
			HUDScreen.SetActiveRecursively(false);
			PauseScreen.SetActiveRecursively(true);
		}
		
		else
		{
			Time.timeScale = 1.0f;
			HUDScreen.SetActiveRecursively(true);
			PauseScreen.SetActiveRecursively(false);
			
		}
	}
	
	public void NextLevel()
	{
		Pause();
		Application.LoadLevel(Application.loadedLevel + 1);
	}
	
	public void Restart()
	{
		Pause();
		Application.LoadLevel(Application.loadedLevel);
	}
	
	public void ToMainMenu()
	{
		Pause();
		Application.LoadLevel(0);
	}
	
	public void EndLevel()
	{
		Pause();
		HUDScreen.SetActiveRecursively(false);
		PauseScreen.SetActiveRecursively(false);
		
		ResultsScreen.SetActiveRecursively(true);
		ResultsScreen.transform.FindChild("BOLTS").GetComponent<UILabel>().text = endLevel.boltCount + " / 3";
		time = Time.timeSinceLevelLoad;
		
		CheckAndSaveData();
	}
	
	public void NextCheckpoint()
	{
		Pause();
		player.SendMessage("ToNextCheckPoint", SendMessageOptions.DontRequireReceiver);
	}
	
	void CheckAndSaveData()
	{
		string boltkeyName = "Level" + Application.loadedLevel + "_Bolts";
		string timekeyName = "Level" + Application.loadedLevel + "_Time";
		
		if(PlayerPrefs.HasKey(boltkeyName))
		{
			if(endLevel.boltCount > PlayerPrefs.GetInt(boltkeyName))
			{
				PlayerPrefs.SetInt(boltkeyName, endLevel.boltCount);
			}
		}
		
		else
		{
			PlayerPrefs.SetInt(boltkeyName, endLevel.boltCount);
		}
		
		if(PlayerPrefs.HasKey(timekeyName))
		{
			if(time < PlayerPrefs.GetFloat(timekeyName))
			{
				PlayerPrefs.SetFloat(timekeyName, time);
			}
		}
		
		else
		{
			PlayerPrefs.SetFloat(timekeyName, time);
		}
		
		PlayerPrefs.Save();
	}
}
