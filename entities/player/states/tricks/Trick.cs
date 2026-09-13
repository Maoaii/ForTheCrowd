using System;
using CarGame.Systems;
using CarGame.Systems.UI;
using Godot;

namespace CarGame.Entities.Player;

public abstract class TrickState : State
{
    private float _score;

    public TrickState(Enum stateName, float score) : base(stateName)
    {
        _score = score;
    }

    public override void Enter(double delta = 0)
    {
        base.Enter(delta);
        _blackboard.Camera.SetFOVKick(Constants.CAMERA_FOV_KICK, Constants.CAMERA_FOV_KICK_TIME_UP_S, Constants.CAMERA_FOV_KICK_TIME_DOWN_S);
        TrickPopupManager.Instance?.PopupTrick(StateName.ToString(), _score);
    }
}