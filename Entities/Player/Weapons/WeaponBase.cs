using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class WeaponBase : Node3D
{
	[Export] public WeaponConfig WeaponConfig;
	[Export] public Marker3D BulletSpawnPoint;

	protected Timer _cooldownTimer;

	public override void _Ready()
	{
		if (WeaponConfig == null)
			GD.PushWarning("WeaponConfig is not assigned in the inspector.");
		if (BulletSpawnPoint == null)
			GD.PushWarning("BulletSpawnPoint is not assigned in the inspector.");
		if (WeaponConfig?.BulletConfig?.FiringSolver == null)
			GD.PushWarning("BulletConfig has no FiringSolver assigned.");


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

	public virtual bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity, out FiringSolution solution)
	{
		if (!_cooldownTimer.IsStopped())
		{
			solution = default;
			return false;
		}

		ShooterState shooter = new ShooterState(BulletSpawnPoint.GlobalPosition, ownerVelocity);
		if (!WeaponConfig.BulletConfig.FiringSolver.TrySolve(shooter, targetInfo, out solution))
			return false;

		SpawnBullet(solution);
		ShootAnimation();
		_cooldownTimer.Start();
		return true;
	}

	private void SpawnBullet(FiringSolution solution)
	{
		BulletBase bullet = WeaponConfig.BulletConfig.BulletPackedScene.Instantiate<BulletBase>();
		bullet.TopLevel = true;
		AddChild(bullet);

		bullet.Setup(WeaponConfig.BulletConfig, BulletSpawnPoint.GlobalPosition, solution);
	}

	public abstract void ShootAnimation();
}
