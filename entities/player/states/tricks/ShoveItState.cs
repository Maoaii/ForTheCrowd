using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public class ShoveItState : TrickState
{
    private float _timeEnteredTrick;
    public ShoveItState() : base(PlayerStates.SHOVE_IT_STATE, Constants.SHOVE_IT_SCORE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.Kinematics.GravityToggler = 0.0f;
        Godot.Vector3 vel = _blackboard.Kinematics.Velocity;
        vel.Y = 0.0f;
        _blackboard.Kinematics.Velocity = vel;
        _timeEnteredTrick = Time.GetTicksMsec();
        _blackboard.State.CanMove = false;

        _blackboard.Animator.PlayShoveItAnimation();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        Godot.Vector3 vel = _blackboard.Kinematics.Velocity;
        vel.Y = 0.0f;
        _blackboard.Kinematics.Velocity = vel;
        float currentTime = Time.GetTicksMsec();
        if (currentTime - _timeEnteredTrick > _blackboard.MovementConfig.ShoveItTrickDuration)
            _stateMachine.TransitionState(PlayerStates.AIR_STATE, delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
        _blackboard.Kinematics.GravityToggler = 1.0f;
        _blackboard.Input.WantsJump = false;
    }
}