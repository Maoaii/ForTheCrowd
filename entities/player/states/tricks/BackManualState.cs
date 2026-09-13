using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public class BackManualState : TrickState
{
    private float _timeEnteredTrick;
    public BackManualState() : base(PlayerStates.BACK_MANUAL_STATE, Constants.BACK_MANUAL_SCORE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.State.WantsGroundPhysics = true;
        _timeEnteredTrick = Time.GetTicksMsec();

        _blackboard.Animator.PlayBackManualAnimation();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        if (_blackboard.Tricks.WantsStopBackManual)
            _stateMachine.TransitionState(PlayerStates.MOVING_STATE, delta);
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.OLLIE_STATE, delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
        _blackboard.State.WantsGroundPhysics = false;
        _blackboard.Animator.PlayBackManualAnimation(true);
    }
}
