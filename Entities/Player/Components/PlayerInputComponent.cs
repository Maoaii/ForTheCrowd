using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Components;

public partial class PlayerInputComponent : Node
{
    [Export] public BlackboardComponent Blackboard;

    public override void _PhysicsProcess(double delta)
    {
        if (Blackboard == null) return;
        
        Vector2 moveInput = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");

        Blackboard.Input.MoveInput = moveInput;
        Blackboard.Input.WantsJump = Input.IsActionJustPressed("jump");
        Blackboard.Input.IsHoldingJump = Input.IsActionPressed("jump");
        Blackboard.Input.WantsBoost = Input.IsActionPressed("boost");
        
        
        Blackboard.Input.WantsActivateSlot1 = Input.IsActionPressed("slot1");
        Blackboard.Input.WantsActivateSlot2 = Input.IsActionPressed("slot2");
        Blackboard.Input.WantsActivateSlot3 = Input.IsActionPressed("slot3");
        Blackboard.Input.WantsActivateSlot4 = Input.IsActionPressed("slot4");


        if (Input.IsActionJustPressed("jump"))
            Blackboard.Input.LastJumpInputTime = Time.GetTicksMsec() / 1000.0f;
        
        Blackboard.Input.ReleasedJump = Input.IsActionJustReleased("jump");

        Blackboard.Tricks.WantsShoveIt = Input.IsActionJustPressed("trick_shove_it");
        Blackboard.Tricks.WantsKickflip = Input.IsActionJustPressed("trick_kickflip");
        Blackboard.Tricks.WantsBackManual = Input.IsActionPressed("trick_back_manual");
        Blackboard.Tricks.WantsStopBackManual = Input.IsActionJustReleased("trick_back_manual");


    }
}