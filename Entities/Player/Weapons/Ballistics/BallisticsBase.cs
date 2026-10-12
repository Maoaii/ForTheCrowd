using CarGame.Entities.Components.AimingSystem;
using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[GlobalClass]
public abstract partial class BallisticsBase : Resource
{
	[Export] public bool InheritOwnerVelocity = true;

	public abstract bool TrySolve(in ShooterState shooter, in TargetInfo target, out FiringSolution solution);

	public abstract void Step(ref ProjectileState state, float delta);
}
