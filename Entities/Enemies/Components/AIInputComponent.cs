using System;
using CarGame.Systems.Blackboards;
using CarGame.Utils.Debug;
using Godot;

namespace CarGame.Entities.Components;

[Tool]
[GlobalClass]
public partial class AIInputComponent : Node
{
	[Export] public BlackboardComponent Blackboard;
	[Export] public EnemyBase Owner;

	[Export(PropertyHint.Range, "0, 100, 1, suffix:m")] public float MinFollowRange;
	[Export(PropertyHint.Range, "0, 100, 1, suffix:m")] public float MaxFollowRange;
	[Export(PropertyHint.Range, "0, 50, 0.1, suffix:m")] public float SeekReachedSensitivity;

	private bool reachedCurrentSeekTarget = false;
	private Vector2 currentSeekOffset = Vector2.Zero;

	private readonly record struct MovementInfo(Vector2 Direction, bool WantsJump, bool WantsBoost);
	private readonly record struct AttackInfo(bool WantsActivateSlot1, bool WantsActivateSlot2, bool WantsActivateSlot3, bool WantsActivateSlot4);

	public Vector3 GetSeekPosition => Owner.Target.GlobalPosition + new Vector3(currentSeekOffset.X, 0, currentSeekOffset.Y);

	public override void _Ready()
	{
		base._Ready();
	}
	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint()) return;
		if (Blackboard == null) return;

		CalculateMovementBehaviour(out MovementInfo movementInfo);
		CalculateAttackBehaviour(out AttackInfo attackInfo);

		Blackboard.Input.MoveInput = movementInfo.Direction;
		Blackboard.Input.WantsJump = movementInfo.WantsJump;
		Blackboard.Input.WantsBoost = movementInfo.WantsBoost;


		Blackboard.Input.WantsActivateSlot1 = attackInfo.WantsActivateSlot1;
		Blackboard.Input.WantsActivateSlot2 = attackInfo.WantsActivateSlot2;
		Blackboard.Input.WantsActivateSlot3 = attackInfo.WantsActivateSlot3;
		Blackboard.Input.WantsActivateSlot4 = attackInfo.WantsActivateSlot4;


		if (Input.IsActionJustPressed("jump"))
			Blackboard.Input.LastJumpInputTime = Time.GetTicksMsec() / 1000.0f;
	}

	private void CalculateMovementBehaviour(out MovementInfo movementInfo)
	{
		bool noSeekTarget = currentSeekOffset == Vector2.Zero;
		if (noSeekTarget || HasReachedSeekTarget())
			SetupNewSeekTarget();

		movementInfo = new MovementInfo()
		{
			Direction = CalculateSeekDirection(),
			WantsJump = false,
			WantsBoost = false
		};

		DebugDrawGizmos();
	}

	private Vector2 CalculateSeekDirection()
	{
		Vector3 directionToTarget = Vector3.Zero;
		if (Owner.Target != null)
		{
			directionToTarget = GetSeekPosition - Owner.GlobalPosition;
			directionToTarget.Y = 0;

			if (directionToTarget != Vector3.Zero)
				directionToTarget = directionToTarget.Normalized();
		}

		float xDir = Owner.Basis.X.Dot(directionToTarget);
		if (xDir >= 0.99)
			xDir = 0;

		float yDir = 1;

		return new Vector2(xDir, yDir);
	}

	private bool HasReachedSeekTarget()
	{
		return Owner.GlobalPosition.DistanceTo(GetSeekPosition) < SeekReachedSensitivity;
	}

	private void SetupNewSeekTarget()
	{
		float distance = (float)GD.RandRange(MinFollowRange, MaxFollowRange);
		float angle = (float)GD.RandRange(0, Mathf.Tau);

		currentSeekOffset = Vector2.Right.Rotated(angle) * distance;
	}

	private void CalculateAttackBehaviour(out AttackInfo attackInfo)
	{
		attackInfo = default;
	}

	private void DebugDrawGizmos()
	{
		Vector3 targetPos = Owner.Target.GlobalPosition;
		VectorRenderer.DrawBand(targetPos, MinFollowRange, MaxFollowRange, Colors.Green, dashed: true);

		VectorRenderer.DrawDisc(GetSeekPosition, SeekReachedSensitivity, Colors.Red);
	}
}