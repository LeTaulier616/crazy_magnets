using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

public class Controllable : MonoBehaviour
{
	public AnimationCurve AccelerationCurve;
	public AnimationCurve DecelerationCurve;
	
	private float accelerationFactor;
	private float decelerationFactor;
	
	private AudioClip jumpSound;

	[HideInInspector]
	public float speed;
	[HideInInspector]
	public float jumpForce;
	
	public Body      playerBody;
	
	public bool 	 isAlive;
	public bool      isWalking;
	public bool      isJumping;
	public bool      isFalling;
	private bool     isCharged;
	private bool     isCubing;
	public bool     isBolting;
	protected bool	attraction;
	protected float 	angle;
	private float 	localGravity;
	[HideInInspector]
	public bool		isGrabbing;
	[HideInInspector]
	public bool      onGround;
	private bool	headStucked;
	protected UnityEngine.Transform target; // cible de la camera
	public int lastDir;
	private bool powerLoop;
	
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
	
	public LineRenderer line;
	private ParticleSystem chargeParticles;
	
	[HideInInspector]
	public GameObject playerMesh;
	
	public bool canMove;
	public bool canCharge;
	public bool canJump;
	
	[HideInInspector]
	public Vector3 chargePosition;
	[HideInInspector]
	public Vector3 grabPosition;
		
	protected void Start ()
	{
		playerBody = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		playerMesh = GlobalVarScript.instance.playerMesh;

		playerBody.FixedRotation = true;
		playerBody.Mass = 1f;
		playerBody.SleepingAllowed = false;
		
		this.jumpSound = GlobalVarScript.instance.JumpSound;
		
		this.isAlive	= true;
		this.isWalking  = false;
		this.isJumping  = false;
		this.isFalling  = false;
		this.isCharged  = false;
		this.isGrabbing = false;
		this.isCubing	= false;
		this.isBolting	= false;
 		this.onGround   = false;
		this.headStucked = false;
		this.onPFM      = false;
		this.bodyPFM    = null;
		walkVelocity    = FVector2.Zero;
		this.lastDir = 1;
		this.attraction = false;
		this.angle = 0;
		this.localGravity = 1;
		this.powerLoop = false;
		
		this.target = GlobalVarScript.instance.cameraTarget;
		
		this.line = this.GetComponent<LineRenderer>();
		this.chargeParticles = this.playerMesh.GetComponentInChildren<ParticleSystem>();
		
		this.accelerationFactor = GlobalVarScript.instance.accelerationFactor;
		this.decelerationFactor = GlobalVarScript.instance.decelerationFactor;
		
		this.frictionFactor = 1f;
		
		this.BroadcastMessage("OccluderOn", SendMessageOptions.DontRequireReceiver);
		
		this.canMove = true;
		this.canCharge = true;
		this.canJump = true;
		
		this.chargePosition = Vector3.zero;
	}
	
	protected void Update ()
	{			
		if (this.isAlive == false)
		{
			return;
		}
		
		/* Appliquer la bonne vélocité suivant les données récoltées */
		ApplyLinearVelocity();
		
		if (this.onGround == false && this.playerBody.LinearVelocity.Y < 0)
		{	
			
			if(this.isJumping)
			{
				this.isJumping = false;
			}
			
			this.isFalling = true;
			
			// pour que le perso tombe plus vite
			this.playerBody.GravityScale = GlobalVarScript.instance.playerGravityScale;
			/*
			RaycastHit hit;
			if (Physics.Raycast(new Vector3(transform.position.x, transform.position.y, transform.position.z), Vector3.down, out hit, 4.5f) && GlobalVarScript.instance.groundTags.Contains(hit.transform.tag))
			{
				Camera.main.SendMessage("ResetFall", SendMessageOptions.DontRequireReceiver);
			}
			*/
			
			if(playerMesh != null && isFalling)
			{
				playerMesh.animation.CrossFade("fall", 0.25f);
			}
		}
		
		if (isWalking)
		{			
			if(playerMesh != null && onGround && !isCubing)
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
				if (playerMesh != null)
				{
					if (!isCubing)
					{
						if (isFalling)
						{
							playerMesh.animation.CrossFade("idle", 0.1f);
						}
						
						else if(!isBolting)
						{
							playerMesh.animation.CrossFade("idle", 0.25f);
						}
						
						else
						{
							playerMesh.animation.Play("power");
						}
					}
					
					else
					{
						if (!powerLoop && !isBolting)
						{
							playerMesh.animation.CrossFade("power", 0.3f);
						
							if(playerMesh.animation["power"].time >= playerMesh.animation["power"].length)
							{
								SetPowerLoop();
							}
						}
						
						else
						{
							playerMesh.animation.CrossFade("powerLoop", 1.0f);
						}
					}
				}
			}
			else
			{
				if(playerMesh != null && isJumping)
				{
					playerMesh.animation.CrossFade("jump", 0.1f);
				}
			}
		}
		
		if(isGrabbing)
		{
			playerMesh.animation.CrossFade("grab", 0.1f);
			if(!this.line.enabled)
			{
				this.SendMessage("SetForGrab");
				this.line.enabled = true;
			}
			
			line.SetPosition(1, transform.InverseTransformPoint(this.grabPosition));
		}
		
		else if(!isBolting)
		{
			if(line != null)
				this.line.enabled = false;
		}
		
		// orientation du joueur
		if (lastDir != 0)
		{
			transform.forward = new Vector3(0, 0, this.lastDir);
		}
		  
		// position cible de la camera
		this.target.transform.localPosition = new Vector3(2.5f, this.angle == 0 ? 2.5f : 0f, 0.0f);
		
		if(isBolting)
		{
			line.SetPosition(1, transform.InverseTransformPoint(this.chargePosition));
		}
		
	}
	
	protected void LateUpdate()
	{
		//this.transform.localRotation = Quaternion.Euler(new Vector3(this.angle, lastDir == 1 ? 0 : 180, 0));
		if (this.attraction == true)
		{
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
		if (this.canMove == false)
		{
			return;
		}
		
		if (dir != 0 && Mathf.Abs(this.walkVelocity.X) > speed/4f)
		{
			this.lastDir = dir;
		}
		
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
		}
		else
		{
			if (this.onGround)
			{
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * frictionFactor), 0);
			}
			else
			{
				float airControlFactor = 0.1f;
				walkVelocity = new FVector2(playerBody.LinearVelocity.X * DecelerationCurve.Evaluate((Time.time - AccelerationTime) * decelerationFactor * airControlFactor), 0);
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
		if (this.canJump && this.onGround)
		{
			audio.clip = this.jumpSound;
			audio.Play();
			this.Bump(this.jumpForce);
			
			/*if(this.onPFM)
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
			}*/
			
			GlobalVarScript.instance.blockCamera(Camera.main.transform.position);
		}
	}
	
	public void Attract(float distance)
	{
		float force = distance < 1f ? 15f : 10f;
		this.angle += Time.deltaTime * 400f / (distance/2f);
		this.playerBody.GravityScale = 2f;
		playerBody.ApplyForce(new FVector2(0, force));
		if (distance < 0.5f)
		{
			this.angle = 180f;
		}
		if (this.angle < 180)
		{
			//playerMesh.animation.CrossFade("fall", 0.25f);
		}
		this.attraction = true;
	}

	public bool IsCharged()
	{
		return this.isCharged;
	}
	
	public void Charge()
	{
		if (this.canCharge)
		{
			this.isCharged = true;
			this.chargeParticles.Play();
		}
	}

	public void Discharge()
	{
		if (this.isCharged && this.onGround)
		{
			ReleaseFocus();
			
			this.SendMessage("SetForBolt");
			line.enabled = true;
			Invoke("DisableSpark", 0.5f);
			this.chargeParticles.Stop();
			this.isBolting = true;
			this.isCharged = false;

		}
	}
	
	public void SetSparkPoint(Vector3 position)
	{
		this.chargePosition = position;
	}
	
	void DisableSpark()
	{
		line.enabled = false;
		this.isBolting = false;
		this.chargePosition = Vector3.zero;
	
		Focus();
	}
	
	public void Bump(float bumpForce)
	{
		playerBody.LinearVelocity = new FVector2(playerBody.LinearVelocity.X, 0f);
		playerBody.ApplyLinearImpulse(new FVector2(0, bumpForce * this.localGravity));
		this.isJumping = true;
		
		if(playerMesh != null)
		{
			this.playerMesh.animation["jump"].time = 0.0f;
			this.playerMesh.animation["fall"].time = 0.0f;
		}
	}
	
	public void ApplyPFMVelocity(float bumpForce)
	{
		playerBody.ApplyLinearImpulse(new FVector2(bumpForce, 0));
		this.onGround = false;
		this.onPFM = false;
		this.bodyPFM = null;
	}
	
	protected virtual void Kill()
	{
		this.SendMessage("ResetPosition", SendMessageOptions.DontRequireReceiver);
	}
	
	protected virtual void CollisionGround(GameObject ground)
	{
		Camera.main.SendMessage("Reset", SendMessageOptions.DontRequireReceiver);
		
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			this.onGround = true;
			
			if(this.isFalling)
			{
				this.isJumping = false;
				this.isFalling = false;
			}
			
			this.playerBody.GravityScale = 1f;
			
			// Gestion surface glissante
			this.frictionFactor = 1f;
			
			if (ground.tag == "Bumper")
			{
				BumperScript bs = ground.transform.gameObject.GetComponent<BumperScript>();
				if (bs != null)
				{
					this.playerBody.Mass = 1f;
					Bump(bs.bumperForce);
					bs.PlayAnimation();
				}
			}
			
			if (this.canMove && GlobalVarScript.instance.cameraTarget.GetInstanceID() == this.target.GetInstanceID())
			{
				// reset la camera uniquement si elle est fixee au controllable
				GlobalVarScript.instance.resetCamera(false);
			}
		}
	}
	
	protected virtual void StayGround(GameObject ground)
	{
		if (GlobalVarScript.instance.groundTags.Contains(ground.tag))
		{
			/*if (this.canMove)
			{
				Camera.main.gameObject.SendMessageUpwards("Reset", SendMessageOptions.DontRequireReceiver);
			}*/
			
			this.onGround = true;
			this.playerBody.GravityScale = 1f;
			
			if (ground.tag == "Slippery" || ground.tag == "Slippery")
			{
				this.frictionFactor = GlobalVarScript.instance.slipperyFactor;
			}
		}
	}
	
	protected virtual void ExitGround(GameObject ground)
	{
		this.onGround = false;
	}

	protected virtual void CollisionHead(GameObject ceiling)
	{
		if (ceiling.tag == "PlayerObject")
		{
			ceiling.SendMessageUpwards("Kill", SendMessageOptions.DontRequireReceiver);
		}
	}

	protected virtual void StayHead(GameObject ceiling)
	{
	}

	protected virtual void ExitHead(GameObject ceiling)
	{
	}
	
	public void GrabObject(Vector3 grab)
	{
		if (grab != Vector3.zero)
		{
			// TODO : halo de couleur sur la main
			//this.playerBody.BodyType = BodyType.Kinematic;
			this.playerBody.Mass = 100f;
			
			if(!isCubing)
			{
				isCubing = true;
				powerLoop = false;
				this.canMove = false;
				this.canJump = false;
				this.walkVelocity = FVector2.Zero;
			}
			
			if(grab.x < this.transform.position.x)
			{
				this.lastDir = -1;
			}
			
			else
			{
				this.lastDir = 1;
			}
		}
		
		else
		{
			//this.playerBody.BodyType = BodyType.Dynamic;
			this.playerBody.ResetDynamics();
			this.playerBody.Mass = 1f;
			
			if(isCubing)
			{
				isCubing = false;
				powerLoop = false;
				this.canMove = true;
				this.canJump = true;
			}
		}
	}
	
	public virtual void Tap()
	{
		// nothing
	}
	
	public void SetPowerLoop()
	{
		if(!this.powerLoop)
		{
			this.powerLoop = true;
		}
	}
	
	public void ReleaseFocus()
	{
		// desactive la gestion du joueur si la camera n'est pas sur lui
		this.playerBody.ResetDynamics();
		this.walkVelocity = FVector2.Zero;
		this.canMove = false;
		this.canJump = false;
		this.isWalking = false;
		GlobalVarScript.instance.player.GetComponent<ControllerMain>().canMagnet = false;
		this.playerBody.Mass = 100f;
	}
	
	public void Focus()
	{
		this.playerBody.ResetDynamics();
		this.canMove = true;
		this.canJump = true;
		GlobalVarScript.instance.player.GetComponent<ControllerMain>().canMagnet = true;
		this.playerBody.Mass = 1f;
	}
}