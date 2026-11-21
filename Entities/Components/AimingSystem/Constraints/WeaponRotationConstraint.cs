using CarGame.Entities.Player.Weapons.Ballistics;
using CarGame.Utils.Debug;
using Godot;

namespace CarGame.Entities.Components.AimingSystem;

[Tool]
[GlobalClass]
public partial class WeaponRotationConstraint : AimConstraint
{
	[Export(PropertyHint.Range, "0.0,180.0,1.0,radians_as_degrees")] public float RotationConeDegrees;

	public override bool ApplyConstraint(FiringSolution solution, Node3D weaponMount, float restYaw)
	{
		Basis parentBasis = weaponMount.GetParentNode3D().GlobalBasis;
		Vector3 direction = parentBasis.Inverse() * solution.MuzzleDirection;
		float yaw = Vector3.Forward.SignedAngleTo(direction, Vector3.Up);

		DebugDrawGizmos(weaponMount, yaw, restYaw);

		return Mathf.Abs(Mathf.AngleDifference(restYaw, yaw)) <= RotationConeDegrees;
	}

	private void DebugDrawGizmos(Node3D weaponMount, float targetYaw, float restYaw)
	{
		if (Engine.IsEditorHint())
			return;

		Basis parentBasis = weaponMount.GetParentNode3D().GlobalBasis;

		Vector3 left = parentBasis * Vector3.Forward.Rotated(Vector3.Up, restYaw + RotationConeDegrees);
		Vector3 right = parentBasis * Vector3.Forward.Rotated(Vector3.Up, restYaw - RotationConeDegrees);

		VectorRenderer.DrawLine(weaponMount.GlobalPosition, weaponMount.GlobalPosition + (left * 2), Colors.Green);
		VectorRenderer.DrawLine(weaponMount.GlobalPosition, weaponMount.GlobalPosition + (right * 2), Colors.Green);
	}
}
