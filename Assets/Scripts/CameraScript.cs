using UnityEngine;
using System.Collections;

public class CameraScript : MonoBehaviour
{
	private float smoothTime;
	private Vector3 velocity;
	private Vector3 lastPos;
	private float limitYMax;
	private float limitYMin;
	private int doDecal;
	private static Coroutine c;

	void Start ()
	{
		smoothTime = GlobalVarScript.instance.cameraSmoothDefault;
		velocity = Vector3.zero;
		lastPos = Vector3.zero;
		limitYMax = 0.9f;
		limitYMin = 0.35f;
		doDecal = 0;
	}
	
	void FixedUpdate ()
	{
		smoothTime = GlobalVarScript.instance.cameraSmooth;
		Vector3 point = camera.WorldToViewportPoint(GlobalVarScript.instance.cameraTarget.position);
        Vector3 delta = GlobalVarScript.instance.cameraTarget.position - camera.ViewportToWorldPoint(new Vector3(0.5f, -1.5f, 0.5f));
        Vector3 destination = transform.position + delta;
		destination.z -= GlobalVarScript.instance.cameraZOffset;
		if (GlobalVarScript.instance.cameraFixedPos != Vector3.zero)
		{
			destination.y = GlobalVarScript.instance.cameraFixedPos.y;
		}
		Vector3 smoothPos = Vector3.SmoothDamp(transform.position, destination, ref velocity, this.smoothTime);
		
		// cas particulier
		if (GlobalVarScript.instance.cameraTarget.tag == "Target")
		{
			// augmente la vitesse de smooth quand le perso est proche du bord de l'ecran
			if (lastPos.y > GlobalVarScript.instance.cameraTarget.position.y)
			{
				// cible en train de tomber
				if (doDecal == 2)
				{
					float posDecal = lastPos.y - GlobalVarScript.instance.cameraTarget.position.y;
					float camDecal = posDecal * 1.5f;
					Vector3 newPos = new Vector3(smoothPos.x, transform.position.y - (point.y < limitYMax ? camDecal : posDecal), smoothPos.z);
					transform.position = newPos;
					GlobalVarScript.instance.cameraFixedPos = Vector3.zero;
				}
				else if (doDecal == 1)
				{
					transform.position = smoothPos;
				}
				else
				{
					transform.position = smoothPos;
				}
			}
			else if (point.y > limitYMax && GlobalVarScript.instance.cameraTarget.position.y > lastPos.y)
			{
				// cible atteint haut ecran
				Vector3 newPos = new Vector3(smoothPos.x, transform.position.y + GlobalVarScript.instance.cameraTarget.position.y - lastPos.y, smoothPos.z);
				transform.position = newPos;
				GlobalVarScript.instance.cameraFixedPos = Vector3.zero;
			}
			else
			{
				transform.position = smoothPos;
			}
			
			if (doDecal == 0 && point.y < limitYMin && lastPos.y > GlobalVarScript.instance.cameraTarget.position.y)
			{
				doDecal = 2;
			}
		}
		else if (c == null)
		{
			c = StartCoroutine(MoveOverTime(transform, destination, smoothTime));
		}
		
		lastPos = GlobalVarScript.instance.cameraTarget.position;
	}
	
	public void ResetFall()
	{
		this.doDecal = 1;
	}
	
	public void Reset()
	{
		this.doDecal = 0;
	}
	
	private static IEnumerator MoveOverTime(Transform theTransform, Vector3 d, float t)
	{
		float rate = (float) (1.0/t);
		float index = 0;
	
		Vector3 startPosition = theTransform.position;
		Vector3 endPosition = d;
		endPosition.z = -10f;
	
		while( index < 1.0 )
		{
			theTransform.position = Vector3.Lerp( startPosition, endPosition, index );
			index += rate * Time.deltaTime;
			yield return true;
		}
		
		theTransform.position = endPosition;
		c = null;
	}
	
	private void ResetPosition(Vector3 pos)
	{
		this.transform.position = pos;
		this.lastPos = pos;
	}
}
