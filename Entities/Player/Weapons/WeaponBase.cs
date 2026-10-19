using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class WeaponBase : Node3D
{
	[Export] public ShootAnimationConfig ShootAnimationConfig;

	[Export] public WeaponConfig WeaponConfig;
	[Export] public Marker3D BulletSpawnPoint;

	protected Timer _cooldownTimer;

	private float? _targetYaw;


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

	public void SetInitialPosition(Vector3 position)
	{
		GlobalPosition = position;
	}

	public virtual bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		ShooterState shooter = new(BulletSpawnPoint.GlobalPosition, ownerVelocity);
		if (!WeaponConfig.BulletConfig.FiringSolver.TrySolve(shooter, targetInfo, out FiringSolution solution))
			return false;

		SmoothLookAt(GlobalPosition + solution.MuzzleDirection, ShootAnimationConfig.UseModelFront);

		if (!_cooldownTimer.IsStopped())
			return false;

		ShootAnimation(solution);
		SpawnBullet(solution);
		_cooldownTimer.Start();

		return true;
	}

	public abstract void ShootAnimation(FiringSolution solution);

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
