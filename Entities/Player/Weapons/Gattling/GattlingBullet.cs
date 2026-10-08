using Godot;
using System;

public partial class GattlingBullet : BulletBase
{
	private Vector3 _direction;
	private float _speed;

    public override void _Ready()
    {
        base._Ready();
        AreaEntered += area => QueueFree();
        BodyEntered += body => QueueFree();
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		GlobalPosition += _direction * _speed * (float) delta;
        LookAt(GlobalPosition + _direction);
    }
    public override void SetDirection(Vector3 direction, float speed)
    {
        _direction = direction;
		_speed = speed;
        LookAt(GlobalPosition + _direction);
    }

    public override void SetLifetime(float lifetime)
    {
        GetTree().CreateTimer(lifetime).Timeout += () => QueueFree();
    }
}
