using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TouchGesture : MonoBehaviour
{
	private static Touch screenTouch;
	private static int touchCount;
	private static Dictionary<int, TouchObject> touchesTab; // tableau associatif : id du touch - objet touché
	
	void Start ()
	{
		screenTouch = new Touch();
		touchCount  = 0;
		touchesTab  = new Dictionary<int, TouchObject>();
	}
	
	void Update ()
	{		
		if (Input.touchCount > 0) 
		{
			foreach (Touch touch in Input.touches)
			{
				if (!touchesTab.ContainsKey(touch.fingerId))
				{
					// ajout du touch dans la biblio
					TouchObject touchObject = new TouchObject(touch.fingerId, touch);
					touchesTab.Add(touch.fingerId, touchObject);
				}
				else
				{
					// maj du touch dans la biblio pour update son statut et sa pos entre autre
					touchesTab[touch.fingerId].touch = touch;
				}
			}
			
			List<TouchObject> touchesToRemove = new List<TouchObject>();
			
			foreach (TouchObject touchObj in touchesTab.Values)
			{
				Touch touch = touchObj.touch;
				
				//Vector3 touchPos = Camera.mainCamera.ScreenToViewportPoint(new Vector3(touch.position.x, touch.position.y, 0.0f));
				
				Ray ray = Camera.mainCamera.ScreenPointToRay (touch.position);
				RaycastHit hitInfo;
				
				switch (touch.phase) 
				{
					case TouchPhase.Began:
						if (Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
						{
							hitInfo.collider.gameObject.SendMessage("TouchBegan", touch.position, SendMessageOptions.DontRequireReceiver);	
						}
					break;
					
					case TouchPhase.Stationary:
						if (Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
						{
							hitInfo.collider.gameObject.SendMessage("TouchStation",  touch.position, SendMessageOptions.DontRequireReceiver);	
						}
					break;
					
					case TouchPhase.Moved:
						if (Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
						{
							hitInfo.collider.gameObject.SendMessage("TouchMove",  touch.position, SendMessageOptions.DontRequireReceiver);	
						}
					break;
					
					case TouchPhase.Ended:
						if (Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
						{
							if (touch.tapCount == 1 && !hitInfo.collider.CompareTag("Untagged"))
							{
								hitInfo.collider.gameObject.SendMessage("TouchTap",  touch.position, SendMessageOptions.DontRequireReceiver);	
							}
							else if(!hitInfo.collider.CompareTag("Untagged"))
							{
								hitInfo.collider.gameObject.SendMessage("TouchEnd", touch.position, SendMessageOptions.DontRequireReceiver);
							}
						}
						touchesToRemove.Add(touchObj);
					break;
				}

				screenTouch = touch;
			}
			
			foreach (TouchObject touchObj in touchesToRemove)
			{
				resetTouch(touchObj.touch.fingerId);
			}
		}
	}
	
	static void checkInputs()
	{
		if (touchCount > Input.touchCount)
		{
			foreach (Touch touch in Input.touches)
			{
				if (touch.fingerId == screenTouch.fingerId)
				{
					break;
				}
			}
		}
		touchCount = Input.touchCount;
	}
	
	static void resetTouch(int fingerId)
	{
		screenTouch = new Touch();
		//gameObject.SendMessage("ReleaseTouch", SendMessageOptions.DontRequireReceiver);
		if (touchesTab.ContainsKey(fingerId))
		{
			touchesTab.Remove(fingerId);
		}
	}
	
	static public bool isTouched()
	{
		return Input.touchCount > 0;
	}
}
