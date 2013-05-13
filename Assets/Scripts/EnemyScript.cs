using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;

public class PatrolState : State
{
	public float speed;
	public float playerDist;

	public GameObject leftWayPoint = null;
	public GameObject rightWayPoint = null;

	private bool hasLeftWayPoint = false;
	private bool hasRightWayPoint = false;

	override public void EnterState(GameObject it)
	{
		if (this.leftWayPoint != null)
			this.hasLeftWayPoint = true;
		if (this.rightWayPoint != null)
			this.hasRightWayPoint = true;
	}

	override public void UpdateState(GameObject it)
	{
		Ray sight = new Ray(it.transform.position, it.transform.right);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(sight, out hit, this.playerDist, LayerMask.NameToLayer("World")) && hit.transform.tag == "PlayerObject")
		{
			PursuitState pursuit = it.GetComponent<EnemyScript>().pursuit;
			this.machine.SwitchState(pursuit);
			return;
		}

		if (!it.GetComponent<EnemyScript>().CanMove()
			|| (it.transform.right.x > 0 && this.hasRightWayPoint && this.rightWayPoint.transform.position.x - it.transform.position.x < 0)
			|| (it.transform.right.x < 0 && this.hasLeftWayPoint && this.leftWayPoint.transform.position.x - it.transform.position.x > 0))
		{
			it.GetComponent<EnemyScript>().mesh.transform.Rotate(it.transform.up, 180);
		}
		else
		{
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(it.transform.right.x * this.speed, 0);
		}
	}
}

public class PursuitState : State
{
	public float speed;
	public float playerDist;

	override public void UpdateState(GameObject it)
	{
		if (Vector3.Distance(it.transform.position, GlobalVarScript.instance.player.transform.position) > this.playerDist)
		{
			PatrolState patrol = it.GetComponent<EnemyScript>().patrol;
			this.machine.SwitchState(patrol);
			return;
		}

		if (((GlobalVarScript.instance.player.transform.position.x - it.transform.position.x) > 0) != (it.transform.right.x > 0))
		{
			it.GetComponent<EnemyScript>().mesh.transform.Rotate(it.transform.up, 180);
		}

		if (it.GetComponent<EnemyScript>().CanMove())
		{
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(it.transform.right.x * this.speed, 0f);
		}
		else
		{
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(0f, 0f);
		}
	}
}

public class ControlledState : State
{
	public float			speed;
	public Transform		target;
	public EnemyScript 		enemy;
	private bool			keyinputed;

	public override void EnterState (GameObject it)
	{
	//	Body body = it.gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
	//	body.BodyType = BodyType.Dynamic;
		GlobalVarScript.instance.cameraTarget = this.target;
		GlobalVarScript.instance.cameraFree = 2;
		Interruptor button = it.gameObject.GetComponentInChildren<Interruptor>();
		button.activator = Interruptor.Activator.TOUCH;
		KillzoneScript killer = it.gameObject.GetComponent<KillzoneScript>();
		killer.Disable();
		
		this.enemy = it.gameObject.GetComponent<EnemyScript>();
		keyinputed = false;
	}

	public override void UpdateState (GameObject it)
	{
		int dir = (this.enemy.controllerMain.isRightTouched() ? 1 : 0) + (this.enemy.controllerMain.isLeftTouched() ? -1 : 0);
		// Gestion clavier temporaire
		if (dir == 0)
		{
			dir = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
		}
		
		if (dir != 0 && Mathf.Abs(this.enemy.walkVelocity.X) > 2f)
		{
			this.enemy.lastDir = dir;
		}
		
		this.enemy.Walk(dir);

		this.enemy.ApplyLinearVelocity();

		if(this.enemy.onGround && (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Space)))
		{
			this.enemy.Jump();
		}
		if(Input.GetKeyDown(KeyCode.Z) || keyinputed)
		{
			keyinputed = true;
			this.enemy.Walk(1);
		}
		
		if (this.enemy.controllerMain.isSliding())
		{
			this.enemy.controllerMain.resetSlide();
			if (this.enemy.onGround)
			{
				this.enemy.Jump();
			}
		}
		
		else if (!this.enemy.isWalking)
		{
			this.enemy.controllerMain.resetSlide();	
		}
		
		if (this.enemy.onGround == false && this.enemy.playerBody.LinearVelocity.Y < 0)
		{			
			// pour que le perso tombe plus vite
			this.enemy.playerBody.GravityScale = GlobalVarScript.instance.playerGravityScale;
			
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(it.transform.position.x, it.transform.position.y, it.transform.position.z), Vector3.down, out hit, 4.5f) && GlobalVarScript.instance.groundTags.Contains(it.transform.tag))
			{
				Camera.main.SendMessage("ResetFall", SendMessageOptions.DontRequireReceiver);
			}
		}
		
			// orientation du joueur
		if (dir != 0 && this.enemy.lastDir != 0)
		{
			this.enemy.transform.forward = new Vector3(0, 0, (dir != 0 ? dir : this.enemy.lastDir));
		}
			  
			// position cible de la camera
		this.target.transform.localPosition = new Vector3(2.5f, 2.5f, 0.0f);
	}

	public override void ExitState (GameObject it)
	{
		GlobalVarScript.instance.resetCamera();
	}
}

public class EnemyScript : StateMachine
{
	public float	patrolingSpeed = 1;
	public float	viewDepth = 2;
	public float	pursuitSpeed = 3;
	public float	alertRange = 4;
	public float	waitingTime = 1;

	public GameObject	leftWayPoint = null;
	public GameObject	rightWayPoint = null;

	public GameObject	mesh;
	public Transform	target;

	private bool	isControlled = false;
	private Ray		ray;
	private float	floorDist;

	public PatrolState		patrol;
	public PursuitState		pursuit;
	public ControlledState	controlled;
	public State			idle;

	// control values

	public AnimationCurve AccelerationCurve;
	public AnimationCurve DecelerationCurve;
	
	public float accelerationFactor;
	public float decelerationFactor;
	
	public ControllerMain controllerMain;
	public float	speed;
	public float	jumpForce;
	public Body		playerBody;
	private bool	headStucked;
	public bool		isWalking;
	public bool		onGround;
	public int		lastDir = 0;
	
	public float dirCoeff = 0;
	public float frictionFactor;
	
	public float     AccelerationTime;
	
	public FVector2  walkVelocity;
	public  bool      onPFM   = false;
	public  Body      bodyPFM = null;

	private GameObject playerMesh;

	void Start()
	{
		playerBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		playerBody.FixedRotation = true;
		playerBody.Mass = 1f;
		
		this.isWalking  = false;
		this.onGround   = true;
		this.headStucked = false;
		this.onPFM      = false;
		this.bodyPFM    = null;
		this.walkVelocity    = FVector2.Zero;

		this.frictionFactor = 1f;

		this.controllerMain = GlobalVarScript.instance.player.GetComponent<ControllerMain>();

		this.accelerationFactor = GlobalVarScript.instance.accelerationFactor;
		this.decelerationFactor = GlobalVarScript.instance.decelerationFactor;
		this.speed = GlobalVarScript.instance.playerSpeed;
		this.jumpForce = GlobalVarScript.instance.playerJumpForce;

		//this.playerMesh = GlobalVarScript.instance.playerMesh;
		this.playerMesh = null;

		// end of control values

		this.patrol = new PatrolState();
		this.patrol.speed = this.patrolingSpeed;
		this.patrol.playerDist = this.viewDepth;
		this.patrol.leftWayPoint = this.leftWayPoint;
		this.patrol.rightWayPoint = this.rightWayPoint;

		this.pursuit = new PursuitState();
		this.pursuit.speed = this.pursuitSpeed;
		this.pursuit.playerDist = this.alertRange;

		this.controlled = new ControlledState();
		this.controlled.speed = this.patrolingSpeed;
		this.controlled.target = this.target;

		this.idle = new State();
		
		this.ray = new Ray(this.transform.position, this.transform.right + Vector3.down);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(this.ray, out hit);
		this.floorDist = hit.distance;

		this.SwitchState(this.patrol);
	}

	public bool CanMove()
	{
		this.ray.origin = this.transform.position;
		this.ray.direction = this.transform.right - this.transform.up;
		RaycastHit hit = new RaycastHit();
		bool ret = (Physics.Raycast(this.ray, out hit, 20.0f, LayerMask.NameToLayer("World")) && Mathf.Approximately(hit.distance, this.floorDist));

		return ret;
	}
	
	public void Control()
	{
		if (this.curState == this.idle)
			return;
		this.isControlled = !this.isControlled;
		State nextState = (this.isControlled ? this.controlled : this.idle);
		this.SwitchState(nextState);
	}

	public void Walk(int dir)
    {
		if(isWalking != (dir != 0))
		{
			this.isWalking = dir != 0;
			AccelerationTime = Time.time;
		}
		
		// gestion d'un coeff de dir pour pas que le joueur se retourne instantanement quand on change de direction
		if (dirCoeff < dir)
		{
			dirCoeff += Time.deltaTime * 10f * frictionFactor;
			dirCoeff = dirCoeff < 1 ? dirCoeff : 1;
		}
		else if (dirCoeff > dir)
		{
			dirCoeff -= Time.deltaTime * 10f * frictionFactor;
			dirCoeff = dirCoeff > -1 ? dirCoeff : -1;
		}
	
		if (isWalking)
		{
			float speedX = speed * AccelerationCurve.Evaluate((Time.time - AccelerationTime) * accelerationFactor) * dirCoeff;
			walkVelocity = new FVector2(speedX, 0);
			
			if(playerMesh != null)
				playerMesh.animation.CrossFade("run", 0.5f);
		}
		else
		{
			if (this.onGround)
			{
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * frictionFactor), 0);
				if(playerMesh != null)
					playerMesh.animation.CrossFade("idle", 0.5f);
			}
			
			else
			{
				// TODO ajuster le parametre
				float airControlFactor = 0.1f;
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * airControlFactor), 0);
			}
		}	
    }
	
	public void ApplyLinearVelocity()
	{
		playerBody.LinearVelocity = new FVector2(0.0f, this.headStucked ? 0 : playerBody.LinearVelocity.Y);
		playerBody.LinearVelocity += walkVelocity;
	}
	
	public void Jump()
	{
		playerBody.LinearVelocity = new FVector2(playerBody.LinearVelocity.X, 0f);
		playerBody.ApplyLinearImpulse(new FVector2(0, jumpForce));
		this.onGround = false;
		this.onPFM = false;
		this.bodyPFM = null;
		GlobalVarScript.instance.blockCamera(Camera.main.transform.position);
	}
	
	public void Bump(float bumpForce)
	{
		playerBody.LinearVelocity = new FVector2(playerBody.LinearVelocity.X, 0);
		playerBody.ApplyLinearImpulse(new FVector2(0, bumpForce));
		this.onGround = false;
		this.onPFM = false;
		this.bodyPFM = null;
		//GlobalVarScript.instance.blockCamera(Camera.main.transform.position);
	}
	
	public void CollisionGround(GameObject ground)
	{
		//Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
		
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			this.onGround = true;
			this.playerBody.GravityScale = 1f;
			
			// Gestion surface glissante
			this.frictionFactor = 1f;
						
			if (ground.tag == "Slippery" || ground.tag == "Slippery")
			{
				this.frictionFactor = GlobalVarScript.instance.slipperyFactor;
			}
			
			if (ground.tag == "Bumper")
			{
				BumperScript bs = ground.transform.gameObject.GetComponent<BumperScript>();
				if (bs != null)
				{
					Bump(bs.bumperForce);
					bs.PlayAnimation();
				}
			}
		}
	}
	
	public void StayGround(GameObject ground)
	{
		//Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
		
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			this.onGround = true;
			this.playerBody.GravityScale = 1f;
		}
	}
	
	public void ExitGround(GameObject ground)
	{
		this.onGround = false;
	}
	
	public void CollisionHead(GameObject ceiling)
	{
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}
	}
	
	public void StayHead(GameObject ceiling)
	{
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}
	}
	
	public void ExitHead(GameObject ceiling)
	{
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = false;
			this.playerBody.IgnoreGravity = false;
		}
	}
}
