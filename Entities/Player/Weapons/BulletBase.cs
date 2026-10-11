using System;
using Godot;

public abstract partial class BulletBase : Area3D
{
    private BulletConfig _config;
    private Vector3 _direction;
    private Vector3 _targetPosition;
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
        MoveBullet(delta);
        OrientTowardsDirection();
    }

    public void Setup(BulletConfig config, Vector3 direction, Vector3 position, Vector3 targetPosition)
    {
        _config = config;
        _direction = direction;
        _speed = config.InitialSpeed;

        GlobalPosition = position;
        _targetPosition = targetPosition;

        SetLifetime(config.BulletLifetime);
        OrientTowardsDirection();
    }

    private void SetLifetime(float lifetime)
    {
        GetTree().CreateTimer(lifetime).Timeout += () => QueueFree();
    }

    public virtual void OrientTowardsDirection()
    {
        LookAt(GlobalPosition + _direction);
    }


    public virtual void MoveBullet(double delta)
    {
        GlobalPosition += _direction * _speed * (float)delta;
    }
}
