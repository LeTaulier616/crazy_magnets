using UnityEngine;
using System.Collections;

public class InterruptorReceiver : MonoBehaviour
{
	// Customisable Datas
	public GameObject[] targets;

	public bool isActivated    = false;
	
	public  int InterruptorsNeeded;
	
	private int interruptorCount;
	private bool isOpen;
	
	private bool firstUse;
	
	void Start()
	{
		this.gameObject.GetComponent<FSBodyComponent>().PhysicsBody.UserData = this.gameObject;
		this.interruptorCount = 0;	
		this.isOpen = false;
		this.firstUse = false;
	}
	
	void Update()
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
		this.isActivated = false;
			
			if(firstUse)
		OnDesactivate();
	}
	
	public void OnActivate()
	{
		isActivated = true;
		
		if((!firstUse))
		{
			firstUse = true;
		}
		
		// Call the correct function or do something
		
		if(this.gameObject.tag == "Platform")
		{
			this.gameObject.GetComponent<FollowRoad>().playRoad();
		}
		
		
		if(gameObject.CompareTag("Door") || gameObject.CompareTag("Ground"))
		{
			if(gameObject.CompareTag("Door"))
			{	
				audio.clip = GlobalVarScript.instance.DoorOpenSound;
				audio.Play();
			}
			
			if(animation != null)
			{
				animation["open"].speed = 1.0f;
				animation.Play();
				this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = true;
			}
			
			else 
			{
				if(gameObject.active)
					gameObject.active = false;
				
				else
					this.gameObject.active = true;
						
				this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = !this.gameObject.active;
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
			
			if(attractor.enabled)
			{
				attractor.enabled = false;
			}
			
			else
			{
				attractor.enabled = true;
			}
		}
		
		if(gameObject.CompareTag("MultiDoor"))
		{
			interruptorCount++;
			Debug.Log("PRESSCOUNT : " + interruptorCount);
			
			if(interruptorCount >= InterruptorsNeeded)
			{
				isOpen = true;
				if(animation != null)
				{
					animation["open"].speed = 1.0f;
					animation.Play();
					this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = true;
				}
				
				else 
				{
					if(gameObject.active)
						gameObject.active = false;
					
					else
						this.gameObject.active = true;
							
					this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = !this.gameObject.active;
				}

				audio.clip = GlobalVarScript.instance.DoorOpenSound;
				audio.Play();
			}
		}
	}
	
	public void OnDesactivate()
	{
		isActivated = false;
		// Call the correct function or do something
			
		if(gameObject.CompareTag("Door") || gameObject.CompareTag("Ground"))
		{
			
			if(animation != null)
			{
					animation["open"].speed = -1.0f;
					animation["open"].time =animation["open"].length;
					animation.Play();
				
					this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = false;
			}
			
			else
			{						
				if(gameObject.active)
				{
					gameObject.active = false;
				}
				
				else
				{
					gameObject.active = true;
				}
				
				GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = !gameObject.active;
				
			}
			
			if(audio != null)
			{
				audio.clip = GlobalVarScript.instance.DoorCloseSound;
				audio.Play();
			}
		}
		
		if(gameObject.CompareTag("Attractor"))
		{
			AttractorScript attractor = gameObject.GetComponent<AttractorScript>();
			
			if(attractor.enabled)
			{
				attractor.enabled = false;
			}
			
			else
			{
				attractor.enabled = true;
			}
		}
		
		if(gameObject.CompareTag("MultiDoor"))
		{
			interruptorCount--;
			Debug.Log("PRESSCOUNT : " + interruptorCount);
			
			if(interruptorCount < InterruptorsNeeded && isOpen)
			{
				isOpen = false;
				
				if(animation != null)
				{
						animation["open"].speed = -1.0f;
						animation["open"].time =animation["open"].length;
						animation.Play();
					
						this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = false;
				}
				
				else
				{						
					if(gameObject.active)
					{
						gameObject.active = false;
					}
					
					else
					{
						gameObject.active = true;
					}
					
					GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = !gameObject.active;
					
				}
		
				if(gameObject.CompareTag("Door"))
				{
					
					audio.clip = GlobalVarScript.instance.DoorCloseSound;
					audio.Play();
				}
			}

		}

	}
}
