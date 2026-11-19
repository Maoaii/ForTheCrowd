using CarGame.Entities.Components.AimingSystem;
using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[Tool]
[GlobalClass]
public abstract partial class BallisticsBase : Resource
{
	[Export] public AimingModifier AimModifier;

	public abstract bool TrySolve(in ShooterState shooter, in TargetInfo target, out FiringSolution solution);

	public abstract void Step(ref ProjectileState state, float delta);
}
