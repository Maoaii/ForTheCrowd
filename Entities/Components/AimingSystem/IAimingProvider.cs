using Godot;

namespace CarGame.Entities.Components.AimingSystem;

public interface IAimingProvider
{
	bool TryGetTarget(Node3D fromNode, out TargetInfo target);
}
