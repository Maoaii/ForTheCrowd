using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public class AirState : State
{
    public AirState() : base(PlayerStates.AIR_STATE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        if (_blackboard.Input.WantsJump)
            ApplyJump();
            
        _blackboard.Input.WantsJump = false;
        _blackboard.State.CanMove = false;

        if (delta != 0)
            _blackboard.Animator.PlayJumpAnimation();
    }

    public override void Update(double delta)
    {
        base.Update(delta);
        float currentTime = Time.GetTicksMsec() / 1000.0f;

        if (_blackboard.State.IsOnGround && _blackboard.Kinematics.Velocity.Y <= 0.0f)
        {
            if (CanJumpBuffer(currentTime))
            {
                _blackboard.Input.WantsJump = true;
                _stateMachine.TransitionState(PlayerStates.AIR_STATE, delta);
            }
            else if (_blackboard.Tricks.WantsBackManual)
                _stateMachine.TransitionState(PlayerStates.BACK_MANUAL_STATE, delta);
            else if (_blackboard.Kinematics.Speed != 0.0f)
                _stateMachine.TransitionState(PlayerStates.MOVING_STATE, delta);
            else
                _stateMachine.TransitionState(PlayerStates.IDLE_STATE, delta);
        }

        if (_blackboard.Tricks.WantsShoveIt)
            _stateMachine.TransitionState(PlayerStates.SHOVE_IT_STATE, delta);
        
        if (_blackboard.Tricks.WantsKickflip)
            _stateMachine.TransitionState(PlayerStates.KICKFLIP_STATE, delta);
    }

    public override void Exit(double delta = 0)
    {
        base.Exit(delta);
        _blackboard.State.CanMove = true;
        _blackboard.Input.LastJumpInputTime = -1.0f;
    }

    private void ApplyJump()
    {
        Godot.Vector3 vel = _blackboard.Kinematics.Velocity;
        vel.Y = _blackboard.MovementConfig.JumpForce;
        
        if (!_blackboard.Input.IsHoldingJump)
        {
            vel.Y *= 0.5f;
        }
        
        _blackboard.Kinematics.Velocity = vel;
    }

    private bool CanJumpBuffer(float currentTime)
    {
        return currentTime - _blackboard.Input.LastJumpInputTime < _blackboard.MovementConfig.JumpBufferThreshold;
    }
}