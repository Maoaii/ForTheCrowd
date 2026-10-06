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

    public override Vector3 GetAimingDirection(Node3D fromNode)
    {
        SphereShape3D shape = new SphereShape3D { Radius = AimRange};
		PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D
		{
			Shape = shape,
			CollisionMask = TargetLayers,
			CollideWithAreas = true,
			CollideWithBodies = true
		};

        Array<Dictionary> results = fromNode.GetWorld3D().DirectSpaceState.IntersectShape(query, 5000);
        if (results.Count == 0)
            return Vector3.Forward;
        
        var sorted = results.OrderBy(item => (item["collider"].AsGodotObject() as Node3D).GlobalPosition.DistanceTo(fromNode.GlobalPosition));

        Dictionary closest = sorted.First();
        Vector3 direction = fromNode.GlobalPosition.DirectionTo((closest["collider"].AsGodotObject() as Node3D).GlobalPosition).Normalized();
		
        VectorRenderer.DrawVector(fromNode.GlobalPosition, direction * 5.0f, Colors.Red);

        return direction;
    }
}
