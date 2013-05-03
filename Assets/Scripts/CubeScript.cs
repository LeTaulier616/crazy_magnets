using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

public class CubeScript : MonoBehaviour
{
	private Body body;
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
		
		this.startPosition = this.transform.position;
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);

	}
	
	void Update ()
	{		
		if(Vector3.Distance(GameObject.FindGameObjectWithTag("PlayerObject").transform.position, this.transform.position) > range)
		{
			SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
		}
		
		else
		{
			SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
		}
		
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
				SendMessage("ConstantParams", Color.red, SendMessageOptions.DontRequireReceiver);
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
				SendMessage("ConstantParams", Color.red, SendMessageOptions.DontRequireReceiver);
			}
		}
		else if (this.body.LinearVelocity.X > 0.1f)
		{
			this.body.LinearVelocity = new FVector2(this.body.LinearVelocity.X - 0.1f, this.body.LinearVelocity.Y);
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
			transform.renderer.material.color = Color.red;
		if (this.selected == 0)
		{
			SendMessage("ConstantParams", Color.green, SendMessageOptions.DontRequireReceiver);
		}
	}
	
	void Move(Vector3 target)
	{
		this.selected = 1;
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
		SendMessage("ConstantOn", Color.red, SendMessageOptions.DontRequireReceiver);
	}
	
	void UnselectObject()
	{
		this.body.GravityScale = 2f;
		this.selected = 0;
		if(renderer != null)
			transform.renderer.material.color = Color.green;
		audioTime = Time.time;
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
	}
	
	public void Attract(float force)
	{
		this.body.ApplyForce(new FVector2(0, force));
		SendMessage("ConstantOn", Color.blue, SendMessageOptions.DontRequireReceiver);
	}
	
	public void ResetPosition()
	{
		Debug.Log("There");
		this.body.SetTransform(new FVector2(startPosition.x, startPosition.y), 0.0f);
	}
}
