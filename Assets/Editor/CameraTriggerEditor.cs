using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(CameraTriggerScript))]
public class CameraTriggerEditor : Editor
{
	CameraTriggerScript target_;
	Vector3 newPos = Vector3.zero;
	float smooth = 1;
	
	void OnEnable()
	{
		this.target_ = this.target as CameraTriggerScript;
		if (this.target_.lp == null)
		{
			this.target_.lp = new List<Vector3>();
		}
		if (this.target_.ls == null)
		{
			this.target_.ls = new List<float>();
		}
	}
	
	public override void OnInspectorGUI()
	{
		DrawDefaultInspector ();
		TransformArray();
		if (GUI.changed)
		{
			EditorUtility.SetDirty(target_);
			//HandleUtility.Repaint();
		}
	}
	
	void TransformArray()
	{
		bool oldEnabled;
		for (int i = 0; i < this.target_.lp.Count; ++i)
		{
			Vector3 pos = this.target_.lp[i];
			float s = this.target_.ls[i];
			EditorGUILayout.BeginHorizontal();
			pos.x = EditorGUILayout.FloatField("", pos.x, GUILayout.Width(40));
			pos.y = EditorGUILayout.FloatField("", pos.y, GUILayout.Width(40));
			pos.z = EditorGUILayout.FloatField("", pos.z, GUILayout.Width(40));
			this.target_.lp[i] = pos;
			s = EditorGUILayout.FloatField("", s, GUILayout.Width(30));
			this.target_.ls[i] = s;
			
			oldEnabled = GUI.enabled;
			GUI.enabled = (i <= 0 ? false : true);
			if (GUILayout.Button("Up", GUILayout.Width(40)))
			{
				Swap (i, i-1);
				break;
			}
			GUI.enabled = (i >= this.target_.lp.Count-1 ? false : true);
			if (GUILayout.Button("Down", GUILayout.Width(50)))
			{
				Swap (i, i+1);
				break;
			}
			GUI.enabled = oldEnabled;
			if (GUILayout.Button("Delete", GUILayout.Width(60)))
			{
				this.target_.lp.RemoveAt(i);
				this.target_.ls.RemoveAt(i);
				i--;
				break;
			}
			EditorGUILayout.EndHorizontal();
		}
		EditorGUILayout.BeginVertical("Box");
		this.newPos.x = EditorGUILayout.FloatField("X :", this.newPos.x);
		this.newPos.y = EditorGUILayout.FloatField("Y :", this.newPos.y);
		this.newPos.z = EditorGUILayout.FloatField("Z :", this.newPos.z);
		this.smooth = EditorGUILayout.FloatField("Smooth :", this.smooth);
		if (GUILayout.Button("Add"))
		{
			if (this.smooth != 0)
			{
				this.target_.lp.Add (this.newPos);
				this.target_.ls.Add (this.smooth);
			}
		}
		EditorGUILayout.EndVertical();
	}
	
	void Swap(int index1, int index2)
	{
		Vector3 tmpP = this.target_.lp[index1];
		float tmpS = this.target_.ls[index1];
		this.target_.lp[index1] = this.target_.lp[index2];
		this.target_.lp[index2] = tmpP;
		this.target_.ls[index1] = this.target_.ls[index2];
		this.target_.ls[index2] = tmpS;
	}
}
