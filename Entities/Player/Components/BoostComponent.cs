using CarGame.Entities.Player;
using CarGame.Systems.Blackboards;
using Godot;
using System;

[GlobalClass]
public partial class BoostComponent : Node
{
    [ExportGroup("References")]
    [Export] public BlackboardComponent Blackboard;
    [Export] public BoostConfig Config;

    private float _progress;
    private float _fuel;
    private float _regenTimer;
    private bool _depleted;

    public override void _Ready()
    {
        base._Ready();
        _fuel = Config.BoostFuel;
    }

    public override void _PhysicsProcess(double delta)
    {
        base._PhysicsProcess(delta);
        float dt = (float)delta;
        UpdateFuel(dt);
        UpdateIntensity(dt);
    }

    private void UpdateFuel(float dt)
    {
        float maxFuel = Mathf.Max(Config.BoostFuel, 0.001f);

        if (_depleted && _fuel >= maxFuel * Config.BoostRestartThreshold)
            _depleted = false;

        bool boosting = Blackboard.Input.WantsBoost && !_depleted && _fuel > 0.0f;
        Blackboard.State.IsBoosting = boosting;
    
        if (boosting)
        {
            Blackboard.Events.OnStartedBoosting?.Invoke();
            _fuel = Mathf.Max(_fuel - dt, 0.0f);
            _regenTimer = Config.BoostRegenDelay;
            if (_fuel <= 0.0f)
                _depleted = true;
        }
        else if (_regenTimer > 0.0f)
        {
            Blackboard.Events.OnStoppedBoosting?.Invoke();
            _regenTimer -= dt;
        }
        else
        {
            Blackboard.Events.OnStoppedBoosting?.Invoke();
            _fuel = Mathf.Min(_fuel + Config.BoostFuelRegenRate * dt, maxFuel);
        }

        Blackboard.Kinematics.BoostFuelFraction = _fuel / maxFuel;
        Blackboard.State.IsBoostDepleted = _depleted;
    }

    private void UpdateIntensity(float dt)
    {
        float target = Blackboard.State.IsBoosting ? 1.0f : 0.0f;
        float rate = 1.0f / (target > _progress ? Config.BoostAttackTime : Config.BoostReleaseTime);
        _progress = Mathf.MoveToward(_progress, target, rate * dt);

        Curve rampCurve = Config.RampCurve;
        float shaped = rampCurve != null ? rampCurve.SampleBaked(_progress) : Mathf.SmoothStep(0.0f, 1.0f, _progress);
        Blackboard.Kinematics.BoostIntensity = Mathf.Clamp(shaped, 0.0f, 1.0f);
    }
}
