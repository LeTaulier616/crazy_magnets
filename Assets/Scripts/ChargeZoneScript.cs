using UnityEngine;
using System.Collections;

public class ChargeZoneScript : MonoBehaviour
{
	private AudioClip chargeZoneSound;
	
	void Start()
	{
		chargeZoneSound = GlobalVarScript.instance.ChargeZoneSound;
		SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
	}
	
	void Update()
	{

	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.gameObject.SendMessageUpwards("Charge", SendMessageOptions.DontRequireReceiver);
			audio.clip = chargeZoneSound;
			audio.Play();
		}
	}
}
