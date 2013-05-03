using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

public class RoadData 
{
	EndBehaviour      endBehaviour;
	Deplacement       deplacement;
	public Activation activation;
	List<KeyPoint>    keyPoints;

	public Vector3 currentPosition;
	public bool    endOfRoad = false;
	
	Vector3 circleCenter;
	Vector3 startPosition;
	Vector3 endPosition;
	float   speedTmp;
	float   currentAngle;
	
	float vx, vy, va;
	int   currentPath;
	float totalTime;
	float currentTime;
	float pathLength;
	float currentTraveled;
	float lastTraveled;
	bool  pause;
	bool  stop;
	float currentWait;
	
	public void reInit()
	{
		currentPosition = keyPoints[0].position;
		endOfRoad    = false;
		vx = vy = va = 0.0f;
		currentPath  = -1;
		currentTime  = 0.0f;
		currentTraveled = 0.0f;
		lastTraveled = 0.0f;
		pause = false;
		stop = false;
	}
	
	public void initWithDatas(EndBehaviour _endBehaviour, Deplacement _deplacement, Activation _activation, List<KeyPoint> _keyPoints, float _speedTmp)
	{
		endBehaviour = _endBehaviour;
		deplacement  = _deplacement;
		keyPoints    = _keyPoints;
		activation   = _activation;
		
		speedTmp     = _speedTmp;
		totalTime    = _speedTmp;
		pathLength   = getPathLength();
		
		reInit();
	}
	
	public RoadData getReverse()
	{
		List<KeyPoint> _keyPoints = new List<KeyPoint>();
		for(int i = 0; i < keyPoints.Count; ++i)
		{
			KeyPoint _tmpKey = keyPoints[keyPoints.Count-1-i];
			_tmpKey.type     = (i == keyPoints.Count-1 ? keyPoints[0].type : getReversedPathOf(keyPoints[keyPoints.Count-1-i-1].type));
			_tmpKey.size     = (i < keyPoints.Count-1 ? keyPoints[keyPoints.Count-1-i-1].size : new Vector2(0, 0));
			_tmpKey.position = keyPoints[keyPoints.Count-1-i].position;
			_tmpKey.trajectoire = (i == keyPoints.Count-1 ? keyPoints[0].trajectoire : keyPoints[keyPoints.Count-1-i-1].trajectoire);
			_keyPoints.Add(_tmpKey);
		}
		RoadData _roadReverse = new RoadData();
		_roadReverse.initWithDatas(endBehaviour, deplacement, activation, _keyPoints, speedTmp);
		return _roadReverse;
	}
	
	public void updateRoad()
	{
		if(this.stop)
		{
			return;
		}
		if(this.pause)
		{
			updateWaitPause();
			return;
		}
		if(currentPath >= this.keyPoints.Count-1)
			return;
		if(endOfRoad)
			return;
		if(currentPath == -1)
			CheckNewPath();
		if(currentPath == -1)
			return;
			
		setVelocities();
			
		followTheRoad();
	}
	
	float getPathLength()
	{
		float _res = 0;
		
		for(int i = 0; i < this.keyPoints.Count-1; ++i)
		{
			if((int)this.keyPoints[i].type < 4)
			{
				_res += this.keyPoints[i].size.x + this.keyPoints[i].size.y;
			}
			else if((int)this.keyPoints[i].type < 8)
			{
				_res += Mathf.Sqrt(this.keyPoints[i].size.x * this.keyPoints[i].size.x + this.keyPoints[i].size.y * this.keyPoints[i].size.y);
			}
			else if((int)this.keyPoints[i].type < 16)
			{
				_res += Mathf.PI * this.keyPoints[i].size.x / 2.0f;
			}
			else
			{
				_res += this.keyPoints[i].size.x + this.keyPoints[i].size.y;
			}
		}
		return _res;
	}
	
	void setVelocities()
	{
		currentTime += Time.deltaTime;
		lastTraveled = currentTraveled;
		updatePosition();
		
		vx = currentTraveled-lastTraveled;
		vy = currentTraveled-lastTraveled;
		va = currentTraveled-lastTraveled;
		if(this.keyPoints[currentPath].trajectoire == Trajectoire.LINE)
		{
			float ratio = Vector3.Distance(this.keyPoints[currentPath].position, this.keyPoints[currentPath+1].position)
						  / (Mathf.Abs(this.keyPoints[currentPath+1].position.x - this.keyPoints[currentPath].position.x)
						  + Mathf.Abs(this.keyPoints[currentPath+1].position.y - this.keyPoints[currentPath].position.y));
			vx *= ratio;
			if(this.keyPoints[currentPath+1].position.x == this.keyPoints[currentPath].position.x)
				vx = 0;
			vy *= ratio;
			if(this.keyPoints[currentPath+1].position.y == this.keyPoints[currentPath].position.y)
				vy = 0;
			va = 0;
		}
		else
		{
			va = va * 180.0f / (Mathf.PI * Mathf.Abs(this.keyPoints[currentPath+1].position.x - this.keyPoints[currentPath].position.x));
			vx = 0;
			vy = 0;
		}
	}
	
	void updatePosition()
	{
		if(this.deplacement == Deplacement.LINEAR)
		{
			currentTraveled = pathLength * currentTime / totalTime;
		}
		else if(this.deplacement == Deplacement.SMOOTH)
		{
			currentTraveled = smoothStep(0.0f, pathLength, currentTime/totalTime);
		}
		else if(this.deplacement == Deplacement.COSINUS)
		{
			currentTraveled = cosinus(0.0f, pathLength, currentTime/totalTime);
		}
		else if(this.deplacement == Deplacement.ACCELERATE)
		{
			currentTraveled = acceleration(0.0f, pathLength, currentTime/totalTime);
		}
		else if(this.deplacement == Deplacement.DECELERATE)
		{
			currentTraveled = deceleration(0.0f, pathLength, currentTime/totalTime);
		}
	}
	
	void followTheRoad()
	{
		if(this.keyPoints[currentPath].trajectoire == Trajectoire.LINE)
		{
			if(this.keyPoints[currentPath+1].position.x != this.keyPoints[currentPath].position.x
				&& this.keyPoints[currentPath+1].position.y != this.keyPoints[currentPath].position.y)
			{
				currentPosition.x += vx * (this.keyPoints[currentPath+1].position.x > this.keyPoints[currentPath].position.x ? 1.0f : -1.0f);
				currentPosition.y += vy * (this.keyPoints[currentPath+1].position.y > this.keyPoints[currentPath].position.y ? 1.0f : -1.0f);
				if((currentPosition.x >= this.keyPoints[currentPath+1].position.x && this.keyPoints[currentPath+1].position.x > this.keyPoints[currentPath].position.x)
				   || (currentPosition.x <= this.keyPoints[currentPath+1].position.x && this.keyPoints[currentPath+1].position.x < this.keyPoints[currentPath].position.x)
				   || currentTime >= totalTime)
				{	
					CheckNewPath();
				}
			}
			else if(this.keyPoints[currentPath+1].position.x != this.keyPoints[currentPath].position.x)
			{
				currentPosition.x += vx * (this.keyPoints[currentPath+1].position.x > this.keyPoints[currentPath].position.x ? 1.0f : -1.0f);
				if((currentPosition.x >= this.keyPoints[currentPath+1].position.x && this.keyPoints[currentPath+1].position.x > this.keyPoints[currentPath].position.x)
				   || (currentPosition.x <= this.keyPoints[currentPath+1].position.x && this.keyPoints[currentPath+1].position.x < this.keyPoints[currentPath].position.x)
				   || currentTime >= totalTime)
				{	
					CheckNewPath();
				}
			}
			else if(this.keyPoints[currentPath+1].position.y != this.keyPoints[currentPath].position.y)
			{
				currentPosition.y += vy * (this.keyPoints[currentPath+1].position.y > this.keyPoints[currentPath].position.y ? 1.0f : -1.0f);
				if((currentPosition.y >= this.keyPoints[currentPath+1].position.y && this.keyPoints[currentPath+1].position.y > this.keyPoints[currentPath].position.y)
				   || (currentPosition.y <= this.keyPoints[currentPath+1].position.y && this.keyPoints[currentPath+1].position.y < this.keyPoints[currentPath].position.y)
				   || currentTime >= totalTime)
				{
					CheckNewPath();
				}
			}
		}
		else
		{
			switch(keyPoints[currentPath].type)
			{
				case PathType.CIRCLEBTOL :
					this.currentAngle -= this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle <= 180.0f)
					{
						this.currentAngle = 180.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLEBTOR :
					this.currentAngle += this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle >= 360.0f)
					{
						this.currentAngle = 360.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLETTOL :
					this.currentAngle += this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle >= 180.0f)
					{
						this.currentAngle = 180.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLETTOR :
					this.currentAngle -= this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle <= 0.0f)
					{
						this.currentAngle = 0.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLELTOT :
					this.currentAngle -= this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle <= 90.0f)
					{
						this.currentAngle = 90.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLELTOB :
					this.currentAngle += this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle >= 270.0f)
					{
						this.currentAngle = 270.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLERTOT :
					this.currentAngle += this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle >= 90.0f)
					{
						this.currentAngle = 90.0f;
						CheckNewPath();
					}
					break;
				case PathType.CIRCLERTOB :
					this.currentAngle -= this.va;
					this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
					if(this.currentAngle <= 270.0f)
					{
						this.currentAngle = 270.0f;
						CheckNewPath();
					}
					break;
				default :
					break;
			}
			if(this.currentPath < this.keyPoints.Count && this.keyPoints[currentPath].trajectoire != Trajectoire.LINE)
			{
				this.currentPosition = 
				   this.circleCenter + 
				   new Vector3(
						Mathf.Cos(this.currentAngle/180.0f*Mathf.PI) * this.keyPoints[currentPath].size.x,
						Mathf.Sin(this.currentAngle/180.0f*Mathf.PI) * this.keyPoints[currentPath].size.y,
						0
				   );
			}
		}
	}
	
	void CheckNewPath()
	{
		if(currentPath == -1)
			currentPath = 0;
		else if(currentPath+1 >= this.keyPoints.Count-1)
		{
			if(this.endBehaviour == EndBehaviour.STOP)
			{
				this.stop = true;
				return;
			}
			else if(this.endBehaviour == EndBehaviour.RESTART)
			{
				currentPosition = keyPoints[0].position;
				currentPath = 0;
				currentAngle = 0.0f;
				currentTime = 0.0f;
				currentTraveled = 0.0f;
				currentWait = 0.0f;
				return;
			}
			else if(this.endBehaviour == EndBehaviour.LOOP)
			{
				endOfRoad = true;
				return;
			}
			else
			{
				return;
			}
		}
		else
			currentPath++;
		
		switch(keyPoints[currentPath].type)
		{
			case PathType.CIRCLEBTOL :
				this.currentAngle = 270;
				this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLEBTOR :
				this.currentAngle = 270;
				this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLETTOL :
				this.currentAngle = 90;
				this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLETTOR :
				this.currentAngle = 90;
				this.circleCenter = new Vector3(this.keyPoints[currentPath].position.x, this.keyPoints[currentPath+1].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLELTOT :
				this.currentAngle = 180.0f;
				this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLELTOB :
				this.currentAngle = 180.0f;
				this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLERTOT :
				this.currentAngle = 0.0f;
				this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
				break;
			case PathType.CIRCLERTOB :
				this.currentAngle = 360.0f;
				this.circleCenter = new Vector3(this.keyPoints[currentPath+1].position.x, this.keyPoints[currentPath].position.y, this.keyPoints[currentPath].position.z);
				break;
			default :
				break;
		}
		checkKeyPointType();
	}
	
	float step(float alpha, float beta, float teta)
	{
		return (teta < .5 ? alpha : beta);
	}
	
	float linear(float alpha, float beta, float teta)
	{
		return alpha + teta * (beta - alpha);
	}
	
	float cosinus(float alpha, float beta, float teta)
	{
		return linear(alpha, beta, (float)(-Mathf.Cos(Mathf.PI * teta) / 2.0 + .5));
	}
	
	float acceleration(float alpha, float beta, float teta)
	{
		return linear(alpha, beta, teta * teta);
	}
	
	float deceleration(float alpha, float beta, float teta)
	{
		return linear(alpha, beta, 1 - (1 - teta * teta) * (1 - teta * teta));
	}
	
	float smoothStep(float alpha, float beta, float teta)
	{	
		return Mathf.SmoothStep(alpha, beta, teta);
	}
	
	PathType getReversedPathOf(PathType _path)
	{
		PathType _result;
		switch(_path)
		{
			case PathType.HORIZONTALRTOL :
				_result = PathType.HORIZONTALLTOR;
			break;
			case PathType.HORIZONTALLTOR : 
				_result = PathType.HORIZONTALRTOL;
			break;
			case PathType.VERTICALTTOB : 
				_result = PathType.VERTICALBTOT;
			break;
			case PathType.VERTICALBTOT : 
				_result = PathType.VERTICALTTOB;
			break;
			case PathType.DIAGONALTLTOBR : 
				_result = PathType.DIAGONALBRTOTL;
			break;
			case PathType.DIAGONALTRTOBL : 
				_result = PathType.DIAGONALBLTOTR;
			break;
			case PathType.DIAGONALBLTOTR : 
				_result = PathType.DIAGONALTRTOBL;
			break;
			case PathType.DIAGONALBRTOTL : 
				_result = PathType.DIAGONALTLTOBR;
			break;
			case PathType.CIRCLETTOR : 
				_result = PathType.CIRCLERTOT;
			break;
			case PathType.CIRCLERTOB : 
				_result = PathType.CIRCLEBTOR; 
			break;
			case PathType.CIRCLEBTOL : 
				_result = PathType.CIRCLELTOB; 
			break;
			case PathType.CIRCLELTOT : 
				_result = PathType.CIRCLETTOL; 
			break;
			case PathType.CIRCLETTOL : 
				_result = PathType.CIRCLELTOT; 
			break;
			case PathType.CIRCLELTOB : 
				_result = PathType.CIRCLEBTOL; 
			break;
			case PathType.CIRCLEBTOR : 
				_result = PathType.CIRCLERTOB; 
			break;
			case PathType.CIRCLERTOT : 
				_result = PathType.CIRCLETTOR;
			break;
			default :
				_result = PathType.CUSTOMLINE;
			break;
		}
		return _result;
	}
	
	public void pauseRoad()
	{
		this.pause = true;
	}
	
	public void playRoad()
	{
		this.pause = false;
	}
	
	public void checkKeyPointType()
	{
		switch(this.keyPoints[this.currentPath].keyType)
		{
			case KeyType.WAITTIME :
				this.pauseRoad();
				this.currentWait = 0.0f;
			break;
			
			case KeyType.WAITMANUAL :
				this.pauseRoad();
			break;
			
			case KeyType.DOSOMETHING :
				typeof(Road)
            		.GetMethod(this.keyPoints[this.currentPath].functionName, BindingFlags.Instance |BindingFlags.NonPublic | BindingFlags.Public)
            		.Invoke(this, new object[0]);
			break;
			
			default :
			break;
		}
		
	}
	
	public void updateWaitPause()
	{
		this.currentWait += Time.deltaTime;
		if(this.currentWait >= this.keyPoints[this.currentPath].wait)
			this.playRoad();
	}
}