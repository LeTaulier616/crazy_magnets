using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;

public class PatrolState : State
{
	public EnemyScript enemy;
	public float speed;
	public float frontSpottingDist;
	public float backSpottingDist;

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
		Ray frontSight = new Ray(it.transform.position, it.transform.right);
		Ray backSight = new Ray(it.transform.position, -it.transform.right);
		RaycastHit hit = new RaycastHit();
		
		if (GlobalVarScript.instance.player.GetComponent<PlayerScript>().isAlive && 
			(Physics.Raycast(frontSight, out hit, this.frontSpottingDist) && hit.transform.tag == "Player" ||
			 Physics.Raycast(backSight, out hit, this.backSpottingDist) && hit.transform.tag == "Player"))
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
	public float reach;
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
		if (((GlobalVarScript.instance.player.transform.position.x - it.transform.position.x) > 0) != (it.transform.right.x > 0))
		{
			//it.transform.Rotate(it.transform.up, 180);
			it.GetComponent<Controllable>().lastDir *= -1;
		}

		float distance = Vector3.Distance(it.transform.position, GlobalVarScript.instance.player.transform.position);
		if (distance > this.playerDist)
		{
			PatrolState patrol = it.GetComponent<EnemyScript>().patrol;
			this.machine.SwitchState(patrol);
			return;
		}
		else if (distance < this.reach)
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

	public float reach;
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
		
				if (GlobalVarScript.instance.player.GetComponent<PlayerScript>().isAlive &&
					Physics.Raycast(sight, out hit, this.reach) && hit.transform.tag == "Player")
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
		Big
	}

	public EnemyType type = EnemyType.Big;

	public GameObject	leftWayPoint = null;
	public GameObject	rightWayPoint = null;

	public GameObject	enemyMesh;
	public Transform	target;

	public Body			playerBody;
	
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
		this.playerBody.OnCollision += OnCollisionEvent;
		Controllable controllable = this.GetComponent<Controllable>();
		controllable.isAlive = false;
		
		controllable.playerMesh = enemyMesh;
		
		this.patrol = new PatrolState();
		this.pursuit = new PursuitState();
		this.attack = new AttackState();
		this.controlled = new ControlledState();
		this.idle = new State();

		GlobalVarScript.EnemyInfo enemyInfo;
		if (this.type == EnemyType.Small)
		{
			enemyInfo = GlobalVarScript.instance.smallEnemy;
			
			if(enemyMesh != null)
			{
				this.enemyMesh.animation["run"].speed = 2.0f;
				this.enemyMesh.animation["chase"].speed = 2.0f;
			}
		}
		
		else// if (this.type == EnemyType.Big)
		{
			enemyInfo = GlobalVarScript.instance.bigEnemy;
			
			if(enemyMesh != null)
			{
				this.enemyMesh.animation["run"].speed = 2.0f;
				this.enemyMesh.animation["chase"].speed = 2.0f;
				this.enemyMesh.animation["idle"].speed = 2.0f;
			}
		}
		controllable.speed = enemyInfo.speed;
		controllable.jumpForce = enemyInfo.jumpForce;
		this.playerBody.LinearDamping = enemyInfo.damping;
		this.patrol.speed = enemyInfo.patrolSpeed;
		this.patrol.frontSpottingDist = enemyInfo.frontSpottingDistance;
		this.patrol.backSpottingDist = enemyInfo.backSpottingDistance;
		this.pursuit.speed = enemyInfo.pursuitSpeed;
		this.pursuit.reach = enemyInfo.reach;
		this.pursuit.playerDist = enemyInfo.alertRange;
		this.attack.reach = enemyInfo.hitDistance;
		this.attack.hitTime = enemyInfo.hitTime;

		this.patrol.leftWayPoint = this.leftWayPoint;
		this.patrol.rightWayPoint = this.rightWayPoint;
		
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
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		/*Body bodyB = fixtureB.Body;

		if(bodyB.UserTag == "PlayerObject" && this.curState != this.idle && this.curState != this.controlled)
		{
			bodyB.UserFSBodyComponent.gameObject.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
			//audio.clip = killSound;
			//audio.Play();
		}*/
		return true;
	}
}