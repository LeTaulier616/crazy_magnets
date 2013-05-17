using UnityEngine;
using System.Collections;

public static class MyDefines 
{
	public static int kNbWorlds          = 4;
	public static int kLevelsByWorld     = 5;
	public static int kNbLevels          = kNbWorlds * kLevelsByWorld;
	public static int kNbScrewsByLevel   = 3;
	public static int kNbScrews          = kNbScrewsByLevel * kNbLevels;
	public static int kNbLevelsAvailable = 5;
	
	public static bool developmentMode   = true;
}
