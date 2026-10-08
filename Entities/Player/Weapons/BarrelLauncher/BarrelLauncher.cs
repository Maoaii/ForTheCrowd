using System;
using Godot;

public partial class BarrelLauncher : WeaponBase
{
	public override bool TryActivate(Vector3 direction)
	{
		if (!_cooldownTimer.IsStopped())
			return false;

		BulletBase bullet = WeaponConfig.BulletPackedScene.Instantiate<BulletBase>();
		bullet.TopLevel = true;
		AddChild(bullet);
		bullet.GlobalPosition = BulletSpawnPoint.GlobalPosition;
		bullet.SetLifetime(WeaponConfig.BulletLifetime);
		bullet.SetDirection(direction, WeaponConfig.InitialSpeed);

		base.TryActivate(direction);
		_cooldownTimer.Start();
		return true;
	}

	public override void ShootAnimation()
	{
		//throw new NotImplementedException();
	}
}
