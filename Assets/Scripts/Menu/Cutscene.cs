using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cutscene : MenuScreen {
	
	public  GameObject[] cutscene;
	public  GameObject   keysPositions;
	private bool         isSliding;
	private int          currentSlide;
	
	private Touch screenTouch;
	private int touchCount;
	private Dictionary<int, TouchObject> touchesTab;
	private Vector3 startSlidePosition;
	private Vector3 currentSlidePosition;
	private float   swipeTimer;
	
	// infos pour le slide
	private bool slide;
	
	void Start () 
	{
		// Menu
		isSliding    = false;
		currentSlide = 0;
		foreach(GameObject obj in cutscene)
			obj.transform.position = keysPositions.transform.FindChild("Right").position;
		
		// Touchs
		screenTouch = new Touch();
		touchCount = 0;
		touchesTab = new Dictionary<int, TouchObject>();
		this.slide = false;
	}
	
	void FixedUpdate () 
	{
		if(isSliding && currentSlide < cutscene.Length - 1)
		{
			cutscene[currentSlide].transform.position   = new Vector3(cutscene[currentSlide].transform.position.x-1, cutscene[currentSlide].transform.position.y, cutscene[currentSlide].transform.position.z);
			cutscene[currentSlide+1].transform.position = new Vector3(cutscene[currentSlide+1].transform.position.x-1, cutscene[currentSlide+1].transform.position.y, cutscene[currentSlide+1].transform.position.z);
		
			if(cutscene[currentSlide].transform.position.x <= keysPositions.transform.FindChild("Left").position.x)
				cutscene[currentSlide].transform.position   = keysPositions.transform.FindChild("Left").position;
			if(cutscene[currentSlide+1].transform.position.x <= keysPositions.transform.FindChild("Center").position.x)
			    cutscene[currentSlide+1].transform.position = keysPositions.transform.FindChild("Center").position;
			
			if(cutscene[currentSlide].transform.position.x == keysPositions.transform.FindChild("Left").position.x
			   && cutscene[currentSlide+1].transform.position.x == keysPositions.transform.FindChild("Center").position.x)
			{
				isSliding = false;
				currentSlide++;
			}
		}
	}
	
	public void SlideMenu()
	{
		isSliding = true;
	}
	
	public override void activateMenu()
	{
		keysPositions.transform.parent.gameObject.SetActive(true);
		exitScreen = false;
		loadLevel = false;
	}
	
	public override void desactivateMenu()
	{
		keysPositions.transform.parent.gameObject.SetActive(false);
	}
	
	void Update ()
	{
		if (Input.touchCount > 0) 
		{
			for (int i = 0; i < Input.touchCount; i++)
			{
				Touch touch = Input.GetTouch(i);
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
				
				// gestion du slide pour les cutscenes
				if (Mathf.Abs(touch.position.y - touchObj.startPos.y) <= GlobalVarScript.instance.comfortZone) 
				{
					float swipeTime = Time.time - touchObj.slideStart;
					float swipeDist = (new Vector3(touch.position.x, 0, 0) - new Vector3(touchObj.startPos.x, 0, 0)).magnitude;
					
					if ((swipeTime > 0 && swipeTime < GlobalVarScript.instance.maxSwipeTime) && (swipeDist > GlobalVarScript.instance.minSwipeDist)) 
					{
						float swipeValue = Mathf.Sign(touch.position.x - touchObj.startPos.x);
						
						if (swipeValue > 0)
						{
							touchObj.slideStart = Time.time;
							touchObj.startPos = touch.position;
							StartCoroutine(HorizontalSlide());
						}
					}
				}
				
				switch (touch.phase) 
				{
					case TouchPhase.Began:
					break;
					
					case TouchPhase.Stationary:
					case TouchPhase.Moved:
					break;
					
					case TouchPhase.Ended:
						touchesToRemove.Add(touchObj);
					break;
					
					case TouchPhase.Canceled:
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
				startSlidePosition = Input.mousePosition;
				swipeTimer = Time.time;
			}
			else if (Input.GetMouseButton(0))
			{
				currentSlidePosition = Input.mousePosition;
				
				// gestion du slide pour les cutscenes
				if (Mathf.Abs(startSlidePosition.y - currentSlidePosition.y) <= 100) 
				{
					float swipeTime = Time.time - swipeTimer;
					float swipeDist = (new Vector3(currentSlidePosition.x, 0, 0) - new Vector3(startSlidePosition.x, 0, 0)).magnitude;
					
					if (swipeTime > 0 && swipeTime < 2 && swipeDist > 100 && !isSliding) 
					{
						Debug.Log("Prout");
						
						swipeTimer = Time.time;
						startSlidePosition = currentSlidePosition;
						StartCoroutine(HorizontalSlide());
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				resetTouch(0);
			}
		}
		
		checkTouched();
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
		if (this.touchesTab.ContainsKey(fingerId))
		{
			this.touchesTab.Remove(fingerId);
		}
	}
	
	public bool isTouched()
	{
		return Input.touchCount > 0;
	}
	
	public void checkTouched()
	{
		float touchTime = 9999;
		foreach (TouchObject touchObj in this.touchesTab.Values)
		{
			if (touchObj.startTime < touchTime)
			{
			}
		}
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
	
	private void resetSlide()
	{
		this.slide = false;
	}
	
	IEnumerator HorizontalSlide()
	{
		isSliding = true;
	    yield return new WaitForSeconds(0.2f);
		resetSlide();
	}
}
