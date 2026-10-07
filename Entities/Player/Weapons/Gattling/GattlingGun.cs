using Godot;

public partial class GattlingGun : WeaponBase
{

  private Timer _cooldownTimer;


    public override void _Ready()
    {
        base._Ready();
        _cooldownTimer = new Timer()
        {
          WaitTime = WeaponConfig.Cooldown,
          Autostart = false,
          OneShot = true,
        };

        AddChild(_cooldownTimer);
    }

  public override bool TryActivate(Vector3 direction)
  {
    if (!_cooldownTimer.IsStopped())
      return false;
        
    GattlingBullet bullet = BulletPackedScene.Instantiate<GattlingBullet>();
    bullet.TopLevel = true; 
    AddChild(bullet);
    bullet.GlobalPosition = GlobalPosition;
    bullet.SetLifetime(WeaponConfig.BulletLifetime);
    bullet.SetDirection(direction, WeaponConfig.InitialSpeed);

    base.TryActivate(direction);
    _cooldownTimer.Start();
    return true;
  }
}
