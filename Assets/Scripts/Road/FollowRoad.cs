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
		
		this.gameObject.GetComponent<FSBodyComponent>().PhysicsBody.UserData = this.gameObject;
		
		roadRecto.road = this.road;
		roadVerso.road = this.road;
	}
	
	void FixedUpdate () 
	{
		if(pause || !activated)
			return;
		
		roadBody.ResetDynamics();
		
		if (!playerScript.isAlive)
			jointConnected = false;
		
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
			playerScript.playerBody.ApplyLinearImpulse(new FVector2(0.001f, 0.0f));
		}
		
		if(cubejointConnected)
		{
			cubeBody.Position  =new FVector2(cubeBody.Position.X + roadBody.Position.X - lastRoadPosition.X,
										   	cubeBody.Position.Y + roadBody.Position.Y - lastRoadPosition.Y);
			cubeBody.ApplyLinearImpulse(new FVector2(0.001f, 0.0f));
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
	
	public void reloadRoad()
	{
		deleteJoin();
		if(this.gameObject.GetComponent<InterruptorReceiver>() != null)
			this.gameObject.GetComponent<InterruptorReceiver>().reloadInterruptor();
		stopRoad();
		if(roadRecto.activation == Activation.AUTO)
			playRoad();
		roadBody.ResetDynamics();
		roadRecto.updateRoad();
		roadBody.Position = new FVector2(roadRecto.currentPosition.x, roadRecto.currentPosition.y);
	}
	
	public void deleteJoin()
	{
		jointConnected = false;
	}
	
	private void OnTriggerEnter(Collider col)
	{
		if (!playerScript.isAlive)
			return;
		if(col.name == "GROUND_HITBOX" && col.transform.parent.name == "PLAYER" && col.transform.position.y > this.transform.position.y)
		{
			playerScript.onGround = true;
			playerScript.onPFM = true;
			playerScript.bodyPFM = col.transform.parent.gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
			lastRoadPosition = roadBody.Position;
			jointConnected = true;
			
			if(roadRecto.activation == Activation.PLAYER)
			{
				playRoad();
			}
		}
		else if(col.tag == "Bloc" && col.name == "Hitbox" && col.transform.position.y > this.transform.position.y)
		{
			cube               = col.transform.parent.gameObject;
			cubeBody           = col.transform.parent.gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
			cubejointConnected = true;
		}
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
		else if(col.tag == "Bloc" && col.name == "Hitbox")
		{
			cube               = null;
			cubeBody           = null;
			cubejointConnected = false;
		}
	}
}
