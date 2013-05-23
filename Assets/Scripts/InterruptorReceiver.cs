using UnityEngine;
using System.Collections;

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
		if (isOpen)
		{
		//	isOpen = false;
			this.OnActivate();
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
		Debug.Log("PRESSCOUNT : " + interruptorCount);
		
		if (!isActivated)
		{
			if (gameObject.CompareTag("MultiDoor"))
			{
				if (interruptorCount >= InterruptorsNeeded)
					ChangeState();
			}
			else
				ChangeState();
		}
	}
	
	public void OnDesactivate()
	{
		interruptorCount--;
		Debug.Log("PRESSCOUNT : " + interruptorCount);
		
		if (isActivated)
		{
			if (gameObject.CompareTag("MultiDoor"))
			{
				if (interruptorCount < InterruptorsNeeded)
					ChangeState();
			}
			else
				ChangeState();
		}
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
			if(gameObject.CompareTag("Door") || gameObject.CompareTag("MultiDoor"))
			{	
				audio.clip = GlobalVarScript.instance.DoorOpenSound;
				audio.Play();
			}
			
			this.GetComponent<FSBodyComponent>().PhysicsBody.IsSensor = isOpen;

			if(animation != null)
			{
				animation["open"].speed = (isOpen ? 1.0f : -1.0f);
				animation.Play();
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
		}
	}
}
