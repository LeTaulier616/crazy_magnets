using UnityEngine;
using System.Collections;

public abstract class MenuScreen : MonoBehaviour 
{
	public bool exitScreen = false;
	public MenuGesture.ScreenMenu screenToGo = MenuGesture.ScreenMenu.NONE;
	public bool loadLevel  = false;
	
	abstract public void activateMenu();
	abstract public void desactivateMenu();
}
