using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	private GameObject player;
	
	private bool showBorders;
	
	private Vector3 viewportWidthLeft;
	private Vector3 viewportWidthRight;
	
	// Use this for initialization
	void Start () 
	{
		player = GlobalVarScript.instance.player;

		showBorders = false;
		
		viewportWidthLeft = Camera.mainCamera.ViewportToScreenPoint(new Vector3(GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		viewportWidthRight = Camera.mainCamera.ViewportToScreenPoint(new Vector3(1.0f - GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		
	}
	
	void MoveControls()
	{
		showBorders = true;
	}
	
	void OnGUI()
	{
		if(showBorders)
		{
			
		}
	}
}
