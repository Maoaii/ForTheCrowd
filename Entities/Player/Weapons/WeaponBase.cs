using System;
using Godot;

public abstract partial class WeaponBase : Node3D
{
	[Export] public WeaponConfig WeaponConfig;
	[Export] public Marker3D BulletSpawnPoint;

	protected Timer _cooldownTimer;

	public override void _Ready()
	{
		if (WeaponConfig == null)
		{
			GD.PushWarning("WeaponConfig is not assigned in the inspector.");
		}
		if (BulletSpawnPoint == null)
		{
			GD.PushWarning("BulletSpawnPoint is not assigned in the inspector.");
		}

		_cooldownTimer = new Timer()
		{
			WaitTime = WeaponConfig.Cooldown,
			Autostart = false,
			OneShot = true,
		};

		AddChild(_cooldownTimer);
	}

	public void SetInitialPosition(Vector3 position)
	{
		GlobalPosition = position;
	}

	public virtual bool TryActivate(Vector3 direction)
	{
		ShootAnimation();

		return true;
	}

	public abstract void ShootAnimation();
}
