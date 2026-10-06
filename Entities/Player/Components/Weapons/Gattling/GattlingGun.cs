using Godot;
using System;

public partial class GattlingGun : Node3D, IWeapon
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

  public void SetInitialPosition(Vector3 position)
  {
    GlobalPosition = position;
  }

  public void TryActivate(Vector3 direction)
  {
    if (!_cooldownTimer.IsStopped())
      return;
    
    GattlingBullet bullet = _bulletPackedScene.Instantiate<GattlingBullet>();
    bullet.TopLevel = true; 
    AddChild(bullet);
    bullet.GlobalPosition = GlobalPosition;
    bullet.SetLifetime(_weaponConfig.BulletLifetime);
    bullet.SetDirection(direction, _weaponConfig.InitialSpeed);

    _cooldownTimer.Start();
  }
}
