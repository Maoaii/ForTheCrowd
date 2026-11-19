using Godot;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public partial class LeadTargetModifier : AimModifier
{
	[Export(PropertyHint.Range, "1,8")] public int Iterations = 3;

	public override Vector3 Apply(in AimContext context, Vector3 aimPoint)
	{
		Vector3 ledPoint = aimPoint;
		for (int i = 0; i < Iterations; i++)
		{
			if (!context.Ballistics.TrySolve(context.MuzzlePosition, context.InheritedVelocity, ledPoint, out _, out float flightTime))
				break;

			ledPoint = aimPoint + context.Target.Velocity * flightTime;
		}

		return ledPoint;
	}
}
