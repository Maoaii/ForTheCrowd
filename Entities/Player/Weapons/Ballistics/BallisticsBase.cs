using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[Tool]
[GlobalClass]
public abstract partial class BallisticsBase : Resource
{
	public abstract bool TrySolve(Vector3 from, Vector3 inheritedVelocity, Vector3 target, out Vector3 launchVelocity, out float flightTime);

	public abstract void Step(ref ProjectileState state, float delta);
}
