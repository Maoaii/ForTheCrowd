using System;
using Godot;

namespace CarGame.Entities.Player;

[Tool]
[GlobalClass]
public partial class BoostConfig : Resource
{
    /// <summary>Boost tank size, in seconds of continuous boosting.</summary>
    [Export] public float BoostFuel { get; set; } = 5.0f;

    /// <summary>Fuel (seconds of boost) regained per second while not boosting. 1 = refills in as long as it takes to empty.</summary>
    [Export] public float BoostFuelRegenRate { get; set; } = 0.5f;

    /// <summary>Seconds after boosting stops before the tank starts refilling.</summary>
    [Export] public float BoostRegenDelay { get; set; } = 1.0f;

    /// <summary>Fraction of the tank (0-1) that must refill after running dry before boost can be used again. Prevents flickering on an empty tank.</summary>
    [Export] public float BoostRestartThreshold { get; set; } = 0.4f;

    /// <summary>Seconds for boost to ramp from off to full when boost is pressed. Lower = punchier start.</summary>
    [Export] public float BoostAttackTime { get; set; } = 0.4f;

    /// <summary>Seconds for boost to fade from full to off after it's released. Higher = longer, gentler slowdown.</summary>
    [Export] public float BoostReleaseTime { get; set; } = 0.8f;

    /// <summary>
    /// Shapes the boost ramp. X is progress (0-1) over the attack/release time, Y is boost intensity (0-1).
    /// Uses smoothstep when unset.
    /// </summary>
    [Export] public Curve RampCurve { get; set; }
}
