using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

using System.Linq;
using System.Reflection;
using System;

[CustomEditor(typeof(Road))]
public class RoadEditor : Editor 
{
	Road target_;
	Transform newTransform;
	string newTransformName = "";
	
	bool setChangeDirty = false;
	
	static string[] methods;
    static string[] ignoreMethods = new string[] { "Start", "Update", "OnEnable", "OnInspectorGUI", "Gen", "BuildSpline", 
												   "BuildSegment", "BuildMesh", "OnDrawGizmos", "GetSpline", "Activate",
												   "ActivatedWithPlayer", "UpdateRoad", "getRoadLength", "getSmoothPosition"};

    static RoadEditor()
    {
        methods =
            typeof(Road)
            .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public) // Instance methods, both public and private/protected
            .Where(x => x.DeclaringType == typeof(Road)) // Only list methods defined in our own class
            .Where(x => x.GetParameters().Length == 0) // Make sure we only get methods with zero argumenrts
            .Where(x => !ignoreMethods.Any(n => n == x.Name)) // Don't list methods in the ignoreMethods array (so we can exclude Unity specific methods, etc.)
            .Select(x => x.Name)
            .ToArray();
    }
	
	void OnEnable()
	{
		this.target_ = this.target as Road;
		if(this.target_.lp == null)
			this.target_.lp = new List<Transform>();
		if(this.target_.pathWays == null)
			this.target_.pathWays = new List<KeyPoint>();
	}
	
	public override void OnInspectorGUI()
	{
		TransformArray();
		EditorGUILayout.BeginVertical("Box");
		if(GUILayout.Button("Gen"))
		{
			this.target_.Gen();
		}
		EditorGUILayout.EndVertical();
		
		if(GUI.changed || setChangeDirty) EditorUtility.SetDirty(target_);
		setChangeDirty = false;
	}
	
	void TransformArray()
	{
		//bool xchanged = false;
		//bool ychanged = false;
		
		bool oldEnable = GUI.enabled;
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		EditorGUILayout.BeginVertical("Box");
			this.target_.activating = (Activation)EditorGUILayout.EnumPopup("Activation : ", this.target_.activating);
			this.target_.endBehaviour = (EndBehaviour)EditorGUILayout.EnumPopup("End Behaviour : ", this.target_.endBehaviour);
			this.target_.speed = (float)EditorGUILayout.FloatField("Speed : ", this.target_.speed);
			this.target_.deplacement = (Deplacement)EditorGUILayout.EnumPopup("Deplacement : ", this.target_.deplacement);
			this.target_.fixedSize = (bool)EditorGUILayout.Toggle("Fixed Path Size : ", this.target_.fixedSize);
			GUI.enabled = this.target_.fixedSize;
			this.target_.pathSize = (float)EditorGUILayout.FloatField("Path Size : ", this.target_.pathSize);
			GUI.enabled = oldEnable;
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		
		for(int i = 0; i < this.target_.lp.Count; ++i)
		{
			if(this.target_.trajectoires.Count < this.target_.lp.Count+1)
				this.target_.trajectoires.Add(Trajectoire.LINE);
			if(this.target_.pathTypes.Count < this.target_.lp.Count+1)
				this.target_.pathTypes.Add(PathType.HORIZONTALLTOR);
		}
		for(int i = 0; i < this.target_.lp.Count; ++i)
		{
			if(i+1 >= this.target_.lp.Count && this.target_.pathWays.Count < this.target_.lp.Count)
			{
				KeyPoint tmppp = new KeyPoint();
				tmppp.size = new Vector2(10, 10);
				this.target_.pathWays.Add(tmppp);
			}
			else if(this.target_.pathWays.Count < this.target_.lp.Count)
			{
				KeyPoint tmppp = new KeyPoint();
				tmppp.size = new Vector2(this.target_.lp[i+1].position.x-this.target_.lp[i].position.x, 
										 this.target_.lp[i+1].position.y-this.target_.lp[i].position.y);
				this.target_.pathWays.Add(tmppp);
			}
			if(this.target_.functionNames.Count < this.target_.lp.Count)
			{
				this.target_.functionNames.Add("");
			}
			if(this.target_.waits.Count < this.target_.lp.Count)
			{
				this.target_.waits.Add(0.0f);
			}
			if(this.target_.keyTypes.Count < this.target_.lp.Count)
			{
				this.target_.keyTypes.Add(KeyType.NOTYPE);
			}
		}
		
		EditorGUILayout.BeginVertical("Box");
			for(int i = 0; i < this.target_.lp.Count; ++i)
			{
				EditorGUILayout.BeginVertical("Box");
				Vector3 oldPosition = this.target_.lp[i].position;
			
				//xchanged = false;
				//ychanged = false;
			
				EditorGUILayout.BeginHorizontal();
					oldEnable = GUI.enabled;
					EditorGUILayout.PrefixLabel("Path " + this.target_.lp[i].name + " : ");
					GUI.enabled = (i <= 1 ? false : true);
					if(GUILayout.Button("Up", GUILayout.Width (60)))
					{
						swap (i, i - 1);
						break;
					}
					GUI.enabled = (i >= this.target_.lp.Count - 1 && i <= 1 ? false : true);
					if(GUILayout.Button("Down", GUILayout.Width (60)))
					{
						swap (i, i + 1);
						break;
					}
					GUI.enabled = (i <= 0 ? false : true);
					if(GUILayout.Button("Delete", GUILayout.Width (60)))
					{
						removePath(i);
						break;
					}
					GUI.enabled = oldEnable;
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
					this.target_.pathTypes[i] = (PathType)EditorGUILayout.EnumPopup("Type : ", this.target_.pathTypes[i]);
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
					this.target_.lp[i].position = EditorGUILayout.Vector3Field("Position : ", this.target_.lp[i].position);
				EditorGUILayout.EndHorizontal();
					
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = (i >= this.target_.lp.Count - 1 ? false : true);
					KeyPoint tmpp = this.target_.pathWays[i];
					Vector2 oldSize = tmpp.size;
					tmpp.size = new Vector2(Mathf.Abs(tmpp.size.x), Mathf.Abs(tmpp.size.y));
					tmpp.size = EditorGUILayout.Vector2Field("Size : ", tmpp.size, GUILayout.Width (300));
					if(tmpp.size.x != oldSize.x)
					{
						//xchanged = true;
					}
					if(tmpp.size.y != oldSize.y)
					{
						//ychanged = true;
					}
					this.target_.pathWays[i] = tmpp;
				GUI.enabled = oldEnable;
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
	                this.target_.keyTypes[i] = (KeyType)EditorGUILayout.EnumPopup("Key Type : ", this.target_.keyTypes[i]);
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = (this.target_.keyTypes[i] == KeyType.WAITTIME);
	                this.target_.waits[i] = (float)EditorGUILayout.FloatField("Wait : ", this.target_.waits[i]);
				GUI.enabled = oldEnable;
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.BeginHorizontal();
				GUI.enabled = (this.target_.keyTypes[i] == KeyType.DOSOMETHING);
	         	    int index;
		            try
		            {
		                index = methods
		                    .Select((vvv, iii) => new { Name = vvv, Index = iii })
		                    .First(x => x.Name == this.target_.functionNames[i])
		                    .Index;
		            }
		            catch
					{
		                index = 0;
					}
	                this.target_.functionNames[i] = (methods.Length > 0 ? 
												methods[EditorGUILayout.Popup("Call Function : ", index, methods)] : 
												EditorGUILayout.TextField("Call Function : ", "No Function"));
				GUI.enabled = oldEnable;
				EditorGUILayout.EndHorizontal();
			
				EditorGUILayout.Space();
			
				if(this.target_.lp.Count > 1)
				{
					updateTrajectoryFromNewPath(i);
					ChangePosition(i, oldPosition);
					replaceNextFromTrajectory(i);
				}
				EditorGUILayout.EndVertical();
			}
		EditorGUILayout.EndVertical();
		
		EditorGUILayout.Space();
		EditorGUILayout.Space();
		EditorGUILayout.BeginVertical("Box");
			this.newTransform = EditorGUILayout.ObjectField("New Node", this.newTransform, typeof(Transform), true) as Transform;
			this.newTransformName = EditorGUILayout.TextField("New Node Name", this.newTransformName);
			GUI.enabled = (this.newTransform != null || this.newTransformName != "");
			if(GUILayout.Button("Add"))
			{
				addPath();
			}
			GUI.enabled = oldEnable;
		EditorGUILayout.EndVertical();
		EditorGUILayout.Space();
		EditorGUILayout.Space();
	}
	
	void swap(int index1, int index2)
	{
		Transform tmp = this.target_.lp[index1];
		this.target_.lp[index1] = this.target_.lp[index2];
		this.target_.lp[index2] = tmp;
		
		Trajectoire tmt = this.target_.trajectoires[index1];
		this.target_.trajectoires[index1] = this.target_.trajectoires[index2];
		this.target_.trajectoires[index2] = tmt;
		
		PathType tmpt = this.target_.pathTypes[index1];
		this.target_.pathTypes[index1] = this.target_.pathTypes[index2];
		this.target_.pathTypes[index2] = tmpt;
		
		KeyPoint tmpw = this.target_.pathWays[index1];
		this.target_.pathWays[index1] = this.target_.pathWays[index2];
		this.target_.pathWays[index2] = tmpw;
		
		KeyType tmpk = this.target_.keyTypes[index1];
		this.target_.keyTypes[index1] = this.target_.keyTypes[index2];
		this.target_.keyTypes[index2] = tmpk;
		
		float tmpwait = this.target_.waits[index1];
		this.target_.waits[index1] = this.target_.waits[index2];
		this.target_.waits[index2] = tmpwait;
		
		String tmps = this.target_.functionNames[index1];
		this.target_.functionNames[index1] = this.target_.functionNames[index2];
		this.target_.functionNames[index2] = tmps;
		
		this.target_.p = this.target_.lp.ToArray();
		
		setChangeDirty = true;
	}
	
	void addPath()
	{
		if(this.newTransform == null)
		{
			GameObject tmpGO = new GameObject(this.newTransformName);
			tmpGO.transform.parent = this.target_.transform;
			this.newTransform = tmpGO.transform;
			if(this.target_.lp.Count == 0)
			{
				this.newTransform.position = new Vector3(0, 0, 0);
			}
			else
			{
				this.newTransform.position = this.target_.lp[this.target_.lp.Count-1].position;
				this.newTransform.position += new Vector3(this.target_.pathWays[this.target_.lp.Count-1].size.x, this.target_.pathWays[this.target_.lp.Count-1].size.y, 0);
			}
		}
	
		this.target_.lp.Add(this.newTransform);
		this.newTransform = null;
		this.newTransformName = "";
		
		this.target_.trajectoires.Add(Trajectoire.LINE);
		this.target_.pathTypes.Add(PathType.HORIZONTALLTOR);
		
		if(this.target_.lp.Count == 1)
			this.target_.lp[this.target_.lp.Count-1].position = this.target_.transform.position;
		else
			this.target_.lp[this.target_.lp.Count-1].position = new Vector3(this.target_.lp[this.target_.lp.Count-1].position.x, this.target_.lp[this.target_.lp.Count-2].position.y, this.target_.lp[this.target_.lp.Count-1].position.z);
	
		KeyPoint tmpw = new KeyPoint();
		tmpw.type = PathType.HORIZONTALLTOR;
		tmpw.position = this.target_.lp[this.target_.lp.Count-1].position;
		tmpw.trajectoire = Trajectoire.LINE;
		if(this.target_.lp.Count == 0)
			tmpw.size = new Vector2(10, 0);
		else
			tmpw.size = new Vector2(this.target_.lp[this.target_.lp.Count-1].position.x - this.target_.lp[this.target_.lp.Count-2].position.x, 0);
		this.target_.pathWays.Add(tmpw);
		
		this.target_.keyTypes.Add(KeyType.NOTYPE);
		
		this.target_.waits.Add(0.0f);
		
		this.target_.functionNames.Add("");
		
		this.target_.p = this.target_.lp.ToArray();
		
		setChangeDirty = true;
	}
	
	void removePath(int val)
	{
        foreach (Transform child in this.target_.gameObject.transform) 
		{
			if(child.gameObject.ToString() == this.target_.lp[val].gameObject.ToString())
			{
				DestroyImmediate(child.gameObject);
				break;
			}
        }
		this.target_.lp.RemoveAt(val);
		this.target_.trajectoires.RemoveAt(val);
		this.target_.pathTypes.RemoveAt(val);
		this.target_.keyTypes.RemoveAt(val);
		this.target_.pathWays.RemoveAt(val);
		this.target_.waits.RemoveAt(val);
		this.target_.functionNames.RemoveAt(val);
		
		this.target_.p = this.target_.lp.ToArray();
		
		setChangeDirty = true;
	}
	
	void updateTrajectoryFromNewPath(int _value)
	{
		if(this.target_.pathTypes[_value] <= PathType.DIAGONALBRTOTL)
		{
			this.target_.trajectoires[_value] = Trajectoire.LINE;
		}
		else
		{
			this.target_.trajectoires[_value] = Trajectoire.CIRCLE;
		}
	}
	
	void ChangePosition(int _value, Vector3 _old)
	{
		float xmodif = this.target_.lp[_value].position.x - _old.x;
		float ymodif = this.target_.lp[_value].position.y - _old.y;
		
		KeyPoint tmpPath = new KeyPoint();
		tmpPath = this.target_.pathWays[_value];
		tmpPath.size += new Vector2(xmodif, ymodif);
		this.target_.pathWays[_value] = tmpPath;
		
		bool xmodified = (xmodif == 0);
		bool ymodified = (ymodif == 0);
		if(xmodif == 0 && ymodif == 0 && _value < this.target_.lp.Count-1
			&& (this.target_.lp[_value].position.x + this.target_.pathWays[_value].size.x != this.target_.lp[_value+1].position.x
			|| this.target_.lp[_value].position.y + this.target_.pathWays[_value].size.y != this.target_.lp[_value+1].position.y))
		{
			xmodified = (this.target_.lp[_value].position.x + this.target_.pathWays[_value].size.x != this.target_.lp[_value+1].position.x);
			ymodified = (this.target_.lp[_value].position.y + this.target_.pathWays[_value].size.y != this.target_.lp[_value+1].position.y);
		}
		
		verifySizeFromTrajectory(_value, xmodified, ymodified);
		
		if(_value == this.target_.lp.Count-2)
			return;
		
		MoveNexts(_value, this.target_.lp[_value].position - _old);
	}
	
	void verifySizeFromTrajectory(int _value, bool _xmodif, bool _ymodif)
	{
		float xsize = this.target_.pathWays[_value].size.x;
		float ysize = this.target_.pathWays[_value].size.y;
		
		KeyPoint tmpPath = this.target_.pathWays[_value];
		
		if(this.target_.pathTypes[_value] == PathType.HORIZONTALLTOR
			|| this.target_.pathTypes[_value] == PathType.HORIZONTALRTOL)
		{
			tmpPath.size = new Vector2(xsize, 0);
		}
		else if(this.target_.pathTypes[_value] == PathType.VERTICALBTOT
			|| this.target_.pathTypes[_value] == PathType.VERTICALTTOB)
		{
			tmpPath.size = new Vector2(0, ysize);
		}
		else if(this.target_.pathTypes[_value] != PathType.CUSTOMLINE)
		{
			if(_xmodif)
				tmpPath.size = new Vector2(xsize, xsize);
			else if(_ymodif)
				tmpPath.size = new Vector2(ysize, ysize);
		}
		this.target_.pathWays[_value] = tmpPath;
	}
	
	void replaceNextFromTrajectory(int _value)
	{
		if(_value >= this.target_.lp.Count-1)
			return;
		
		Vector3 tmpPos = this.target_.lp[_value+1].position;
		
		float directionValueX, directionValueY;
		if(this.target_.pathTypes[_value] == PathType.CIRCLEBTOL
		   || this.target_.pathTypes[_value] == PathType.CIRCLETTOL
		   || this.target_.pathTypes[_value] == PathType.CIRCLERTOB
		   || this.target_.pathTypes[_value] == PathType.CIRCLERTOT
		   || this.target_.pathTypes[_value] == PathType.HORIZONTALRTOL
		   || this.target_.pathTypes[_value] == PathType.DIAGONALTRTOBL
		   || this.target_.pathTypes[_value] == PathType.DIAGONALBRTOTL)
		{
			directionValueX = -1.0f;
		}
		else
		{
			directionValueX = 1.0f;
		}
		if(this.target_.pathTypes[_value] == PathType.CIRCLETTOL
		   || this.target_.pathTypes[_value] == PathType.CIRCLETTOR
		   || this.target_.pathTypes[_value] == PathType.CIRCLERTOB
		   || this.target_.pathTypes[_value] == PathType.CIRCLELTOB
		   || this.target_.pathTypes[_value] == PathType.VERTICALTTOB
		   || this.target_.pathTypes[_value] == PathType.DIAGONALTRTOBL
		   || this.target_.pathTypes[_value] == PathType.DIAGONALTLTOBR)
		{
			directionValueY = -1.0f;
		}
		else
		{
			directionValueY = 1.0f;
		}
		
		this.target_.lp[_value+1].position = new Vector3(
			this.target_.lp[_value].position.x + this.target_.pathWays[_value].size.x * directionValueX,
			this.target_.lp[_value].position.y + this.target_.pathWays[_value].size.y * directionValueY,
			this.target_.lp[_value].position.z
		);
		
		MoveNexts(_value+1, tmpPos);
	}
	
	void FixeAllSizePath()
	{
		for(int i = 0; i < this.target_.lp.Count-1; ++i)
		{
			Vector3 oldSize = new Vector3(
				Mathf.Abs(this.target_.lp[i+1].position.x - this.target_.lp[i].position.x),
				Mathf.Abs(this.target_.lp[i+1].position.y - this.target_.lp[i].position.y),
				0
			);
			
			KeyPoint tmpPath = this.target_.pathWays[i];
			tmpPath.size = new Vector2(this.target_.pathSize, this.target_.pathSize);
			this.target_.pathWays[i] = tmpPath;
			float tmpX = this.target_.lp[i].position.x;
			float tmpY = this.target_.lp[i].position.y;
			//float tmpZ = this.target_.lp[i].position.z;
			float tmpXp = this.target_.lp[i+1].position.x;
			float tmpYp = this.target_.lp[i+1].position.y;
			float tmpZp = this.target_.lp[i+1].position.z;
			
			if(tmpXp > tmpX)
			{
				this.target_.lp[i+1].position = new Vector3(tmpX + this.target_.pathSize, tmpYp, tmpZp);
			}
			if(tmpXp < tmpX)
			{
				this.target_.lp[i+1].position = new Vector3(tmpX - this.target_.pathSize, tmpYp, tmpZp);
			}
			
			if(tmpYp > tmpY)
			{
				this.target_.lp[i+1].position = new Vector3(tmpXp, tmpY + this.target_.pathSize, tmpZp);
			}
			if(tmpYp < tmpY)
			{
				this.target_.lp[i+1].position = new Vector3(tmpXp, tmpY - this.target_.pathSize, tmpZp);
			}
			
			Vector3 newSize = new Vector3(
				Mathf.Abs(this.target_.lp[i+1].position.x - this.target_.lp[i].position.x),
				Mathf.Abs(this.target_.lp[i+1].position.y - this.target_.lp[i].position.y),
				0
			);
			
			MoveNexts(i, newSize-oldSize);
		}
	}
	
	void MoveNexts(int _value, Vector3 _offset)
	{
		for(int i = _value+1; i < this.target_.lp.Count; ++i)
		{
			this.target_.lp[i].position += _offset;
		}
	}
}
