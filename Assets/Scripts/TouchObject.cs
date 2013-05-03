using UnityEngine;
using System.Collections;

public class TouchObject
{
	private int id_;
	public int id
	{
		get { return this.id_; }
		set { this.id_ = value; }
	}
	
	private Touch touch_;
	public Touch touch
	{
		get { return this.touch_; }
		set { this.touch_ = value; }
	}
	
	private GameObject selectedObject_;
	public GameObject selectedObject
	{
		get { return this.selectedObject_; }
		set { this.selectedObject_ = value; }
	}
	
	private bool leftTouched_;
	public bool leftTouched
	{
		get { return this.leftTouched_; }
		set { this.leftTouched_ = value; }
	}
	
	private bool rightTouched_;
	public bool rightTouched
	{
		get { return this.rightTouched_; }
		set { this.rightTouched_ = value; }
	}
	
	private Vector2 startPos_;
	public Vector2 startPos
	{
		get { return this.startPos_; }
		set { this.startPos_ = value; }
	}
	
	private float startTime_;
	public float startTime
	{
		get { return this.startTime_; }
		set { this.startTime_ = value; }
	}
	
	private float slideStart_;
	public float slideStart
	{
		get { return this.slideStart_; }
		set { this.slideStart_ = value; }
	}
	
	public TouchObject(int id)
	{
		this.id = id;
	}
	
	public TouchObject(int id, Touch touch)
	{
		this.id = id;
		this.touch = touch;
		this.leftTouched = false;
		this.rightTouched = false;
	}
}
