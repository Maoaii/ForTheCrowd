using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public partial class OllieState : TrickState
{
    public override void _Ready()
    {
    }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _stateMachine.TransitionState(PlayerStates.Air, delta);
    }
}