using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public abstract partial class BulletBase : Area3D
{
	private BulletConfig _config;
	private Vector3 _aimPoint;

	private ProjectileState _state;

	public override void _Ready()
	{
		base._Ready();

		AreaEntered += area => QueueFree();
		BodyEntered += body => QueueFree();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_state.Age += (float)delta;

		_config.FiringSolver.Step(ref _state, (float)delta);
		GlobalPosition = _state.Position;
		OrientTowardsDirection();

	}

	public void Setup(BulletConfig config, Vector3 position, in FiringSolution solution)
	{
		_config = config;

		GlobalPosition = position;
		_aimPoint = solution.AimPoint;

		_state = new() { Position = position, Velocity = solution.LaunchVelocity, };

		SetLifetime(config.BulletLifetime);
		OrientTowardsDirection();
	}

	private void SetLifetime(float lifetime)
	{
		GetTree().CreateTimer(lifetime).Timeout += QueueFree;
	}

	public virtual void OrientTowardsDirection()
	{
		if (_state.Velocity.LengthSquared() < 0.0001f)
			return;

		LookAt(GlobalPosition + _state.Velocity);
	}

}
