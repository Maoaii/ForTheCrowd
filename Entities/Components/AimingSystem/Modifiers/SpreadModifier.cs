using Godot;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public partial class SpreadModifier : AimModifier
{
	[Export(PropertyHint.Range, "0.0,1.0,0.01")] public float Chance = 1.0f;
	[Export(PropertyHint.Range, "0.0,45.0,0.1,suffix:m")] public float SpreadMeters = 1.0f;

	public override Vector3 Apply(in AimContext context, Vector3 aimPoint)
	{
		RandomNumberGenerator random = context.Random;
		if (random.Randf() >= Chance)
			return aimPoint;

		return aimPoint + new Vector3(
			random.Randfn(0f, SpreadMeters),
			random.Randfn(0f, SpreadMeters),
			random.Randfn(0f, SpreadMeters));
	}
}
