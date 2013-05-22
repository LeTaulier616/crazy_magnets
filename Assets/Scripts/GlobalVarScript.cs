using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GlobalVarScript : MonoBehaviour
{
	public GameObject player;
	public GameObject playerMesh;
	
	public float hudLimitX;
	public float hudLimitY;
	public float comfortZone;
	public float minSwipeDist;
	public float maxSwipeTime;
	public float maxTapTime;

	public GameObject playerGameObject;
	public float playerSpeed;
	public float playerJumpForce;
	public float playerDamping;
	public float playerGravityScale;
	public float accelerationFactor;
	public float decelerationFactor;
	public float slipperyFactor;

	public float smallEnemySpeed;
	public float smallEnemyJumpForce;
	public float smallEnemyDamping;
	public float smallEnemyGravityScale;
	public float smallEnemyPatrolSpeed;
	public float smallEnemyPursuitSpeed;
	public float smallEnemyLocateDistance;
	public float smallEnemyAlertRange;

	public float bigEnemySpeed;
	public float bigEnemyJumpForce;
	public float bigEnemyDamping;
	public float bigEnemyGravityScale;
	public float bigEnemyPatrolSpeed;
	public float bigEnemyPursuitSpeed;
	public float bigEnemyLocateDistance;
	public float bigEnemyAlertRange;
	
	public Vector3 cameraFixedPos = Vector3.zero;
	public int cameraFree = 0; // 0 : forcee sur la target; 1 : libre; 2 : libre avec controles joueur bloques
	public Transform cameraTarget;
	private Transform cameraTargetDefault;
	public float cameraSmooth;
	public float cameraSmoothDefault;
	public float cameraZOffset;
	
	public float ChargeButtonRadius;
	
	public float ButtonRadius;
	
	public float BlockRadius;
	public float BlockForce;
	
	public float GrabRadius;
		
	public List<string> groundTags;
	
	public AudioClip AttractionSound;
	public AudioClip RepulsionSound;
	public AudioClip InterruptorSound;
	public AudioClip InterruptorReleaseSound;
	public AudioClip ButtonSound;
	public AudioClip ElectricButtonSound;
	public AudioClip ChargeZoneSound;
	public AudioClip KillSound;
	public AudioClip GrabSound;
	public AudioClip AttractorSound;
	public AudioClip BumperSound;
	public AudioClip DoorOpenSound;
	public AudioClip DoorCloseSound;
	public AudioClip BoltSound;
	public AudioClip JumpSound;
	
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
	
	public static GlobalVarScript instance;
	
	void Awake()
	{
		instance = this;
		player = GameObject.FindGameObjectWithTag("PlayerObject");
		playerMesh = GameObject.FindGameObjectWithTag("PlayerMesh");
		cameraTargetDefault = cameraTarget;
		cameraSmoothDefault = cameraSmooth;
		groundTags = new List<string>();
		groundTags.AddRange(new string[]{"Ground", "Slippery", "Bumper", "Bloc", "Attractor", "Platform", "Door", "MultiDoor"});
	}
	
	public void SetCameraTarget(Transform target, bool throwFocus)
	{
		if (throwFocus)
		{
			cameraTarget.SendMessageUpwards("ReleaseFocus", SendMessageOptions.DontRequireReceiver);
		}
		cameraTarget = target;
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
		cameraFixedPos = Vector3.zero;
		cameraFree = 0;
		SetCameraTarget(cameraTargetDefault, throwFocus);
		cameraSmooth = cameraSmoothDefault;
	}
}
