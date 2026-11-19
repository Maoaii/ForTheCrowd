using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

[Tool]
[GlobalClass]
public partial class ArcBallistics : BallisticsBase
{
	[Export] public float Gravity = 10.0f;
	// apex height above the higher of muzzle/target, as a fraction of horizontal distance
	[Export] public float ArcHeightRatio = 0.35f;
	[Export] public float MinArcHeight = 2.0f;
	[Export] public float MaxArcHeight = 30.0f;

	public override void Step(ref ProjectileState state, float delta)
	{
		// exact parabola for constant gravity
		state.Position += state.Velocity * delta + Vector3.Down * (0.5f * Gravity * delta * delta);
		state.Velocity += Vector3.Down * Gravity * delta;
	}

	public override bool TrySolve(Vector3 from, Vector3 inheritedVelocity, Vector3 target, out Vector3 launchVelocity, out float flightTime)
	{
		launchVelocity = Vector3.Zero;
		flightTime = 0f;
		if (Gravity <= 0f)
			return false;

		SolveArcBallistics(from, target, Gravity, GetArcHeight(from, target), out launchVelocity, out flightTime);
		return float.IsFinite(flightTime) && flightTime > 0f;
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
