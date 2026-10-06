using System.Linq;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Components.AimingSystem;

[GlobalClass]
public partial class ClosestTargetAutoAimer : Node3D, IAimingProvider
{
	[Export] 
	public float AimRange;
	
	[Export(PropertyHint.Layers3DPhysics)]
	public uint TargetLayers { get; set; }

	public Vector3 GetAimingDirection()
	{
		SphereShape3D shape = new SphereShape3D { Radius = AimRange};
		PhysicsShapeQueryParameters3D query = new PhysicsShapeQueryParameters3D
		{
			Shape = shape,
			CollisionMask = (uint)TargetLayers,
			CollideWithAreas = false,
			CollideWithBodies = true
		};

		Array<Dictionary> results = GetWorld3D().DirectSpaceState.IntersectShape(query);
        if (results.Count == 0)
            return Vector3.Forward;
        
        var sorted = results.OrderBy(item => (item["collider"].AsGodotObject() as Node3D).GlobalPosition.DistanceTo(GlobalPosition));

        Dictionary closest = sorted.First();
    
		return GlobalPosition.DirectionTo((closest["collider"].AsGodotObject() as Node3D).GlobalPosition).Normalized();
	}
}
