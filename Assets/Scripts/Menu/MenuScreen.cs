using UnityEngine;
using System.Collections;

public abstract class MenuScreen : MonoBehaviour 
{
	public bool exitScreen = false;
	public bool loadLevel = false;
	
	public MenuGesture.ScreenMenu screenToGo = MenuGesture.ScreenMenu.NONE;
	
	abstract public void activateMenu();
	abstract public void desactivateMenu();
}
