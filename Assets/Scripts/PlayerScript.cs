using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

public class PlayerScript : Controllable
{
	private Vector3   grabTarget;
	private float grabRange;
	
	private bool canResurrect;
	private List<Vector3> checkpoints = new List<Vector3>();
	private int checkpointIndex;
	
	public bool hasWon;
	
	void Start()
	{
		base.Start();
				
		this.playerMesh = GlobalVarScript.instance.playerMesh;
	
		this.speed = GlobalVarScript.instance.playerSpeed;
		this.jumpForce = GlobalVarScript.instance.playerJumpForce;
		this.playerBody.LinearDamping = GlobalVarScript.instance.playerDamping;
		
		this.grabTarget = Vector3.zero;
		this.grabRange = GlobalVarScript.instance.GrabRadius;
		
		this.canResurrect = false;
		this.checkpointIndex = 0;
		this.checkpoints.Add(transform.position);
		GetCheckpoints();
		
		this.hasWon = false;
	}
	
	void Update()
	{
		base.Update();
		
		if (GlobalVarScript.instance.cameraTarget != this.target)
		{
			if(this.hasWon && this.onGround)
			{
				playerMesh.animation.CrossFade("win", 0.25f);
			}
			else
			{
				playerMesh.animation.CrossFade("idle", 0.25f);
			}
			
			return;
		}
		
		if (this.grabTarget != Vector3.zero)
		{
			if (Vector3.Distance(this.transform.position, grabTarget) < grabRange)
			{
				float dist = Vector3.Distance(transform.position, this.grabTarget);
				Vector3 rayTest = new Vector3(this.grabTarget.x - transform.position.x, this.grabTarget.y - transform.position.y, this.grabTarget.z - transform.position.z);
				RaycastHit hit;
				if (Physics.Raycast(transform.position, rayTest.normalized, out hit, dist) && hit.transform.tag != "Grab"
					// cas particulier
					&& hit.transform.name != "HEAD_HITBOX")
				{
					this.grabTarget = Vector3.zero;
				}
				else
				{
					FVector2 grabForce = new FVector2(rayTest.x, rayTest.y) * 13.0f * this.playerBody.Mass * this.playerBody.GravityScale * Time.deltaTime;
					playerBody.ApplyLinearImpulse(new FVector2(grabForce.X * 25f, grabForce.Y));
				}
			}
			else
			{
				this.grabTarget = Vector3.zero;
				this.canMove = false;
			}
		}
	}
	
	void LateUpdate()
	{
		base.LateUpdate();
	}
	
	public void Grab(Vector3 target)
	{
		this.grabTarget = target;
		this.canMove = false;
	}
	
	public void CheckpointReached()
	{
		if(checkpointIndex <= checkpoints.Count - 1)
			this.checkpointIndex++;
	}
	
	private void GetCheckpoints()
	{
		GameObject checkPointParent = GameObject.FindGameObjectWithTag("CheckPoint");
		
		foreach(UnityEngine.Transform child in checkPointParent.transform)
		{
			checkpoints.Add(child.position);
		}
	}
	
	public void ToNextCheckPoint()
	{
		CheckpointReached();
		
		if(checkpointIndex > checkpoints.Count)
			checkpointIndex = checkpoints.Count - 1;
		
		Vector3 currentCheckpoint = checkpoints[checkpointIndex];
		this.playerBody.Position = new FVector2(currentCheckpoint.x, currentCheckpoint.y);
	}
	
	public override void Tap ()
	{
		if (this.canResurrect && this.checkpoints.Count > 0)
		{
			this.Resurrect();
		}
	}
	
	private void Resurrect()
	{
		Vector3 lastCheckpoint = this.checkpoints[checkpointIndex];
		this.playerBody.Position = new FVector2(lastCheckpoint.x, lastCheckpoint.y);
		this.transform.position = lastCheckpoint;
		this.playerBody.BodyType = BodyType.Dynamic;
		this.playerBody.Enabled = true;
		this.playerBody.ResetDynamics();
		this.playerBody.Mass = 1f;
		GlobalVarScript.instance.resetCamera(true);
		
		if(playerMesh != null)
			this.playerMesh.SetActiveRecursively(true);
		
		else
			this.renderer.enabled = true;
		
		this.canResurrect = false;
		this.isAlive = true;
	}
	
	private void AbleResurrection()
	{
		this.canResurrect = true;
	}
	
	protected override void Kill()
	{
		this.isAlive = false;
		this.playerBody.BodyType = BodyType.Static;
		this.playerBody.Enabled = false;
		Invoke("AbleResurrection", 2f);
		// TODO
		if(playerMesh != null)
			this.playerMesh.SetActiveRecursively(false);
		else
			this.renderer.enabled =false;
		
		GameObject platforms = GameObject.FindGameObjectWithTag("PlatForms");
		GameObject interruptors = GameObject.FindGameObjectWithTag("Interruptors");
		
		FollowRoad[] roads = platforms.GetComponentsInChildren<FollowRoad>();
		Interruptor[] buttons = interruptors.GetComponentsInChildren<Interruptor>();
		
		foreach(FollowRoad road in roads)
		{
			road.reloadRoad();
		}
		
		foreach(Interruptor button in buttons)
		{
			//button.reloadInterruptor();
		}
	}
	
	private void CollisionHead(GameObject ceiling)
	{
		/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}
		*/
	}
	
	private void StayHead(GameObject ceiling)
	{	/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}
		*/
		if (this.angle > 0)
		{
			this.angle = 180;
		}
		if (this.grabTarget != Vector3.zero)
		{
			this.grabTarget = Vector3.zero;
			this.canMove = true;
		}
	}
	
	private void ExitHead(GameObject ceiling)
	{
		/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = false;
			this.playerBody.IgnoreGravity = false;
		}
		*/
	}
}