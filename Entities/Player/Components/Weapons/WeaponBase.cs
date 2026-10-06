using Godot;
using System;

public abstract partial class WeaponBase : Node3D
{
	public void SetInitialPosition(Vector3 position)
	{
		GlobalPosition = position;
	}

	public virtual bool TryActivate(Vector3 direction)
	{
		ShootAnimation();

		return true;
	}

	Tween tween;
	private void ShootAnimation()
	{
		if (tween != null && tween.IsValid())
		{
			tween.Kill();
		}
		tween = CreateTween();

		tween.TweenProperty(this, "scale", new Vector3(1.3f, 1.3f, 1.3f), 0.05);
		tween.TweenProperty(this, "scale", new Vector3(1.0f, 1.0f, 1.0f), 0.1);
	}
}
