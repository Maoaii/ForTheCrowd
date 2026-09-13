using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public partial class IdleState : PlayerState
{
    [ExportGroup("Landing Camera Kick")]
    [Export] public float LandingFovKick = (Mathf.Pi / 4.0f) * 1.03f;
    [Export] public float LandingFovKickTimeUpS = 0.05f;
    [Export] public float LandingFovKickTimeDownS = 0.3f;

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.Kinematics.Speed = 0.0f;

        if (delta != 0 && !_blackboard.State.WasOnGround)
        {
            _blackboard.Animator.PlayLandingAnimation();
            _blackboard.Camera.SetFOVKick(LandingFovKick, LandingFovKickTimeUpS, LandingFovKickTimeDownS);
            
            _blackboard.Events.OnLanded?.Invoke(_blackboard.Kinematics.Velocity.Y);
        }
        
        TrickPopupManager.Instance?.ResetTricks();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        if (_blackboard.Input.MoveInput != Vector2.Zero)
            _stateMachine.TransitionState(PlayerStates.Moving, delta);
        
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.Ollie, delta);
    }
}