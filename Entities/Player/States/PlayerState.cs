using System;
using CarGame.Systems;
using Godot;

namespace CarGame.Entities.Player;

public abstract partial class PlayerState : State
{
    [Export] public PlayerStates StateId;

    public override Enum StateName => StateId;
}