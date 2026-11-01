using Godot;

namespace CarGame.Entities.Components.AimingSystem;

[GlobalClass]
public abstract partial class AimingProviderBase : Resource, IAimingProvider
{
	public abstract bool TryGetTarget(Node3D fromNode, out TargetInfo target);
}
