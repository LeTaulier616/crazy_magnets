using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class Interruptor : MonoBehaviour
{
	// Structures
	public enum Activator {
		PLAYER_OR_CUBE = 0, TOUCH, ELECTRIC_TOUCH
	};
	public enum Type {
		PERMANENT = 0, ONOFF, TIMER, STAY
	};
	
	// Customisable Datas
	public GameObject[] targets;
	public Type       type          = Type.PERMANENT;
	public Activator  activator     = Activator.PLAYER_OR_CUBE;
	public float      timeToRevoke  = 0.0f; // Time between the Unpush and the Desactivation
	public bool       isEnnemy      = false;
	public  bool      activated     = false;
	
	public GameObject Pipes;
	
	// Engine Datas
	private float     pushTime      = 0.0f;
	private float     unpushTime    = 0.0f;
	[HideInInspector]
	public  bool      isPushed      = false;
	private Body      body          = null;
	private int       pushCounter   = 0;
	private float     tmpPorteeElec;
	private float     tmpPorteeNorm;
	private bool      pushedByPlayer = false;
	private bool      pushedByEnnemi = false;
	
	private AudioClip interruptorSound;
	private AudioClip interruptorReleaseSound;
	private AudioClip buttonSound;
	private AudioClip electricButtonSound;
	
	private AudioClip clockSound1;
	private AudioClip clockSound2;
	private AudioClip clockSound3;
	private AudioClip clockSound4;
	
	private AudioSource audio1;
	private AudioSource audio2;
	
	private GameObject player;
	
	private ParticleSystem rangeParticle;
	
	void Start()
	{
		player = GlobalVarScript.instance.player;
		
		activated = false;
		isPushed = false;
		
		tmpPorteeElec = GlobalVarScript.instance.ChargeButtonRadius;
		tmpPorteeNorm = GlobalVarScript.instance.ButtonRadius;
		
		unpushTime = timeToRevoke + 0.1f;

		FSBodyComponent bodyComponent = gameObject.GetComponent<FSBodyComponent>();
		
		if (bodyComponent != null)
		{
			body = gameObject.GetComponent<FSBodyComponent>().PhysicsBody;

			body.IsSensor = true;
		
			body.OnCollision  += OnCollisionEvent;
			body.OnSeparation += OnSeparationEvent;
		}
				
		interruptorSound = GlobalVarScript.instance.InterruptorSound;
		interruptorReleaseSound = GlobalVarScript.instance.InterruptorReleaseSound;
		buttonSound = GlobalVarScript.instance.ButtonSound;
		electricButtonSound = GlobalVarScript.instance.ElectricButtonSound;
		
			clockSound1 = GlobalVarScript.instance.ClockSounds[0];
			clockSound2 = GlobalVarScript.instance.ClockSounds[1];
			clockSound3 = GlobalVarScript.instance.ClockSounds[2];
			clockSound4 = GlobalVarScript.instance.ClockSounds[3];
		
		if(activator == Activator.ELECTRIC_TOUCH)
			SendMessage("ConstantParams", Color.cyan, SendMessageOptions.DontRequireReceiver);
		
		else
			SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);

		SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
		
		foreach(AudioSource source in this.GetComponents<AudioSource>())
		{
			if(source.loop)
				audio2 = source;
			
			else
				audio1 = source;
		}
		
		if(Pipes != null)
		{
			foreach(Transform child in Pipes.transform)
			{
				if(child.GetComponent<HighlightableObject>() != null)
				{
					child.gameObject.AddComponent<HighlightableObject>();
					child.gameObject.GetComponent<HighlightableObject>().ConstantParams(Color.cyan);
				}
			}
		}

		if(animation != null || this.GetComponentInChildren<Animation>() != null)
		{
			if(activator == Activator.TOUCH)
			{
				animation["action"].speed = -animation["action"].speed;
			}
			
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				this.GetComponentInChildren<Animation>().animation["press"].speed = -this.GetComponentInChildren<Animation>().animation["press"].speed;
			}
		}
		
		rangeParticle = this.GetComponentInChildren<ParticleSystem>();
	}
	
	void FixedUpdate()
	{
		if (this.type == Type.TIMER)
		{
			if (this.activated)
			{
				if (Time.time > this.unpushTime)
				{
					this.setOff();
					if (audio2 != null)
						audio2.Stop();
				}
				else if (audio2 != null)
				{
					float timeLeft = (this.unpushTime - Time.time) / timeToRevoke;

					if (timeLeft < 0.25f)
						audio2.clip = clockSound4;
					else if (timeLeft < 0.5f)
						audio2.clip = clockSound3;
					else if (timeLeft < 0.75f)
						audio2.clip = clockSound2;
					else// if (timeLeft < 1.0f)
						audio2.clip = clockSound1;

					if(!audio2.isPlaying)
						audio2.Play();
				}
			}
		}
		
		if(this.activator == Activator.ELECTRIC_TOUCH) //&& !activated)
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) <= tmpPorteeElec && player.GetComponent<PlayerScript>().IsCharged())
			{
				SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
				if(!rangeParticle.isPlaying)
					this.rangeParticle.Play();
			}
			
			else
			{
				SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
				if(!rangeParticle.isStopped)
					this.rangeParticle.Stop();
			}
		}
		
		else if(this.activator == Activator.TOUCH) //&& !activated)
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) <= tmpPorteeNorm)
			{
				SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
				if(!rangeParticle.isPlaying)
					this.rangeParticle.Play();
			}
			
			else
			{
				SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
				if(!rangeParticle.isStopped)
					this.rangeParticle.Stop();
			}
		}
	}
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if (this.activator != Activator.PLAYER_OR_CUBE)
			return false;
		
		Body bodyB = fixtureB.Body;
		if( (bodyB.UserTag == "PlayerObject")
			|| ( (bodyB.UserData is GameObject) && (bodyB.UserData as GameObject).GetComponent<InterruptorPusher>().isPusher))
		{
			if (!activated)
				setOn();
			else if (type == Type.ONOFF)
				setOff();

			pushCounter++;
			
			if(bodyB.UserTag == "PlayerObject")
				pushedByPlayer = true;
			if(bodyB.UserTag == "Enemy")
				pushedByEnnemi = true;
		}
		return true;
	}
	
	
	private void OnSeparationEvent(Fixture fixtureA, Fixture fixtureB)
	{
		if (this.activator != Activator.PLAYER_OR_CUBE)
			return;
		
		Body bodyB = fixtureB.Body;
		if( (bodyB.UserTag == "PlayerObject")
			|| ( (bodyB.UserData is GameObject) && (bodyB.UserData as GameObject).GetComponent<InterruptorPusher>().isPusher))
		{
			pushCounter = Mathf.Max(pushCounter - 1, 0);
			if (pushCounter == 0)
			{
				if(type == Type.STAY)
					this.setOff();
				else if (type == Type.TIMER)
					this.unpushTime = Time.time + this.timeToRevoke;
			}
			
			if(bodyB.UserTag == "PlayerObject")
				pushedByPlayer = false;
			if(bodyB.UserTag == "Enemy")
				pushedByEnnemi = false;
		}
	}
	
	public void TouchTap()
	{
		if (this.activator != Activator.TOUCH && this.activator != Activator.ELECTRIC_TOUCH)
			return;

		if (!(this.activator == Activator.ELECTRIC_TOUCH && !GlobalVarScript.instance.player.GetComponent<PlayerScript>().IsCharged())
			&& (Vector3.Distance(GlobalVarScript.instance.player.transform.position, gameObject.transform.position) < (this.activator == Activator.ELECTRIC_TOUCH ? tmpPorteeElec : tmpPorteeNorm) || isEnnemy) )
		{
			if(this.activator == Activator.ELECTRIC_TOUCH)
			{
				GlobalVarScript.instance.player.SendMessageUpwards("SetSparkPoint", this.transform.position, SendMessageOptions.DontRequireReceiver);
				GlobalVarScript.instance.player.SendMessageUpwards("Discharge", SendMessageOptions.DontRequireReceiver);
			}

			if (!activated)
			{
				setOn();
				if (type == Type.TIMER)
					this.unpushTime = Time.time + this.timeToRevoke;
			}
			else if (type == Type.ONOFF)
				setOff();
		}
	}
	
	
	public void MouseLeft()
	{
		if(Application.isEditor)
		{
			TouchTap();
		}
	}
	
	private void setOn()
	{
		if (activated)
			return;
		activated = true;
		//pushCounter  = pushCounter + 1;
		//unpushTime   = 0.0f;
		
		if(!isPushed)
			//pushTime = 0.0f;
			pushTime = Time.time;
		
		isPushed = true;
		
		launchAnimation();
		
		if(Pipes != null)
		{
			foreach(Transform child in Pipes.transform)
			{
				if(child.GetComponent<HighlightableObject>() != null)
				{
					child.gameObject.GetComponent<HighlightableObject>().ConstantOn();
				}
			}
		}
		
		SendMessage("ConstantParams", Color.green, SendMessageOptions.DontRequireReceiver);

		for(int trg = 0; trg < targets.Length; ++trg)
		{
			targets[trg].GetComponent<InterruptorReceiver>().OnActivate();
		}
		
		if(audio1 != null && !audio1.isPlaying)
		{
			if(activator == Activator.TOUCH)
			{
				audio1.clip = buttonSound;
				audio1.Play();
			}
			
			else if(activator == Activator.ELECTRIC_TOUCH)
			{
				audio1.clip = electricButtonSound;
				audio1.Play();
			}
						
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				audio1.clip = interruptorSound;
				audio1.Play();
			}
		}
	}
	
	private void setOff()
	{
		if (!activated)
			return;
		activated = false;
		pushCounter = 0;
		//unpushTime   = 0.0f;
		isPushed = false;

		launchAnimation();
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);

		for(int trg = 0; trg < targets.Length; ++trg)
		{
			targets[trg].GetComponent<InterruptorReceiver>().OnDesactivate();
		}
		
		if(activator == Activator.PLAYER_OR_CUBE && (type == Type.STAY || type == Type.ONOFF))
		{
			audio1.clip = interruptorReleaseSound;
			audio1.Play();
		}
		
		if(activator == Activator.TOUCH && (type == Type.ONOFF || type == Type.TIMER) && audio1 != null)
		{
			audio1.clip = buttonSound;
			audio1.Play();
		}
		
	}
	
	public void reloadInterruptor()
	{
		if(activated)
		{
			for(int trg = 0; trg < targets.Length; ++trg)
			{
				if(targets[trg].tag == "Platform")
				{
					if(!this.pushedByEnnemi)
					{
						setOff();
					}
					else
					{
						targets[trg].GetComponent<InterruptorReceiver>().OnActivate();
					}
				}
			}
			if(activator == Activator.PLAYER_OR_CUBE && activated && pushCounter > 0)
			{
				if(this.pushedByPlayer)
				{
					this.pushCounter--;
					this.pushedByPlayer = false;
					if(pushCounter == 0)
					{
						setOff();
					}
				}
			}
		}
	}
	
	private void launchAnimation()
	{
		if(animation != null || this.GetComponentInChildren<Animation>() != null)
		{
			if(activator == Activator.TOUCH)
			{
				animation["action"].speed = -animation["action"].speed;
				if (animation["action"].speed < 0f)
					animation["action"].time = animation["action"].length;
			}
			
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				this.GetComponentInChildren<Animation>().animation["press"].speed = -this.GetComponentInChildren<Animation>().animation["press"].speed;
				if (this.GetComponentInChildren<Animation>().animation["press"].speed < 0)
					this.GetComponentInChildren<Animation>().animation["press"].time = this.transform.GetComponentInChildren<Animation>().animation["press"].length;
			}
			this.GetComponentInChildren<Animation>().Play();
		}
		
		if(Pipes != null)
		{
			foreach(Transform child in Pipes.transform)
			{
				if(child.GetComponent<HighlightableObject>() != null)
				{
					child.gameObject.GetComponent<HighlightableObject>().ConstantOff();
				}
			}
		}
	}
}
