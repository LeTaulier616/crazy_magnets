using UnityEngine;
using System.Collections;

public class InterruptorPusher : MonoBehaviour
{
	// Customisable Datas
	public bool isElectrifable = false;
	public bool isPusher       = false;
	//[HideInInspector]
	public bool isElectrified  = false;
	
	void Start()
	{
		this.gameObject.GetComponent<FSBodyComponent>().PhysicsBody.UserData = this.gameObject;
	}
	
	void Update()
	{
		if(isElectrifable && gameObject.tag == "PlayerObject")
		{
			PlayerScript player = gameObject.GetComponentInChildren<PlayerScript>();
			if (player != null)
				isElectrified = player.IsCharged();
		}
	}
}
