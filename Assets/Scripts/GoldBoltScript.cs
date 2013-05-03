using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class GoldBoltScript : MonoBehaviour {
	
	private Body boltBody;
	private EndLevelScript endLevel;
	
	// Use this for initialization
	void Start () 
	{
		boltBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		boltBody.OnCollision += OnCollisionEvent;
		
		endLevel = GameObject.FindGameObjectWithTag("EndLevel").GetComponent<EndLevelScript>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(Vector3.up, 5.0f);
	}
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		Body bodyB = fixtureB.Body;
		if(bodyB.UserTag == "PlayerObject")
		{
			if(endLevel.boltCount <= 3)
				endLevel.boltCount++;
			
			Destroy(gameObject);
		}	
		return false;
	}
}
