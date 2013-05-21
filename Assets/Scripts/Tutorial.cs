using UnityEngine;
using System.Collections;

public class Tutorial : MonoBehaviour {
	
	public GameObject ControlLabel;
	public GameObject JumpLabel;
	public GameObject MagnetismLabel;
	public GameObject EndLabel;
	
	public GameObject CubePrefab;
	
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
				
		showBorders = false;
		checkDistance = false;
		
		viewportWidthLeft = Camera.mainCamera.ViewportToScreenPoint(new Vector3(GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		viewportWidthRight = Camera.mainCamera.ViewportToScreenPoint(new Vector3(1.0f - GlobalVarScript.instance.hudLimitX, 0.0f, 0.0f));
		
		walkDistance = 0.0f;
		
		ShowMoveControls();	
	}
	
	// Update is called once per frame
	void Update () 
	{
		if(checkDistance)
		{
			if(!playerScript.isWalking)
			{
				playerPosition = player.transform.position;
			}
			
			else
			{
				walkDistance += Vector3.Distance(playerPosition, player.transform.position);
			}
			
			if(walkDistance >= 1000.0f)
			{
				checkDistance = false;
				playerScript.canJump = true;
				ShowJumpControls();
			}
		}
		
		if(checkJumps)
		{
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
		
		GameObject cubeInstance = Instantiate(CubePrefab,
			playerScript.transform.position + playerScript.playerMesh.transform.forward * 5.0f,
			Quaternion.identity) as GameObject;
		
		Invoke("ToggleControls", 3.0f);
		Invoke("ToggleMagnetismLabel", 3.0f);
		Invoke("EndTutorial", 15.0f);
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
	
	void EndTutorial()
	{
		EndLabel.SetActive(true);
		ToggleControls();
		
		Invoke("LoadNextLevel", 5.0f);
	}
	
	void LoadNextLevel()
	{
		Application.LoadLevel(1);
	}
	
	void OnGUI()
	{
		if(showBorders)
		{
			Rect leftBorder = new Rect(0.0f, 0.0f, viewportWidthLeft.x, Screen.height);
			Rect rightBorder = new Rect(viewportWidthRight.x, 0.0f, Screen.width - viewportWidthRight.x, Screen.height);
			
			if(playerController.isLeftTouched())
				GUI.DrawTexture(leftBorder, GreenControlTexture);
			
			else
				GUI.DrawTexture(leftBorder, RedControlTexture);
			
			if(playerController.isRightTouched())
				GUI.DrawTexture(rightBorder, GreenControlTexture);
			
			else
				GUI.DrawTexture(rightBorder, RedControlTexture);
		}
	}
}
