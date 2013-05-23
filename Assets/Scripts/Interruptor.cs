using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics;

public class Interruptor : MonoBehaviour
{
	// Structures
	public enum Direction {
		NONE = 0, TOP, RIGHT, BOTTOM, LEFT
	};
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
	public Direction  pushDirection = Direction.BOTTOM;
	public float      timeToExecute = 0.0f; // Time Between the Push   and the Activation
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
	private bool      isElectrified;
	private bool      waitActiveToSetOff = false;
	
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
	
	void Start()
	{
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
					child.gameObject.GetComponent<HighlightableObject>().ConstantParams(Color.green);
				}
			}
		}
	}
	
	void FixedUpdate()
	{
		bool resultat = activated;
		
		pushTime   = ((isPushed || unpushTime < 0.1f) ? pushTime   + Time.deltaTime : pushTime  );
		unpushTime = (!isPushed                       ? unpushTime + Time.deltaTime : unpushTime);
		
		bool wasActivated = activated;
		
		if(activated && type == Type.TIMER)
		{
			if (audio2 != null)
			{
				if(unpushTime / timeToRevoke >= 0.75f)
				{
					audio2.clip = clockSound4;
				}
				
				else if(unpushTime / timeToRevoke >= 0.5f)
				{
					audio2.clip = clockSound3;
				}
				
				else if(unpushTime / timeToRevoke >= 0.25f)
				{
					audio2.clip = clockSound2;
				}
				
				else if(unpushTime / timeToRevoke >= 0.0f)
				{
					audio2.clip = clockSound1;
				}
				
				if(!audio2.isPlaying)
					audio2.Play();
			}
		}
		
		else
		{
			if(audio2 != null)
				audio2.Stop();
		}
		
		if(!activated && isPushed && pushTime >= timeToExecute)
		{
			activated = true;
		}
		
		if(!wasActivated && activated && type == Type.TIMER)
		{
			setOff();
		}
		
		if((type == Type.TIMER || type == Type.ONOFF || type == Type.STAY) && activated && !isPushed && unpushTime >= timeToRevoke+0.1f)
		{
			activated = false;
		}
		
		if(!wasActivated && activated)
		{
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
			
						
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				audio1.clip = interruptorSound;
				audio1.Play();
			}
		}
		
		else if(wasActivated && !activated)
		{
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
		
		if(resultat != activated)
		{
			Debug.Log("Push value is : " + (activated ? "Pushed" : "Unpushed"));
		}
		
		if(activated && waitActiveToSetOff)
		{
			setOff ();
			waitActiveToSetOff = false;
		}
		
		isElectrified = GlobalVarScript.instance.player.GetComponent<InterruptorPusher>().isElectrified;
		
		if (this.type != Type.STAY)
		{
			if(activator == Activator.TOUCH)
			{
				if (Vector3.Distance(GameObject.FindGameObjectWithTag("PlayerObject").transform.position, gameObject.transform.position) < tmpPorteeNorm)
				{
					if(!isPushed && type != Type.TIMER)
						SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
					
					SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
				}
				
				else
				{
					if(!isPushed)
						SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
				}
			}
			
			else if (activator == Activator.ELECTRIC_TOUCH)
			{
				if (Vector3.Distance(GameObject.FindGameObjectWithTag("PlayerObject").transform.position, gameObject.transform.position) < tmpPorteeElec)
				{
					SendMessage("ConstantParams", Color.cyan, SendMessageOptions.DontRequireReceiver);
					SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
				}
				
				else
				{
					SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
				}	
			}
		}
		
		if(wasActivated && !activated)
			launchAnimation();
	}
	
	private bool OnCollisionEvent(Fixture fixtureA, Fixture fixtureB, Contact contact)
	{
		if(activator == Activator.TOUCH || activator == Activator.ELECTRIC_TOUCH)
			return false;
		
		Body bodyB = fixtureB.Body;
		if( (bodyB.UserTag == "PlayerObject" && this.activator == Activator.PLAYER_OR_CUBE)
			|| ( (bodyB.UserData is GameObject) && (bodyB.UserData as GameObject).GetComponent<InterruptorPusher>().isPusher && this.activator == Activator.PLAYER_OR_CUBE ))
		{
			Direction tmpDirection = Direction.NONE;
			FVector2 colNorm = contact.Manifold.LocalNormal;
			
			if(Mathf.Abs(colNorm.X) > 0.1f)
			{
				if (colNorm.X > 0)
				    tmpDirection = Direction.LEFT;
				else
				    tmpDirection = Direction.RIGHT;
			}
			
			if(tmpDirection != pushDirection && Mathf.Abs(colNorm.Y) > 0.1f)
			{
				if (colNorm.Y > 0)
				    tmpDirection = Direction.BOTTOM;
				else
				    tmpDirection = Direction.TOP;
			}
			
			if(tmpDirection == pushDirection || pushDirection == Direction.NONE)
			{
				if(activated && type == Type.ONOFF)
				{
					setOff();
				}
				else if(!activated)
				{
					setOn();
				}
				
				else
				{
					pushCounter++;
				}
			}
		}
		return true;
	}
	
	
	private void OnSeparationEvent(Fixture fixtureA, Fixture fixtureB)
	{
		if(activator == Activator.TOUCH || activator == Activator.ELECTRIC_TOUCH)
			return;
		
		Body bodyB = fixtureB.Body;
		if( (bodyB.UserTag == "PlayerObject" && this.activator == Activator.PLAYER_OR_CUBE)
			|| ( (bodyB.UserData is GameObject) && (bodyB.UserData as GameObject).GetComponent<InterruptorPusher>().isPusher && this.activator == Activator.PLAYER_OR_CUBE ))
		{
			unpushTime         = 0.0f;
			if(type == Type.TIMER || type == Type.STAY)
			{
				pushCounter    = Mathf.Max(pushCounter - 1, 0);
				if(pushCounter == 0)
				{
					if(type == Type.STAY || type == Type.TIMER)
						waitActiveToSetOff = true;
					//setOff ();
				}
			}
		}
	}
	
	public void TouchTap()
	{
		if(this.activator != Activator.TOUCH && this.activator != Activator.ELECTRIC_TOUCH)
			return;
		
		if(!(this.activator == Activator.ELECTRIC_TOUCH && !isElectrified)
			&& (Vector3.Distance(GameObject.FindGameObjectWithTag("PlayerObject").transform.position, gameObject.transform.position) < (isElectrified ? tmpPorteeElec : tmpPorteeNorm) || isEnnemy) )
		{
			if(this.activator == Activator.ELECTRIC_TOUCH)
			{
				GlobalVarScript.instance.player.SendMessageUpwards("SetSparkPoint", this.transform.position, SendMessageOptions.DontRequireReceiver);
				GlobalVarScript.instance.player.SendMessageUpwards("Discharge", SendMessageOptions.DontRequireReceiver);
			}
			
			if(activated && type == Type.ONOFF)
			{
				setOff();
			}
			
			else if(!activated)
			{
				setOn();
			}
		}
	}
	
	
	public void MouseLeft()
	{
		if(Application.isEditor)
		{
			//TouchTap();
			if(this.activator != Activator.TOUCH && this.activator != Activator.ELECTRIC_TOUCH)
				return;
			
			if(!(this.activator == Activator.ELECTRIC_TOUCH && !isElectrified)
				&& (Vector3.Distance(GameObject.FindGameObjectWithTag("PlayerObject").transform.position, gameObject.transform.position) < (isElectrified ? tmpPorteeElec : tmpPorteeNorm) || isEnnemy) )
			{
				if(this.activator == Activator.ELECTRIC_TOUCH)
				{
					GlobalVarScript.instance.player.SendMessageUpwards("SetSparkPoint", this.transform.position, SendMessageOptions.DontRequireReceiver);
					GlobalVarScript.instance.player.SendMessageUpwards("Discharge", SendMessageOptions.DontRequireReceiver);
				}
				
				if(activated && type == Type.ONOFF)
				{
					setOff();
				}
				else if(!activated)
				{
					setOn();
				}
			}
		}
	}
	
	private void setOn()
	{
		pushCounter  = pushCounter + 1;
		unpushTime   = 0.0f;
		
		if(!isPushed)
			pushTime = 0.0f;
		
		isPushed     = true;
		
		if(animation != null || this.GetComponentInChildren<Animation>() != null)
		{
			if(activator == Activator.TOUCH)
			{
				animation["action"].speed = 1.0f;
				animation.Play();
			}
			
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				this.GetComponentInChildren<Animation>().animation["press"].speed = 1.0f;
				this.GetComponentInChildren<Animation>().Play();
			}
		}
		
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

		Debug.Log("Set On");
	}
	
	private void setOff()
	{
		pushCounter  = 0;
		unpushTime   = 0.0f;
		isPushed     = false;
		
		Debug.Log("Set Off");
	}
	
	public void reloadInterruptor()
	{
		if(activated)
		{
			pushCounter  = 0;
			pushTime     = timeToExecute + 0.1f;
			unpushTime   = timeToRevoke + 0.1f;
			isPushed     = false;
			activated    = false;
			waitActiveToSetOff = false;
			launchAnimation();
		}
	}
	
	private void launchAnimation()
	{		
		if(animation != null || this.GetComponentInChildren<Animation>() != null)
		{
			if(activator == Activator.TOUCH)
			{
				animation["action"].speed = -1.0f;
				animation["action"].time = animation["action"].length;
				this.GetComponentInChildren<Animation>().Play();
			}
			
			else if (activator == Activator.PLAYER_OR_CUBE)
			{
				this.GetComponentInChildren<Animation>().animation["press"].speed = -1.0f;
				this.GetComponentInChildren<Animation>().animation["press"].time = this.transform.GetComponentInChildren<Animation>().animation["press"].length;
				this.GetComponentInChildren<Animation>().Play();
			}
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
		
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);

	}
}
