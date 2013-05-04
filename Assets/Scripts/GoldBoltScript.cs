using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class GoldBoltScript : MonoBehaviour {
	
	private Body boltBody;
	private EndLevelScript endLevel;
	
	private GameObject boltMesh;
	private AudioClip boltSound;
	
	private bool pickedUp;
	
	// Use this for initialization
	void Start () 
	{
		boltBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		boltBody.OnCollision += OnCollisionEvent;
		
		boltMesh = transform.FindChild("MESH").gameObject;

		boltSound = GlobalVarScript.instance.BoltSound;
		
		GameObject endGameObject = GameObject.FindGameObjectWithTag("EndLevel");
				
		if(endGameObject != null)
			endLevel = endGameObject.GetComponent<EndLevelScript>();
		
		pickedUp = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		transform.Rotate(Vector3.up, 5.0f);
	}
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{		
		Body bodyB = fixtureB.Body;
		
		if(bodyB.UserTag == "PlayerObject" && !pickedUp)
		{
			if(endLevel != null && endLevel.boltCount <= 3)
				endLevel.boltCount++;
			
			if(audio != null)
			{
				audio.clip = boltSound;
				audio.Play();
			}
			
			if(boltMesh != null)
				Destroy(boltMesh);
			
			pickedUp = true;
		}	
		
		return false;
	}
}
