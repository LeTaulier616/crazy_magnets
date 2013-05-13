using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class KillzoneScript : MonoBehaviour
{
	private Body killBody;
	private AudioClip killSound;
	
	void Start()
	{
		killBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		killSound = GlobalVarScript.instance.KillSound;	
		
		killBody.OnCollision += OnCollisionEvent;
	}

	public void Disable()
	{
		killBody.OnCollision -= OnCollisionEvent;
	}
	
	/*
	void OnTriggerEnter(Collider other)
	{
		Debug.Log(other.gameObject.ToString());
		if (other.gameObject.tag == "Player")
		{
			other.transform.gameObject.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
			audio.clip = killSound;
			audio.Play();
		}
		
		else if (other.gameObject.tag == "Bloc")
		{
			Debug.Log("Here");
		}
	}
	*/
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		Body bodyB = fixtureB.Body;
		if(bodyB.UserTag == "PlayerObject")
		{
			bodyB.UserFSBodyComponent.gameObject.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
			audio.clip = killSound;
			audio.Play();
		}
		
		else if (bodyB.UserTag == "Bloc")
		{
			bodyB.UserFSBodyComponent.gameObject.SendMessageUpwards("ResetPosition", SendMessageOptions.DontRequireReceiver);
		}
		
		return true;
	}
}
