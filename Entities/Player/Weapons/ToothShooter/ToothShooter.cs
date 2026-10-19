using System;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class ToothShooter : WeaponBase
{
	private ToothShooterAnimationConfig ToothShooterConfig => ShootAnimationConfig as ToothShooterAnimationConfig;
	private Tween scaleTween;
	private Tween recoilTween;

	public override void _Ready()
	{
		base._Ready();

		if (ToothShooterConfig == null)
			GD.PushWarning("ShootAnimationConfig must be a ToothShooterAnimationConfig.");
	}

	public override void ShootAnimation(FiringSolution solution, Action fire)
	{
		fire();

		if (ToothShooterConfig == null)
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
		scaleTween.TweenProperty(this, "scale", ToothShooterConfig.ScaleUpAmount, ToothShooterConfig.ScaleUpTime)
			.SetTrans(ToothShooterConfig.ScaleUpTweenTransition)
			.SetEase(ToothShooterConfig.ScaleUpTweenEase);
		scaleTween.TweenProperty(this, "scale", new Vector3(1f, 1f, 1f), ToothShooterConfig.ScaleDownTime)
			.SetTrans(ToothShooterConfig.ScaleDownTweenTransition)
			.SetEase(ToothShooterConfig.ScaleDownTweenEase);
	}

	private void RecoilAnimation()
	{
		if (recoilTween != null && recoilTween.IsValid())
		{
			recoilTween.Kill();
		}

		recoilTween = CreateTween();

		recoilTween.TweenProperty(this, "position", Position + new Vector3(0.0f, 0.0f, -ToothShooterConfig.RecoilDistance).Rotated(Vector3.Up, Rotation.Y), ToothShooterConfig.RecoilUpTime)
			.SetTrans(ToothShooterConfig.RecoilUpTweenTransition)
			.SetEase(ToothShooterConfig.RecoilUpTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation:x", Rotation.X - Mathf.DegToRad(ToothShooterConfig.RecoilRotationAmount), ToothShooterConfig.RecoilUpTime)
			.SetTrans(ToothShooterConfig.RecoilUpTweenTransition)
			.SetEase(ToothShooterConfig.RecoilUpTweenEase);

		recoilTween.TweenProperty(this, "position", Vector3.Zero, ToothShooterConfig.RecoilDownTime)
			.SetTrans(ToothShooterConfig.RecoilDownTweenTransition)
			.SetEase(ToothShooterConfig.RecoilDownTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation:x", 0.0f, ToothShooterConfig.RecoilDownTime)
			.SetTrans(ToothShooterConfig.RecoilDownTweenTransition)
			.SetEase(ToothShooterConfig.RecoilDownTweenEase);
	}
}
