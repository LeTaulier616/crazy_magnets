using UnityEngine;
using System.Collections;

public class ButtonScript : MonoBehaviour
{
	public PlayerScript player;
	public GameObject[] links;

	private bool tapIsValid = false;
	private float radius;
		
	void Start()
	{
		radius = GlobalVarScript.instance.ButtonRadius;
	}

	private bool PlayerIsNear()
	{
		if (Vector3.Distance(this.transform.position, player.transform.position) < radius)
			return true;
		return false;
	}

	public void SelectObject()
	{
		if (PlayerIsNear())
			tapIsValid = true;
	}

	public void Move(Vector3 t)
	{
		tapIsValid = false;
	}

	public void UnselectObject()
	{
		if (tapIsValid)
		{
			foreach (GameObject link in links)
			{
				if (link != null)
				{
					link.SendMessage("OnActivate");
				}
			}
		}
		tapIsValid = false;
	}
}
