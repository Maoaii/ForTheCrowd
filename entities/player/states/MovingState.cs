using System;
using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public class MovingState : State
{
    public bool JustJumped => !_blackboard.State.IsOnGround && _blackboard.State.WasOnGround;
    public bool JustHitGround => _blackboard.State.IsOnGround && !_blackboard.State.WasOnGround;
    public MovingState() : base(PlayerStates.MOVING_STATE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.State.WantsGroundPhysics = true;

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

        TryChangingStates(delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
        _blackboard.State.WantsGroundPhysics = false;
    }

    private void TryChangingStates(double delta)
    {
        bool isAlmostStopped = Math.Abs(_blackboard.Kinematics.Speed) <= 0.5f && _blackboard.Input.MoveInput == Godot.Vector2.Zero;

        if (isAlmostStopped)
            _stateMachine.TransitionState(PlayerStates.IDLE_STATE, delta);
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.OLLIE_STATE, delta);
        if (_blackboard.Tricks.WantsBackManual)
            _stateMachine.TransitionState(PlayerStates.BACK_MANUAL_STATE, delta);
    }
}
