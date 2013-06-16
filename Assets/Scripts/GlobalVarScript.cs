using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVarScript : MonoBehaviour
{
	public GameObject player;
	public GameObject playerMesh;
	
	public bool controlParams;
	
	public float hudLimitX;
	public float hudLimitY;
	public float comfortZone;
	public float minSwipeDist;
	public float maxSwipeTime;
	public float maxTapTime;
	
	public bool playerParams;

	public GameObject playerGameObject;
	public float playerSpeed;
	public float playerJumpForce;
	public float playerDamping;
	public float playerGravityScale;
	public float accelerationFactor;
	public float decelerationFactor;
	public float slipperyFactor;
	
	[System.Serializable]
	public class EnemyInfo
	{
		public float speed;
		public float jumpForce;
		public float damping;
		public float gravityScale;
		public float patrolSpeed;
		public float frontSpottingDistance;
		public float backSpottingDistance;
		public float pursuitSpeed;
		public float reach;
		public float alertRange;
		public float hitDistance;
		public float hitTime;
	}
	
	public bool smallEnemyParams;
	public EnemyInfo smallEnemy = new EnemyInfo();
	
	public bool bigEnemyParams;
	public EnemyInfo bigEnemy = new EnemyInfo();
	
	public bool cameraParams;
	
	public Vector3 cameraFixedPos = Vector3.zero;
	public int cameraFree = 0; // 0 : forcee sur la target; 1 : libre; 2 : libre avec controles joueur bloques
	public Transform cameraTarget;
	private Transform cameraTargetDefault;
	public float cameraSmooth;
	public float cameraSmoothDefault;
	public float cameraZOffset;
	
	public bool objectParams;
	
	public float ChargeButtonRadius;
	public float ButtonRadius;
	public float BlockRadius;
	public float BlockForce;	
	public Color BlockRangeColor;
	public Color BlockUseColor;
	public float GrabRadius;
		
	public List<string> groundTags;
	
	public bool soundParams;
	
	public AudioClip AttractionSound;
	public AudioClip RepulsionSound;
	public AudioClip InterruptorSound;
	public AudioClip InterruptorReleaseSound;
	public AudioClip ButtonSound;
	public AudioClip ElectricButtonSound;
	public AudioClip ChargeZoneSound;
	public AudioClip KillSound;
	public AudioClip GrabSound;
	public AudioClip AttractorOnSound;
	public AudioClip AttractorOffSound;
	public AudioClip BumperSound;
	public AudioClip DoorOpenSound;
	public AudioClip DoorCloseSound;
	public AudioClip BoltSound;
	public AudioClip JumpSound;
	public AudioClip WinSound;
	
	public bool ClockSoundsExpand = true;
	public int ClockSoundsSize = 1;
	public AudioClip[] ClockSounds;
	
	public bool WalkSoundsExpand = true;
	public int WalkSoundsSize = 1;
	public AudioClip[] WalkSounds;
	
	public bool SteamSoundsExpand = true;
	public int SteamSoundsSize = 1;
	public AudioClip[] SteamSounds;
	
	public bool MechSoundsExpand = true;
	public int MechSoundsSize = 1;
	public AudioClip[] MechSounds;
	
	public bool BigSoundsExpand = true;
	public int BigSoundsSize = 1;
	public AudioClip[] BigSounds;
	
	public bool BigChaseSoundsExpand = true;
	public int BigChaseSoundsSize = 1;
	public AudioClip[] BigChaseSounds;
	
	public bool SmallSoundsExpand = true;
	public int SmallSoundsSize = 1;
	public AudioClip[] SmallSounds;
	
	public bool SmallChaseSoundsExpand = true;
	public int SmallChaseSoundsSize = 1;
	public AudioClip[] SmallChaseSounds;
	
	public GameObject AudioManager;
	
	public static GlobalVarScript instance;

	
	void Awake()
	{
		instance = this;
		player = GameObject.FindGameObjectWithTag("PlayerObject");
		playerMesh = GameObject.FindGameObjectWithTag("PlayerMesh");
		AudioManager = GameObject.FindGameObjectWithTag("AudioManager");
		cameraTargetDefault = cameraTarget;
		cameraSmoothDefault = cameraSmooth;
		groundTags = new List<string>();
		groundTags.AddRange(new string[]{"Ground", "Slippery", "Bumper", "Bloc", "Attractor", "Platform", "Door", "MultiDoor", "Enemy"});
	}
	
	public void SetCameraTarget(Transform target, bool throwFocus)
	{
		if (throwFocus)
		{
			cameraTarget.SendMessageUpwards("ReleaseFocus", SendMessageOptions.DontRequireReceiver);
		}
		
		cameraTarget = target;
		
		if (cameraTarget.GetInstanceID() != cameraTargetDefault.GetInstanceID())
		{
			cameraFree = 2;
		}
		
		cameraFixedPos = Vector3.zero;
		
		if (throwFocus)
		{
			cameraTarget.SendMessageUpwards("Focus", SendMessageOptions.DontRequireReceiver);
		}
	}
	
	public void blockCamera(Vector3 pos)
	{
		cameraFixedPos = pos;
	}
	
	public void resetCamera(bool throwFocus)
	{
		cameraFree = 0;
		SetCameraTarget(cameraTargetDefault, throwFocus);
		cameraSmooth = cameraSmoothDefault;
	}
}
