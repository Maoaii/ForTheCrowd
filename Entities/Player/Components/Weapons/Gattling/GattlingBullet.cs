using Godot;
using System;

public partial class GattlingBullet : Node3D, IBullet
{
	private Vector3 _direction;
	private float _speed;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
		Position += _direction * _speed * (float) delta;
    }
    public void SetDirection(Vector3 direction, float speed)
    {
        _direction = direction;
		_speed = speed;
    }
}
