using UnityEngine;
using System.Collections;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

public class FSRoundedSquareShape : MonoBehaviour
{
	public float radius = 1f;

	private Body body;

	void Start()
	{
		FSBodyComponent bodyComponent = this.GetComponent<FSBodyComponent>();
		if (bodyComponent == null)
			return;
		this.body = bodyComponent.PhysicsBody;
		
		float size = this.radius/2f - this.radius/10f;

		CircleShape circleUpLeft = new CircleShape(this.radius/10f, 1f);
		circleUpLeft.Position = new FVector2(-size, -size);
		CircleShape circleUpRight = new CircleShape(this.radius/10f, 1f);
		circleUpRight.Position = new FVector2(size, -size);
		CircleShape circleDownLeft = new CircleShape(this.radius/10f, 1f);
		circleDownLeft.Position = new FVector2(-size, size);
		CircleShape circleDownRight = new CircleShape(this.radius/10f, 1f);
		circleDownRight.Position = new FVector2(size, size);

		Vertices vertices1 = new Vertices();
		vertices1.Add(new FVector2(-size, -this.radius/2f));
		vertices1.Add(new FVector2( size, -this.radius/2f));
		vertices1.Add(new FVector2( size,  this.radius/2f));
		vertices1.Add(new FVector2(-size,  this.radius/2f));
		PolygonShape shape1 = new PolygonShape(vertices1, 1f);
		
		Vertices vertices2 = new Vertices();
		vertices2.Add(new FVector2(-this.radius/2f, -size));
		vertices2.Add(new FVector2( this.radius/2f, -size));
		vertices2.Add(new FVector2( this.radius/2f,  size));
		vertices2.Add(new FVector2(-this.radius/2f,  size));
		PolygonShape shape2 = new PolygonShape(vertices2, 1f);
		
		Fixture fixUpLeft = this.body.CreateFixture(circleUpLeft);
		fixUpLeft.Friction = 0.75f;
		fixUpLeft.Restitution = 0;
		Fixture fixUpRight = this.body.CreateFixture(circleUpRight);
		fixUpRight.Friction = 0.75f;
		fixUpRight.Restitution = 0;
		Fixture fixBottomLeft = this.body.CreateFixture(circleDownLeft);
		fixBottomLeft.Friction = 0.75f;
		fixBottomLeft.Restitution = 0;
		Fixture fixDownRight = this.body.CreateFixture(circleDownRight);
		fixDownRight.Friction = 0.75f;
		fixDownRight.Restitution = 0;
		Fixture fix1 = this.body.CreateFixture(shape1);
		fix1.Friction = 0.75f;
		fix1.Restitution = 0;
		Fixture fix2 = this.body.CreateFixture(shape2);
		fix2.Friction = 0.75f;
		fix2.Restitution = 0;
		
		this.body.Mass = 1f;
	}

	void OnDrawGizmos()
	{
		float size = this.radius/2f - this.radius/10f;
		
		Gizmos.DrawLine(this.transform.position + new Vector3(-size, -this.radius/2), this.transform.position + new Vector3( size, -this.radius/2));
		Gizmos.DrawLine(this.transform.position + new Vector3( size, -this.radius/2), this.transform.position + new Vector3( size,  this.radius/2));
		Gizmos.DrawLine(this.transform.position + new Vector3( size,  this.radius/2), this.transform.position + new Vector3(-size,  this.radius/2));
		Gizmos.DrawLine(this.transform.position + new Vector3(-size,  this.radius/2), this.transform.position + new Vector3(-size, -this.radius/2));
		
		Gizmos.DrawLine(this.transform.position + new Vector3(-this.radius/2, -size), this.transform.position + new Vector3( this.radius/2, -size));
		Gizmos.DrawLine(this.transform.position + new Vector3( this.radius/2, -size), this.transform.position + new Vector3( this.radius/2,  size));
		Gizmos.DrawLine(this.transform.position + new Vector3( this.radius/2,  size), this.transform.position + new Vector3(-this.radius/2,  size));
		Gizmos.DrawLine(this.transform.position + new Vector3(-this.radius/2,  size), this.transform.position + new Vector3(-this.radius/2, -size));
		
		Vector3 pos = new Vector3(-size, size, 0f);
		Gizmos.DrawWireSphere(this.transform.position + pos, this.radius / 10f);
		pos = new Vector3(-size, -size, 0f);
		Gizmos.DrawWireSphere(this.transform.position + pos, this.radius / 10f);
		pos = new Vector3(size, -size, 0f);
		Gizmos.DrawWireSphere(this.transform.position + pos, this.radius / 10f);
		pos = new Vector3(size, size, 0f);
		Gizmos.DrawWireSphere(this.transform.position + pos, this.radius / 10f);
	}
}
