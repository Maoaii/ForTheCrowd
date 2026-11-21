using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class ShooterWeaponBase : Node3D, IWeapon
{
	[Export] public Marker3D BulletSpawnPoint;

	public ShooterWeaponConfig WeaponConfig { get; private set; }
	public ShootAnimationConfig ShootAnimationConfig { get; private set; }
	protected BlackboardComponent _blackboard;
	protected Timer _cooldownTimer;


	private WeaponSlot _weaponMount;
	private float _targetYaw = 0.0f;

	private readonly RandomNumberGenerator _random = new();
	private ulong _shotSeed;


	public void Setup(WeaponConfig config, BlackboardComponent blackboard, bool enabled, WeaponSlot weaponMount)
	{
		WeaponConfig = config as ShooterWeaponConfig;
		ShootAnimationConfig = WeaponConfig.ShootAnimationConfig;
		_blackboard = blackboard;
		_weaponMount = weaponMount;
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

		_shotSeed = GD.Randi();
	}

	public override void _Process(double delta)
	{
		float weight = 1.0f - Mathf.Exp(-ShootAnimationConfig.LookSmoothing * (float)delta);

		Rotation = new Vector3(
			Rotation.X,
			Mathf.LerpAngle(Rotation.Y, _targetYaw, weight),
			Rotation.Z);
	}

	public virtual bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		if (!_cooldownTimer.IsStopped())
			return false;

		if (!TryPlanShot(targetInfo, ownerVelocity, out FiringSolution solution))
		{
			_targetYaw = 0.0f;
			return false;
		}

		// TODO: extract this out of try activate so it is called every frame
		SmoothLookAt(GlobalPosition + solution.MuzzleDirection);

		bool fired = false;

		void Fire()
		{
			if (fired)
				return;
			fired = true;

			SpawnBullet(solution);
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
		_random.Seed = _shotSeed;

		BallisticsBase ballistics = WeaponConfig.BulletConfig.Ballistics;
		Vector3 muzzlePosition = BulletSpawnPoint.GlobalPosition;
		Vector3 inheritedVelocity = WeaponConfig.InheritOwnerVelocity ? ownerVelocity : Vector3.Zero;

		AimContext context = new(muzzlePosition, inheritedVelocity, target, ballistics, _random);

		Vector3 aimPoint = ApplyAimModifiers(target, context);

		if (!ballistics.TrySolve(muzzlePosition, inheritedVelocity, aimPoint, out Vector3 launchVelocity, out float flightTime))
			return false;

		Vector3 muzzleVelocity = (launchVelocity - inheritedVelocity).Normalized();

		solution = new FiringSolution(aimPoint, launchVelocity, muzzleVelocity, flightTime);

		return CanShootWithConstraints(solution, context);
	}

	private Vector3 ApplyAimModifiers(TargetInfo target, AimContext context)
	{
		Vector3 aimPoint = target.Position;
		foreach (AimModifier modifier in WeaponConfig.AimModifiers)
		{
			if (modifier == null)
				continue;

			aimPoint = modifier.Apply(context, aimPoint);
		}

		return aimPoint;
	}

	private bool CanShootWithConstraints(FiringSolution solution, AimContext context)
	{
		foreach (AimConstraint constraint in WeaponConfig.AimConstraints)
		{
			if (constraint == null)
				continue;

			// TODO: make applyconstraint receive aim context with weapon mount basis
			if (!constraint.ApplyConstraint(solution, _weaponMount))
				return false;
		}

		return true;
	}

	protected void SmoothLookAt(Vector3 shootingDirection)
	{
		Vector3 localShootingDirection = ToLocal(shootingDirection);
		localShootingDirection = localShootingDirection.Rotated(Vector3.Right, Rotation.X);
		localShootingDirection.Y = 0f;

		if (localShootingDirection.LengthSquared() < 0.0001f)
			return;

		_targetYaw = Rotation.Y + Vector3.Forward.SignedAngleTo(localShootingDirection, Vector3.Up);
	}

	private void SpawnBullet(FiringSolution solution)
	{
		BulletBase bullet = WeaponConfig.BulletConfig.BulletPackedScene.Instantiate<BulletBase>();
		bullet.TopLevel = true;
		AddChild(bullet);

		bullet.Setup(WeaponConfig.BulletConfig, BulletSpawnPoint.GlobalPosition, solution);
	}
}
