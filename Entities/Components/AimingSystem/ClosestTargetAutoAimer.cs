using System.Linq;
using CarGame.Utils.Debug;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public partial class ClosestTargetAutoAimer : AimingProviderBase
{
    [Export] public float AimRange;

    [Export(PropertyHint.Layers3DPhysics)] public uint TargetLayers { get; set; }

    public override (Vector3 direction, Vector3 targetPosition) GetAimingDirection(Node3D fromNode, Vector3 ownerVelocity)
    {
        SphereShape3D shape = new SphereShape3D { Radius = AimRange };
        PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D
        {
            Transform = new Transform3D(Basis.Identity, fromNode.GlobalPosition),
            Shape = shape,
            CollisionMask = TargetLayers,
            CollideWithAreas = true,
            CollideWithBodies = true
        };

        Array<Dictionary> results = fromNode.GetWorld3D().DirectSpaceState.IntersectShape(query, 5000);
        if (results.Count == 0)
            return (Vector3.Zero, Vector3.Zero);

        var sorted = results.OrderBy(item => (item["collider"].AsGodotObject() as Node3D).GlobalPosition.DistanceTo(fromNode.GlobalPosition));

        Node3D closest = sorted.First()["collider"].AsGodotObject() as Node3D;

        float lookaheadAngle = Mathf.Tan(fromNode.GlobalPosition.DistanceTo(closest.GlobalPosition) / (closest.GlobalPosition + ownerVelocity).Length());
        GD.Print(lookaheadAngle);
        Vector3 direction = fromNode.GlobalPosition.DirectionTo(closest.GlobalPosition).Rotated(Vector3.Up, lookaheadAngle).Normalized();

        return (direction, closest.GlobalPosition);
    }
}
