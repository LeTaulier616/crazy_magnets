using UnityEngine;
using System.Collections;

public class BumperScript : MonoBehaviour
{
	public float bumperForce = 10f;
	
	private AudioClip bumperSound;
	
	void Start()
	{
		SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
		bumperSound = GlobalVarScript.instance.BumperSound;
	}
	
	public void PlayAnimation()
	{
		if(this.GetComponentInChildren<Animation>() != null)
			this.transform.GetComponentInChildren<Animation>().Play();
		audio.clip = bumperSound;
		audio.Play();
	}
}
