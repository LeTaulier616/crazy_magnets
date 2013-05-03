using UnityEngine;
using System.Collections;

public class EndLevelScript : MonoBehaviour
{
	public int boltCount;
	
	void Start()
	{
		boltCount = 0;
	}
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			Camera.mainCamera.BroadcastMessage("EndLevel");
		}
	}
}