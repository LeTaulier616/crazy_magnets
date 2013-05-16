using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;

public class PatrolState : State
{
	public EnemyScript enemy;
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
		GameObject playerMesh = it.GetComponent<EnemyScript>().playerMesh;
		
		if(playerMesh != null)
			playerMesh.animation.CrossFade("patrol", 0.5f);
	}

	override public void UpdateState(GameObject it)
	{
		Ray sight = new Ray(it.transform.position, it.transform.right);
		RaycastHit hit = new RaycastHit();
		if (Physics.Raycast(sight, out hit, this.playerDist) && hit.transform.tag == "Player" && GlobalVarScript.instance.player.GetComponent<PlayerScript>().isAlive)
		{
			PursuitState pursuit = it.GetComponent<EnemyScript>().pursuit;
			this.machine.SwitchState(pursuit);
			return;
		}

		if (!it.GetComponent<EnemyScript>().CanMove()
			|| (it.transform.right.x > 0 && this.hasRightWayPoint && this.rightWayPoint.transform.position.x - it.transform.position.x < 0)
			|| (it.transform.right.x < 0 && this.hasLeftWayPoint && this.leftWayPoint.transform.position.x - it.transform.position.x > 0))
		{
			it.transform.Rotate(it.transform.up, 180);
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
	public EnemyScript enemy;
	public float speed;
	public float playerDist;
	public bool	 stopped;

	public override void EnterState (GameObject it)
	{
		GameObject playerMesh = it.GetComponent<EnemyScript>().playerMesh;
		if(playerMesh != null)
			playerMesh.animation.CrossFade("patrol", 0.5f);
		this.stopped = false;
	}

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
			it.transform.Rotate(it.transform.up, 180);
		}

		if (Vector3.Distance(GlobalVarScript.instance.player.transform.position, it.transform.position) < 2f)
		{
			AttackState attack = it.GetComponent<EnemyScript>().attack;
			this.machine.SwitchState(attack);
		}

		if (it.GetComponent<EnemyScript>().CanMove())
		{
			if (this.stopped == true)
			{
				GameObject playerMesh = it.GetComponent<EnemyScript>().playerMesh;
				if(playerMesh != null)
					playerMesh.animation.CrossFade("patrol", 0.5f);
				this.stopped = false;
			}
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(it.transform.right.x * this.speed, 0f);
		}
		else
		{
			if (this.stopped == false)
			{
				GameObject playerMesh = it.GetComponent<EnemyScript>().playerMesh;
		//		if(playerMesh != null)
		//			playerMesh.animation.CrossFade("idle", 0.5f);
				this.stopped = true;
			}
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(0f, 0f);
		}
	}
}

public class AttackState : State
{
	public EnemyScript enemy;
	public GameObject player;

	public override void EnterState (GameObject it)
	{
		this.enemy = it.gameObject.GetComponent<EnemyScript>();
		this.player = GlobalVarScript.instance.player;
		GameObject playerMesh = it.GetComponent<EnemyScript>().playerMesh;
//		if(playerMesh != null)
//			playerMesh.animation.CrossFade("attack", 0.5f);
	}

	public override void UpdateState (GameObject it)
	{
		//wait animation
		if (Vector3.Distance(it.transform.position, this.player.transform.position) < 3f)
		{
			this.player.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
		}
		this.machine.SwitchState(this.enemy.patrol);
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
			if (this.enemy.type == EnemyScript.EnemyType.Small)
				this.enemy.playerBody.GravityScale = GlobalVarScript.instance.smallEnemyGravityScale;
			else //if (this.enemy.type == EnemyScript.EnemyType.Big)
				this.enemy.playerBody.GravityScale = GlobalVarScript.instance.bigEnemyGravityScale;
			
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

	public override void LateUpdateState(GameObject it)
	{
		//this.transform.localRotation = Quaternion.Euler(new Vector3(this.angle, lastDir == 1 ? 0 : 180, 0));
		if (this.enemy.attraction == true)
		{
			this.enemy.angle += Time.deltaTime * 400f;
			this.enemy.attraction = false;
			if (this.enemy.angle > 180)
			{
				this.enemy.angle = 180;
			}
			
			// gestion gravite inverse
			this.enemy.localGravity = -1f;
			this.enemy.playerBody.ApplyForce(new FVector2(0, 9.8f * this.enemy.playerBody.Mass * this.enemy.playerBody.GravityScale));
		}
		else
		{
			this.enemy.angle -= Time.deltaTime * 400f;
			if (this.enemy.angle < 0)
			{
				this.enemy.angle = 0;
			}
			
			// gestion gravite
			this.enemy.localGravity = 1f;
		}
		this.enemy.transform.localRotation = Quaternion.Euler(new Vector3(this.enemy.angle, this.enemy.lastDir == 1 ? 0 : 180, 0));
	}


	public override void ExitState (GameObject it)
	{
		GlobalVarScript.instance.resetCamera();
	}
}

public class EnemyScript : StateMachine
{

	public enum EnemyType
	{
		Small,
		Big,
		Whatever
	}

	public EnemyType type = EnemyType.Big;

	public GameObject	leftWayPoint = null;
	public GameObject	rightWayPoint = null;

	public GameObject	playerMesh;
	public Transform	target;

	[HideInInspector]
	public float	patrolingSpeed = 1;
	[HideInInspector]
	public float	viewDepth = 2;
	[HideInInspector]
	public float	pursuitSpeed = 3;
	[HideInInspector]
	public float	alertRange = 4;
	[HideInInspector]
	public float	waitingTime = 1;
	
	private bool	isControlled = false;
	private Ray		ray;
	private float	floorDist;
	private Vector3	startPosition;
	
	public PatrolState		patrol;
	public PursuitState		pursuit;
	public AttackState		attack;
	public ControlledState	controlled;
	public State			idle;

	// control values
	
	[HideInInspector]
	public AnimationCurve AccelerationCurve;
	[HideInInspector]
	public AnimationCurve DecelerationCurve;
	
	[HideInInspector]
	public float accelerationFactor;
	[HideInInspector]
	public float decelerationFactor;
	
	[HideInInspector]
	public ControllerMain controllerMain;
	[HideInInspector]
	public float	speed;
	[HideInInspector]
	public float	jumpForce;
	public Body		playerBody;
	private bool	headStucked;
	[HideInInspector]
	public bool		isWalking;
	[HideInInspector]
	public bool	attraction;
	[HideInInspector]
	public float	angle;
	[HideInInspector]
	public float	localGravity;
	[HideInInspector]
	public bool		onGround;
	[HideInInspector]
	public int		lastDir = 0;
	
	[HideInInspector]
	public float dirCoeff = 0;
	[HideInInspector]
	public float frictionFactor;
	
	[HideInInspector]
	public float     AccelerationTime;
	
	[HideInInspector]
	public FVector2  walkVelocity;
	[HideInInspector]
	public  bool      onPFM   = false;
	public  Body      bodyPFM = null;

	void Start()
	{
		playerBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		playerBody.FixedRotation = true;
		playerBody.Mass = 1f;

		this.startPosition = this.transform.position;
		
		this.isWalking  = false;
		this.attraction = false;
		this.angle = 0;
		this.localGravity = 1;
		this.onGround   = true;
		this.headStucked = false;
		this.onPFM      = false;
		this.bodyPFM    = null;
		this.walkVelocity    = FVector2.Zero;

		this.frictionFactor = 1f;

		this.controllerMain = GlobalVarScript.instance.player.GetComponent<ControllerMain>();

		if (this.type == EnemyType.Small)
		{
			this.patrolingSpeed = GlobalVarScript.instance.smallEnemyPatrolSpeed;
			this.viewDepth = GlobalVarScript.instance.smallEnemyLocateDistance;
			this.pursuitSpeed = GlobalVarScript.instance.smallEnemyPursuitSpeed;
			this.alertRange = GlobalVarScript.instance.smallEnemyAlertRange;
			this.waitingTime = 0; // unused
			this.speed = GlobalVarScript.instance.smallEnemySpeed;
			this.jumpForce = GlobalVarScript.instance.smallEnemyJumpForce;
			this.playerBody.LinearDamping = GlobalVarScript.instance.smallEnemyDamping;
		}
		else// if (this.type == EnemyType.Big)
		{
			this.patrolingSpeed = GlobalVarScript.instance.bigEnemyPatrolSpeed;
			this.viewDepth = GlobalVarScript.instance.bigEnemyLocateDistance;
			this.pursuitSpeed = GlobalVarScript.instance.bigEnemyPursuitSpeed;
			this.alertRange = GlobalVarScript.instance.bigEnemyAlertRange;
			this.waitingTime = 0; // unused
			this.speed = GlobalVarScript.instance.bigEnemySpeed;
			this.jumpForce = GlobalVarScript.instance.bigEnemyJumpForce;
			this.playerBody.LinearDamping = GlobalVarScript.instance.bigEnemyDamping;
		}
		this.accelerationFactor = GlobalVarScript.instance.accelerationFactor;
		this.decelerationFactor = GlobalVarScript.instance.decelerationFactor;

		//this.playerMesh = GlobalVarScript.instance.playerMesh;
		//this.playerMesh = null;
/*
		if(playerMesh != null)
		{
			this.playerMesh.animation["patrol"].speed = 1.5f;
			this.playerMesh.animation.Play("idle");
		}
*/
		// end of control values

		this.patrol = new PatrolState();
		this.patrol.speed = this.patrolingSpeed;
		this.patrol.playerDist = this.viewDepth;
		this.patrol.leftWayPoint = this.leftWayPoint;
		this.patrol.rightWayPoint = this.rightWayPoint;

		this.pursuit = new PursuitState();
		this.pursuit.speed = this.pursuitSpeed;
		this.pursuit.playerDist = this.alertRange;
		
		this.attack = new AttackState();
		
		this.controlled = new ControlledState();
		this.controlled.speed = this.patrolingSpeed;
		this.controlled.target = this.target;

		this.idle = new State();

		FSCapsuleShape capsule = this.GetComponent<FSCapsuleShape>();
		Vector3 size;
		if (capsule.direction == FSCapsuleShape.Diretion.Y)
			size = new Vector3(2f * capsule.radius, capsule.length, 0f);
		else
			size = new Vector3(capsule.length, 2f * capsule.radius, 0f);
		Vector3 direction = this.transform.right * size.x - this.transform.up * size.y;
		direction.Normalize();
		this.ray = new Ray(this.transform.position + new Vector3(0f, 0.1f, .0f), direction);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(this.ray, out hit);
		this.floorDist = hit.distance;

		this.SwitchState(this.patrol);
	}

	public bool CanMove()
	{
		this.ray.origin = new Vector3(this.transform.position.x, this.ray.origin.y, this.ray.origin.z);
		if (Mathf.Sign(this.ray.direction.x) != Mathf.Sign(this.transform.right.x))
			this.ray.direction = new Vector3(-this.ray.direction.x, this.ray.direction.y, this.ray.direction.z);
		RaycastHit hit = new RaycastHit();
		bool ret = (Physics.Raycast(this.ray, out hit, 20.0f, LayerMask.NameToLayer("World")) && floatCompare(hit.distance, this.floorDist));

		Debug.DrawRay(this.ray.origin, this.ray.direction);
		if (!ret)
			Debug.Log("distance is: " + hit.distance + " instead of: " + this.floorDist);

		return ret;
	}

	private bool floatCompare(float a, float b)
	{
		a = Mathf.Floor(a * 1000);
		b = Mathf.Floor(b * 1000);
		//return (Mathf.Abs(a - b) < Mathf.Epsilon);
		return Mathf.Approximately(a, b);
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
				playerMesh.animation.CrossFade("patrol", 0.5f);
		}
		else
		{
			if (this.onGround)
			{
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * frictionFactor), 0);
			//	if(playerMesh != null)
			//		playerMesh.animation.CrossFade("idle", 0.5f);
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
		playerBody.ApplyLinearImpulse(new FVector2(0, jumpForce * this.localGravity));
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
	
	public void ResetPosition()
	{
		//Debug.Log("There");
		this.playerBody.SetTransform(new FVector2(startPosition.x, startPosition.y), 0.0f);
		KillzoneScript killer = this.GetComponent<KillzoneScript>();
		killer.Enable();
		this.SwitchState(this.patrol);
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
	{/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}*/
	}
	
	public void StayHead(GameObject ceiling)
	{/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = true;
			this.playerBody.IgnoreGravity = true;
		}*/
	}
	
	public void ExitHead(GameObject ceiling)
	{/*
		if (ceiling.transform.tag == "Attractor")
		{
			this.headStucked = false;
			this.playerBody.IgnoreGravity = false;
		}*/
	}

	public void Attract(float force)
	{
		if (this.angle < 180)
		{
			playerBody.ApplyForce(new FVector2(0, force));
		}
		this.attraction = true;
	}
}
