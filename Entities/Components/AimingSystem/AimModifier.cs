using Godot;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public abstract partial class AimModifier : Resource
{
	public abstract Vector3 Apply(in AimContext context, Vector3 aimPoint);
}
