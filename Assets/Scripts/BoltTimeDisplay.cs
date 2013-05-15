using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BoltTimeDisplay : MonoBehaviour {
	
	public string KeyName;
	
	// Use this for initialization
	void Start () 
	{
		if(PlayerPrefs.HasKey(KeyName))
		{			
			if(this.gameObject.name == "Bolts")
			{
				this.GetComponent<UILabel>().text = PlayerPrefs.GetInt(KeyName) + " / 3";
			}
			
			else
			{
				this.GetComponent<UILabel>().text = FormatTime(PlayerPrefs.GetFloat(KeyName));
			}
		}

	}
	
	// Update is called once per frame
	void Update () 
	{
	
	}
	
	public static string FormatTime(float time)
	{
		string formatTime = ""; 
		
		float Minutes = Mathf.Floor(time / 60.0f);
		float Seconds = Mathf.Floor(time % 60.0f);
		float Decisecond = Mathf.Floor((time * 100.0f) % 100.0f);
		
		string txtMinutes = Minutes < 10 ? "0" + Minutes : "" + Minutes;
		string txtSeconds = Seconds < 10 ? "0" + Seconds : "" + Seconds;
		string txtDecisecond = Decisecond < 10 ? "0" + Decisecond : "" + Decisecond;
		
		formatTime = txtMinutes + "' " + txtSeconds + "'' " + txtDecisecond + "'''";
		
		return formatTime;
	}
}
