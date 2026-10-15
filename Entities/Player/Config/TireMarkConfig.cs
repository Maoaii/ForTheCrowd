using System;
using Godot;

namespace CarGame.Entities.Player;

[Tool]
[GlobalClass]
public partial class TireMarkConfig : Resource
{
    /// <summary>Distance the car must move before another tire-mark point is added.</summary>
    [Export] public float TrailSampleDistance { get; set; } = 0.15f;

    /// <summary>Width of each tire mark, in world units.</summary>
    [Export] public float TrailWidth { get; set; } = 0.4f;

    /// <summary>Seconds before tire marks fade out.</summary>
    [Export] public float TrailMaxLifetime { get; set; } = 4.0f;

    /// <summary>Drift factor (0-1) above which tire marks are drawn while steering.</summary>
    [Export] public float TrailDriftThreshold { get; set; } = 0.15f;

    /// <summary>Seconds the current tire mark keeps going after drifting stops before it ends, so brief gaps don't split it.</summary>
    [Export] public float TrailCooldown { get; set; } = 0.05f;
}
