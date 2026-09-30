using Godot;
using System;

public partial class GattlingGun : Node3D, IWeapon
{
	[Export] private WeaponConfig _weaponConfig;
	[Export] private PackedScene _bulletPackedScene;

  public void SetInitialBasis(Basis basis)
  {
    GlobalBasis = basis;
  }

  public void TryActivate(Vector3 direction)
  {
    GattlingBullet bullet = _bulletPackedScene.Instantiate<GattlingBullet>();
    bullet.TopLevel = true; 
    AddChild(bullet);
    bullet.GlobalPosition = GlobalPosition;
    bullet.SetLifetime(_weaponConfig.BulletLifetime);
    bullet.SetDirection(direction, _weaponConfig.InitialSpeed);
  }
}
