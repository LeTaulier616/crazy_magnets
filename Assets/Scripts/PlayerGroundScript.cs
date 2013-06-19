using UnityEngine;
using System.Collections;

public class PlayerGroundScript : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		gameObject.SendMessageUpwards("CollisionGround", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerStay(Collider other)
	{
		gameObject.SendMessageUpwards("StayGround", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnTriggerExit(Collider other)
	{
		gameObject.SendMessageUpwards("ExitGround", other.gameObject, SendMessageOptions.DontRequireReceiver);
	}
}
