using System;
using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public partial class MovingState : PlayerState
{
    public bool JustJumped => !_blackboard.State.IsOnGround && _blackboard.State.WasOnGround;
    public bool JustHitGround => _blackboard.State.IsOnGround && !_blackboard.State.WasOnGround;

    [ExportGroup("Landing Camera Kick")]
    [Export] public float LandingFovKick = 1.0f;
    [Export] public float LandingFovKickTimeUpS = 0.05f;
    [Export] public float LandingFovKickTimeDownS = 0.3f;

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.State.WantsGroundPhysics = true;

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

        TryChangingStates(delta);

        if (TrickPopupManager.Instance.StaleTricks())
            TrickPopupManager.Instance?.ResetTricks();
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
            _stateMachine.TransitionState(PlayerStates.Idle, delta);
        if (_blackboard.Input.WantsJump)
            _stateMachine.TransitionState(PlayerStates.Ollie, delta);
        if (_blackboard.Tricks.WantsBackManual)
            _stateMachine.TransitionState(PlayerStates.BackManual, delta);
    }
}
