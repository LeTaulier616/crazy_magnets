using UnityEngine;
using System.Collections;

	
// Trajectoires Enumerator
public enum Trajectoire {
	LINE,
	CIRCLE
};

// Mode d'activation
public enum Activation {
	AUTO,
	PLAYER,
	INTERUPTEUR,
	ONSCREEN,
	MANUAL
};

// Mode de déplacement
public enum Deplacement {
	SMOOTH,
	COSINUS,
	LINEAR,
	ACCELERATE,
	DECELERATE
};

// Comportement de fin de trajectoire
public enum EndBehaviour {
	LOOP,
	RESTART,
	GOBACK,
	WAIT,
	STOP,
	FALL,
	JUMP
};

// Path Enumerator
public enum PathType {
	HORIZONTALRTOL,
	HORIZONTALLTOR,
	VERTICALTTOB,
	VERTICALBTOT,
	DIAGONALTLTOBR,
	DIAGONALTRTOBL,
	DIAGONALBLTOTR,
	DIAGONALBRTOTL,
	CIRCLETTOR,
	CIRCLERTOB,
	CIRCLEBTOL,
	CIRCLELTOT,
	CIRCLETTOL,
	CIRCLELTOB,
	CIRCLEBTOR,
	CIRCLERTOT,
	CUSTOMLINE
};

// Key Types
public enum KeyType {
	NOTYPE,
	WAITTIME,
	WAITMANUAL,
	DOSOMETHING,
	WAITPLAYER,
};

// KeyPoints of the Road
public struct KeyPoint 
{
	public Trajectoire trajectoire;
	public PathType type;
	public Vector3 position;
	public Vector2 size;
	public KeyType keyType;
	public float wait;
	public string functionName;
};
