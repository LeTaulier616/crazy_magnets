using UnityEngine;
using System.Collections;

public class CheckpointScript : MonoBehaviour
{
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			other.transform.gameObject.SendMessageUpwards("CheckpointReached", transform.position, SendMessageOptions.DontRequireReceiver);
			Destroy (gameObject);
		}
	}
}
