using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

public class PlayerScript : MonoBehaviour
{
	public AnimationCurve AccelerationCurve;
	public AnimationCurve DecelerationCurve;
	
	private float accelerationFactor;
	private float decelerationFactor;
	
	private float speed;
	private float jumpForce;
	
	private ControllerMain controllerMain;
	
	public Body      playerBody;
	
	public bool isAlive;
	private bool 	canResurrect;
	public bool      isWalking;
	public bool      isJumping;
	public bool      isFalling;
	private bool     isCharged;
	private bool	attraction;
	private float 	angle;
	private float 	localGravity;
	[HideInInspector]
	public bool		isGrabbing;
	[HideInInspector]
	public bool      onGround;
	private bool	headStucked;
	private Vector3   grabTarget;
	private UnityEngine.Transform target; // cible de la camera
	private int lastDir;
	private bool tap;
	
	//private List<Contact> lastContacts;
	
	private float     AccelerationTime;
	
	private FVector2  walkVelocity;
	[HideInInspector]
	public  bool      onPFM       = false;
	public  Body      bodyPFM     = null;
	public  bool      jumpFromPFM = false;
	public  float     pfmVelocity = 0.0f;
	
	private float dirCoeff = 0;
	private float frictionFactor;
	
	private List<Vector3> checkpoints = new List<Vector3>();
	
	private LineRenderer Line;
	
	private float grabRange;
	
	[HideInInspector]
	public GameObject playerMesh;
	
	private int checkpointIndex;
	
	public bool canJump;
	
	void Start ()
	{
		controllerMain = GetComponent<ControllerMain>() as ControllerMain;
		
		playerBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		
		playerBody.FixedRotation = true;
		playerBody.Mass = 1f;
		
		this.isAlive	= true;
		this.canResurrect = false;
		this.isWalking  = false;
		this.isJumping  = false;
		this.isFalling  = false;
		this.isCharged  = false;
		this.isGrabbing = false;
		this.onGround   = true;
		this.headStucked = false;
		this.onPFM      = false;
		this.bodyPFM    = null;
		walkVelocity    = FVector2.Zero;
		this.grabTarget = Vector3.zero;
		this.lastDir = 1;
		this.tap = false;
		this.attraction = false;
		this.angle = 0;
		this.localGravity = 1;
		
		this.target = GlobalVarScript.instance.cameraTarget;
		Line = this.GetComponent<LineRenderer>();
		
		this.accelerationFactor = GlobalVarScript.instance.accelerationFactor;
		this.decelerationFactor = GlobalVarScript.instance.decelerationFactor;
		this.speed = GlobalVarScript.instance.playerSpeed;
		this.jumpForce = GlobalVarScript.instance.playerJumpForce;
		this.playerBody.LinearDamping = GlobalVarScript.instance.playerDamping;
		
		this.frictionFactor = 1f;
		
		this.checkpoints.Add(transform.position);
		
		GetCheckpoints();
				
		//this.checkpoints.AddRange(GameObject.FindGameObjectsWithTag("Checkpoint"));
		
		this.grabRange = GlobalVarScript.instance.GrabRadius;
		
	
			
		this.playerMesh = GlobalVarScript.instance.playerMesh;
		
		if(playerMesh != null)
		{
			this.playerMesh.animation["run"].speed = 5.0f;
			this.playerMesh.animation["jump"].speed = 2.0f;
			this.playerMesh.animation["fall"].speed = 8.0f;
			this.playerMesh.animation["idle"].speed = 2.0f;
			this.playerMesh.animation.Play("idle");
		}
		
		this.checkpointIndex = 0;
		
		this.BroadcastMessage("ConstantParams", Color.cyan, SendMessageOptions.DontRequireReceiver);
		//this.BroadcastMessage("OccluderOn", SendMessageOptions.DontRequireReceiver);
		
		this.canJump = true;
	}
	
	bool keyinputed = false;
	
	void Update ()
	{	
		if (this.isAlive == false)
		{
			if (this.canResurrect && this.checkpoints.Count > 0 && (this.controllerMain.isTouched() || Input.GetKeyDown(KeyCode.Return)))
			{
				Vector3 lastCheckpoint = this.checkpoints[checkpointIndex];
				this.playerBody.Position = new FVector2(lastCheckpoint.x, lastCheckpoint.y);
				this.transform.position = lastCheckpoint;
				this.playerBody.BodyType = BodyType.Dynamic;
				this.playerBody.Enabled = true;
				this.playerBody.ResetDynamics();
				this.playerBody.Mass = 1f;
				GlobalVarScript.instance.resetCamera();
				// TODO
				if(playerMesh != null)
					this.playerMesh.SetActiveRecursively(true);
				
				else
					this.renderer.enabled = true;
				
				this.canResurrect = false;
				this.isAlive = true;
			}
			return;
		}
		
		if (GlobalVarScript.instance.cameraFree == 2)
		{
			// desactive la gestion du joueur si la camera n'est pas sur lui
			this.playerBody.ResetDynamics();
			
			if (Input.GetKeyDown(KeyCode.Return))
			{
				GlobalVarScript.instance.resetCamera();
			}
			
			playerMesh.animation.CrossFade("idle", 0.25f);
			return;
		}
		
		/* Calculer l'ensemble des véloctés à appliquer */
		int dir = (this.controllerMain.isRightTouched() ? 1 : 0) + (this.controllerMain.isLeftTouched() ? -1 : 0);
		// Gestion clavier temporaire
		if (dir == 0)
		{
			dir = (Input.GetKey(KeyCode.RightArrow) ? 1 : 0) + (Input.GetKey(KeyCode.LeftArrow) ? -1 : 0);
		}
		
		if (dir != 0 && Mathf.Abs(this.walkVelocity.X) > speed/4f)
		{
			this.lastDir = dir;
		}
		// //
		
		if (this.grabTarget == Vector3.zero)
		{
			Walk(dir);
		}
		
		/* Appliquer la bonne vélocité suivant les données récoltées */
		ApplyLinearVelocity();

		if(Input.GetKeyDown(KeyCode.Space))
		{
			this.controllerMain.activeSlide();
		}
		if(Input.GetKeyDown(KeyCode.Z) || keyinputed)
		{
			keyinputed = true;
			Walk(1);
		}
		
		if (this.controllerMain.isSliding() || this.tap)
		{
			if (this.playerBody.LinearVelocity.Y < 0)
			{
				StartCoroutine(ResetTouch());
			}
			else
			{
				this.controllerMain.resetSlide();
				this.tap = false;
			}
			
			if (this.onGround)
			{
				Jump ();
			}
		}
		
		else if (!isWalking)
		{
			this.controllerMain.resetSlide();	
		}
		
		if (this.onGround == false && this.playerBody.LinearVelocity.Y < 0)
		{			
			if(this.isJumping)
			{
				this.isJumping = false;
				this.isFalling = true;
			}
			
			// pour que le perso tombe plus vite
			this.playerBody.GravityScale = GlobalVarScript.instance.playerGravityScale;
			
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out hit, 4.5f) && GlobalVarScript.instance.groundTags.Contains(transform.tag))
			{
				Camera.main.SendMessage("ResetFall", SendMessageOptions.DontRequireReceiver);
			}
			
			if(playerMesh != null && isFalling)
			{
				playerMesh.animation.CrossFade("fall", 0.4f);
			}
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
			}
		}
		
		// orientation du joueur
		if (dir != 0 && lastDir != 0)
		{
			transform.forward = new Vector3(0, 0, (dir != 0 ? dir : lastDir));
		}
		  
		// position cible de la camera
		this.target.transform.localPosition = new Vector3(2.5f, 2.5f, 0.0f);
	}
	
	void LateUpdate()
	{
		//this.transform.localRotation = Quaternion.Euler(new Vector3(this.angle, lastDir == 1 ? 0 : 180, 0));
		if (this.attraction == true)
		{
			this.angle += Time.deltaTime * 400f;
			this.attraction = false;
			if (this.angle > 180)
			{
				this.angle = 180;
			}
			
			// gestion gravite inverse
			this.localGravity = -1f;
			this.playerBody.ApplyLinearImpulse(new FVector2(0, 9.8f * 2 * this.playerBody.Mass * this.playerBody.GravityScale * Time.deltaTime));
		}
		else
		{
			this.angle -= Time.deltaTime * 400f;
			if (this.angle < 0)
			{
				this.angle = 0;
			}
			
			// gestion gravite
			this.localGravity = 1f;
		}
		this.transform.localRotation = Quaternion.Euler(new Vector3(this.angle, lastDir == 1 ? 0 : 180, 0));
	}
	
	private void Walk(int dir)
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
			
			if(playerMesh != null && onGround)
			{
				playerMesh.animation.CrossFade("run", 0.25f);
			}
			
			else if(playerMesh != null && isJumping)
			{
				playerMesh.animation.CrossFade("jump", 0.1f);
			}
		}
		
		else
		{
			if (this.onGround)
			{
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * frictionFactor), 0);
				
				if(playerMesh != null && isFalling)
				{
					playerMesh.animation.CrossFade("idle", 0.1f);
				}
				
				else if (playerMesh != null && !isFalling && !isJumping)
				{
					playerMesh.animation.CrossFade("idle", 0.25f);
				}
			}
			
			else
			{
				// TODO ajuster le parametre
				float airControlFactor = 0.1f;
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * airControlFactor), 0);
				
				if(playerMesh != null && isJumping)
				{
					Debug.Log("There");
					playerMesh.animation.CrossFade("jump", 0.1f);
				}
			}
		}	
    }
	
	private void ApplyLinearVelocity()
	{
		playerBody.LinearVelocity = new FVector2(0.0f, this.headStucked ? 0 : playerBody.LinearVelocity.Y);
		playerBody.LinearVelocity += walkVelocity;
		//if(this.jumpFromPFM)
		//	playerBody.LinearVelocity += new FVector2(this.pfmVelocity, 0.0f);
	}
	
	private void Jump()
	{	
		if(canJump)
		{
			playerBody.LinearVelocity = new FVector2(playerBody.LinearVelocity.X, 0f);
			playerBody.ApplyLinearImpulse(new FVector2(0, jumpForce * this.localGravity));
			this.isJumping = true;
			
			if(playerMesh != null)
			{
				this.playerMesh.animation["jump"].time = 0.0f;
				this.playerMesh.animation["fall"].time = 0.0f;
			}
			
			if(this.onPFM)
			{
				this.jumpFromPFM = true;
				FollowRoad tmpfroad = (this.bodyPFM.UserData as GameObject).GetComponent<FollowRoad>();
				if(tmpfroad.back)
				{
					this.pfmVelocity = tmpfroad.roadVerso.vx / Time.deltaTime / 10.0f;
				}
				else
				{
					this.pfmVelocity = tmpfroad.roadRecto.vx / Time.deltaTime / 10.0f;
				}
			}
			
			GlobalVarScript.instance.blockCamera(Camera.main.transform.position);	
		}
	}			
	
	public void Attract(float force)
	{
		if (this.angle < 180)
		{
			playerBody.ApplyForce(new FVector2(0, force));
		}
		this.attraction = true;
	}
	
	public void Grab(Vector3 target)
	{
		if (this.grabTarget == Vector3.zero)
		{
			this.grabTarget = target;
			//this.playerBody.ResetDynamics();
		}
	}
	
	public void ReleaseTouch()
	{
		//this.grabTarget = Vector3.zero;
	}

	public bool IsCharged()
	{
		return this.isCharged;
	}
	
	public void Charge()
	{
		this.isCharged = true;
		this.BroadcastMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
	}

	public void Discharge()
	{
		Line.enabled = true;
		Invoke("DisableSpark", 0.5f);
		this.isCharged = false;
		this.BroadcastMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
	}
	
	public void SetSparkPoint(Vector3 position)
	{
		Line.SetPosition(1, transform.InverseTransformPoint(position));
	}
	
	void DisableSpark()
	{
		Line.enabled = false;
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
	
	public void ApplyPFMVelocity(float bumpForce)
	{
		playerBody.ApplyLinearImpulse(new FVector2(bumpForce, 0));
		this.onGround = false;
		this.onPFM = false;
		this.bodyPFM = null;
	}
	
	public void CheckpointReached()
	{
		this.checkpointIndex++;
		Debug.Log(checkpointIndex);
	}
	
	public void Kill()
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
	}
	
	private void AbleResurrection()
	{
		this.canResurrect = true;
	}
	
	private void CollisionGround(GameObject ground)
	{
		Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
		
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
												
			if(Application.loadedLevelName == "CM_Level_0" && !this.onGround)
			{
				if(Tutorial.instance.checkJumps)
					Tutorial.instance.jumpCount++;
			}
			
			this.onGround = true;
			
			if(this.isFalling)
			{
				this.isJumping = false;
				this.isFalling = false;
			}
			
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
			
			if (GlobalVarScript.instance.cameraTarget.GetInstanceID() == this.target.GetInstanceID() && Application.loadedLevelName != "CM_Level_0")
			{
				// reset la camera uniquement si elle est fixee au joueur
				GlobalVarScript.instance.resetCamera();
			}
		}
	}
	
	private void StayGround(GameObject ground)
	{
		Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
		
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			this.onGround = true;
			this.playerBody.GravityScale = 1f;
		}
	}
	
	private void ExitGround(GameObject ground)
	{
		this.onGround = false;
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
		if (this.grabTarget != Vector3.zero)
		{
			this.grabTarget = Vector3.zero;
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
	
	public void GrabObject(bool grab)
	{
		if (grab)
		{
			// TODO : halo de couleur sur la main
			//this.playerBody.BodyType = BodyType.Kinematic;
			this.playerBody.Mass = 100f;
		}
		else
		{
			//this.playerBody.BodyType = BodyType.Dynamic;
			this.playerBody.ResetDynamics();
			this.playerBody.Mass = 1f;
		}
	}
	
	public void Tap()
	{
		this.tap = true;
	}
	
	IEnumerator ResetTouch()
	{
	    yield return new WaitForSeconds(0.2f);
		this.controllerMain.resetSlide();
		this.tap = false;
	}
}