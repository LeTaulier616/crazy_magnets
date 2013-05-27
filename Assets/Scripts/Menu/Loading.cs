using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {
	
	GameObject[] frames;
	int          frame;
	private int  lastFrame;
	
	void Start () 
	{
		Transform framesParent = this.transform.FindChild("frames");
		frame  = 0;
		lastFrame = 0;
		frames = new GameObject[framesParent.childCount];
		for(int iii = 1; iii <= frames.Length; ++iii)
			frames[iii-1] = framesParent.FindChild("loading"+iii).gameObject;
	}
	
	void Update () 
	{
		Debug.Log("Update");
		frame++;
		int speed = 8;
		if(frame%speed == 0)
		{
			frames[lastFrame].SetActive(false);
			frames[(frame/speed)%frames.Length].SetActive(true);
			lastFrame = (frame/speed)%frames.Length;
			
			frames[(frame/speed)%frames.Length].GetComponent<UISprite>().alpha = 1.0f;
			frames[(frame/speed)%frames.Length].GetComponent<UISprite>().color = new Color(1.0f,1.0f ,1.0f,1.0f);
		}
	}
}
