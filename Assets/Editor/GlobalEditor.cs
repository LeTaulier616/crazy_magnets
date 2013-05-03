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
		
		closeBox ();
		
		
		
		openBox ("Parametres joueurs");
		
		this.target_.playerSpeed = addFloatField (this.target_.playerSpeed, "Vitesse", "Vitesse du déplacement du personnage au sol et en l'air", "unites/s");
		
		this.target_.playerJumpForce = addFloatField (this.target_.playerJumpForce, "Hauteur du saut", "Puissance de l'impulsion de saut", "unites");

		
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
		
		this.target_.DoorOpenSound = addSoundField(this.target_.DoorOpenSound,
			false,
			"Son d'ouverture de porte",
			"Son d'ouverture de porte");
		
		this.target_.DoorCloseSound = addSoundField(this.target_.DoorCloseSound,
			false,
			"Son de fermeture de porte",
			"Son de fermeture de porte");
		/*	
		for(int i=0; i < target_.WalkSounds.Length; i++)
		{
			this.target_.WalkSounds[i] = addSoundField(this.target_.WalkSounds[i],
				false,
				"Sons de marche " + (i + 1),
				"");
		}*/
				
		target_.WalkSoundsExpand = EditorGUILayout.Foldout(target_.WalkSoundsExpand, "Sons de marche");
		
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
