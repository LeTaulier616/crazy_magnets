using UnityEngine;
using System.Collections;

public class PlayerHeadScript : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		gameObject.SendMessageUpwards("CollisionHead", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerStay(Collider other)
	{
		gameObject.SendMessageUpwards("StayHead", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider other)
	{
		gameObject.SendMessageUpwards("ExitHead", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
