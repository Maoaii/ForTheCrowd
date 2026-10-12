using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

[GlobalClass]
public partial class ArcBallistics : BallisticsBase
{
	[Export] public float Gravity = 10.0f;
	// apex height above the higher of muzzle/target, as a fraction of horizontal distance
	[Export] public float ArcHeightRatio = 0.35f;
	[Export] public float MinArcHeight = 2.0f;
	[Export] public float MaxArcHeight = 30.0f;
	[Export(PropertyHint.Range, "1,8")] public int Iterations = 3;

	public override void Step(ref ProjectileState state, float delta)
	{
		// exact parabola for constant gravity
		state.Position += state.Velocity * delta + Vector3.Down * (0.5f * Gravity * delta * delta);
		state.Velocity += Vector3.Down * Gravity * delta;
	}

	public override bool TrySolve(in ShooterState shooter, in TargetInfo target, out FiringSolution solution)
	{
		solution = default;
		if (Gravity <= 0f)
			return false;

		Vector3 relativeVelocity = InheritOwnerVelocity
			? target.Velocity - shooter.Velocity
			: target.Velocity;

		Vector3 launchVelocity = Vector3.Zero;
		float t = 0f;

		for (int i = 0; i < Iterations; i++)
		{
			Vector3 aimPoint = target.Position + relativeVelocity * t;
			float arcHeight = GetArcHeight(shooter.MuzzlePosition, aimPoint);

			SolveArcBallistics(shooter.MuzzlePosition, aimPoint, Gravity, arcHeight, out launchVelocity, out t);
		}

		if (!float.IsFinite(t) || t <= 0f)
			return false;

		Vector3 muzzleDirection = launchVelocity.Normalized();

		if (InheritOwnerVelocity)
			launchVelocity += shooter.Velocity;

		Vector3 worldAimPoint = target.Position + target.Velocity * t;

		solution = new FiringSolution(launchVelocity, muzzleDirection, worldAimPoint, t);
		return true;
	}

	private float GetArcHeight(Vector3 from, Vector3 to)
	{
		float d = new Vector2(to.X - from.X, to.Z - from.Z).Length();
		return Mathf.Clamp(d * ArcHeightRatio, Mathf.Max(MinArcHeight, 0.01f), Mathf.Max(MaxArcHeight, MinArcHeight));
	}

	public static void SolveArcBallistics(
		Vector3 from, Vector3 to, float gravity, float arcHeight,
		out Vector3 fireVelocity, out float timeToTarget)
	{
		float apexY = Mathf.Max(from.Y, to.Y) + arcHeight;
		float rise = apexY - from.Y;
		float fall = apexY - to.Y;

		float vy = Mathf.Sqrt(2f * gravity * rise);
		timeToTarget = (vy / gravity) + Mathf.Sqrt(2f * fall / gravity);   // time up + time down

		fireVelocity = new Vector3(to.X - from.X, 0f, to.Z - from.Z) / timeToTarget;
		fireVelocity.Y = vy;
	}
}
