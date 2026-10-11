using CarGame.Entities.Components.AimingSystem;
using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[GlobalClass]
public partial class LinearBallistics : BallisticsBase
{
	public override void Step(ref ProjectileState state, float delta)
	{
		state.Position += state.Velocity * delta;
	}

	public override bool TrySolve(in ShooterState shooter, in TargetInfo target, out FiringSolution solution)
	{
		Vector3 velocity = shooter.MuzzlePosition.DirectionTo(target.Position) * Speed;
		if (InheritOwnerVelocity)
			velocity += shooter.Velocity;

		solution = new FiringSolution(velocity, target.Position, shooter.MuzzlePosition.DistanceTo(target.Position) / Speed);
		return true;
	}
}
