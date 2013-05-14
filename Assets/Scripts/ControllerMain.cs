using UnityEngine;GlobalVarScript.instance.maxTapTime
using System.Collections;
using System.Collections.Generic;

public class ControllerMain : MonoBehaviour
{
	private Touch screenTouch;
	private int touchCount;
	private Dictionary<int, TouchObject> touchesTab; // tableau associatif : id du touch - objet touché
	
	// infos pour le slide
	private bool slide;
	private GameObject selectedObject;
	
	private float blockRange;
	
	private TouchObject mouseObject;
	
	void Start ()
	{
		screenTouch = new Touch();
		touchCount = 0;
		touchesTab = new Dictionary<int, TouchObject>();
		this.slide = false;
		blockRange = GlobalVarScript.instance.BlockRadius;
		
		mouseObject = new TouchObject(0);
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
			
			foreach (TouchObject touchObj in this.touchesTab.Values)
			{
				Touch touch = touchObj.touch;
				
				Vector3 touchPos = Camera.mainCamera.ScreenToViewportPoint(new Vector3(touch.position.x, touch.position.y, 0.0f));

				// bords de l'écran
				if (!hasObjectSelected())
				{
					switch (touch.phase) 
					{
						case TouchPhase.Began:
							resetSlide ();
							touchObj.startPos = touch.position;
							touchObj.startTime = Time.time;
						break;
						
						case TouchPhase.Stationary:
							touchObj.startPos = touch.position;
							touchObj.slideStart = Time.time;
							touchObj.leftTouched = touchPos.x < GlobalVarScript.instance.hudLimitX;
							touchObj.rightTouched = touchPos.x > 1 - GlobalVarScript.instance.hudLimitX;
						
							if (touchObj.startPos.y > touch.position.y)
							{
								touchObj.startPos = touch.position;
							}
						break;
						
						case TouchPhase.Moved:
							touchObj.leftTouched = touchPos.x < GlobalVarScript.instance.hudLimitX;
							touchObj.rightTouched = touchPos.x > 1 - GlobalVarScript.instance.hudLimitX;
						
							if (touchObj.startPos.y > touch.position.y)
							{
								touchObj.startPos = touch.position;
							}			
						break;					
						
						case TouchPhase.Ended:
							if (touchObj.leftTouched || touchObj.rightTouched)
							{
								float tapTime = Time.time - touchObj.startTime;
								if (tapTime < GlobalVarScript.instance.maxTapTime && this.touchesTab.Count > 1)
								{
									// tap : saut
									this.SendMessage("Tap", SendMessageOptions.DontRequireReceiver);
								}
							}
							touchesToRemove.Add(touchObj);
						break;
					}
				}
				else
				{
					touchObj.leftTouched = false;
					touchObj.rightTouched = false;
				}
				
				// gestion du slide pour le saut
				if ((touchObj.leftTouched || touchObj.rightTouched) && Mathf.Abs(touch.position.x - touchObj.startPos.x) <= GlobalVarScript.instance.comfortZone) 
				{
					float swipeTime = Time.time - touchObj.slideStart;
					float swipeDist = (new Vector3(0, touch.position.y, 0) - new Vector3(0, touchObj.startPos.y, 0)).magnitude;
					
					if ((swipeTime > 0 && swipeTime < GlobalVarScript.instance.maxSwipeTime) && (swipeDist > GlobalVarScript.instance.minSwipeDist)) 
					{
						float swipeValue = Mathf.Sign(touch.position.y - touchObj.startPos.y);
						
						if (swipeValue > 0)
						{
							touchObj.slideStart = Time.time;
							touchObj.startPos = touch.position;
							this.slide = true;
						}
					}
				}

				// intérieur de l'écran : active seulement si on a le controle du joueur
				switch (touch.phase) 
				{
					case TouchPhase.Began:
						Ray ray = Camera.mainCamera.ScreenPointToRay (touch.position);
						RaycastHit hitInfo;
						if (GlobalVarScript.instance.cameraFree != 2 && Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
						{
							//Debug.Log(hitInfo.collider.gameObject.name);
							// si on pointe un objet on l'attache à l'objet touch
							if (hitInfo.transform.gameObject.tag == "Bloc" || hitInfo.transform.gameObject.tag == "Player"
								|| hitInfo.transform.gameObject.tag == "Grab" || hitInfo.transform.gameObject.tag == "Button")
							{
								touchObj.selectedObject = hitInfo.transform.gameObject;
								touchObj.selectedObject.SendMessageUpwards("SelectObject", SendMessageOptions.DontRequireReceiver);
							}
						}
					break;
					
					case TouchPhase.Stationary:
					case TouchPhase.Moved:
						if (GlobalVarScript.instance.cameraFree != 2 && touchObj.selectedObject != null)
						{
							Ray cRay = Camera.mainCamera.ScreenPointToRay (touch.position);
							RaycastHit cHitInfo;
							if (Physics.Raycast(cRay, out cHitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
							{
								// gestion de l'objet sélectionné en fonction de son tag
								/*
								if (cHitInfo.transform.gameObject.tag == "Player")
								{
									//if(Vector3.Distance(touchObj.selectedObject.transform.position, this.transform.position) < blockRange)
										//touchObj.selectedObject.SendMessageUpwards("Move", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
								}
								else if (cHitInfo.transform.gameObject.tag == "Bloc" && touchObj.selectedObject.tag == "Player")
								{
									if(Vector3.Distance(cHitInfo.transform.position, this.transform.position) < blockRange)
									{
										this.selectedObject = cHitInfo.transform.gameObject;
										//cHitInfo.transform.gameObject.SendMessageUpwards("Repulse", touchesTab[touch.fingerId].selectedObject.transform.position, SendMessageOptions.DontRequireReceiver);
									}
								}
								*/
								if (cHitInfo.transform.gameObject.tag == "Bloc" && Vector3.Distance(touchObj.selectedObject.transform.position, this.transform.position) < blockRange)
								{
									touchObj.selectedObject.SendMessageUpwards("Move", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
								}
								else if (cHitInfo.transform.gameObject.tag == "Grab")
								{
									touchObj.selectedObject = cHitInfo.transform.gameObject;
									gameObject.SendMessage("Grab", touchesTab[touch.fingerId].selectedObject.transform.position, SendMessageOptions.DontRequireReceiver);
								}
							}
						}
					break;
					
					case TouchPhase.Ended:
						if (touchObj.selectedObject != null)
						{
							touchObj.selectedObject.SendMessageUpwards("UnselectObject", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
						}
						if (this.selectedObject != null)
						{
							this.selectedObject.SendMessageUpwards("UnselectObject", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
							this.selectedObject = null;
						}
						touchesToRemove.Add(touchObj);
					break;
				}

				this.screenTouch = touch;
			}
			
			foreach (TouchObject touchObj in touchesToRemove)
			{
				resetTouch(touchObj.touch.fingerId);
			}
		}
		else
		{
			// controle souris
			if (Input.GetMouseButtonDown(0))
			{
				Ray ray = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
				RaycastHit hitInfo;
				if (GlobalVarScript.instance.cameraFree != 2 && Physics.Raycast(ray, out hitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
				{
					//Debug.Log(hitInfo.collider.gameObject.name);
					// si on pointe un objet on l'attache à l'objet touch
					if (hitInfo.transform.gameObject.tag == "Bloc" || hitInfo.transform.gameObject.tag == "Player"
						|| hitInfo.transform.gameObject.tag == "Grab" || hitInfo.transform.gameObject.tag == "Button")
					{
						mouseObject.selectedObject = hitInfo.transform.gameObject;
						mouseObject.selectedObject.SendMessageUpwards("SelectObject", SendMessageOptions.DontRequireReceiver);
					}
				}
			}
			else if (Input.GetMouseButton(0))
			{
				if (GlobalVarScript.instance.cameraFree != 2 && mouseObject != null && mouseObject.selectedObject != null)
				{
					Ray cRay = Camera.mainCamera.ScreenPointToRay (Input.mousePosition);
					RaycastHit cHitInfo;
					if (Physics.Raycast(cRay, out cHitInfo, Camera.mainCamera.far, Camera.mainCamera.cullingMask))
					{
						// gestion de l'objet sélectionné en fonction de son tag
						/*
						if (cHitInfo.transform.gameObject.tag == "Player")
						{
							if(Vector3.Distance(mouseObject.selectedObject.transform.position, this.transform.position) < blockRange)
								mouseObject.selectedObject.SendMessageUpwards("Move", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
						}
						else if (cHitInfo.transform.gameObject.tag == "Bloc" && mouseObject.selectedObject.tag == "Player")
						{
							if(Vector3.Distance(cHitInfo.transform.position, this.transform.position) < blockRange)
							{
								this.selectedObject = cHitInfo.transform.gameObject;
								cHitInfo.transform.gameObject.SendMessageUpwards("Repulse", mouseObject.selectedObject.transform.position, SendMessageOptions.DontRequireReceiver);
							}
						}
						*/
						if (cHitInfo.transform.gameObject.tag == "Bloc" && Vector3.Distance(mouseObject.selectedObject.transform.position, this.transform.position) < blockRange)
						{
							mouseObject.selectedObject.SendMessageUpwards("Move", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
						}
						else if (cHitInfo.transform.gameObject.tag == "Grab")
						{
							mouseObject.selectedObject = cHitInfo.transform.gameObject;
							gameObject.SendMessage("Grab", mouseObject.selectedObject.transform.position, SendMessageOptions.DontRequireReceiver);
						}
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				if (mouseObject.selectedObject != null)
				{
					mouseObject.selectedObject.SendMessageUpwards("UnselectObject", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
				}
				if (this.selectedObject != null)
				{
					this.selectedObject.SendMessageUpwards("UnselectObject", gameObject.transform.position, SendMessageOptions.DontRequireReceiver);
					this.selectedObject = null;
				}
				mouseObject.selectedObject = null;
				resetTouch(0);
			}
		}
	}
	
	void checkInputs()
	{
		if (this.touchCount > Input.touchCount)
		{
			foreach (Touch touch in Input.touches)
			{
				if (touch.fingerId == this.screenTouch.fingerId)
				{
					break;
				}
			}
		}
		this.touchCount = Input.touchCount;
	}
	
	void resetTouch(int fingerId)
	{
		this.screenTouch = new Touch();
		//this.leftTouched = false;
		//this.rightTouched = false;
		gameObject.SendMessage("ReleaseTouch", SendMessageOptions.DontRequireReceiver);
		if (this.touchesTab.ContainsKey(fingerId))
		{
			this.touchesTab.Remove(fingerId);
		}
	}
	
	public bool isTouched()
	{
		return Input.touchCount > 0;
	}
	
	public bool isRightTouched()
	{
		bool ret = false;
		float touchTime = 9999;
		foreach (TouchObject touchObj in this.touchesTab.Values)
		{
			if (touchObj.startTime < touchTime)
			{
				if (touchObj.rightTouched)
				{
					ret = true;
					touchTime = touchObj.startTime;
				}
				if (touchObj.leftTouched)
				{
					ret = false;
					touchTime = touchObj.startTime;
				}
			}
		}
		return ret;
	}
	
	public bool isLeftTouched()
	{
		bool ret = false;
		float touchTime = 9999;
		foreach (TouchObject touchObj in this.touchesTab.Values)
		{
			if (touchObj.startTime < touchTime)
			{
				if (touchObj.leftTouched)
				{
					ret = true;
					touchTime = touchObj.startTime;
				}
				if (touchObj.rightTouched)
				{
					ret = false;
					touchTime = touchObj.startTime;
				}
			}
		}
		return ret;
	}
	
	private bool hasObjectSelected()
	{
		bool ret = false;
		foreach (TouchObject touchObj in this.touchesTab.Values)
		{
			if (touchObj.selectedObject != null)
			{
				ret = true;
			}
		}
		return ret;
	}
	
	// DEBUG
	public void activeSlide()
	{
		this.slide = true;
	}
	
	public bool isSliding()
	{
		return this.slide;
	}
	
	public void resetSlide()
	{
		this.slide = false;
	}
}
