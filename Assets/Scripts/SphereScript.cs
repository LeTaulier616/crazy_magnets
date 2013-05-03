using UnityEngine;
using System.Collections;

public class SphereScript : MonoBehaviour
{
	public Material Off;
	public Material On;
	
	private bool state = false;

	void Start ()
	{
		renderer.material = Off;
	}
	
	public void OnActivate()
	{
		state = !state;
		if (state)
			renderer.material = On;
		else
			renderer.material = Off;
	}
}
