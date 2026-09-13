using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public partial class BackManualState : TrickState
{
    private float _timeEnteredTrick;

    public override void _Ready()
    {
    }

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
            _stateMachine.TransitionState(PlayerStates.Moving, delta);
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.Ollie, delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
        _blackboard.State.WantsGroundPhysics = false;
        _blackboard.Animator.PlayBackManualAnimation(true);
    }
}
