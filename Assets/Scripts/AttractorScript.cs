using UnityEngine;
using System.Collections;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;

public class AttractorScript : MonoBehaviour
{	
	public string type = "default";
	
	public float Range;
	
	private Color startColor;
	
	private float size = 1f;
	
	private GameObject player;
	
	private GameObject particles;
	
	
	void Start ()
	{
		MeshFilter mf = GetComponentInChildren(typeof(MeshFilter)) as MeshFilter;
		Mesh mesh = mf.sharedMesh;
		Vector3 scale = transform.localScale;
		size = mesh.bounds.size.x * scale.x;
		
		player = GlobalVarScript.instance.player;
		
		if(type == "default")
		{  
			Range = GlobalVarScript.instance.GrabRadius;
			SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
		}
		
		else if(type == "attract")
		{
			SendMessage("ConstantParams", Color.red, SendMessageOptions.DontRequireReceiver);
			SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
					
			particles = this.transform.FindChild("ATTRACTOR_FX").gameObject;
			Debug.Log(particles);
			
			if(this.enabled)
			{
				particles.SetActive(true);
			}
		}
	}
	
	void Update ()
	{
		if (this.type == "attract")
		{
			RaycastHit hit;
			Vector3 pos = transform.position;
			if ((transform.position.x - size / 2f) + 1f < transform.position.x + size / 2f)
			{
				for (float newX = (transform.position.x - size / 2f) + 1f; newX < transform.position.x + size / 2f; newX += 0.5f)
				{
					pos.x = newX;
			        if (Physics.Raycast(pos, Vector3.down, out hit, Range))
					{
			            hit.collider.gameObject.SendMessageUpwards("Attract", hit.distance, SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else
			{
				pos.x = transform.position.x - 0.5f;
				if (Physics.Raycast(pos, Vector3.down, out hit, Range))
				{
		            hit.collider.gameObject.SendMessageUpwards("Attract", hit.distance, SendMessageOptions.DontRequireReceiver);
				}
				pos.x = transform.position.x + 0.5f;
				if (Physics.Raycast(pos, Vector3.down, out hit, Range))
				{
		            hit.collider.gameObject.SendMessageUpwards("Attract", hit.distance, SendMessageOptions.DontRequireReceiver);
				}
			}
		}
		
		else if(this.type == "default")
		{
			if(Vector3.Distance(this.transform.position, player.transform.position) < Range)
			{
				SendMessage("ConstantOn", SendMessageOptions.DontRequireReceiver);
			}
			
			else
			{
				SendMessage("ConstantOff", SendMessageOptions.DontRequireReceiver);
			}
		}
	}
	
	void SelectObject()
	{
		if(renderer != null)
			transform.renderer.material.color = Color.red;
		SendMessage("ConstantParams", Color.red, SendMessageOptions.DontRequireReceiver);
	}
	
	void UnselectObject()
	{
		SendMessage("ConstantParams", Color.white, SendMessageOptions.DontRequireReceiver);
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.red;
		Gizmos.DrawLine(new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z), new Vector3(this.transform.position.x, this.transform.position.y - Range, this.transform.position.z));
	}
	
	void OnEnable()
	{
		if(type == "attract" && particles != null)
		{
			particles.SetActive(true);
		}
	}
	
	void OnDisable()
	{
		if(type == "attract" && particles != null)
		{
			particles.SetActive(false);
		}
	}
}
