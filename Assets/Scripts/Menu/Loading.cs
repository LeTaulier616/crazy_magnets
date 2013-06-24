using UnityEngine;
using System.Collections;

public class Loading : MonoBehaviour {
	
	GameObject[] frames;
	int          frame;
	private int  lastFrame;
	private int  speed = 8;
	
	void Start () 
	{
		Transform framesParent = this.transform.FindChild("frames");
		frame  = 0;
		lastFrame = 0;
		frames = new GameObject[framesParent.childCount];
		for(int iii = 1; iii <= frames.Length; ++iii)
		{
			frames[iii-1] = framesParent.FindChild("loading"+iii).gameObject;
		}
	}
	
	void Update () 
	{
		frame++;
		if(frame%speed == 0)
		{
			int newFrame = (frame/speed)%frames.Length;
			frames[newFrame].SetActive(true);
			frames[lastFrame].SetActive(false);
			lastFrame = newFrame;
		}
	}
}
