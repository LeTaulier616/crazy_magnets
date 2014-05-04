using UnityEngine;
using System.Collections;

public class Timer : MonoBehaviour {
	
	UILabel TimeLabel;
	
	public float time;
	
	float Minutes;
	float Seconds;
	float Decisecond;
	
	// Use this for initialization
	void Start () 
	{
		TimeLabel = transform.GetComponent<UILabel>();
		
		time = 0.0f;
		Minutes = 0.0f;
		Seconds = 0.0f;
		Decisecond = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		time = Time.timeSinceLevelLoad;
		Minutes = Mathf.Floor(time / 60.0f);
		Seconds = Mathf.Floor(time % 60.0f);
		Decisecond = Mathf.Floor((time * 100.0f) % 100.0f);
		
		string txtMinutes = Minutes < 10 ? "0" + Minutes : "" + Minutes;
		string txtSeconds = Seconds < 10 ? "0" + Seconds : "" + Seconds;
		string txtDecisecond = Decisecond < 10 ? "0" + Decisecond : "" + Decisecond;
		
		TimeLabel.text = txtMinutes + "' " + txtSeconds + "'' " + txtDecisecond + "''' ";
	}
}