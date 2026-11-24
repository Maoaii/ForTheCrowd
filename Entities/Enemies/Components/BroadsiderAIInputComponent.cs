using CarGame.Systems.Blackboards;
using CarGame.Utils.Debug;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Components;

[GlobalClass]
public partial class BroadsiderAIInputComponent : Node
{
	[Export] public BlackboardComponent Blackboard;
	[Export] public EnemyBase Owner;

	[Export(PropertyHint.Range, "0, 100, 1, suffix:m")] public float MinFollowRange;
	[Export(PropertyHint.Range, "0, 100, 1, suffix:m")] public float MaxFollowRange;
	[Export(PropertyHint.Range, "0, 50, 0.1, suffix:m")] public float SeekReachedSensitivity;
	[Export(PropertyHint.Range, "0, 100, 0.1, suffix:m")] public float SlowdownRadius = 10.0f;
	[Export(PropertyHint.Range, "0, 20000, 1.0, suffix:ms")] public float SeekLingerTime;
	[Export(PropertyHint.Range, "0, 20000, 1.0, suffix:ms")] public float SeekMaxLifetime;

	private readonly record struct MovementInfo(Vector2 Direction, bool WantsJump, bool WantsBoost);

	private Vector2 currentSeekOffset = Vector2.Zero;
	private ulong seekPickedAtMsec;
	private ulong? seekReachedAtMsec;
	private int _currentSeekSide = -1;

	public Vector3 SeekPosition => Owner.Target.GlobalPosition + new Vector3(currentSeekOffset.X, 0, currentSeekOffset.Y);

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint()) return;
		if (Blackboard == null) return;
		if (Owner.Target == null) return;

		UpdateSeekTarget();
		CalculateMovementBehaviour(out MovementInfo movementInfo);

		Blackboard.Input.MoveInput = movementInfo.Direction;
		Blackboard.Input.SpeedLimitFactor = CalculateSpeedLimitFactor();
		Blackboard.Input.WantsJump = movementInfo.WantsJump;
		Blackboard.Input.WantsBoost = movementInfo.WantsBoost;

		if (Input.IsActionJustPressed("jump"))
			Blackboard.Input.LastJumpInputTime = Time.GetTicksMsec() / 1000.0f;

		DebugDrawGizmos();
	}

	private void UpdateSeekTarget()
	{
		ulong now = Time.GetTicksMsec();

		bool hasSeekTarget = currentSeekOffset != Vector2.Zero;
		bool nearSeekTarget = hasSeekTarget && IsNearSeekTarget();
		if (nearSeekTarget)
			seekReachedAtMsec ??= now;

		bool doneLingering = nearSeekTarget && now - seekReachedAtMsec.Value > SeekLingerTime;
		bool expired = SeekMaxLifetime > 0 && now - seekPickedAtMsec >= SeekMaxLifetime;


		UpdateSeekPosition();
		if (!hasSeekTarget || doneLingering || expired)
			PickNewSeekTarget();
	}

	private void UpdateSeekPosition()
	{
		Basis targetBasis = Owner.Target.GlobalBasis;

		Vector3 newSeekPos = targetBasis.X * _currentSeekSide * currentSeekOffset.Length();
		currentSeekOffset = new Vector2(newSeekPos.X, newSeekPos.Z);
	}

	private void PickNewSeekTarget()
	{
		_currentSeekSide = (int)new Array { -1, 1 }.PickRandom();

		float distance = (float)GD.RandRange(MinFollowRange, MaxFollowRange);
		Vector3 newSeekPos = Owner.Target.GlobalBasis.X * _currentSeekSide * distance;

		currentSeekOffset = new Vector2(newSeekPos.X, newSeekPos.Z);
		seekPickedAtMsec = Time.GetTicksMsec();
		seekReachedAtMsec = null;
	}

	private bool IsNearSeekTarget()
	{
		return Owner.GlobalPosition.DistanceTo(SeekPosition) < SeekReachedSensitivity;
	}

	private void CalculateMovementBehaviour(out MovementInfo movementInfo)
	{
		movementInfo = new MovementInfo()
		{
			Direction = CalculateSeekDirection(),
			WantsJump = false,
			WantsBoost = false
		};
	}

	private Vector2 CalculateSeekDirection()
	{
		// TODO: take into account target's velocity
		Vector3 directionToSeekPosition = FlatOffsetToSeekPosition();
		if (directionToSeekPosition != Vector3.Zero)
			directionToSeekPosition = directionToSeekPosition.Normalized();

		float xDir = Owner.Basis.X.Dot(directionToSeekPosition);
		if (xDir >= 0.99)
			xDir = 0;

		float yDir = 1;

		return new Vector2(xDir, yDir);
	}

	private float CalculateSpeedLimitFactor()
	{
		if (SlowdownRadius <= 0)
			return 1f;

		Vector3 toSeekPosition = FlatOffsetToSeekPosition();
		if (toSeekPosition.Length() >= SlowdownRadius)
			return 1f;

		float distanceAhead = toSeekPosition.Dot(-Owner.Basis.Z);
		return Mathf.Clamp(distanceAhead / SlowdownRadius, 0f, 1f);
	}

	private Vector3 FlatOffsetToSeekPosition()
	{
		Vector3 offset = SeekPosition - Owner.GlobalPosition;
		offset.Y = 0;
		return offset;
	}

	private void DebugDrawGizmos()
	{
		Vector3 targetPos = Owner.Target.GlobalPosition;
		VectorRenderer.DrawBand(targetPos, MinFollowRange, MaxFollowRange, Colors.Green, dashed: true);

		VectorRenderer.DrawDisc(SeekPosition, SeekReachedSensitivity, Colors.Red);
	}
}
