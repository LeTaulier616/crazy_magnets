using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

[ExecuteInEditMode]
public class Road : MonoBehaviour 
{
	// Don't draw Gizmos if false
	public bool mainRoad = true;
	
	// Transform array
	public List<Transform> lp = new List<Transform>();
	public Transform[]      p;
	
	// Road Datas
	public bool  fixedSize  = false;
	public float pathSize   = 10.0f;
    public bool  loop       = false; // Si la plateforme boucle à l'infini ou pas
	public float speed      = 1.0f; // Vitesse de déplacemnt de a plateforme
	public bool  resetAtEnd = false;
	
	// Type d'activation
    public bool playerActivation,
		 interuptorActivation,
		 activable,
		 activated;
	
	// Type de trajectoire
    public EndBehaviour endBehaviour = EndBehaviour.LOOP;
	public Activation activating     = Activation.AUTO;
    public Trajectoire trajectoire   = Trajectoire.LINE;
	public Deplacement deplacement   = Deplacement.SMOOTH;
	
	// Type de trajectoire
    public List<Trajectoire> trajectoires;
    public List<PathType>    pathTypes;
    public List<float>       waits;
    public List<KeyType>     keyTypes;
    public List<string>      functionNames;
    public List<KeyPoint>    pathWays;
	
	private AudioClip EndSound;
	
	void OnEnable()
	{
		if(this.lp.Count <= 0) return;
		Gen ();
	}
	
	void Start()
	{
		if(this.lp.Count <= 0) return;
		Gen();
	}
	
	public void Gen()
	{
		if(this.lp.Count <= 1)
			return;
		this.p = this.lp.ToArray();
	}

	void OnDrawGizmos()
	{
		if(!this.mainRoad)
			return;
	
		if(this.lp.Count <= 0) return;
		foreach(Transform t in this.p)
		{
			Gizmos.color = Color.red;
			Gizmos.DrawCube(t.position, new Vector3(1.5f, 1.5f, 1.5f));
			Gizmos.color = Color.white;
		}
		
		for(int i = 0; i < this.lp.Count-1; ++i)
		{
			if(this.trajectoires[i] == Trajectoire.LINE)
				Gizmos.DrawLine(this.lp[i].position, this.lp[i+1].position);
			else if(this.pathTypes[i] == PathType.CIRCLELTOT || this.pathTypes[i] == PathType.CIRCLELTOB
				|| this.pathTypes[i] == PathType.CIRCLERTOB || this.pathTypes[i] == PathType.CIRCLERTOT)
			{
				Vector3 startGizmo = this.lp[i].position;
				Vector3 endGizmo   = new Vector3(
					this.lp[i].position.x + 1.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 3.5f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 2.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 5.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 2.5f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 5.5f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 3.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 6.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 4.5f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 7.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 8.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 8.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
			}
			else if(this.pathTypes[i] == PathType.CIRCLEBTOR || this.pathTypes[i] == PathType.CIRCLEBTOL
				|| this.pathTypes[i] == PathType.CIRCLETTOR || this.pathTypes[i] == PathType.CIRCLETTOL)
			{
				Vector3 startGizmo = this.lp[i].position;
				Vector3 endGizmo   = new Vector3(
					this.lp[i].position.x + 3.5f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 1.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 5.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 2.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 5.5f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 2.5f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 6.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 3.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 7.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 4.5f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
				startGizmo = endGizmo;
				endGizmo = new Vector3(
					this.lp[i].position.x + 8.0f*(this.lp[i+1].position.x - this.lp[i].position.x)/8.0f,
					this.lp[i].position.y + 8.0f*(this.lp[i+1].position.y - this.lp[i].position.y)/8.0f,
					this.lp[i].position.z
				);
				Gizmos.DrawLine(startGizmo, endGizmo);
			}
		}
	}
	
	public void PlayEndSound()
	{
		if(audio!= null && !audio.isPlaying)
		{
			this.audio.clip = EndSound;
			audio.Play();
		}
	}
}
