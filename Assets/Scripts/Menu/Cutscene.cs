using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Cutscene : MonoBehaviour {
	
	private bool  isSliding;
	private int   slideDirection;
	public  int   currentSlide;
	private bool  doNothing;  
	
	private Touch screenTouch;
	private int touchCount;
	private Dictionary<int, TouchObject> touchesTab;
	private Vector3 startSlidePosition;
	private Vector3 currentSlidePosition;
	private float   swipeTimer;
	
	private Transform[] cutscene;
	private Transform[] keysPositions;
	
	public float scrollSpeed = 1.0f;
	private bool canSlide = false;
	
	// infos pour le slide
	private bool slide;
	
	private float alpha = 1;
	private float alphaDir = -1;
	
	void Start ()
	{
		// Menu
		isSliding    = false;
		currentSlide = 0;
		doNothing    = false;
		slideDirection = 1;
		canSlide = true;
	
		Transform images = transform.FindChild("Images");
		Transform keys = transform.FindChild("KeyPositions");
	
		cutscene = new Transform[images.childCount];
		for(int iii = 1; iii <= images.childCount; ++iii)
			cutscene[iii-1] = images.FindChild("Cutscene"+iii);
		
		keysPositions = new Transform[3];
		keysPositions[0] = keys.FindChild("Left");	
		keysPositions[1] = keys.FindChild("Center");	
		keysPositions[2] = keys.FindChild("Right");	
		
		for(int iii = 1; iii < images.childCount; ++iii)
			cutscene[iii].position = keysPositions[2].position;
		cutscene[0].position = keysPositions[1].position;
		
		// Touchs
		screenTouch = new Touch();
		touchCount = 0;
		touchesTab = new Dictionary<int, TouchObject>();
		this.slide = true;
	}
	
	void FixedUpdate () 
	{
		if (this.alphaDir == 1 && this.alpha >= 1)
			endCutscene();
		
		if(doNothing)
			return;
		
		if(isSliding && currentSlide < cutscene.Length - 1)
		{
			if(slideDirection == 1)
			{
				cutscene[currentSlide].position   = new Vector3(cutscene[currentSlide].position.x-0.05f*scrollSpeed, cutscene[currentSlide].position.y, cutscene[currentSlide].position.z);
				cutscene[currentSlide+1].position = new Vector3(cutscene[currentSlide+1].position.x-0.05f*scrollSpeed, cutscene[currentSlide+1].position.y, cutscene[currentSlide+1].position.z);
			
				bool currentOk = false;
				bool nextOk    = false;
				if(cutscene[currentSlide].position.x <= keysPositions[0].position.x)
				{
					cutscene[currentSlide].position   = keysPositions[0].position;
					currentOk = true;
				}
				if(cutscene[currentSlide+1].position.x <= keysPositions[1].position.x)
				{
				    cutscene[currentSlide+1].position = keysPositions[1].position;
					nextOk = true;
				}
				
				if(currentOk && nextOk)
				{
					isSliding = false;
					currentSlide++;
				}
			}
			else if(currentSlide > 0)
			{
				cutscene[currentSlide].position   = new Vector3(cutscene[currentSlide].position.x+0.05f*scrollSpeed, cutscene[currentSlide].position.y, cutscene[currentSlide].position.z);
				cutscene[currentSlide-1].position = new Vector3(cutscene[currentSlide-1].position.x+0.05f*scrollSpeed, cutscene[currentSlide-1].position.y, cutscene[currentSlide-1].position.z);
				
				bool currentOk  = false;
				bool previousOk = false;
				if(cutscene[currentSlide].position.x >= keysPositions[2].position.x)
				{
					cutscene[currentSlide].position   = keysPositions[2].position;
					currentOk = true;
				}
				if(cutscene[currentSlide-1].position.x >= keysPositions[1].position.x)
				{
				    cutscene[currentSlide-1].position = keysPositions[1].position;
					previousOk = true;
				}
				
				if(currentOk && previousOk)
				{
					isSliding = false;
					currentSlide--;
				}
			}
		}
	}
	
	public void SlideMenu()
	{
		if(currentSlide < cutscene.Length - 1)
			isSliding = true;
		else if (this.alphaDir == -1)
			this.alphaDir = 1;
	}
	
	public void endCutscene()
	{
		if (this.alpha == 0)
		{
			this.alphaDir = 1;
			return;
		}
		
		if(doNothing)
			return;
		
		if(Application.loadedLevelName == "Cutscene")
		{
			doNothing = true;
			GameObject.Find("Anchor").transform.FindChild("LOADING_PANEL").gameObject.SetActive(true);
			StartCoroutine(LoadTutoToLoad());
		}
		else
		{
			doNothing = true;
			GameObject.Find("Anchor").transform.FindChild("LOADING_PANEL").gameObject.SetActive(true);
			if (Application.loadedLevelName == "Cutscene2")
			{
				this.alphaDir = 0;
				this.alpha = 0;
			}
			StartCoroutine(LoadLevelToLoad());
		}
	}
	
    IEnumerator LoadLevelToLoad() {
        AsyncOperation async = Application.LoadLevelAsync(Datas.sharedDatas().datas.selectedWorld * MyDefines.kLevelsByWorld + Datas.sharedDatas().datas.selectedLevel + 1);
		yield return async;
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
				
				switch (touch.phase) 
				{
					case TouchPhase.Began:
						startSlidePosition = touch.position;
						swipeTimer = Time.time;
					break;
					
					case TouchPhase.Stationary:
					case TouchPhase.Moved:
						// gestion du slide pour les cutscenes
						currentSlidePosition = touch.position;
					
						if (Mathf.Abs(startSlidePosition.y - currentSlidePosition.y) <= 100) 
						{
							float swipeTime = Time.time - swipeTimer;
							float swipeDist = (new Vector3(currentSlidePosition.x, 0, 0) - new Vector3(startSlidePosition.x, 0, 0)).magnitude;
							
							if(currentSlide == 0 && startSlidePosition.x < currentSlidePosition.x)
								break;
							
							if (swipeTime > 0 && swipeTime < 2 && swipeDist > 100 && !isSliding && canSlide)
							{
								if(startSlidePosition.x > currentSlidePosition.x)
									slideDirection = 1;
								else
									slideDirection = -1;
								swipeTimer = Time.time;
								startSlidePosition = currentSlidePosition;
								StartCoroutine(HorizontalSlide());
								canSlide = false;
							}
						}
					break;
					
					case TouchPhase.Ended:
						canSlide = true;
						touchesToRemove.Add(touchObj);
					break;
					
					case TouchPhase.Canceled:
						canSlide = true;
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
					
					if(!(currentSlide == 0 && startSlidePosition.x < currentSlidePosition.x))
					{
						if (swipeTime > 0 && swipeTime < 2 && swipeDist > 100 && !isSliding && canSlide)
						{
							if(startSlidePosition.x > currentSlidePosition.x)
								slideDirection = 1;
							else
								slideDirection = -1;
							swipeTimer = Time.time;
							startSlidePosition = currentSlidePosition;
							StartCoroutine(HorizontalSlide());
							canSlide = false;
						}
					}
				}
			}
			else if (Input.GetMouseButtonUp(0))
			{
				canSlide = true;
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
		if(this.touchesTab != null)
		{
			foreach (TouchObject touchObj in this.touchesTab.Values)
			{
				if (touchObj.startTime < touchTime)
				{
				}
			}
		}
	}
	
	private bool hasObjectSelected()
	{
		bool ret = false;
		if(this.touchesTab != null)
		{
			foreach (TouchObject touchObj in this.touchesTab.Values)
			{
				if (touchObj.selectedObject != null)
				{
					ret = true;
				}
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
		SlideMenu();
	    yield return new WaitForSeconds(0.2f);
		resetSlide();
	}
	
	IEnumerator LoadTutoToLoad() {
		AsyncOperation async = Application.LoadLevelAsync("CM_Level_0_FINAL");
		yield return async;
    }
	
	void OnGUI()
	{
		if (this.alphaDir == 1)
		{
			this.alpha += Time.deltaTime / 2f;
			if (this.alpha > 1)
			{
				this.alpha = 1;
			}
		}
		else if (this.alphaDir == -1)
		{
			this.alpha -= Time.deltaTime / 2f;
			if (this.alpha < 0)
			{
				this.alpha = 0;
			}
		}
		Color color = new Color(0, 0, 0, this.alpha);
		Texture2D texture = new Texture2D(1, 1);
    	texture.SetPixel(0,0,color);
	    texture.Apply();
	    GUI.skin.box.normal.background = texture;
	    GUI.Box(new Rect(0, 0, Screen.width, Screen.height), GUIContent.none);
	}

	public void LeftArrow()
	{
		if(!isSliding)
		{
			slideDirection = -1;
			SlideMenu();
		}
	}

	public void RightArrow()
	{
		if(!isSliding)
		{
			slideDirection = 1;
			SlideMenu();
		}
	}
}
