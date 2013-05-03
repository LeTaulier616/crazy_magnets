using UnityEngine;
using System.Collections;

public class DoorScript : MonoBehaviour
{
	public void OnActivate()
	{
		Destroy(gameObject);
	}
}
