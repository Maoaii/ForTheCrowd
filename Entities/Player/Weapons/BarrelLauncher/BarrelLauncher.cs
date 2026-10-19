using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class BarrelLauncher : WeaponBase
{
	[Export] public MeshInstance3D BulletDecoy;
	private BarrelLauncherAnimationConfig BarrelLauncherConfig => ShootAnimationConfig as BarrelLauncherAnimationConfig;

	public override void _Ready()
	{
		base._Ready();

		if (BarrelLauncherConfig == null)
			GD.PushWarning("ShootAnimationConfig must be a BarrelLauncherAnimationConfig.");
	}

	public override void ShootAnimation(FiringSolution solution)
	{
		if (BarrelLauncherConfig == null)
			return;

		// Show barrel bullet decoy
		BulletDecoy.Visible = true;

		// Tween rotate barrel on X axis, using the back as the pivot point
		Tween tween = CreateTween()
			.SetEase(BarrelLauncherConfig.TweenEase)
			.SetTrans(BarrelLauncherConfig.TweenTransition);

		tween.TweenProperty(this, "rotation_degrees:x", BarrelLauncherConfig.TiltAngle, BarrelLauncherConfig.TiltTime);

		// Hide barrel bullet decoy
		tween.TweenCallback(Callable.From(() => BulletDecoy.Visible = false));

		// Tween rotate back to original position
		tween.TweenProperty(this, "rotation_degrees:x", 0.0f, BarrelLauncherConfig.ReturnTime);
		tween.TweenCallback(Callable.From(() => BulletDecoy.Visible = true));
	}
}
