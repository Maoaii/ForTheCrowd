using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

[Tool]
[GlobalClass]
public partial class LinearBallistics : BallisticsBase
{
	[Export] public float Speed = 100.0f;
	[Export] public float MaxFlightTime = 2.0f;

	public override void Step(ref ProjectileState state, float delta)
	{
		state.Position += state.Velocity * delta;
	}

	public override bool TrySolve(Vector3 from, Vector3 inheritedVelocity, Vector3 target, out Vector3 launchVelocity, out float flightTime)
	{
		launchVelocity = Vector3.Zero;
		flightTime = 0f;
		if (Speed <= 0f)
			return false;

		Vector3 toTarget = target - from;
		float a = inheritedVelocity.LengthSquared() - (Speed * Speed);
		float b = -2f * toTarget.Dot(inheritedVelocity);
		float c = toTarget.LengthSquared();

		if (!TrySmallestPositiveRoot(a, b, c, out flightTime) || flightTime > MaxFlightTime)
			return false;

		launchVelocity = toTarget / flightTime;
		return true;
	}

	private static bool TrySmallestPositiveRoot(float a, float b, float c, out float root)
	{
		root = 0f;
		if (Mathf.Abs(a) < 0.0001f)
		{
			if (Mathf.Abs(b) < 0.0001f)
				return false;
			root = -c / b;
			return root > 0f && float.IsFinite(root);
		}

		float discriminant = (b * b) - (4f * a * c);
		if (discriminant < 0f)
			return false;

		float sqrt = Mathf.Sqrt(discriminant);
		float first = (-b - sqrt) / (2f * a);
		float second = (-b + sqrt) / (2f * a);
		float smaller = Mathf.Min(first, second);
		float larger = Mathf.Max(first, second);

		root = smaller > 0f ? smaller : larger;
		return root > 0f && float.IsFinite(root);
	}
}
