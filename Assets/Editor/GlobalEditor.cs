using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(GlobalVarScript))]
public class GlobalEditor : Editor
{
	GlobalVarScript target_;
	
	void OnEnable()
	{
		this.target_ = this.target as GlobalVarScript;
	}
	
	public override void OnInspectorGUI()
	{
		openBox ("Parametres globaux");
		
		this.target_.hudLimitX = 0.01f * addSlider (100.0f * this.target_.hudLimitX, 0.0f, 40.0f, "Largeur de zone", "Largeur de la zone aux bords de l'ecran pour le deplacement", "%");
		
		this.target_.comfortZone = addSlider (this.target_.comfortZone, 20f, 100f, "Deadzone de slide", "Zone de securite du slide", "px.");
		
		this.target_.minSwipeDist = addSlider (this.target_.minSwipeDist, 10f, 200f, "Distance de slide min.", "Distance minimum de slide pour activer le saut", "px.");
		
		this.target_.maxSwipeTime = addSlider (this.target_.maxSwipeTime, 0.1f, 2f, "Temps de slide max.", "Temps maximal de detection du slide", "sec.");
		
		this.target_.maxTapTime = addSlider (this.target_.maxTapTime, 0.1f, 0.5f, "Temps de tap max.", "Temps maximal de detection du tap pour le saut", "sec.");
		
		closeBox ();
		
		
		
		openBox ("Parametres joueurs");
		
		this.target_.playerSpeed = addFloatField (this.target_.playerSpeed, "Vitesse", "Vitesse du déplacement du personnage au sol et en l'air", "unites/s");
		
		this.target_.playerJumpForce = addFloatField (this.target_.playerJumpForce, "Hauteur du saut", "Puissance de l'impulsion de saut", "unites");
		
		this.target_.playerDamping = addFloatField (this.target_.playerDamping, "Amortissement", "Friction de l'air en saut", "u.a");
		
		this.target_.playerGravityScale = addFloatField (this.target_.playerGravityScale, "Acceleration en chute", "Facteur d'acceleration pendant une chute", "u.a");

		
		this.target_.accelerationFactor = 
			Mathf.Pow(
				addFloatField(
					Mathf.Pow(this.target_.accelerationFactor, -1.0f),
					"Duree de l'acceleration",
					"Temps que met le personnage a atteindra sa vitesse maximale",
					"sec."),
			-1.0f);
			
		this.target_.decelerationFactor = 
			Mathf.Pow(	
				addFloatField(
				Mathf.Pow(this.target_.decelerationFactor, -1.0f),
				"Duree de la deceleration",
				"Temps que met le personnage a s'arreter completement",
				"sec."),
			-1.0f);
		
		this.target_.slipperyFactor = 1.0f - addFloatField(
			this.target_.slipperyFactor + ((0.5f - this.target_.slipperyFactor) * 2.0f),
			"Facteur de glisse",
			"Plus la valeur est haute, plus le personnage glisse",
			"u.a");
		closeBox ();
		
		
		openBox ("Parametres enemis");
		
		openBox ("Petits enemis");
		
		this.target_.smallEnemySpeed = addFloatField (this.target_.smallEnemySpeed, "Vitesse", "Vitesse du déplacement du personnage au sol et en l'air", "unites/s");
		this.target_.smallEnemyJumpForce = addFloatField (this.target_.smallEnemyJumpForce, "Hauteur du saut", "Puissance de l'impulsion de saut", "unites");
		this.target_.smallEnemyDamping = addFloatField (this.target_.smallEnemyDamping, "Amortissement", "Friction de l'air en saut", "u.a");
		this.target_.smallEnemyGravityScale = addFloatField (this.target_.smallEnemyGravityScale, "Acceleration en chute", "Facteur d'acceleration pendant une chute", "u.a");
		this.target_.smallEnemyPatrolSpeed = addFloatField (this.target_.smallEnemyPatrolSpeed, "Vitesse de Patrouille", "Vitesse du déplacement de l'enemis en patrouille", "unites/s");
		this.target_.smallEnemyPursuitSpeed = addFloatField (this.target_.smallEnemyPursuitSpeed, "Vitesse de Poursuite", "Vitesse du déplacement de l'enemis en poursuite", "unites/s");
		this.target_.smallEnemyLocateDistance = addFloatField (this.target_.smallEnemyLocateDistance, "Distance de Reperage", "Distance maximale de reperage du personnage", "unites");
		this.target_.smallEnemyAlertRange = addFloatField (this.target_.smallEnemyAlertRange, "Distance d'Alerte", "Distance d'abandon de la poursuite", "unites");
		
		closeBox ();
		
		openBox ("Gros enemis");
		
		this.target_.bigEnemySpeed = addFloatField (this.target_.bigEnemySpeed, "Vitesse", "Vitesse du déplacement du personnage au sol et en l'air", "unites/s");
		this.target_.bigEnemyJumpForce = addFloatField (this.target_.bigEnemyJumpForce, "Hauteur du saut", "Puissance de l'impulsion de saut", "unites");
		this.target_.bigEnemyDamping = addFloatField (this.target_.bigEnemyDamping, "Amortissement", "Friction de l'air en saut", "u.a");
		this.target_.bigEnemyGravityScale = addFloatField (this.target_.bigEnemyGravityScale, "Acceleration en chute", "Facteur d'acceleration pendant une chute", "u.a");
		this.target_.bigEnemyPatrolSpeed = addFloatField (this.target_.bigEnemyPatrolSpeed, "Vitesse de Patrouille", "Vitesse du déplacement de l'enemis en patrouille", "unites/s");
		this.target_.bigEnemyPursuitSpeed = addFloatField (this.target_.bigEnemyPursuitSpeed, "Vitesse de Poursuite", "Vitesse du déplacement de l'enemis en poursuite", "unites/s");
		this.target_.bigEnemyLocateDistance = addFloatField (this.target_.bigEnemyLocateDistance, "Distance de Reperage", "Distance maximale de reperage du personnage", "unites");
		this.target_.bigEnemyAlertRange = addFloatField (this.target_.bigEnemyAlertRange, "Distance d'Alerte", "Distance d'abandon de la poursuite", "unites");
		
		closeBox ();
		
		closeBox ();
		
		
		openBox("Parametres camera");
		
		this.target_.cameraTarget = addTransformField(this.target_.cameraTarget, true, "Cible camera", "Objet suivi par la camera");
		
		this.target_.cameraSmooth = addSlider(this.target_.cameraSmooth, 0.0f, 1.0f, "Duree d'interpolation", "Duree d'interpolation du mouvement de la camera", "sec.");
		
		this.target_.cameraZOffset = addSlider(this.target_.cameraZOffset, 0.0f, 30.0f, "Distance camera", "Distance entre la caméra et le joueur (zoom/dezoom)", "unites");
		
		closeBox();
		
		
		
		openBox("Parametres objets");
		
		this.target_.BlockRadius = addFloatField(this.target_.BlockRadius,
			"Portee du magnetisme",
			"Distance maximum pour attirer/repousser des objets",
			"unites");
		
		this.target_.BlockForce = addFloatField(this.target_.BlockForce,
			"Force du magnetisme",
			"Force de deplacement des cubes",
			"u.a");
		
		this.target_.ButtonRadius = addFloatField(this.target_.ButtonRadius,
			"Portee du levier",
			"Distance maximum pour pouvoir actionner un levier",
			"unites");
		
		this.target_.ChargeButtonRadius = addFloatField(this.target_.ChargeButtonRadius,
			"Portee de l'interrupteur electrique",
			"Distance maximum pour pouvoir actionner un interrupteur electrique",
			"unites");
		
		this.target_.GrabRadius = addFloatField(this.target_.GrabRadius,
			"Portee du grappin",
			"Distance maximum pour pouvoir actionner un grappin",
			"unites");
		
		closeBox();
		
		openBox("Parametres son");
		
		this.target_.AttractionSound = addSoundField(this.target_.AttractionSound,
			false,
			"Son de l'attraction",
			"");
		
		this.target_.RepulsionSound = addSoundField(this.target_.RepulsionSound,
			false,
			"Son de la repulsion",
			"");
		
		this.target_.InterruptorSound = addSoundField(this.target_.InterruptorSound,
			false,
			"Son du bouton ON",
			"Pression du bouton");
		
		this.target_.InterruptorReleaseSound = addSoundField(this.target_.InterruptorReleaseSound,
			false, 
			"Son du bouton OFF",
			"Relachement du bouton");
		
		this.target_.ButtonSound = addSoundField(this.target_.ButtonSound,
			false,
			"Son du levier",
			"");
		
		this.target_.ElectricButtonSound = addSoundField(this.target_.ElectricButtonSound,
			false,
			"Son de l'interrupteur electrique", 
			"Son de l'interrupteur electrique");
		
		this.target_.ChargeZoneSound = addSoundField(this.target_.ChargeZoneSound,
			false,
			"Son de la zone de charge", 
			"Son de la zone de charge");
		
		this.target_.KillSound = addSoundField(this.target_.KillSound,
			false,
			"Son de mort", 
			"");
		
		this.target_.GrabSound = addSoundField(this.target_.GrabSound,
			false,
			"Son du grappin",
			"");
		
		this.target_.AttractorSound = addSoundField(this.target_.AttractorSound,
			false, 
			"Son des plaques d'attraction", 
			"Son des plaques d'attraction");
		
		this.target_.JumpSound = addSoundField(this.target_.JumpSound, 
			false,
			"Son du saut",
			"");
		
		this.target_.BumperSound = addSoundField(this.target_.BumperSound,
			false,
			"Son du bumper",
			"");
		
		this.target_.BoltSound = addSoundField(this.target_.BoltSound,
			false,
			"Son du boulon d'or",
			"");
				
		this.target_.DoorOpenSound = addSoundField(this.target_.DoorOpenSound,
			false,
			"Son d'ouverture de porte",
			"Son d'ouverture de porte");
		
		this.target_.DoorCloseSound = addSoundField(this.target_.DoorCloseSound,
			false,
			"Son de fermeture de porte",
			"Son de fermeture de porte");
		
		EditorGUILayout.Space();
			
		target_.ClockSoundsExpand = EditorGUILayout.Foldout(target_.ClockSoundsExpand, "Sons du timer");
		
		if(target_.ClockSoundsExpand)
		{
			int x = 0;	
			target_.ClockSoundsSize = EditorGUILayout.IntField("Size", target_.ClockSoundsSize);
			
			if(target_.ClockSounds.Length != target_.ClockSoundsSize)
			{
				AudioClip[] newClockSounds = new AudioClip[target_.ClockSoundsSize];
				
				for(x = 0; x < target_.ClockSoundsSize; x++)
				{
					if(target_.ClockSounds.Length > x)
					{
						newClockSounds[x] = target_.ClockSounds[x];
					}
				}
				target_.ClockSounds = newClockSounds;
			}
			
			for(x = 0; x < target_.ClockSounds.Length; x++)
			{
				target_.ClockSounds[x] = addSoundField(target_.ClockSounds[x],
					false,
					"Vitesse " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
			
		target_.WalkSoundsExpand = EditorGUILayout.Foldout(target_.WalkSoundsExpand, "Sons de marche - Magnobot");
		
		if(target_.WalkSoundsExpand)
		{
			int x = 0;	
			target_.WalkSoundsSize = EditorGUILayout.IntField("Size", target_.WalkSoundsSize);
			
			if(target_.WalkSounds.Length != target_.WalkSoundsSize)
			{
				AudioClip[] newWalkSounds = new AudioClip[target_.WalkSoundsSize];
				
				for(x = 0; x < target_.WalkSoundsSize; x++)
				{
					if(target_.WalkSounds.Length > x)
					{
						newWalkSounds[x] = target_.WalkSounds[x];
					}
				}
				target_.WalkSounds = newWalkSounds;
			}
			
			for(x = 0; x < target_.WalkSounds.Length; x++)
			{
				target_.WalkSounds[x] = addSoundField(target_.WalkSounds[x],
					false,
					"Sons de marche " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
			
		target_.BigSoundsExpand = EditorGUILayout.Foldout(target_.BigSoundsExpand, "Sons de marche - Grand ennemi");
		
		if(target_.BigSoundsExpand)
		{
			int x = 0;	
			target_.BigSoundsSize = EditorGUILayout.IntField("Size", target_.BigSoundsSize);
			
			if(target_.BigSounds.Length != target_.BigSoundsSize)
			{
				AudioClip[] newBigSounds = new AudioClip[target_.BigSoundsSize];
				
				for(x = 0; x < target_.BigSoundsSize; x++)
				{
					if(target_.BigSounds.Length > x)
					{
						newBigSounds[x] = target_.BigSounds[x];
					}
				}
				target_.BigSounds = newBigSounds;
			}
			
			for(x = 0; x < target_.BigSounds.Length; x++)
			{
				target_.BigSounds[x] = addSoundField(target_.BigSounds[x],
					false,
					"Sons de marche " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
			
		target_.BigChaseSoundsExpand = EditorGUILayout.Foldout(target_.BigChaseSoundsExpand, "Sons de poursuite - Grand ennemi");
		
		if(target_.BigChaseSoundsExpand)
		{
			int x = 0;	
			target_.BigChaseSoundsSize = EditorGUILayout.IntField("Size", target_.BigChaseSoundsSize);
			
			if(target_.BigChaseSounds.Length != target_.BigChaseSoundsSize)
			{
				AudioClip[] newBigChaseSounds = new AudioClip[target_.BigChaseSoundsSize];
				
				for(x = 0; x < target_.BigChaseSoundsSize; x++)
				{
					if(target_.BigChaseSounds.Length > x)
					{
						newBigChaseSounds[x] = target_.BigChaseSounds[x];
					}
				}
				target_.BigChaseSounds = newBigChaseSounds;
			}
			
			for(x = 0; x < target_.BigChaseSounds.Length; x++)
			{
				target_.BigChaseSounds[x] = addSoundField(target_.BigChaseSounds[x],
					false,
					"Sons de poursuite " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
			
		target_.SmallSoundsExpand = EditorGUILayout.Foldout(target_.SmallSoundsExpand, "Sons de marche - Petit ennemi");
		
		if(target_.SmallSoundsExpand)
		{
			int x = 0;	
			target_.SmallSoundsSize = EditorGUILayout.IntField("Size", target_.SmallSoundsSize);
			
			if(target_.SmallSounds.Length != target_.SmallSoundsSize)
			{
				AudioClip[] newSmallSounds = new AudioClip[target_.SmallSoundsSize];
				
				for(x = 0; x < target_.SmallSoundsSize; x++)
				{
					if(target_.SmallSounds.Length > x)
					{
						newSmallSounds[x] = target_.SmallSounds[x];
					}
				}
				target_.SmallSounds = newSmallSounds;
			}
			
			for(x = 0; x < target_.SmallSounds.Length; x++)
			{
				target_.SmallSounds[x] = addSoundField(target_.SmallSounds[x],
					false,
					"Sons de marche " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
			
		target_.SmallChaseSoundsExpand = EditorGUILayout.Foldout(target_.SmallChaseSoundsExpand, "Sons de poursuite - Petit ennemi");
		
		if(target_.SmallChaseSoundsExpand)
		{
			int x = 0;	
			target_.SmallChaseSoundsSize = EditorGUILayout.IntField("Size", target_.SmallChaseSoundsSize);
			
			if(target_.SmallChaseSounds.Length != target_.SmallChaseSoundsSize)
			{
				AudioClip[] newSmallChaseSounds = new AudioClip[target_.SmallChaseSoundsSize];
				
				for(x = 0; x < target_.SmallChaseSoundsSize; x++)
				{
					if(target_.SmallChaseSounds.Length > x)
					{
						newSmallChaseSounds[x] = target_.SmallChaseSounds[x];
					}
				}
				target_.SmallChaseSounds = newSmallChaseSounds;
			}
			
			for(x = 0; x < target_.SmallChaseSounds.Length; x++)
			{
				target_.SmallChaseSounds[x] = addSoundField(target_.SmallChaseSounds[x],
					false,
					"Sons de poursuite " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();
		
		target_.SteamSoundsExpand = EditorGUILayout.Foldout(target_.SteamSoundsExpand, "Effets de marche - Vapeur");
		
		if(target_.SteamSoundsExpand)
		{
			int x = 0;	
			target_.SteamSoundsSize = EditorGUILayout.IntField("Size", target_.SteamSoundsSize);
			
			if(target_.SteamSounds.Length != target_.SteamSoundsSize)
			{
				AudioClip[] newSteamSounds = new AudioClip[target_.SteamSoundsSize];
				
				for(x = 0; x < target_.SteamSoundsSize; x++)
				{
					if(target_.SteamSounds.Length > x)
					{
						newSteamSounds[x] = target_.SteamSounds[x];
					}
				}
				target_.SteamSounds = newSteamSounds;
			}
			
			for(x = 0; x < target_.SteamSounds.Length; x++)
			{
				target_.SteamSounds[x] = addSoundField(target_.SteamSounds[x],
					false,
					"Effet de vapeur " + (x + 1),
					"");
			}
		}
		
		EditorGUILayout.Space();

		target_.MechSoundsExpand = EditorGUILayout.Foldout(target_.MechSoundsExpand, "Effets de marche - Mecanique");
		
		if(target_.MechSoundsExpand)
		{
			int x = 0;	
			target_.MechSoundsSize = EditorGUILayout.IntField("Size", target_.MechSoundsSize);
			
			if(target_.MechSounds.Length != target_.MechSoundsSize)
			{
				AudioClip[] newMechSounds = new AudioClip[target_.MechSoundsSize];
				
				for(x = 0; x < target_.MechSoundsSize; x++)
				{
					if(target_.MechSounds.Length > x)
					{
						newMechSounds[x] = target_.MechSounds[x];
					}
				}
				target_.MechSounds = newMechSounds;
			}
			
			for(x = 0; x < target_.MechSounds.Length; x++)
			{
				target_.MechSounds[x] = addSoundField(target_.MechSounds[x],
					false,
					"Effet de mecanique " + (x + 1),
					"");
			}
		}
		
		closeBox();

		if(GUI.changed)
		{
			EditorUtility.SetDirty(target_);
		}
	}
	
	private void openBox(string title)
	{
		EditorGUILayout.LabelField(title);
		EditorGUILayout.BeginVertical("Box");
		EditorGUI.indentLevel = 1;
	}
	
	private void closeBox()
	{
		EditorGUILayout.EndVertical();
		EditorGUI.indentLevel = 0;
	}
	
	private float addSlider(float val, float min, float max, string text, string tooltip, string unit)
	{
		EditorGUILayout.BeginHorizontal();
		float newVal = EditorGUILayout.Slider(
			new GUIContent(text, tooltip),
			val,
			min,
			max
		);
		EditorGUILayout.LabelField(unit);
		EditorGUILayout.EndHorizontal();
		
		return newVal;
	}
	
	private float addFloatField(float val, string text, string tooltip, string unit)
	{
		EditorGUILayout.BeginHorizontal();
		float newVal = EditorGUILayout.FloatField(
			new GUIContent(text, tooltip),
			val
		);
		EditorGUILayout.LabelField(unit);
		EditorGUILayout.EndHorizontal();
		
		return newVal;
	}
	
	private Transform addTransformField(Transform transform, bool allowscene, string text, string tooltip)
	{
		EditorGUILayout.BeginHorizontal();
		Transform newTransform = (Transform) EditorGUILayout.ObjectField(new GUIContent(text, tooltip),
			transform,
			typeof(Transform),
			true
		);
		
		EditorGUILayout.Space();
		
		EditorGUILayout.EndHorizontal();
		
		return newTransform;
	}
	
			
	private AudioClip addSoundField(AudioClip clip, bool allowscene, string text, string tooltip)
	{
		EditorGUILayout.BeginHorizontal();
		AudioClip newClip = (AudioClip) EditorGUILayout.ObjectField(new GUIContent(text, tooltip),
			clip,
			typeof(AudioClip),
			allowscene
		);
		
		EditorGUILayout.Space();

		EditorGUILayout.EndHorizontal();
		
		return newClip;
	}
}
