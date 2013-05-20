using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class FollowRoad : MonoBehaviour {
	
	private GameObject   player = null;
	private PlayerScript playerScript = null;
	private FVector2 lastPosition;
	private FVector2 lastRoadPosition;
	
	private Body roadBody;
	
	private bool jointConnected = false;
	
	private GameObject cube = null;
	private Body cubeBody = null;
	private bool cubejointConnected = false;
	
	public Road road;
	
	bool activated = false;
	bool pause     = false;
	
	[HideInInspector]
	public bool back = false;
	public RoadData roadRecto;
	public RoadData roadVerso;
	
	public FVector2 pfVelocity = FVector2.Zero;
	
	void Start () 
	{	
		roadRecto = getRoadDataFromRoad();
	
		roadRecto.reInit();
		
		roadVerso = roadRecto.getReverse();
		
		roadVerso.reInit();
		
		if(roadRecto.activation == Activation.AUTO)
			playRoad();
		
		roadBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		
		roadBody.SetTransform(new FVector2(roadRecto.currentPosition.x, roadRecto.currentPosition.y), 0);
		roadBody.IgnoreGravity = true;
		roadBody.FixedRotation = true;
		roadBody.ResetDynamics();
		
		player       = GameObject.FindGameObjectWithTag("PlayerObject");
		playerScript = player.GetComponent<PlayerScript>();
		
		lastRoadPosition = new FVector2(roadRecto.currentPosition.x, roadRecto.currentPosition.y);
		
		roadBody.OnCollision  += OnCollisionEvent;
		roadBody.OnSeparation += OnSeparationEvent;
		
		this.gameObject.GetComponent<FSBodyComponent>().PhysicsBody.UserData = this.gameObject;
	}
	
	void FixedUpdate () 
	{
		if(pause || !activated)
			return;
		
		roadBody.ResetDynamics();
		
		if(!playerScript.onGround && jointConnected)
		{
			playerScript.onGround = false;
			playerScript.onPFM    = false;
			playerScript.bodyPFM  = null;
		}
		
		if(!back)
		{
			roadRecto.updateRoad();
			roadBody.Position = new FVector2(roadRecto.currentPosition.x, roadRecto.currentPosition.y);
			if(roadRecto.endOfRoad)
			{
				roadVerso.reInit();
				back = true;
			}
		}
		else
		{
			roadVerso.updateRoad();
			
			roadBody.Position = new FVector2(roadVerso.currentPosition.x, roadVerso.currentPosition.y);
			if(roadVerso.endOfRoad)
			{
				roadRecto.reInit();
				back = false;
			}
		}
		
		if(roadRecto.resetAtEnd && roadRecto.endOfRoad)
		{
			stopRoad();
			playerScript.onGround = false;
			playerScript.onPFM    = false;
			playerScript.bodyPFM  = null;
			jointConnected        = false;
			cube                  = null;
			cubeBody              = null;
			cubejointConnected    = false;
			playerScript.Bump(1);
		}
		
		if(jointConnected)
		{
			playerScript.playerBody.Position = new FVector2(playerScript.playerBody.Position.X + roadBody.Position.X - lastRoadPosition.X,
										   					playerScript.playerBody.Position.Y + roadBody.Position.Y - lastRoadPosition.Y);
		}
		
		if(cubejointConnected)
		{
			cubeBody.Position  =new FVector2(cubeBody.Position.X + roadBody.Position.X - lastRoadPosition.X,
										   	cubeBody.Position.Y + roadBody.Position.Y - lastRoadPosition.Y);
		}
		lastRoadPosition = roadBody.Position;
	}
	
	public RoadData getRoadDataFromRoad()
	{
		List<KeyPoint> _keyPoints = new List<KeyPoint>();
		for(int i = 0; i < road.lp.Count; ++i)
		{
			KeyPoint _tmpKey = new KeyPoint();
			_tmpKey.trajectoire = road.trajectoires[i];
			_tmpKey.type = road.pathTypes[i];
			_tmpKey.position = road.lp[i].position;
			_tmpKey.size = (i < road.lp.Count-1 ? new Vector3(
														Mathf.Abs(road.lp[i].position.x - road.lp[i+1].position.x),
														Mathf.Abs(road.lp[i].position.y - road.lp[i+1].position.y),
														0
												  )
												: new Vector3(0, 0, 0)
							);
			_tmpKey.keyType = road.keyTypes[i];
			_tmpKey.wait = road.waits[i];
			_tmpKey.functionName = road.functionNames[i];
			_keyPoints.Add(_tmpKey);
		}
		RoadData _road = new RoadData();
		_road.initWithDatas(road.endBehaviour, road.deplacement, road.activating, _keyPoints, road.speed, road.resetAtEnd);
		return _road;
	}
	
	public void pauseRoad()
	{
		pause = true;
	}
	
	public void playRoad()
	{
		pause     = false;
		activated = true;
		if(back)
			roadVerso.playRoad();
		else
			roadRecto.playRoad();
	}
	
	public void stopRoad()
	{
		roadRecto.reInit();
		roadVerso.reInit();
		activated = false;
		pause     = false;
		back      = false;
		gameObject.transform.position = roadRecto.currentPosition;
	}
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		Body bodyA = fixtureA.Body;
		Body bodyB = fixtureB.Body;
		/*if((bodyB.UserTag == "PlayerObject") && !jointConnected)
		{
			FVector2 colNorm = contact.Manifold.LocalNormal;
			if (Mathf.Abs(colNorm.X) > Mathf.Abs(colNorm.Y))
			{}
			else
			{
				if (colNorm.Y > 0 || bodyB.UserFSBodyComponent.transform.position.y > this.transform.position.y)
				{
					playerScript.onGround = true;
					playerScript.onPFM = true;
					playerScript.bodyPFM = bodyB;
					lastRoadPosition = roadBody.Position;
					jointConnected = true;
					
					if(roadRecto.activation == Activation.PLAYER)
					{
						playRoad();
						Debug.Log("Play Road");
					}
				}
			}
		}
		
		else if(bodyB.UserTag == "Bloc")
		{
			FVector2 colNorm = contact.Manifold.LocalNormal;
			if (Mathf.Abs(colNorm.X) > Mathf.Abs(colNorm.Y))
			{}
			else
			{
				if (colNorm.Y > 0)
				{
					cube = bodyB.UserFSBodyComponent.gameObject;
					cubeBody = bodyB;
					cubejointConnected = true;
				}
			}
		}*/
		
		if((bodyB.UserTag == "PlayerObject") && !jointConnected)
		{
			FVector2 colNorm = contact.Manifold.LocalNormal;
			if (colNorm.Y > 0 || bodyB.UserFSBodyComponent.transform.position.y > this.transform.position.y)
			{
				playerScript.onGround = true;
				playerScript.onPFM = true;
				playerScript.bodyPFM = bodyA;
				lastRoadPosition = roadBody.Position;
				jointConnected = true;
				
				if(roadRecto.activation == Activation.PLAYER)
				{
					playRoad();
				}
			}
		}
		else if(bodyB.UserTag == "Bloc")
		{
			FVector2 colNorm = contact.Manifold.LocalNormal;
			if (colNorm.Y > 0 || bodyB.UserFSBodyComponent.transform.position.y > this.transform.position.y)
			{
				cube               = bodyB.UserFSBodyComponent.gameObject;
				cubeBody           = bodyB;
				cubejointConnected = true;
			}
		}
		return true;
	}
	
	private void OnSeparationEvent(Fixture fixtureA, Fixture fixtureB)
	{

	}
	
	private void OnTriggerExit(Collider col)
	{
		if(col.name == "GROUND_HITBOX" && col.transform.parent.name == "PLAYER")
		{
			playerScript.onGround = false;
			playerScript.onPFM    = false;
			playerScript.bodyPFM  = null;
			jointConnected        = false;
		}
		else if(col.tag == "Bloc" && col.name != "Hitbox")
		{
			cube               = null;
			cubeBody           = null;
			cubejointConnected = false;
		}
	}
}
