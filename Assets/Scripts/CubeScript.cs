using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

public class CubeScript : MonoBehaviour
{
	public Body body;
	private int selected;
	private Vector3 target;
	private Vector3 origin;
	
	private AudioClip AttractionSound;
	private AudioClip RepulsionSound;
	
	private AnimationCurve fadeInCurve;
	private AnimationCurve fadeOutCurve;
	
	private float audioTime;
	private float range;
	
	private Vector3 startPosition;
	
	private GameObject player;
	
	protected FixedMouseJoint mouseJoint;
	
	static public float MouseXWorldPhys = 0f;
	static public float MouseYWorldPhys = 0f;
	
	private float cubeForce;
	
	private Color blockRangeColor;
	private Color blockUseColor;
	
	private ParticleSystem magnetParticle;
	private ParticleSystem rangeParticle;
	
	void Start ()
	{		
		this.body = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;
		this.selected = 0;
		this.target = Vector3.zero;
		this.origin = Vector3.zero;
				
		this.AttractionSound = GlobalVarScript.instance.AttractionSound;
		this.RepulsionSound = GlobalVarScript.instance.RepulsionSound;
		
		this.fadeInCurve = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);
		this.fadeOutCurve = AnimationCurve.Linear(0.0f, 1.0f, 1.0f, 0.0f);
		
		this.audioTime = 0.0f;
		this.range = GlobalVarScript.instance.BlockRadius;
		this.cubeForce = GlobalVarScript.instance.BlockForce;
		
		this.startPosition = this.transform.position;
		
		this.blockRangeColor = GlobalVarScript.instance.BlockRangeColor;
		this.blockUseColor = GlobalVarScript.instance.BlockUseColor;
		
		this.magnetParticle = this.transform.FindChild("FX_MAGNET").GetComponent<ParticleSystem>();
		this.rangeParticle = this.transform.FindChild("FX_RANGE").GetComponent<ParticleSystem>();
		
		SendMessage("ConstantParams", blockRangeColor, SendMessageOptions.DontRequireReceiver);
		
		player = GameObject.FindGameObjectWithTag("PlayerObject");
	}
	
	void Update ()
	{
		if(Vector3.Distance(this.player.transform.position, this.transform.position) < range && selected == 0)
		{
			//SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
			
			if(!rangeParticle.isPlaying)
				rangeParticle.Play();
		}
		
		else
		{
			//SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
			if(rangeParticle.isPlaying)
				rangeParticle.Stop();
		}
		
		/*
		if (this.selected == 1 && Mathf.Abs(this.target.x - transform.position.x) > 1.2f)
		{
			int dir = transform.position.x < this.target.x ? 1 : -1;
			//this.body.LinearVelocity = new FVector2(4f * dir, this.body.LinearVelocity.Y);
			this.body.ApplyForce(new FVector2(14f * dir, 0f));
			if (dir == 1)
			{
				SendMessage("ConstantParams", Color.blue, SendMessageOptions.DontRequireReceiver);
			}
			else if (dir == -1)
			{
				SendMessage("ConstantParams", Color.green, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (this.selected == -1)
		{
			int dir = transform.position.x > this.origin.x ? 1 : -1;
			//this.body.LinearVelocity = new FVector2(4f * dir, this.body.LinearVelocity.Y);
			this.body.ApplyForce(new FVector2(14f * dir, 0f));
			if (dir == 1)
			{
				SendMessage("ConstantParams", Color.blue, SendMessageOptions.DontRequireReceiver);
			}
			else if (dir == -1)
			{
				SendMessage("ConstantParams", Color.green, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (this.body.LinearVelocity.X > 0.1f)
		{
			this.body.LinearVelocity = new FVector2(this.body.LinearVelocity.X - 0.1f, this.body.LinearVelocity.Y);
		}
		*/
		UpdateMouseWorld();
		MouseDrag();
		

		Vector3 cubePos = Camera.main.WorldToScreenPoint(this.transform.position);
		if ((cubePos.x > Camera.main.GetScreenWidth() && this.body.LinearVelocity.X > 1f)
			|| (cubePos.x < 0 && this.body.LinearVelocity.X < -1f))
		{
			FVector2 newVelocity = new FVector2(-this.body.LinearVelocity.X/3f, this.body.LinearVelocity.Y);
			this.body.LinearVelocity = newVelocity;
		}
		if (cubePos.y > Camera.main.GetScreenHeight())
		{
			FVector2 newVelocity = new FVector2(this.body.LinearVelocity.X, 0);
			this.body.LinearVelocity = newVelocity;
		}

		
		if(audio.isPlaying)
		{
			if(this.selected == 1 || this.selected == -1)
			{
				audio.volume = fadeInCurve.Evaluate(Time.time - audioTime);
			}
			
			else
			{
				audio.volume = fadeOutCurve.Evaluate(Time.time - audioTime);
				
								
				if((Time.time - audioTime) > 1.0f)
				{
					audio.Stop();
				}
			}
		}
	}
	
	void SelectObject()
	{
		this.body.GravityScale = 1f;
		if(renderer != null)
			transform.renderer.material.color = Color.green;
		
		if (this.selected == 0)
		{
			SendMessage("ConstantParams", blockUseColor, SendMessageOptions.DontRequireReceiver);
		}
		
		this.magnetParticle.Play();
		
		this.selected = 1;
	}
	
	void Move(Vector3 target)
	{
		if (this.selected != 1)
		{
			return;
		}
		this.player.SendMessage("GrabObject", this.gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
		this.target = target;
		if(!audio.isPlaying)
		{
			audioTime = Time.time;
			audio.clip = AttractionSound;
			audio.Play();
		}
	}
	
	void Repulse(Vector3 origin)
	{
		this.selected = -1;
		this.origin = origin;
		SelectObject();
		if(!audio.isPlaying)
		{
			audioTime = Time.time;
			audio.clip = RepulsionSound;
			audio.Play();
		}
		SendMessage("ConstantOn", blockUseColor, SendMessageOptions.DontRequireReceiver);
	}
	
	void UnselectObject()
	{
		this.player.SendMessage("GrabObject", Vector3.zero, SendMessageOptions.DontRequireReceiver);
		this.body.GravityScale = 2f;
		this.selected = 0;
		if(renderer != null)
			transform.renderer.material.color = Color.green;
		audioTime = Time.time;
		SendMessage("ConstantParams", blockRangeColor, SendMessageOptions.DontRequireReceiver);
		this.magnetParticle.Stop();
		GlobalVarScript.instance.player.GetComponent<ControllerMain>().ResetDrag(this.gameObject);
	}
	
	public void Attract(float distance)
	{
		float force = distance < 1f ? 15f : 10f;
		this.body.ApplyForce(new FVector2(0, force));
		SendMessage("ConstantOn", Color.blue, SendMessageOptions.DontRequireReceiver);
	}
	
	public void ResetPosition()
	{
		this.UnselectObject();
		this.body.SetTransform(new FVector2(startPosition.x, startPosition.y), 0.0f);
		this.body.ResetDynamics();
	}
	
	public void UpdateMouseWorld()
	{
		Vector3 fwd = Camera.main.transform.forward;
		Vector3 pos = Camera.main.transform.position + fwd * (-Camera.main.transform.position.z);
		// create a plane at distance, and facing the camera:
		Plane plane = new Plane(-fwd, pos);
		Ray ray; // ray to intersect the plane
		float dist = 0; // will contain the distance along the ray
		
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		plane.Raycast(ray, out dist); // find the distance of the hit point
		Vector3 wp = ray.GetPoint(dist);
		
		MouseXWorldPhys = wp.x;// -parent.position.x;
		MouseYWorldPhys = wp.y;// - parent.position.y;
	}
	
	public void MouseDrag()
	{
		// mouse press
		if(this.selected == 1 && mouseJoint == null)
		{
			FVector2 target = new FVector2(MouseXWorldPhys, MouseYWorldPhys);
			mouseJoint = JointFactory.CreateFixedMouseJoint(FSWorldComponent.PhysicsWorld, this.body, target);
			mouseJoint.CollideConnected = true;
			mouseJoint.MaxForce = this.cubeForce * this.body.Mass;
			this.body.Awake = true;
		}
		// mouse release
		if(this.selected == 0 && mouseJoint != null)
		{
			FSWorldComponent.PhysicsWorld.RemoveJoint(mouseJoint);
			mouseJoint = null;
		}
		
		// mouse move
		if(mouseJoint != null)
		{
			FVector2 p2 = new FVector2(MouseXWorldPhys, MouseYWorldPhys);
			mouseJoint.WorldAnchorB = p2;
		}
	}
}
