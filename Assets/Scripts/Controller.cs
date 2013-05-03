using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Controller : MonoBehaviour {
	
	private GUITexture gui;	
	
	private bool down;

	void Start ()
	{
		this.gui = gameObject.GetComponent<GUITexture>();
		Color guiColor = new Color(this.gui.color.r, this.gui.color.g, this.gui.color.b, 0.1f);
		this.gui.color = guiColor;
		
		this.down = false;
		
		Input.multiTouchEnabled = true;
		
		//float decal = (gui.pixelInset.width > gui.pixelInset.x ? gui.pixelInset.width : gui.pixelInset.x) + 20f;
	}
	
	void Update ()
	{		
		if (Input.touchCount > 0) 
		{
			foreach (Touch touch in Input.touches)
			{
				checkInput(touch);
			}
		}
		else
		{
			resetTouch();
		}
	}
	
	void checkInput(Touch touch)
	{
		if (this.gui.HitTest(touch.position))
		{
			if (this.gui.name == "ButtonRestart")
			{
				Application.LoadLevel(Application.loadedLevel);
			}
			colorButton();
			this.down = true;
		}
	}
	
	void uncolorButton()
	{
		Color guiColor = new Color(this.gui.color.r, this.gui.color.g, this.gui.color.b, 0.1f);
		this.gui.color = guiColor;
	}
	
	void colorButton()
	{	
		Color guiColor = new Color(this.gui.color.r, this.gui.color.g, this.gui.color.b, 0.5f);
		this.gui.color = guiColor;
	}
	
	public bool isDown()
	{
		return this.down;
	}
	
	private void resetTouch()
	{
		uncolorButton();
		this.down = false;
	}
}
