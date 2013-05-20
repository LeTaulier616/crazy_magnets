using UnityEngine;
using System.Collections;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision.Shapes;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics.Contacts;

public class FSCapsuleShape : MonoBehaviour
{
	public enum Diretion
	{
		X,
		Y
	}
	public float radius = 0.5f;
	public Diretion direction = Diretion.Y;
	[Range(0f, float.MaxValue)]
	public float length = 2f;
	private float margin = 0.01f;

	private Body body;

	void Start()
	{
		FSBodyComponent bodyComponent = this.GetComponent<FSBodyComponent>();
		if (bodyComponent == null)
			return;
		this.body = bodyComponent.PhysicsBody;

		float curLength = Mathf.Max(0f, this.length / 2f - this.radius);
		FVector2 pos;
		if (this.direction == Diretion.Y)
			pos = new FVector2(0f, curLength);
		else
			pos = new FVector2(curLength, 0f);

		CircleShape circleHead = new CircleShape(this.radius, 1f);
		circleHead.Position = pos;
		
		CircleShape circleFoots = new CircleShape(this.radius, 1f);
		circleFoots.Position = -pos;

		if (this.direction == Diretion.Y)
			pos.X = this.radius - this.margin;
		else
			pos.Y = this.radius - this.margin;

		Vertices vertices = new Vertices();
		vertices.Add(new FVector2(-pos.X, -pos.Y));
		vertices.Add(new FVector2( pos.X, -pos.Y));
		vertices.Add(new FVector2( pos.X,  pos.Y));
		vertices.Add(new FVector2(-pos.X,  pos.Y));
		PolygonShape shape = new PolygonShape(vertices, 1f);
		
		Fixture fixHead = this.body.CreateFixture(circleHead);
		fixHead.Friction = 0;
		fixHead.Restitution = 0;
		Fixture fixFoots = this.body.CreateFixture(circleFoots);
		fixFoots.Friction = 0;
		fixFoots.Restitution = 0;
		Fixture fix = this.body.CreateFixture(shape);
		fix.Friction = 0;
		fix.Restitution = 0;
		
		this.body.Mass = 1f;
	}

	void OnDrawGizmos()
	{
		float curLength = Mathf.Max(0f, this.length / 2f - this.radius);
		Vector3 pos;
		if (this.direction == Diretion.Y)
			pos = new Vector3(0f, curLength, 0f);
		else
			pos = new Vector3(curLength, 0f, 0f);

		Gizmos.DrawWireSphere(this.transform.position + pos, this.radius);
		Gizmos.DrawWireSphere(this.transform.position - pos, this.radius);

		if (this.direction == Diretion.Y)
			pos.x = this.radius - this.margin;
		else
			pos.y = this.radius - this.margin;

		Gizmos.DrawLine(this.transform.position + new Vector3(-pos.x, -pos.y), this.transform.position + new Vector3( pos.x, -pos.y));
		Gizmos.DrawLine(this.transform.position + new Vector3( pos.x, -pos.y), this.transform.position + new Vector3( pos.x,  pos.y));
		Gizmos.DrawLine(this.transform.position + new Vector3( pos.x,  pos.y), this.transform.position + new Vector3(-pos.x,  pos.y));
		Gizmos.DrawLine(this.transform.position + new Vector3(-pos.x,  pos.y), this.transform.position + new Vector3(-pos.x, -pos.y));
	}
}
