using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class ShooterWeaponBase : Node3D, IWeapon
{
	[Export] public ShootAnimationConfig ShootAnimationConfig;
	[Export] public Marker3D BulletSpawnPoint;

	public ShooterWeaponConfig WeaponConfig { get; private set; }
	protected BlackboardComponent _blackboard;
	protected Timer _cooldownTimer;

	private float? _targetYaw;


	public void Setup(WeaponConfig config, BlackboardComponent blackboard, bool enabled)
	{
		WeaponConfig = config as ShooterWeaponConfig;
		_blackboard = blackboard;
	}

	public override void _Ready()
	{
		if (WeaponConfig == null)
			GD.PushWarning("WeaponConfig is missing or not a ShooterWeaponConfig. Was Setup called?");
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

	public override void _Process(double delta)
	{
		if (_targetYaw is not float targetYaw)
			return;

		float weight = 1.0f - Mathf.Exp(-ShootAnimationConfig.LookSmoothing * (float)delta);
		GlobalRotation = new Vector3(
			GlobalRotation.X,
			Mathf.LerpAngle(GlobalRotation.Y, targetYaw, weight),
			GlobalRotation.Z);
	}

	public virtual bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		ShooterState shooter = new(BulletSpawnPoint.GlobalPosition, ownerVelocity);
		if (!WeaponConfig.BulletConfig.FiringSolver.TrySolve(shooter, targetInfo, out FiringSolution solution))
			return false;

		SmoothLookAt(GlobalPosition + solution.MuzzleDirection, ShootAnimationConfig.UseModelFront);

		if (!_cooldownTimer.IsStopped())
			return false;

		bool fired = false;
		ulong activatedAtMs = Time.GetTicksMsec();
		void Fire()
		{
			if (fired)
				return;
			fired = true;

			// the shoot animation may delay firing and move the muzzle, so re-solve from the actual spawn state
			float elapsed = (Time.GetTicksMsec() - activatedAtMs) / 1000.0f;
			TargetInfo predictedTarget = targetInfo with { Position = targetInfo.Position + targetInfo.Velocity * elapsed };
			ShooterState spawnShooter = new(BulletSpawnPoint.GlobalPosition, ownerVelocity);
			if (WeaponConfig.BulletConfig.FiringSolver.TrySolve(spawnShooter, predictedTarget, out FiringSolution spawnSolution))
				SpawnBullet(spawnSolution);
			else
				SpawnBullet(solution);
		}

		ShootAnimation(solution, Fire);
		_cooldownTimer.Start();

		return true;
	}

	public abstract void ShootAnimation(FiringSolution solution, Action fire);

	protected void SmoothLookAt(Vector3 targetPosition, bool useModelFront)
	{
		Vector3 direction = targetPosition - GlobalPosition;
		if (direction.LengthSquared() < 0.0001f)
			return;

		_targetYaw = Basis.LookingAt(direction, Vector3.Up, useModelFront).GetEuler().Y;
	}

	private void SpawnBullet(FiringSolution solution)
	{
		BulletBase bullet = WeaponConfig.BulletConfig.BulletPackedScene.Instantiate<BulletBase>();
		bullet.TopLevel = true;
		AddChild(bullet);

		bullet.Setup(WeaponConfig.BulletConfig, BulletSpawnPoint.GlobalPosition, solution);
	}
}
