using UnityEngine;
using System.Collections;

using Microsoft.Xna.Framework;

public class InterruptorReceiver : MonoBehaviour
{
	// Customisable Datas
	public GameObject[] targets;

	public bool isActivated = false;
	
	public  int InterruptorsNeeded;
	
	private int interruptorCount;
	public bool isOpen;
	
	void Start()
	{
		this.gameObject.GetComponent<FSBodyComponent>().PhysicsBody.UserData = this.gameObject;
		this.interruptorCount = 0;
		
		if(this.CompareTag("Ground") && animation != null)
		{
			this.animation["open"].speed = 2.0f;
		}
		
		if (isOpen)
		{
			if(animation != null)
			{
				animation["open"].time = animation["open"].length;
				animation.Play();
			}
			
			else
				this.gameObject.active = !isOpen;
			this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = true;
		}
	}
	
//	void Update()
	public void InputChangeState()
	{
		bool oneIsActivated = false;
		for(int trg = 0; trg < targets.Length; ++trg)
		{
			if(targets[trg].GetComponent<Interruptor>().activated && !isActivated)
			{
				oneIsActivated = true;
				OnActivate();
			}
		}
		if(!oneIsActivated && isActivated)
		{
			for(int trg = 0; trg < targets.Length; ++trg)
			{
				if(!targets[trg].GetComponent<Interruptor>().activated)
				{
					OnDesactivate();
					break;
				}
			}
		}
	}
	
	public void reloadInterruptor()
	{
		this.interruptorCount = 0;	
		this.isOpen = false;
		if(isActivated)
			OnDesactivate();
	}
	
	public void OnActivate()
	{
		interruptorCount++;

		if (gameObject.CompareTag("MultiDoor"))
		{
			if (!isActivated && interruptorCount >= InterruptorsNeeded)
				ChangeState();
		}
		else
			ChangeState();
	}
	
	public void OnDesactivate()
	{
		if (gameObject.CompareTag("MultiDoor"))
		{
			if (isActivated && interruptorCount >= InterruptorsNeeded)
				ChangeState();
		}
		else
			ChangeState();

		interruptorCount--;
	}

	private void ChangeState()
	{
		this.isActivated = !this.isActivated;
		if(this.gameObject.tag == "Platform")
		{
			this.gameObject.GetComponent<FollowRoad>().playRoad();
		}
		
		if(gameObject.CompareTag("MultiDoor") || gameObject.CompareTag("Door") || gameObject.CompareTag("Ground"))
		{
			isOpen = !isOpen;
			
			if(audio != null)
			{	
				if(gameObject.CompareTag("Door") || gameObject.CompareTag("MultiDoor"))
					audio.clip = isOpen ? GlobalVarScript.instance.DoorOpenSound : GlobalVarScript.instance.DoorCloseSound;
				
				else if(gameObject.CompareTag("Ground"))
					audio.clip = isOpen ? GlobalVarScript.instance.TrapOpenSound : GlobalVarScript.instance.TrapCloseSound;
							
				if(gameObject.CompareTag("Ground"))
					Debug.Log(audio.clip);
				
				audio.Play();
			}
			
			CubeScript[] cubes = GameObject.Find("BLOCKS").GetComponentsInChildren<CubeScript>();
			
			foreach (CubeScript cube in cubes)
			{
				if (cube == null)
					continue;
				cube.body.Awake = true;
			}
			
			GlobalVarScript.instance.player.GetComponent<PlayerScript>().onGround = false;
			
			this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = isOpen;

				
			if(animation != null)
			{
				if (gameObject.CompareTag("Door") || gameObject.CompareTag("MultiDoor"))
				{
					animation["open"].speed = (isOpen ? 1.0f : -1.0f);
					
					if (!isOpen)
						animation["open"].time = animation["open"].length;
					
					animation.Play();
				}
				
				else
					animation.Play((isOpen ? "open" : "close"));
			}
			
			else 
			{
				this.gameObject.active = !isOpen;
			}
		}

		if(gameObject.CompareTag("Enemy"))
		{
			EnemyScript enemy = gameObject.GetComponent<EnemyScript>();

			enemy.Control();
		}
		
		if(gameObject.CompareTag("Attractor"))
		{
			AttractorScript attractor = gameObject.GetComponent<AttractorScript>();
			
			attractor.enabled = !attractor.enabled;
			
			if(audio != null && !audio.isPlaying)
			{
				if(attractor.enabled)
				{
					audio.clip = GlobalVarScript.instance.AttractorOnSound;
					audio.Play();
				}
				
				else
				{
					audio.clip = GlobalVarScript.instance.AttractorOffSound;
					audio.Play();
				}
			}
		}
	}
}
