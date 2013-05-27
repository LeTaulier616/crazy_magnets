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
		
		this.enemy = it.GetComponent<EnemyScript>();
		
		if(this.enemy.enemyMesh != null)
		{
			this.enemy.enemyMesh.animation.CrossFade("run", 0.5f);
		}
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
			//it.transform.Rotate(it.transform.up, 180);
			it.GetComponent<Controllable>().lastDir *= -1;
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
		this.enemy = it.GetComponent<EnemyScript>();
		
		if(this.enemy.enemyMesh != null)
			this.enemy.enemyMesh.animation.CrossFade("chase", 0.25f);
		
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
			//it.transform.Rotate(it.transform.up, 180);
			it.GetComponent<Controllable>().lastDir *= -1;
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
				if(this.enemy.enemyMesh != null)
					this.enemy.enemyMesh.animation.CrossFade("chase", 0.5f);
				this.stopped = false;
			}
			
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(it.transform.right.x * this.speed, 0f);
		}
		else
		{
			if (this.stopped == false)
			{
				if(this.enemy.enemyMesh != null)
					this.enemy.enemyMesh.animation.CrossFade("idle", 0.5f);
				this.stopped = true;
			}
			Body body = it.GetComponent<FSBodyComponent>().PhysicsBody;
			body.LinearVelocity = new FVector2(0f, 0f);
		}
	}
}

public class AttackState : State
{
	private EnemyScript enemy;
	private GameObject player;
	
	public float hitTime;

	private float endTime;
	private float actionTime;
	private bool hitDone;

	public override void EnterState (GameObject it)
	{
		this.enemy = it.gameObject.GetComponent<EnemyScript>();
		this.player = GlobalVarScript.instance.player;
		
		if(this.enemy.enemyMesh != null)
			this.enemy.enemyMesh.animation.CrossFade("attack", 0.5f);

		this.endTime = this.enemy.enemyMesh.animation["attack"].length;
		this.actionTime = Time.time + this.hitTime;
		this.hitDone = false;
	}

	public override void UpdateState (GameObject it)
	{
		if (Time.time > this.actionTime)
		{
			if (!this.hitDone)
			{
				Ray sight = new Ray(it.transform.position, it.transform.right);
				RaycastHit hit = new RaycastHit();
		
				if (Physics.Raycast(sight, out hit, 0.7f) && hit.transform.tag == "Player" && GlobalVarScript.instance.player.GetComponent<PlayerScript>().isAlive)
				{
					this.player.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
				}
				this.actionTime += this.endTime - this.hitTime;
				this.hitDone = true;
			}
			else
				this.machine.SwitchState(this.enemy.patrol);	
		}
	}
}

public class ControlledState : State
{
	public Transform		target;

	public override void EnterState (GameObject it)
	{
		GlobalVarScript.instance.SetCameraTarget(this.target, true);
		GlobalVarScript.instance.cameraFree = 0;
		Interruptor button = it.gameObject.GetComponentInChildren<Interruptor>();
		button.activator = Interruptor.Activator.TOUCH;
		GlobalVarScript.instance.player.GetComponent<PlayerScript>().playerBody.Mass = 1.0f;
		it.GetComponent<Controllable>().isAlive = true;
	}

	public override void ExitState (GameObject it)
	{
		Interruptor button = it.gameObject.GetComponentInChildren<Interruptor>();
		button.activator = Interruptor.Activator.ELECTRIC_TOUCH;
		GlobalVarScript.instance.player.GetComponent<PlayerScript>().playerBody.Mass = 100.0f;
		GlobalVarScript.instance.resetCamera(true);
		it.GetComponent<Controllable>().isAlive = false;
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

	public GameObject	enemyMesh;
	public Transform	target;

	public Body			playerBody;

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
	private Vector3	dimension;
	
	public PatrolState		patrol;
	public PursuitState		pursuit;
	public AttackState		attack;
	public ControlledState	controlled;
	public State			idle;

	void Start()
	{
		this.playerBody = this.GetComponent<FSBodyComponent>().PhysicsBody;
		this.playerBody.Mass = 100f;
		Controllable controllable = this.GetComponent<Controllable>();
		controllable.isAlive = false;
		
		controllable.playerMesh = enemyMesh;
		
		this.patrol = new PatrolState();
		this.pursuit = new PursuitState();
		this.attack = new AttackState();
		this.idle = new State();
		
		if(this.enemyMesh != null)
		{ 
			this.enemyMesh.BroadcastMessage("OccluderOn", SendMessageOptions.DontRequireReceiver);
		}
		
		if (this.type == EnemyType.Small)
		{
			this.patrolingSpeed = GlobalVarScript.instance.smallEnemyPatrolSpeed;
			this.viewDepth = GlobalVarScript.instance.smallEnemyLocateDistance;
			this.pursuitSpeed = GlobalVarScript.instance.smallEnemyPursuitSpeed;
			this.alertRange = GlobalVarScript.instance.smallEnemyAlertRange;
			this.waitingTime = 0; // unused
			this.attack.hitTime = GlobalVarScript.instance.smallEnemyHitTime;
			controllable.speed = GlobalVarScript.instance.smallEnemySpeed;
			controllable.jumpForce = GlobalVarScript.instance.smallEnemyJumpForce;
			this.playerBody.LinearDamping = GlobalVarScript.instance.smallEnemyDamping;
			
			if(enemyMesh != null)
			{
				this.enemyMesh.animation["run"].speed = 2.0f;
				this.enemyMesh.animation["chase"].speed = 2.0f;
			}
		}
		
		else// if (this.type == EnemyType.Big)
		{
			this.patrolingSpeed = GlobalVarScript.instance.bigEnemyPatrolSpeed;
			this.viewDepth = GlobalVarScript.instance.bigEnemyLocateDistance;
			this.pursuitSpeed = GlobalVarScript.instance.bigEnemyPursuitSpeed;
			this.alertRange = GlobalVarScript.instance.bigEnemyAlertRange;
			this.waitingTime = 0; // unused
			this.attack.hitTime = GlobalVarScript.instance.bigEnemyHitTime;
			controllable.speed = GlobalVarScript.instance.bigEnemySpeed;
			controllable.jumpForce = GlobalVarScript.instance.bigEnemyJumpForce;
			this.playerBody.LinearDamping = GlobalVarScript.instance.bigEnemyDamping;
			
			if(enemyMesh != null)
			{
				this.enemyMesh.animation["run"].speed = 2.0f;
				this.enemyMesh.animation["chase"].speed = 2.0f;
				this.enemyMesh.animation["idle"].speed = 2.0f;
			}
		}

		this.controlled = new ControlledState();
		this.patrol.speed = this.patrolingSpeed;
		this.patrol.playerDist = this.viewDepth;
		this.patrol.leftWayPoint = this.leftWayPoint;
		this.patrol.rightWayPoint = this.rightWayPoint;

		this.pursuit.speed = this.pursuitSpeed;
		this.pursuit.playerDist = this.alertRange;
		
		this.controlled.target = this.target;


		FSCapsuleShape capsule = this.GetComponent<FSCapsuleShape>();
		if (capsule.direction == FSCapsuleShape.Diretion.Y)
			this.dimension = new Vector3(capsule.radius, capsule.length / 2f, 0f);
		else
			this.dimension = new Vector3(capsule.length / 2f, capsule.radius, 0f);
		Vector3 direction = new Vector3(0.1f, 0f, 0f) - this.transform.up;
		direction.Normalize();
		this.ray = new Ray(new Vector3(this.transform.position.x + Mathf.Sign(this.transform.right.x) * this.dimension.x, this.transform.position.y, this.transform.position.z), direction);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(this.ray, out hit);
		this.floorDist = hit.distance;

		this.SwitchState(this.patrol);
	}

	public bool CanMove()
	{
		this.ray.origin = new Vector3(this.transform.position.x + Mathf.Sign(this.transform.right.x) * this.dimension.x, this.ray.origin.y, this.ray.origin.z);
		if (Mathf.Sign(this.ray.direction.x) != Mathf.Sign(this.transform.right.x))
			this.ray.direction = new Vector3(-this.ray.direction.x, this.ray.direction.y, this.ray.direction.z);
		RaycastHit hit = new RaycastHit();
		bool ret = (Physics.Raycast(this.ray, out hit, 20.0f, LayerMask.NameToLayer("World")) && floatCompare(hit.distance, this.floorDist));

		Debug.DrawRay(this.ray.origin, this.ray.direction);
		//if (!ret)
			//Debug.Log("distance is: " + hit.distance + " instead of: " + this.floorDist);

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
		//if (this.curState == this.idle)
		//	return;
		this.isControlled = !this.isControlled;
		State nextState = (this.isControlled ? this.controlled : this.idle);
		this.SwitchState(nextState);
	}

	public void ResetPosition()
	{
		//Debug.Log("There");
		this.playerBody.SetTransform(new FVector2(startPosition.x, startPosition.y), 0.0f);
		this.SwitchState(this.patrol);
	}

}