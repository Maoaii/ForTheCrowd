using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class ToothShooter : WeaponBase
{
	[Export] public ShootAnimationConfig ShootAnimationConfig;

	private Tween scaleTween;
	private Tween recoilTween;

	public override void _Ready()
	{
		base._Ready();

		if (ShootAnimationConfig == null)
			GD.PushWarning("ShootAnimationConfig is not assigned in the inspector.");
	}

	public override void ShootAnimation()
	{
		if (ShootAnimationConfig == null)
			return;

		ScaleAnimation();
		RecoilAnimation();
	}

	private void ScaleAnimation()
	{
		if (scaleTween != null && scaleTween.IsValid())
		{
			scaleTween.Kill();
		}
		scaleTween = CreateTween();
		scaleTween.TweenProperty(this, "scale", ShootAnimationConfig.ScaleUpAmount, ShootAnimationConfig.ScaleUpTime)
			.SetTrans(ShootAnimationConfig.ScaleUpTweenTransition)
			.SetEase(ShootAnimationConfig.ScaleUpTweenEase);
		scaleTween.TweenProperty(this, "scale", new Vector3(1f, 1f, 1f), ShootAnimationConfig.ScaleDownTime)
			.SetTrans(ShootAnimationConfig.ScaleDownTweenTransition)
			.SetEase(ShootAnimationConfig.ScaleDownTweenEase);
	}

	private void RecoilAnimation()
	{
		if (recoilTween != null && recoilTween.IsValid())
		{
			recoilTween.Kill();
		}

		recoilTween = CreateTween();

		recoilTween.TweenProperty(this, "position", Position + new Vector3(0.0f, 0.0f, -ShootAnimationConfig.RecoilDistance).Rotated(Vector3.Up, Rotation.Y), ShootAnimationConfig.RecoilUpTime)
			.SetTrans(ShootAnimationConfig.RecoilUpTweenTransition)
			.SetEase(ShootAnimationConfig.RecoilUpTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", Rotation + new Vector3(-Mathf.DegToRad(ShootAnimationConfig.RecoilRotationAmount), 0, 0), ShootAnimationConfig.RecoilUpTime)
			.SetTrans(ShootAnimationConfig.RecoilUpTweenTransition)
			.SetEase(ShootAnimationConfig.RecoilUpTweenEase);

		recoilTween.TweenProperty(this, "position", Vector3.Zero, ShootAnimationConfig.RecoilDownTime)
			.SetTrans(ShootAnimationConfig.RecoilDownTweenTransition)
			.SetEase(ShootAnimationConfig.RecoilDownTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", new Vector3(0, Rotation.Y, Rotation.Z), ShootAnimationConfig.RecoilDownTime)
			.SetTrans(ShootAnimationConfig.RecoilDownTweenTransition)
			.SetEase(ShootAnimationConfig.RecoilDownTweenEase);
	}
}
