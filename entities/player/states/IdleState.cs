using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public class IdleState : State
{
    public IdleState() : base(PlayerStates.IDLE_STATE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.Kinematics.Speed = 0.0f;

        if (delta != 0 && !_blackboard.State.WasOnGround)
        {
            _blackboard.Animator.PlayLandingAnimation();
            _blackboard.Camera.SetFOVKick(Constants.LANDING_FOV_KICK, Constants.LANDING_FOV_KICK_TIME_UP_S, Constants.LANDING_FOV_KICK_TIME_DOWN_S);
            
            _blackboard.Events.OnLanded?.Invoke(_blackboard.Kinematics.Velocity.Y);
        }
        
        TrickPopupManager.Instance?.ResetTricks();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        if (_blackboard.Input.MoveInput != Vector2.Zero)
            _stateMachine.TransitionState(PlayerStates.MOVING_STATE, delta);
        
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.OLLIE_STATE, delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
    }
}