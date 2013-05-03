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
		if (Vector3.Distance(it.transform.position, GlobalVarScript.instance.playerGameObject.transform.position) > this.playerDist)
		{
			PatrolState patrol = it.GetComponent<EnemyScript>().patrol;
			this.machine.SwitchState(patrol);
			return;
		}

		if (((GlobalVarScript.instance.playerGameObject.transform.position.x - it.transform.position.x) > 0) != (it.transform.right.x > 0))
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
	public float		speed;
	public Transform	target;

	public override void EnterState (GameObject it)
	{
		GlobalVarScript.instance.cameraTarget = this.target;
	}

	public override void UpdateState (GameObject it)
	{
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

	void Start()
	{
		this.ray = new Ray(this.transform.position, this.transform.right + Vector3.down);
		RaycastHit hit = new RaycastHit();
		Physics.Raycast(this.ray, out hit);
		this.floorDist = hit.distance;

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

		this.idle = null;

		this.SwitchState(this.patrol);
	}

	public bool CanMove()
	{
		this.ray.origin = this.transform.position;
		this.ray.direction = this.transform.right - this.transform.up;
		RaycastHit hit = new RaycastHit();

		return (Physics.Raycast(this.ray, out hit, 20.0f, LayerMask.NameToLayer("World")) && Mathf.Approximately(hit.distance, this.floorDist));
	}
	
	void OnActivate()
	{
		this.isControlled = !this.isControlled;
		State nextState = (this.isControlled ? this.controlled : this.idle);
		this.SwitchState(nextState);
	}
}
