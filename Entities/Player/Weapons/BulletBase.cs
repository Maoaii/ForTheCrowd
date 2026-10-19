using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class BulletBase : Area3D
{
	[Export] public float TurnSmoothing = 20.0f;

	private BulletConfig _config;
	private Vector3 _aimPoint;

	private ProjectileState _state;
	private bool _hasCollided = false;

	public override void _Ready()
	{
		base._Ready();

		AreaEntered += area => Collided();
		BodyEntered += body => Collided();
	}

	public override void _PhysicsProcess(double delta)
	{
		base._PhysicsProcess(delta);
		_state.Age += (float)delta;

		_config.FiringSolver.Step(ref _state, (float)delta);
		GlobalPosition = _state.Position;
		OrientTowardsDirection((float)delta);
	}

	public void Setup(BulletConfig config, Vector3 position, in FiringSolution solution)
	{
		_config = config;

		GlobalPosition = position;
		_aimPoint = solution.AimPoint;

		_state = new() { Position = position, Velocity = solution.LaunchVelocity, };

		SetLifetime(config.BulletLifetime);
		SnapToDirection();
	}

	public virtual void OrientTowardsDirection(float delta)
	{
		if (_state.Velocity.LengthSquared() < 0.0001f)
			return;

		float weight = 1.0f - Mathf.Exp(-TurnSmoothing * delta);
		GlobalBasis = GlobalBasis.Orthonormalized().Slerp(Basis.LookingAt(_state.Velocity), weight);
	}

	public virtual void SnapToDirection()
	{
		if (_state.Velocity.LengthSquared() < 0.0001f)
			return;

		GlobalBasis = Basis.LookingAt(_state.Velocity);
	}

	public virtual void Collided()
	{
		if (_hasCollided)
			return;

		_hasCollided = true;

		if (_config.ImpactScene != null)
		{
			// Root has an identity transform, so a local position is a global one. Deferred because
			// adding a physics body inside an Area3D signal happens while physics queries are flushing.
			Node3D impact = _config.ImpactScene.Instantiate<Node3D>();
			impact.Position = GlobalPosition;
			GetTree().Root.CallDeferred(Node.MethodName.AddChild, impact);
		}

		GetTree().CreateTimer(_config.TimeToFreeAfterCollision).Timeout += QueueFree;
	}

	private void SetLifetime(float lifetime)
	{
		GetTree().CreateTimer(lifetime).Timeout += QueueFree;
	}
}
