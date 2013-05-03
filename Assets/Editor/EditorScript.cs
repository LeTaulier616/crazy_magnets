using UnityEngine;
using System.Collections;
using UnityEditor;

public class EditorScript : Editor
{
	void Start ()
	{
		PlayerSettings.statusBarHidden = true;
		Input.multiTouchEnabled = true;
	}
	
	void Update ()
	{
	
	}
}
