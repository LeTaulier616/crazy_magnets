using UnityEngine;
using System.Collections;

public class AttractorZoneScript : MonoBehaviour
{
	public float force = -1f;
	
	void Start ()
	{
	
	}
	
	void Update ()
	{
	
	}
	
	void OnTriggerEnter(Collider other)
	{
		Debug.Log ("trigger");
		if (other != null)
		{
			other.gameObject.SendMessage("Attract", this.force, SendMessageOptions.DontRequireReceiver);
		}
	}
}
