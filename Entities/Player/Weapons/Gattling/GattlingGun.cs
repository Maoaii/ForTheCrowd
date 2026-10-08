using Godot;

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

		recoilTween.TweenProperty(this, "position", Position + new Vector3(0, 0, -WeaponConfig.RecoilDistance), WeaponConfig.RecoilUpTime)
			.SetTrans(WeaponConfig.RecoilUpTweenTransition)
			.SetEase(WeaponConfig.RecoilUpTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", Rotation + new Vector3(Mathf.DegToRad(WeaponConfig.RecoilRotationAmount), 0, 0), WeaponConfig.RecoilUpTime)
			.SetTrans(WeaponConfig.RecoilUpTweenTransition)
			.SetEase(WeaponConfig.RecoilUpTweenEase);

		recoilTween.TweenProperty(this, "position", Vector3.Zero, WeaponConfig.RecoilDownTime)
			.SetTrans(WeaponConfig.RecoilDownTweenTransition)
			.SetEase(WeaponConfig.RecoilDownTweenEase);
		recoilTween.Parallel().TweenProperty(this, "rotation", Vector3.Zero, WeaponConfig.RecoilDownTime)
			.SetTrans(WeaponConfig.RecoilDownTweenTransition)
			.SetEase(WeaponConfig.RecoilDownTweenEase);
	}
}
