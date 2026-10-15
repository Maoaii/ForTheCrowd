using System;
using Godot;

namespace CarGame.Entities.Player;

[Tool]
[GlobalClass]
public partial class DriftConfig : Resource
{
    /// <summary>Drift starts building above this speed (units/s) and is full at MaxSpeedForward.</summary>
    [Export] public float MinDriftSpeed { get; set; } = 30.0f;

    /// <summary>Per second. How fast the drift factor rises toward its target. Higher = drift kicks in sooner.</summary>
    [Export] public float DriftBuildRate { get; set; } = 40.0f;

    /// <summary>Per second. How fast the drift factor falls when not drifting (6.3 is about 0.16s).</summary>
    [Export] public float DriftDecayRate { get; set; } = 6.3f;

    /// <summary>Per second. How fast travel direction catches up to facing with no drift. Higher = tighter grip.</summary>
    [Export] public float HighGripRate { get; set; } = 20.0f;

    /// <summary>Per second. Same catch-up rate at full drift. Lower = longer slides.</summary>
    [Export] public float LowGripRate { get; set; } = 6.0f;

    /// <summary>Hard cap on the angle between where the car faces and where it travels, in degrees.</summary>
    [Export] public float MaxSlipAngleDegrees { get; set; } = 50.0f;
}
