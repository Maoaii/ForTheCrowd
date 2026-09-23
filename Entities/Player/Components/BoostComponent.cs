using CarGame.Entities.Player;
using CarGame.Systems.Blackboards;
using Godot;
using System;

[GlobalClass]
public partial class BoostComponent : Node
{
    [ExportGroup("References")]
    [Export] public BlackboardComponent Blackboard;

    private float _progress;
    private float _fuel;
    private float _regenTimer;
    private bool _depleted;

    public override void _Ready()
    {
        base._Ready();
        _fuel = Blackboard.MovementConfig.BoostFuel;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float dt = (float)delta;
        var config = Blackboard.MovementConfig;

        UpdateFuel(config, dt);
        UpdateIntensity(config, dt);
    }

    private void UpdateFuel(MovementConfig config, float dt)
    {
        float maxFuel = Mathf.Max(config.BoostFuel, 0.001f);

        if (_depleted && _fuel >= maxFuel * config.BoostRestartThreshold)
            _depleted = false;

        bool boosting = Blackboard.Input.WantsBoost && !_depleted && _fuel > 0.0f;
        Blackboard.State.IsBoosting = boosting;

        if (boosting)
        {
            _fuel = Mathf.Max(_fuel - dt, 0.0f);
            _regenTimer = config.BoostRegenDelay;
            if (_fuel <= 0.0f)
                _depleted = true;
        }
        else if (_regenTimer > 0.0f)
            _regenTimer -= dt;
        else
            _fuel = Mathf.Min(_fuel + config.BoostFuelRegenRate * dt, maxFuel);

        Blackboard.Kinematics.BoostFuelFraction = _fuel / maxFuel;
    }

    private void UpdateIntensity(MovementConfig config, float dt)
    {
        float target = Blackboard.State.IsBoosting ? 1.0f : 0.0f;
        float rate = 1.0f / (target > _progress ? config.BoostAttackTime : config.BoostReleaseTime);
        _progress = Mathf.MoveToward(_progress, target, rate * dt);

        Curve rampCurve = config.RampCurve;
        float shaped = rampCurve != null ? rampCurve.SampleBaked(_progress) : Mathf.SmoothStep(0.0f, 1.0f, _progress);
        Blackboard.Kinematics.BoostIntensity = Mathf.Clamp(shaped, 0.0f, 1.0f);
    }
}
