using System;
using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public abstract partial class TrickState : PlayerState
{
    [Export] public float Score;

    [ExportGroup("FOV Kick")]
    [Export] public float FovKick = 1.05f;
    [Export] public float FovKickTimeUpS = 0.15f;
    [Export] public float FovKickTimeDownS = 0.35f;

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.Camera.SetFOVKick(FovKick, FovKickTimeUpS, FovKickTimeDownS);
        TrickPopupManager.Instance?.PopupTrick(StateName.ToString(), Score);
    }
}