using UnityEngine;
using System.Collections;


[System.Serializable]
public class QuadBez {
	public Vector3 st, en, ctrl;
	
	public QuadBez(Vector3 st, Vector3 en, Vector3 ctrl) {
		this.st = st;
		this.en = en;
		this.ctrl = ctrl;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * st + 2f * d * t * ctrl + t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (2f * st - 4f * ctrl + 2f * en) * t + 2f * ctrl - 2f * st;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl);
		Gizmos.DrawLine(ctrl, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
	

}


[System.Serializable]
public class CubicBez {
	public Vector3 st, en, ctrl1, ctrl2;
	
	public CubicBez(Vector3 st, Vector3 en, Vector3 ctrl1, Vector3 ctrl2) {
		this.st = st;
		this.en = en;
		this.ctrl1 = ctrl1;
		this.ctrl2 = ctrl2;
	}
	
	
	public Vector3 Interp(float t) {
		float d = 1f - t;
		return d * d * d * st + 3f * d * d * t * ctrl1 + 3f * d * t * t * ctrl2 + t * t * t * en;
	}
	
	
	public Vector3 Velocity(float t) {
		return (-3f * st + 9f * ctrl1 - 9f * ctrl2 + 3f * en) * t * t
			+  (6f * st - 12f * ctrl1 + 6f * ctrl2) * t
			-  3f * st + 3f * ctrl1;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.red;
		Gizmos.DrawLine(st, ctrl1);
		Gizmos.DrawLine(ctrl2, en);
		
		Gizmos.color = Color.white;
		Vector3 prevPt = st;
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
}


[System.Serializable]
public class CRSpline {
	public Vector3[] pts;
	
	public CRSpline(params Vector3[] pts) {
		this.pts = new Vector3[pts.Length];
		System.Array.Copy(pts, this.pts, pts.Length);
	}
	
	
	public Vector3 Interp(float t) {
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];
		
		return .5f * (
			(-a + 3f * b - 3f * c + d) * (u * u * u)
			+ (2f * a - 5f * b + 4f * c - d) * (u * u)
			+ (-a + c) * u
			+ 2f * b
		);
	}
	
	
	public Vector3 Velocity(float t) {
		int numSections = pts.Length - 3;
		int currPt = Mathf.Min(Mathf.FloorToInt(t * (float) numSections), numSections - 1);
		float u = t * (float) numSections - (float) currPt;
				
		Vector3 a = pts[currPt];
		Vector3 b = pts[currPt + 1];
		Vector3 c = pts[currPt + 2];
		Vector3 d = pts[currPt + 3];

		return 1.5f * (-a + 3f * b - 3f * c + d) * (u * u)
				+ (2f * a -5f * b + 4f * c - d) * u
				+ .5f * c - .5f * a;
	}
	
	
	public void GizmoDraw(float t) {
		Gizmos.color = Color.white;
		Vector3 prevPt = Interp(0);
		
		for (int i = 1; i <= 20; i++) {
			float pm = (float) i / 20f;
			Vector3 currPt = Interp(pm);
			Gizmos.DrawLine(currPt, prevPt);
			prevPt = currPt;
		}
		
		Gizmos.color = Color.blue;
		Vector3 pos = Interp(t);
		Gizmos.DrawLine(pos, pos + Velocity(t));
	}
}