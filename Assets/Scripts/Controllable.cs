using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;

public class WalkState : State
{
	public Controllable player;
	public ControllerMain controllerMain;

	public override void EnterState (GameObject it)
	{
		this.player.onGround = true;
	}

	public override void UpdateState (GameObject it)
	{
		if (this.controllerMain.isSliding() || Input.GetKeyDown(KeyCode.Space))
		{
			this.controllerMain.resetSlide();
			this.machine.SwitchState(this.player.jump);
			return;
		}

		this.player.Walk(this.player.GetDir());
	}

	public override void ExitState (GameObject it)
	{
		this.player.onGround = false;
	}
}

public class JumpState : State
{
	public Controllable	player;
	public float		jumpForce;
	public Body			playerBody;

	public override void EnterState (GameObject it)
	{
		playerBody.LinearVelocity = new FVector2(this.playerBody.LinearVelocity.X, 0f);
		playerBody.ApplyLinearImpulse(new FVector2(0, this.jumpForce));
		GlobalVarScript.instance.blockCamera(Camera.main.transform.position);
	}

	public override void UpdateState (GameObject it)
	{
		if (playerBody.LinearVelocity.Y < 0)
		{
			this.machine.SwitchState(this.player.fall);
			return;
		}

		this.player.Walk(this.player.GetDir());
	}
}

public class FallState : State
{
	public Controllable	player;
	public Body			playerBody;

	public override void EnterState (GameObject it)
	{
		this.playerBody.GravityScale = 2f;
	}
	
	public override void UpdateState (GameObject it)
	{
		this.player.Walk(this.player.GetDir());
	}

	public override void ExitState (GameObject it)
	{
		this.playerBody.GravityScale = 1f;
	}
}

public class AttractState : FallState
{
	public float attractionForce;

	public override void EnterState (GameObject it)
	{
		this.player.transform.Rotate(Vector3.right, 180f);
		//this.playerBody.Mass = -this.playerBody.Mass;
		this.playerBody.ApplyForce(new FVector2(0,this.attractionForce));
		base.EnterState (it);
	}

	public override void UpdateState (GameObject it)
	{
		this.playerBody.ApplyForce(new FVector2(0,this.attractionForce));
		base.UpdateState (it);
	}

	public override void ExitState (GameObject it)
	{
		this.player.transform.Rotate(Vector3.right, 180f);
		//this.playerBody.Mass = -this.playerBody.Mass;
		base.ExitState (it);
	}
}

public class GrabState : State
{
	public Controllable	player;
	public Body			playerBody;
	public Vector3		grabTarget;

	public override void UpdateState (GameObject it)
	{
		float dist = Vector3.Distance(it.transform.position, this.grabTarget);
		Vector3 rayTest = new Vector3(this.grabTarget.x - it.transform.position.x, this.grabTarget.y - it.transform.position.y, this.grabTarget.z - it.transform.position.z);
		rayTest = Vector3.Normalize(rayTest);
		RaycastHit hit;
		if (Physics.Raycast(it.transform.position, rayTest, out hit, dist) && hit.transform.tag != "Grab")
		{
			// objet en travers : ne rien faire
			this.machine.SwitchState(this.player.fall);
		}
		else
		{
			if (Vector3.Distance(it.transform.position, this.grabTarget) < 2f)
				this.machine.SwitchState(this.player.fall);
			FVector2 grabForce = new FVector2(rayTest.x, rayTest.y);
			playerBody.ApplyLinearImpulse(new FVector2(grabForce.X, grabForce.Y));
		}
	}

	public override void ExitState (GameObject it)
	{
		this.grabTarget = Vector3.zero;
	}
}

public class MagnetState : State
{
	public Controllable player;
}

public class DeadState : State
{
	public Controllable 	player;
	public Body				playerBody;
	public ControllerMain	controllerMain;

	public override void EnterState (GameObject it)
	{
		this.playerBody.BodyType = BodyType.Static;
		// TODO
		it.GetComponent<MeshRenderer>().enabled = false;
	}

	public override void UpdateState (GameObject it)
	{
		if (this.controllerMain.isTouched() || Input.GetKeyDown(KeyCode.Return))
			this.machine.SwitchState(this.player.fall);
	}

	public override void ExitState (GameObject it)
	{
		Vector3 lastCheckpoint = this.player.checkpoints[this.player.checkpoints.Count - 1];
		this.playerBody.Position = new FVector2(lastCheckpoint.x, lastCheckpoint.y);
		it.transform.position = lastCheckpoint;
		this.playerBody.BodyType = BodyType.Dynamic;
		this.playerBody.ResetDynamics();
		this.playerBody.Mass = 1f;
		GlobalVarScript.instance.resetCamera();
		// TODO
		it.GetComponent<MeshRenderer>().enabled = true;
	}
}

public class Controllable : StateMachine
{
	public AnimationCurve accelerationCurve;
	public AnimationCurve decelerationCurve;
	
	private float	accelerationFactor;
	private float	decelerationFactor;

	private float			speed;
	private float			jumpForce;
	private ControllerMain	controllerMain;
	public Body				playerBody;

	private Transform	target;

	public WalkState	walk;
	public JumpState	jump;
	public JumpState	bump;
	public FallState	fall;
	public AttractState	attract;
	public GrabState	grab;
	public DeadState	dead;
	public State		idle;

	private bool		isWalking;
	private FVector2	walkVelocity;
	private float		accelerationTime = 0;
	private float		dirCoeff = 0;
	private float		frictionFactor;

	public List<Vector3> checkpoints = new List<Vector3>();

	public bool	onGround { get; set; }
	public bool	onPFM { get; set; }
	public Body	bodyPFM;

	// todo change to chargable
	public bool	isCharged { get; private set; }
	LineRenderer Line;
	
	void Start()
	{
		this.accelerationFactor = GlobalVarScript.instance.accelerationFactor;
		this.decelerationFactor = GlobalVarScript.instance.decelerationFactor;

		this.speed = GlobalVarScript.instance.playerSpeed;
		this.jumpForce = GlobalVarScript.instance.playerJumpForce;
		this.controllerMain = GetComponent<ControllerMain>() as ControllerMain;
		this.playerBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		playerBody.FixedRotation = true;
		playerBody.Mass = 1f;
		this.checkpoints.Add(this.transform.position);
		
		this.isWalking = false;
		this.walkVelocity = new FVector2(0, 0);

		this.onGround = false;
		this.onPFM = false;

		this.target = GlobalVarScript.instance.cameraTarget;
		Line = GetComponent<LineRenderer>();

		this.walk = new WalkState();
		this.walk.player = this;
		this.walk.controllerMain = controllerMain;

		this.jump = new JumpState();
		this.jump.player = this;
		this.jump.jumpForce = this.jumpForce;
		this.jump.playerBody = playerBody;

		this.bump = new JumpState();
		this.bump.player = this;
		this.bump.playerBody = playerBody;

		this.fall = new FallState();
		this.fall.player = this;
		this.fall.playerBody = playerBody;

		this.attract = new AttractState();
		this.attract.player = this;
		this.attract.playerBody = playerBody;

		this.grab = new GrabState();
		this.grab.player = this;
		this.grab.playerBody = playerBody;

		this.dead = new DeadState();
		this.dead.player = this;
		this.dead.playerBody = playerBody;
		this.dead.controllerMain = this.controllerMain;

		this.idle = null;

		this.SwitchState(this.fall);
	}

	public int GetDir()
	{
		int dir = (this.controllerMain.isRightTouched() ? 1 : 0) + (this.controllerMain.isLeftTouched() ? -1 : 0);
		if (dir == 0)
			dir = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
		return dir;
	}
	
	public void Walk(int dir)
    {
		if (this.isWalking != (dir != 0))
		{
			this.isWalking = dir != 0;
			this.accelerationTime = Time.time;
		}
		
		// gestion d'un coeff de dir pour pas que le joueur se retourne instantanement quand on change de direction
		if (this.dirCoeff < dir)
		{
			this.dirCoeff += Time.deltaTime * 10f * this.frictionFactor;
			this.dirCoeff = this.dirCoeff < 1 ? this.dirCoeff : 1;
		}
		else if (dirCoeff > dir)
		{
			this.dirCoeff -= Time.deltaTime * 10f * this.frictionFactor;
			this.dirCoeff = this.dirCoeff > -1 ? this.dirCoeff : -1;
		}
	
		if (this.isWalking)
		{
			float speedX = this.speed * this.accelerationCurve.Evaluate((Time.time - this.accelerationTime) * this.accelerationFactor) * this.dirCoeff;
			this.walkVelocity = new FVector2(speedX, 0);
		}
		else
		{
			if (this.curState == this.walk)
			{
				this.walkVelocity = new FVector2(this.playerBody.LinearVelocity.X * this.decelerationCurve.Evaluate((Time.time - this.accelerationTime) * this.decelerationFactor * this.frictionFactor), 0);
			}
			else if (this.curState == this.jump
				||	 this.curState == this.fall
				||	 this.curState == this.attract)
			{
				// TODO ajuster le parametre
				float airControlFactor = 0.1f;
				this.walkVelocity = new FVector2(this.playerBody.LinearVelocity.X * this.decelerationCurve.Evaluate((Time.time - this.accelerationTime) * this.decelerationFactor * airControlFactor), 0);
			}
		}	
		this.playerBody.LinearVelocity = new FVector2(walkVelocity.X, this.playerBody.LinearVelocity.Y);
    }

	public void Move(Vector2 dir)
	{
		
	}
	
	public void CheckpointReached(Vector3 pos)
	{
		Debug.Log("checkpoint Reached");
		this.checkpoints.Add(pos);
	}

	public void Kill()
	{
		this.SwitchState(this.dead);
	}

	public void Attract(float force)
	{
		playerBody.ApplyForce(new FVector2(0, force));
	}
	
	public void Grab(Vector3 target)
	{
		if (this.grab.grabTarget == Vector3.zero)
		{
			this.grab.grabTarget = target;
			this.SwitchState(this.grab);
		}
	}
	
	public void SetSparkPoint(Vector3 position)
	{
		Line.SetPosition(1, transform.InverseTransformPoint(position));
	}
	
	public void DisableSpark()
	{
		Line.enabled = false;
	}

	public bool IsCharged()
	{
		return this.isCharged;
	}

	public void Charge()
	{
		this.isCharged = true;
		renderer.material.color = Color.cyan;
	}

	public void Discharge()
	{
		this.isCharged = false;
		renderer.material.color = Color.white;
		Line.enabled = true;
		Invoke("DisableSpark", 0.5f);
	}
	
	private void CollisionGround(GameObject ground)
	{	
		Debug.Log("touch ground");
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			// Gestion surface glissante
			this.frictionFactor = 1f;
			if (ground.tag == "Slippery")
			{
				this.frictionFactor = GlobalVarScript.instance.slipperyFactor;
			}
			
			if (ground.tag == "Bumper")
			{
				BumperScript bs = ground.transform.gameObject.GetComponent<BumperScript>();
				if (bs != null)
				{
					//Bump(bs.bumperForce);
					this.bump.jumpForce = bs.bumperForce;
					this.SwitchState(this.bump);
					return;
				}
			}
			
			if (GlobalVarScript.instance.cameraTarget.GetInstanceID() == this.target.GetInstanceID())
			{
				// reset la camera uniquement si elle est fixee au joueur
				GlobalVarScript.instance.resetCamera();
			}
			this.SwitchState(this.walk);
		}
	}
	
	private void StayGround(GameObject ground)
	{
		Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
	}
	
	private void ExitGround(GameObject ground)
	{
		if (this.curState == walk)
			this.SwitchState(this.fall);
	}
}