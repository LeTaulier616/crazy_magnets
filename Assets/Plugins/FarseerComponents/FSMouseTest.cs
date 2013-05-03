using UnityEngine;
using System.Collections;
using FarseerPhysics.Collision;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Common;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;

using Transform = UnityEngine.Transform;

[AddComponentMenu("FarseerUnity/Debug/Mouse Test Component")]
public class FSMouseTest : MonoBehaviour {
	
	protected FixedMouseJoint mouseJoint;

	public virtual void Update()
	{
		UpdateMouseWorld();
		MouseDrag();
	}
	
	static public float MouseXWorldPhys = 0f;
	static public float MouseYWorldPhys = 0f;
	public virtual void UpdateMouseWorld()
	{
		//Vector3 wp = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		
		Vector3 fwd = Camera.main.transform.forward;
		Vector3 pos = Camera.main.transform.position + fwd * (-Camera.main.transform.position.z);
		// create a plane at distance, and facing the camera:
		Plane plane = new Plane(-fwd, pos);
		Ray ray; // ray to intersect the plane
		float dist = 0; // will contain the distance along the ray
		
		ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		plane.Raycast(ray, out dist); // find the distance of the hit point
		Vector3 wp = ray.GetPoint(dist);
		
		MouseXWorldPhys = wp.x;// -parent.position.x;
		MouseYWorldPhys = wp.y;// - parent.position.y;
		//Debug.Log("MX: " + MouseXWorldPhys + " MY: " + MouseYWorldPhys);
		//dynB.Position = new FVector2(MouseXWorldPhys, MouseYWorldPhys);
	}
	
	protected FVector2 mousePVec = new FVector2();
	public virtual Body GetBodyAtMouse()
	{
		return GetBodyAtMouse(false);
	}
	public virtual Body GetBodyAtMouse(bool includeStatic)
	{
		// Make a small box
		mousePVec.X = MouseXWorldPhys;
		mousePVec.Y = MouseYWorldPhys;
		//UnityEngine.Debug.Log (mousePVec.X);
		FVector2 lowerBound = new FVector2(MouseXWorldPhys - 1f, MouseYWorldPhys - 1f);
		FVector2 upperBound = new FVector2(MouseXWorldPhys + 1f, MouseYWorldPhys + 1f);
		AABB aabb = new AABB(lowerBound, upperBound);
		Body body = null;
		
		// Query the world for overlapping shapes
		System.Func<Fixture, bool> GetBodyCallback = delegate (Fixture fixture0)
		{
			Shape shape = fixture0.Shape;
			if(fixture0.Body.BodyType != BodyType.Static || includeStatic)
			{
				FarseerPhysics.Common.Transform transform0;
				fixture0.Body.GetTransform(out transform0);
				bool inside = shape.TestPoint(ref transform0, ref mousePVec);
				if(inside)
				{
					body = fixture0.Body;
					return false;
				}
			}
			return true;
		};
		FSWorldComponent.PhysicsWorld.QueryAABB(GetBodyCallback, ref aabb);
		return body;
	}
	
	public virtual void MouseDrag()
	{
		// mouse press
		if(Input.GetMouseButtonDown(0) && mouseJoint == null)
		{
			Body body = GetBodyAtMouse();
			if(body != null)
			{
				FVector2 target = new FVector2(MouseXWorldPhys, MouseYWorldPhys);
				mouseJoint = JointFactory.CreateFixedMouseJoint(FSWorldComponent.PhysicsWorld, body, target);
				mouseJoint.CollideConnected = true;
				mouseJoint.MaxForce = 300f * body.Mass;
				body.Awake = true;
			}
		}
		// mouse release
		if(Input.GetMouseButtonUp(0))
		{
			if(mouseJoint != null)
			{
				FSWorldComponent.PhysicsWorld.RemoveJoint(mouseJoint);
				mouseJoint = null;
			}
		}
		
		// mouse move
		if(mouseJoint != null)
		{
			FVector2 p2 = new FVector2(MouseXWorldPhys, MouseYWorldPhys);
			mouseJoint.WorldAnchorB = p2;
		}
	}
}
