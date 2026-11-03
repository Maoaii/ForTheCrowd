using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Components;

[Tool]
[GlobalClass]
public partial class AIInputComponent : Node
{
	[Export] public BlackboardComponent Blackboard;

	private readonly record struct MovementInfo(Vector2 Direction, bool WantsJump, bool WantsBoost);
	private readonly record struct AttackInfo(bool WantsActivateSlot1, bool WantsActivateSlot2, bool WantsActivateSlot3, bool WantsActivateSlot4);
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
		movementInfo = new MovementInfo(Vector2.Up, false, false);
	}

	private void CalculateAttackBehaviour(out AttackInfo attackInfo)
	{
		attackInfo = default;
	}
}