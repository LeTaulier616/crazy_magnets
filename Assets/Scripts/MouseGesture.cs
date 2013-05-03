using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MouseGesture : MonoBehaviour 
{
	Vector3 mousePosition;
	
	
	// Use this for initialization
	void Start () 
	{
	
	}
	
	// Update is called once per frame
	void Update () 
	{
		Ray ray = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
		RaycastHit hitInfo;
		
		if (Input.GetMouseButtonDown(0))
		{
			if (Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
			{
				hitInfo.transform.gameObject.SendMessage("MouseLeft",  Input.mousePosition, SendMessageOptions.DontRequireReceiver);	
			}
		}
	}
}
