using UnityEngine;
using System.Collections;

public class TextureRotate : MonoBehaviour {
	
	public float RotationSpeed;
	public float angle;
	
	// Use this for initialization
	void Start () 
	{
		this.angle = 0.0f;
	}
	
	// Update is called once per frame
	void Update () 
	{
		this.angle += 1.0f * RotationSpeed;
		
		if(angle >= 360.0f)
		{
			angle = 0.0f;
		}
		
		this.renderer.material.SetFloat("_Angle", angle);
	}
}
