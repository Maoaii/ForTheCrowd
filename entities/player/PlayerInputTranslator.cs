using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player;

public class PlayerInputTranslator
{
    private Blackboard _blackboard;

    public PlayerInputTranslator(Blackboard blackboard)
    {
        _blackboard = blackboard;
    }

    public void Update(double delta)
    {
        Vector2 moveInput = Input.GetVector("move_left", "move_right", "move_backward", "move_forward");

        _blackboard.Input.MoveInput = moveInput;
        _blackboard.Input.WantsJump = Input.IsActionJustPressed("jump");
        _blackboard.Input.IsHoldingJump = Input.IsActionPressed("jump");
        
        if (Input.IsActionJustPressed("jump"))
            _blackboard.Input.LastJumpInputTime = Time.GetTicksMsec() / 1000.0f;
        
        _blackboard.Input.ReleasedJump = Input.IsActionJustReleased("jump");

        _blackboard.Tricks.WantsShoveIt = Input.IsActionJustPressed("trick_shove_it");
        _blackboard.Tricks.WantsKickflip = Input.IsActionJustPressed("trick_kickflip");
        _blackboard.Tricks.WantsBackManual = Input.IsActionPressed("trick_back_manual");
        _blackboard.Tricks.WantsStopBackManual = Input.IsActionJustReleased("trick_back_manual");
    }
}