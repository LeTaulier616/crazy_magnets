using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour
{
	public GameObject ControlLabel;
	public GameObject JumpLabel;
	public GameObject MagnetismLabel;
	public GameObject EndLabel;
	
	public GameObject CubePrefab;
	public Transform CubePosition;
	
	public Texture RedControlTexture;
	public Texture GreenControlTexture;
	
	private GameObject player;
	private bool showBorders;
	
	private Vector3 viewportWidthLeft;
	private Vector3 viewportWidthRight;
	
	private ControllerMain playerController;
	private PlayerScript playerScript;
	
	private bool checkDistance;
	private float walkDistance;
	
	public bool checkJumps;
	private bool canCheckJump;
	public int jumpCount;
	
	private Vector3 playerPosition;
	
	public static Tutorial instance;
	
	// Use this for initialization
	void Start () 
	{
		instance = this;
		player = GlobalVarScript.instance.player;
		
		playerController = player.GetComponent<ControllerMain>();
		playerScript = player.GetComponent<PlayerScript>();
		
		playerScript.canJump = false;
		canCheckJump = false;
				
		showBorders = false;
		checkDistance = false;
		
		viewportWidthLeft = Camera.mainCamera.ViewportToScreenPoint(new Vector3(GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		viewportWidthRight = Camera.mainCamera.ViewportToScreenPoint(new Vector3(1.0f - GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		
		walkDistance = 0.0f;
		
		playerPosition = player.transform.position;
		
		ShowMoveControls();
	}
	
	// Update is called once per frame
	void Update () 
	{
		// debug
		if (Input.GetKeyDown(KeyCode.Return))
		{
			EndTutorial();
		}
		
		if(checkDistance)
		{
			walkDistance = Vector3.Distance(playerPosition, player.transform.position);
			
			if(walkDistance >= 18.0f)
			{
				checkDistance = false;
				playerScript.canJump = true;
				ShowJumpControls();
			}
		}
		
		if(checkJumps)
		{
			if (canCheckJump && playerScript.isJumping && playerScript.onGround == false)
			{
				jumpCount++;
				canCheckJump = false;
			}
			if (!canCheckJump && playerScript.onGround == true)
			{
				canCheckJump = true;
			}
			if(jumpCount >= 5 && playerScript.onGround)
			{
				checkJumps = false;
				ShowMagnetismControls();
			}
		}
		
	}
	
	void ShowMoveControls()
	{
		ToggleControls();
		ToggleBorders();
		ToggleControlLabel();
		
		Invoke("ToggleControls", 3.0f);
		Invoke("ToggleControlLabel", 3.0f);
		Invoke("ToggleDistanceCheck", 3.0f);
	}
	
	void ShowJumpControls()
	{
		ToggleControls();
		ToggleJumpLabel();
		
		Invoke("ToggleControls", 3.0f);
		Invoke("ToggleJumpLabel", 3.0f);
		Invoke("ToggleJumpCheck", 3.0f);
	}
	
	void ShowMagnetismControls()
	{
		ToggleControls();
		ToggleBorders();
		ToggleMagnetismLabel();
		
		GameObject cubeInstance = Instantiate(CubePrefab, CubePosition.position, Quaternion.identity) as GameObject;
		
		GlobalVarScript.instance.SetCameraTarget(cubeInstance.transform, true);
		
		Invoke("ResetCamera" , 3.0f);
		Invoke("ToggleControls", 3.0f);
		Invoke("ToggleMagnetismLabel", 3.0f);
		Invoke("EndTutorial", 20.0f);
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
		if(playerScript.canMove)
			playerScript.ReleaseFocus();
		else
			playerScript.Focus();
	}
	
	void ToggleControlLabel()
	{
		if(ControlLabel.activeSelf)
			ControlLabel.SetActive(false);
		
		else
			ControlLabel.SetActive(true);
	}
	
	void ToggleJumpLabel()
	{
		if(JumpLabel.activeSelf)
			JumpLabel.SetActive(false);
		
		else
			JumpLabel.SetActive(true);
	}
	
	void ToggleMagnetismLabel()
	{
		if(MagnetismLabel.activeSelf)
			MagnetismLabel.SetActive(false);
		
		else
			MagnetismLabel.SetActive(true);
	}
	
	void ToggleDistanceCheck()
	{
		if(checkDistance)
			checkDistance = false;
		
		else
			checkDistance = true;
	}
	
	void ToggleJumpCheck()
	{
		if(checkJumps)
			checkJumps = false;
		
		else
			checkJumps = true;
	}
	
	void ResetCamera()
	{
		GlobalVarScript.instance.cameraTarget = player.transform.FindChild("TARGET");
	}
	void EndTutorial()
	{
		EndLabel.SetActive(true);
		ToggleControls();
		
		GameObject.Find("Menus").GetComponent<MenuGesture>().endTuto();
	}
	
	void OnGUI()
	{
		
		if(showBorders)
		{
			Rect leftBorder = new Rect(0.0f, 0.0f, viewportWidthLeft.x, Screen.height);
			Rect rightBorder = new Rect(viewportWidthRight.x, 0.0f, Screen.width - viewportWidthRight.x, Screen.height);
			
			if(playerScript.lastDir == 1)
				GUI.DrawTexture(leftBorder, RedControlTexture);
			else
				GUI.DrawTexture(leftBorder, GreenControlTexture);
			
			if(playerScript.lastDir == -1)
				GUI.DrawTexture(rightBorder, RedControlTexture);
			else
				GUI.DrawTexture(rightBorder, GreenControlTexture);
		}
	}
}
