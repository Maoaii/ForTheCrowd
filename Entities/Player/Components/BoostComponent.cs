using CarGame.Systems.Blackboards;
using Godot;
using System;

[GlobalClass]
public partial class BoostComponent : Node
{
    [ExportGroup("References")]
    [Export] public BlackboardComponent Blackboard;

    private float _progress;

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float target = Blackboard.Input.WantsBoost ? 1.0f : 0.0f;
        var config = Blackboard.MovementConfig;
        float rate = 1.0f / (target > _progress ? config.BoostAttackTime : config.BoostReleaseTime);
        _progress = Mathf.MoveToward(_progress, target, rate * (float)delta);

        Curve rampCurve = config.RampCurve;
        float shaped = rampCurve != null ? rampCurve.SampleBaked(_progress) : Mathf.SmoothStep(0.0f, 1.0f, _progress);
        Blackboard.Kinematics.BoostIntensity = Mathf.Clamp(shaped, 0.0f, 1.0f);
    }
}
