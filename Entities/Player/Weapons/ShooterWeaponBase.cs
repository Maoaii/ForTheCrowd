using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using CarGame.Systems.Blackboards;
using CarGame.Utils.Debug;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class ShooterWeaponBase : Node3D, IWeapon
{
	[Export] public ShootAnimationConfig ShootAnimationConfig;
	[Export] public Marker3D BulletSpawnPoint;

	public ShooterWeaponConfig WeaponConfig { get; private set; }
	protected BlackboardComponent _blackboard;
	protected Timer _cooldownTimer;

	private float _restYaw;
	private float _targetYaw = 0.0f;

	private readonly RandomNumberGenerator _random = new();
	private ulong _shotSeed;


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
		if (WeaponConfig?.BulletConfig?.Ballistics == null)
			GD.PushWarning("BulletConfig has no Ballistics assigned.");

		_cooldownTimer = new Timer()
		{
			WaitTime = WeaponConfig.Cooldown,
			Autostart = false,
			OneShot = true,
		};

		AddChild(_cooldownTimer);

		_restYaw = Rotation.Y;
		_shotSeed = GD.Randi();
	}

	public override void _Process(double delta)
	{
		float weight = 1.0f - Mathf.Exp(-ShootAnimationConfig.LookSmoothing * (float)delta);

		Rotation = new Vector3(
			Rotation.X,
			Mathf.LerpAngle(Rotation.Y, _targetYaw, weight),
			Rotation.Z);

		DebugDrawGizmos();
	}

	public virtual bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		if (!TryPlanShot(targetInfo, ownerVelocity, out FiringSolution solution))
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
			SpawnBullet(TryPlanShot(predictedTarget, ownerVelocity, out FiringSolution spawnSolution) ? spawnSolution : solution);
			_shotSeed = GD.Randi();
		}

		ShootAnimation(solution, Fire);
		_cooldownTimer.Start();

		return true;
	}

	public abstract void ShootAnimation(FiringSolution solution, Action fire);

	private bool TryPlanShot(in TargetInfo target, Vector3 ownerVelocity, out FiringSolution solution)
	{
		solution = default;
		BallisticsBase ballistics = WeaponConfig.BulletConfig.Ballistics;
		Vector3 muzzlePosition = BulletSpawnPoint.GlobalPosition;
		Vector3 inheritedVelocity = WeaponConfig.InheritOwnerVelocity ? ownerVelocity : Vector3.Zero;

		_random.Seed = _shotSeed;
		AimContext context = new(muzzlePosition, inheritedVelocity, target, ballistics, _random);

		Vector3 aimPoint = target.Position;
		foreach (AimModifier modifier in WeaponConfig.AimModifiers)
		{
			if (modifier == null)
				continue;

			aimPoint = modifier.Apply(context, aimPoint);
		}

		if (!ballistics.TrySolve(muzzlePosition, inheritedVelocity, aimPoint, out Vector3 launchVelocity, out float flightTime))
			return false;

		Vector3 muzzleVelocity = launchVelocity - inheritedVelocity;
		Vector3 muzzleDirection = muzzleVelocity.LengthSquared() > 0.0001f ? muzzleVelocity.Normalized() : launchVelocity.Normalized();

		solution = new FiringSolution(aimPoint, launchVelocity, muzzleDirection, flightTime);
		return true;
	}

	protected void SmoothLookAt(Vector3 shootingDirection, bool useModelFront)
	{
		Vector3 localShootingDirection = ToLocal(shootingDirection);
		localShootingDirection = localShootingDirection.Rotated(Vector3.Right, Rotation.X);
		localShootingDirection.Y = 0f;

		if (localShootingDirection.LengthSquared() < 0.0001f)
			return;

		_targetYaw = Rotation.Y + (useModelFront ? Vector3.Forward : Vector3.Back).SignedAngleTo(localShootingDirection, Vector3.Up);
		_targetYaw = Mathf.Clamp(_targetYaw, -WeaponConfig.RotationConeDegrees, WeaponConfig.RotationConeDegrees);
	}

	private void SpawnBullet(FiringSolution solution)
	{
		BulletBase bullet = WeaponConfig.BulletConfig.BulletPackedScene.Instantiate<BulletBase>();
		bullet.TopLevel = true;
		AddChild(bullet);

		bullet.Setup(WeaponConfig.BulletConfig, BulletSpawnPoint.GlobalPosition, solution);
	}

	private void DebugDrawGizmos()
	{
		if (Engine.IsEditorHint())
			return;

		Vector3 leftRotationLimit = Vector3.Forward.Rotated(Vector3.Up, GlobalRotation.Y - _targetYaw + _restYaw - WeaponConfig.RotationConeDegrees);
		Vector3 rightRotationLimit = Vector3.Forward.Rotated(Vector3.Up, GlobalRotation.Y - _targetYaw + _restYaw + WeaponConfig.RotationConeDegrees);
		VectorRenderer.DrawLine(GlobalPosition, GlobalPosition + (leftRotationLimit * 2), Colors.Green);
		VectorRenderer.DrawLine(GlobalPosition, GlobalPosition + (rightRotationLimit * 2), Colors.Green);
	}
}
