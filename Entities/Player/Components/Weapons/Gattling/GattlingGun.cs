using Godot;
using System;

public partial class GattlingGun : WeaponBase
{
	[Export] private WeaponConfig _weaponConfig;
	[Export] private PackedScene _bulletPackedScene;

  private Timer _cooldownTimer;


    public override void _Ready()
    {
        base._Ready();
        _cooldownTimer = new Timer()
        {
          WaitTime = _weaponConfig.Cooldown,
          Autostart = false,
          OneShot = true,
        };

        AddChild(_cooldownTimer);
    }

  public override bool TryActivate(Vector3 direction)
  {
    
    if (!_cooldownTimer.IsStopped())
      return false;
        
    GattlingBullet bullet = _bulletPackedScene.Instantiate<GattlingBullet>();
    bullet.TopLevel = true; 
    AddChild(bullet);
    bullet.GlobalPosition = GlobalPosition;
    bullet.SetLifetime(_weaponConfig.BulletLifetime);
    bullet.SetDirection(direction, _weaponConfig.InitialSpeed);

    base.TryActivate(direction);
    _cooldownTimer.Start();
    return true;
  }
}
