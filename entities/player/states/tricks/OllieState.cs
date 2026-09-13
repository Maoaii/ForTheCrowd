using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public class OllieState : TrickState
{
    public OllieState() : base(PlayerStates.OLLIE_STATE, Constants.OLLIE_SCORE) { }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _stateMachine.TransitionState(PlayerStates.AIR_STATE, delta);
    }
}