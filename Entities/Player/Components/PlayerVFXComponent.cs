using Godot;
using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Components;

public partial class PlayerVFXComponent : Node
{
    [Export] public BlackboardComponent Blackboard;
    [Export] public GpuParticles3D LandDustFL;
    [Export] public GpuParticles3D LandDustFR;
    [Export] public GpuParticles3D LandDustRL;
    [Export] public GpuParticles3D LandDustRR;
    [Export] public GpuParticles3D DriftDustRL;
    [Export] public GpuParticles3D DriftDustRR;
    
    [ExportGroup("Particle Counts")]
    [Export] public int MinDriftParticles = 1;
    [Export] public int MaxDriftParticles = 32;
    [Export] public int MinLandingParticles = 4;
    [Export] public int MaxLandingParticles = 16;

    private double _lastDriftTime = -1.0;

    public override void _Ready()
    {
        if (Blackboard != null)
        {
            Blackboard.Events.OnLanded += HandleLanding;
            Blackboard.Events.OnDrifting += HandleDrifting;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        // If we haven't received a drift event in the last 100ms (roughly 6 physics frames), turn them off
        if (Time.GetTicksMsec() - _lastDriftTime > 100)
        {
            if (DriftDustRL != null) DriftDustRL.Emitting = false;
            if (DriftDustRR != null) DriftDustRR.Emitting = false;
        }
    }

    private void HandleLanding(float impactVelocity)
    {
        LandDustFL?.Restart();
        LandDustFR?.Restart();
        LandDustRL?.Restart();
        LandDustRR?.Restart();
    }
    
    private void HandleDrifting(float driftFactor)
    {
        _lastDriftTime = Time.GetTicksMsec();
        
        if (DriftDustRL != null) DriftDustRL.Emitting = true;
        if (DriftDustRR != null) DriftDustRR.Emitting = true;
    }
}
