using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class GattlingGun : WeaponBase
{
	private Tween scaleTween;
	private Tween recoilTween;

	public override void ShootAnimation()
	{
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
		scaleTween.TweenProperty(this, "scale", WeaponConfig.ScaleUpAmount, WeaponConfig.ScaleUpTime)
			.SetTrans(WeaponConfig.ScaleUpTweenTransition)
			.SetEase(WeaponConfig.ScaleUpTweenEase);
		scaleTween.TweenProperty(this, "scale", new Vector3(1f, 1f, 1f), WeaponConfig.ScaleDownTime)
			.SetTrans(WeaponConfig.ScaleDownTweenTransition)
			.SetEase(WeaponConfig.ScaleDownTweenEase);
	}

	private void RecoilAnimation()
	{
		if (recoilTween != null && recoilTween.IsValid())
		{
			recoilTween.Kill();
		}

		recoilTween = CreateTween();

		recoilTween.TweenProperty(this, "position", Position + new Vector3(0.0f, 0.0f, -WeaponConfig.RecoilDistance).Rotated(Vector3.Up, Rotation.Y), WeaponConfig.RecoilUpTime)
			.SetTrans(WeaponConfig.RecoilUpTweenTransition)
			.SetEase(WeaponConfig.RecoilUpTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", Rotation + new Vector3(-Mathf.DegToRad(WeaponConfig.RecoilRotationAmount), 0, 0), WeaponConfig.RecoilUpTime)
			.SetTrans(WeaponConfig.RecoilUpTweenTransition)
			.SetEase(WeaponConfig.RecoilUpTweenEase);

		recoilTween.TweenProperty(this, "position", Vector3.Zero, WeaponConfig.RecoilDownTime)
			.SetTrans(WeaponConfig.RecoilDownTweenTransition)
			.SetEase(WeaponConfig.RecoilDownTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", new Vector3(0, Rotation.Y, Rotation.Z), WeaponConfig.RecoilDownTime)
			.SetTrans(WeaponConfig.RecoilDownTweenTransition)
			.SetEase(WeaponConfig.RecoilDownTweenEase);
	}
}
