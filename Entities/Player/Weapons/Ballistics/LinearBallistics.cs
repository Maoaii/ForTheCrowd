using CarGame.Entities.Components.AimingSystem;
using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[GlobalClass]
public partial class LinearBallistics : BallisticsBase
{
	[Export] public float MaxFlightTime = 2.0f;
	[Export(PropertyHint.Range, "1,8")] public int Iterations = 3;

	public override void Step(ref ProjectileState state, float delta)
	{
		state.Position += state.Velocity * delta;
	}

	public override bool TrySolve(in ShooterState shooter, in TargetInfo target, out FiringSolution solution)
	{
		solution = default;
		if (Speed <= 0f)
			return false;


		Vector3 relativeVelocity = InheritOwnerVelocity
			? target.Velocity - shooter.Velocity
			: target.Velocity;

		float t = shooter.MuzzlePosition.DistanceTo(target.Position) / Speed;
		Vector3 relativeAimPoint = target.Position;

		for (int i = 0; i < Iterations; i++)
		{
			relativeAimPoint = target.Position + (relativeVelocity * t);
			t = shooter.MuzzlePosition.DistanceTo(relativeAimPoint) / Speed;
		}

		if (!float.IsFinite(t) || t > MaxFlightTime)
			return false;

		Vector3 muzzleDirection = shooter.MuzzlePosition.DirectionTo(relativeAimPoint);
		Vector3 launchVelocity = muzzleDirection * Speed;
		if (InheritOwnerVelocity)
			launchVelocity += shooter.Velocity;

		Vector3 worldAimPoint = target.Position + (target.Velocity * t);

		solution = new FiringSolution(launchVelocity, muzzleDirection, worldAimPoint, t);
		return true;
	}
}
