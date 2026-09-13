using System;
using Godot;

namespace CarGame.Entities.Player;

[GlobalClass]
public partial class MovementConfig : Resource
{
    [Export] public float Acceleration { get; set; } = 105.0f;
    [Export] public float CoastingFriction { get; set; } = 0.65f;
    [Export] public float AirFriction { get; set; } = 0.01f;
    [Export] public float MaxSpeedForward { get; set; } = 45.0f;
    [Export] public float MaxSpeedBackwards { get; set; } = 35.0f;
    [Export] public float SpeedLowerThreshold { get; set; } = 0.1f;
    [Export] public float JumpForce { get; set; } = 45.0f;
    [Export] public float JumpBufferThreshold { get; set; } = 0.2f;
    [Export] public float GravityForce { get; set; } = 9.81f * 5.0f;
    [Export] public float FallMultiplier { get; set; } = 3.0f;
    [Export] public float MaxSteerAngleDegrees { get; set; } = 15.0f;

    [ExportGroup("Drift")]
    [Export] public float MinDriftSpeed { get; set; } = 30.0f;
    [Export] public float DriftBuildRate { get; set; } = 1.0f;
    [Export] public float DriftDecayRate { get; set; } = 0.1f;
    [Export] public float HighGripRate { get; set; } = 20.0f;
    [Export] public float LowGripRate { get; set; } = 6.0f;

    [ExportGroup("Vehicle Dimensions")]
    [Export] public float SteerRate { get; set; } = 0.25f; // seconds to reach maximum steer angle
    [Export] public float Wheelbase { get; set; } = 2.5f; // distance between front and rear wheels in world units
    [Export] public float GroundHeight { get; set; } = 1.0f; 
    
    [ExportGroup("Trick Timings")]
    [Export] public float ShoveItTrickDuration { get; set; } = 200.0f; 
    [Export] public float KickflipTrickDuration { get; set; } = 200.0f; 
    [Export] public float BackManualTrickDuration { get; set; } = 1000.0f; 

    [ExportGroup("Trails/Tire marks")]
    [Export] public float TrailSampleDistance { get; set; } = 0.15f;
    [Export] public float TrailWidth { get; set; } = 0.4f;
    [Export] public float TrailMaxLifetime { get; set; } = 4.0f;
    [Export] public float TrailDriftThreshold { get; set; } = 0.15f;
    [Export] public float TrailCooldown { get; set; } = 0.05f;
}