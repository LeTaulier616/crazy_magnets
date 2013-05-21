using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CameraTriggerScript : MonoBehaviour
{
	[HideInInspector]
	public List<Vector3> lp;
	[HideInInspector]
	public List<float> ls;
	private Transform target;
	private int cameraIndex = 0;
	private bool asc = true;
	private bool activated = false;
	public bool deactivateControls = false;
	public bool activeOnce = false;
	public bool loopBack = false;
	
	void Start ()
	{
		foreach (Transform t in transform)
		{
			this.target = t;
			break;
		}
		
		if (this.lp != null)
		{
			this.lp.Insert(0, transform.position);
		}
		if (this.ls != null)
		{
			this.ls.Insert(0, 1f / GlobalVarScript.instance.cameraSmoothDefault);
		}
	}
	
	void Update ()
	{
		
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			this.activated = true;
			this.cameraIndex = 0;
			GlobalVarScript.instance.cameraFree = (this.deactivateControls == true ? 2 : 1);
			target.position = lp[cameraIndex];
			GlobalVarScript.instance.cameraTarget = target;
		}
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.transform.tag == "Player" && this.activated)
		{
			target.position = lp[cameraIndex];
			GlobalVarScript.instance.cameraSmooth = ls[asc ? cameraIndex : cameraIndex + 1];
			
			if (Vector3.Distance(Camera.main.transform.position, target.position) < 0.1f + Mathf.Abs(Camera.main.transform.position.z))
			{
				cameraIndex += asc ? 1 : -1;
				if (asc == false && this.loopBack == false)
				{
					cameraIndex = -1;
				}
				if (cameraIndex == lp.Count - 1)
				{
					asc = !asc;
					this.ls.Add(this.ls[this.ls.Count - 1]);
				}
				if (cameraIndex == -1)
				{
					this.activated = false;
					GlobalVarScript.instance.cameraSmooth = GlobalVarScript.instance.cameraSmoothDefault;
					Desactivate();
				}
			}
		}
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.transform.tag == "Player")
		{
			Desactivate();
		}
	}
	
	private void Desactivate()
	{
		this.activated = false;
		this.cameraIndex = 0;
		this.asc = true;
		GlobalVarScript.instance.resetCamera(false);
		
		if (activeOnce == true)
		{
			Destroy(gameObject);
		}
	}
	
	void OnDrawGizmos()
	{
		Gizmos.color = Color.green;
		Gizmos.DrawSphere(transform.position, 0.5f);
		if (this.lp.Count <= 0) return;
		
		for (int i = 0; i < this.lp.Count; i++)
		{
			Vector3 pos = this.lp[i];
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(pos, 0.5f);
			
			if (i == 0)
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(transform.position, pos);
			}
			else
			{
				Gizmos.color = Color.white;
				Gizmos.DrawLine(this.lp[i-1], pos);
			}
		}
	}
}
