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
	private UnityEngine.Transform   grabTarget;
	private float grabRange;
	
	private bool canResurrect;
	private List<Vector3> checkpoints = new List<Vector3>();
	private int checkpointIndex;

	private List<GoldBoltScript> boltsToValidate = new List<GoldBoltScript>();
	
	public bool hasWon;
	
	void Start()
	{
		base.Start();

		this.playerMesh = GlobalVarScript.instance.playerMesh;
		
		if(playerMesh != null)
		{
			this.playerMesh.animation["run"].speed = 5.0f;
			this.playerMesh.animation["jump"].speed = 2.0f;
			this.playerMesh.animation["fall"].speed = 8.0f;
			this.playerMesh.animation["idle"].speed = 2.0f;
			this.playerMesh.animation["power"].speed = 4.0f;
			this.playerMesh.animation["powerLoop"].speed = 2.0f;
			this.playerMesh.animation["win"].speed = 2.0f;
			this.playerMesh.animation.Play("idle");
		}
	
		this.speed = GlobalVarScript.instance.playerSpeed;
		this.jumpForce = GlobalVarScript.instance.playerJumpForce;
		this.playerBody.LinearDamping = GlobalVarScript.instance.playerDamping;
		
		this.grabTarget = null;
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
		
		if (this.grabTarget != null)
		{
			if (Vector3.Distance(this.transform.position, grabTarget.position) < grabRange)
			{
				float dist = Vector3.Distance(transform.position, this.grabTarget.position);
				Vector3 rayTest = new Vector3(this.grabTarget.position.x - transform.position.x, this.grabTarget.position.y - transform.position.y, this.grabTarget.position.z - transform.position.z);
				RaycastHit hit;
				if (Physics.Raycast(transform.position, rayTest.normalized, out hit, dist) && hit.transform.tag != "Grab"
					// cas particulier
					&& hit.transform.name != "HEAD_HITBOX")
				{
					this.grabTarget = null;
					this.canMove = true;
					this.playerBody.IgnoreGravity = false;
				}
				else
				{
					FVector2 grabForce = new FVector2(rayTest.x, rayTest.y) * 13.0f * this.playerBody.Mass * this.playerBody.GravityScale * Time.deltaTime;
					playerBody.ApplyLinearImpulse(new FVector2(grabForce.X * 13f, grabForce.Y));
					this.grabTarget.SendMessageUpwards("PlaySound", SendMessageOptions.DontRequireReceiver);
					this.playerBody.IgnoreGravity = true;
				}
			}
			else
			{
				this.grabTarget = null;
				this.canMove = true;
				this.playerBody.IgnoreGravity = false;
			}
		}
	}
	
	void LateUpdate()
	{
		base.LateUpdate();
	}
	
	public void Grab(UnityEngine.Transform target)
	{
		this.grabTarget = target;
		this.canMove = false;
	}
	
	public void CheckpointReached()
	{
		if(checkpointIndex < checkpoints.Count - 1)
			this.checkpointIndex++;
		
		else
			this.checkpointIndex = checkpoints.Count - 1;
		
		Debug.Log("Checkpoint index : " + checkpointIndex);
		
		this.boltsToValidate.Clear();
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
		
		Vector3 currentCheckpoint = checkpoints[checkpointIndex];
		this.playerBody.Position = new FVector2(currentCheckpoint.x, currentCheckpoint.y);
	}
	
	public void GetBolt(GoldBoltScript bolt)
	{
		this.boltsToValidate.Add(bolt);
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
		// pour teleporter la camera et faire un leger dezoom
		Camera.main.SendMessage("ResetPosition", new Vector3(this.target.transform.position.x, this.target.transform.position.y, Camera.main.transform.position.z / 2f), SendMessageOptions.DontRequireReceiver);
		
		if(playerMesh != null)
			this.playerMesh.SetActiveRecursively(true);
		
		else
			this.renderer.enabled = true;
		
		this.canResurrect = false;
		this.isAlive = true;
		
		foreach(FollowRoad followroad in GameObject.Find("WORLD").GetComponentsInChildren<FollowRoad>())
		{
			followroad.reloadRoad();
		}
		foreach(FollowRoad followroad in GameObject.Find("LD").GetComponentsInChildren<FollowRoad>())
		{
			followroad.reloadRoad();
		}
		
		foreach(Interruptor interruptor in GameObject.Find("WORLD").GetComponentsInChildren<Interruptor>())
		{
			interruptor.reloadInterruptor();
		}
		foreach(Interruptor interruptor in GameObject.Find("LD").GetComponentsInChildren<Interruptor>())
		{
			interruptor.reloadInterruptor();
		}
		
		foreach (GoldBoltScript bolt in this.boltsToValidate)
		{
			bolt.Reset();
		}
	}
	
	private void AbleResurrection()
	{
		this.canResurrect = true;
	}
	
	protected override void Kill()
	{
		this.ReleaseFocus();
		this.isAlive = false;
		this.playerBody.BodyType = BodyType.Static;
		this.playerBody.Enabled = false;
		Invoke("AbleResurrection", 2f);
		// TODO
		if(playerMesh != null)
			this.playerMesh.SetActiveRecursively(false);
		else
			this.renderer.enabled =false;
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
		if (this.attraction && this.angle > 0)
		{
			this.angle = 180;
		}
		if (this.grabTarget != null)
		{
			this.grabTarget.SendMessageUpwards("StopSound", SendMessageOptions.DontRequireReceiver);
			this.grabTarget = null;
			this.canMove = true;
			this.playerBody.IgnoreGravity = false;
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