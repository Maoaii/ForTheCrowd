using System.Linq;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public partial class ClosestTargetAutoAimer : AimingProviderBase
{
	[Export] public float AimRange;

	[Export(PropertyHint.Layers3DPhysics)] public uint TargetLayers { get; set; }

	public override bool TryGetTarget(Node3D fromNode, out TargetInfo target)
	{
		SphereShape3D shape = new() { Radius = AimRange };
		PhysicsShapeQueryParameters3D query = new()
		{
			Transform = new Transform3D(Basis.Identity, fromNode.GlobalPosition),
			Shape = shape,
			CollisionMask = TargetLayers,
			CollideWithAreas = true,
			CollideWithBodies = true
		};

		Array<Dictionary> results = fromNode.GetWorld3D().DirectSpaceState.IntersectShape(query, 5000);
		if (results.Count == 0)
		{
			target = default;
			return false;
		}

		var sorted = results.OrderBy(item => (item["collider"].AsGodotObject() as Node3D).GlobalPosition.DistanceTo(fromNode.GlobalPosition));

		Node3D closest = sorted.First()["collider"].AsGodotObject() as Node3D;

		target = new TargetInfo()
		{
			Position = closest.GlobalPosition,
			GroundPosition = closest.GlobalPosition * new Vector3(1.0f, 0.0f, 1.0f),
			Velocity = closest is IVelocityHolder holder ? holder.GetVelocity() : Vector3.Zero,
		};

		return true;
	}
}
