using UnityEngine;
using System.Collections;

public class ChargeButtonScript : MonoBehaviour
{
	public PlayerScript player;
	public GameObject[] links;
	
	private bool tapIsValid = false;
	
	void Start()
	{
	}
/*	
	private bool PlayerIsNear()
	{
		if (Vector3.Distance(this.transform.position, player.transform.position) < radius && player.IsCharged())
			return true;
		return false;
	}

	public void SelectObject()
	{
		if (PlayerIsNear())
			tapIsValid = true;
	}
*/
	public void Move(Vector3 t)
	{
		tapIsValid = false;
	}

	public void UnselectObject()
	{
		if (tapIsValid)
		{
			foreach (GameObject link in links)
				link.SendMessage("OnActivate");
			player.SetSparkPoint(transform.position);
			player.Discharge();
		}
		tapIsValid = false;
	}
}
