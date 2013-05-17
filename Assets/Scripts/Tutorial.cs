using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	private GameObject player;
	
	private bool showBorders;
	
	private Vector3 viewportWidthLeft;
	private Vector3 viewportWidthRight;
	
	public Texture RedControlTexture;
	public Texture GreenControlTexture;
	
	public GameObject ControlLabel;
	
	// Use this for initialization
	void Start () 
	{
		player = GlobalVarScript.instance.player;
		
		showBorders = false;
		
		viewportWidthLeft = Camera.mainCamera.ViewportToScreenPoint(new Vector3(GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		viewportWidthRight = Camera.mainCamera.ViewportToScreenPoint(new Vector3(1.0f - GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		
		ShowControls();
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void ShowControls()
	{
		ToggleControls();
		ToggleBorders();
		ControlLabel.SetActive(true);
		
		Invoke("ToggleControls", 3.0f);
		Invoke("ToggleBorders", 3.0f);
		Invoke("ToggleControlLabel", 3.0f);
	}
	
	void ToggleBorders()
	{
		if(showBorders)
			showBorders = false;
		
		else
			showBorders = true;
	}
	
	void ToggleControls()
	{
		if(GlobalVarScript.instance.cameraFree == 0)
			GlobalVarScript.instance.cameraFree = 2;
		
		else
			GlobalVarScript.instance.cameraFree = 0;
	}
	
	void ToggleControlLabel()
	{
		if(ControlLabel.activeSelf)
			ControlLabel.SetActive(false);
		
		else
			ControlLabel.SetActive(true);
	}
	
	void OnGUI()
	{
		if(showBorders)
		{
			Rect leftBorder = new Rect(0.0f, 0.0f, viewportWidthLeft.x, Screen.height);
			Rect rightBorder = new Rect(viewportWidthRight.x, 0.0f, Screen.width - viewportWidthRight.x, Screen.height);
			
			GUI.DrawTexture(leftBorder, RedControlTexture);
			GUI.DrawTexture(rightBorder, RedControlTexture);
		}
	}
}
