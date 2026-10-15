using System;
using Godot;

namespace CarGame.Entities.Player;

[Tool]
[GlobalClass]
public partial class MovementConfig : Resource
{
    /// <summary>Forward/backward acceleration while throttling, in units/s².</summary>
    [Export] public float Acceleration { get; set; } = 105.0f;

    /// <summary>Fraction of speed lost per second when there is no throttle. Higher = stops sooner.</summary>
    [Export] public float CoastingFriction { get; set; } = 0.65f;

    /// <summary>Fraction of speed lost per second while throttling (light drag).</summary>
    [Export] public float AirFriction { get; set; } = 0.01f;

    /// <summary>Top forward speed without boost, in units/s.</summary>
    [Export] public float MaxSpeedForward { get; set; } = 45.0f;

    /// <summary>Top reverse speed, in units/s.</summary>
    [Export] public float MaxSpeedBackwards { get; set; } = 35.0f;

    /// <summary>Speeds below this (units/s) snap to 0 so the car doesn't creep.</summary>
    [Export] public float SpeedLowerThreshold { get; set; } = 0.1f;

    /// <summary>Upward velocity applied when jumping, in units/s.</summary>
    [Export] public float JumpForce { get; set; } = 45.0f;

    /// <summary>Seconds. A jump pressed this shortly before landing still fires on landing.</summary>
    [Export] public float JumpBufferThreshold { get; set; } = 0.2f;

    /// <summary>Downward acceleration while airborne, in units/s².</summary>
    [Export] public float GravityForce { get; set; } = 9.81f * 5.0f;

    /// <summary>Multiplies gravity while airborne. Higher = snappier, heavier falls.</summary>
    [Export] public float FallMultiplier { get; set; } = 3.0f;

    /// <summary>Maximum front-wheel steer angle at low speed, in degrees.</summary>
    [Export] public float MaxSteerAngleDegrees { get; set; } = 15.0f;

    /// <summary>
    /// Steer limit multiplier at MaxSpeedForward (1 = no reduction). Compounds above it,
    /// e.g. 0.7 gives 0.49 at twice the speed. Lower = heavier steering at speed.
    /// </summary>
    [Export] public float HighSpeedSteerScale { get; set; } = 0.7f;

    /// <summary>Maximum rotation speed of the car in rad/s. Caps the steer limit at high speed so the car can't spin faster than this.</summary>
    [Export] public float MaxYawRate { get; set; } = 3.4f;

    /// <summary>Multiplies MaxSpeedForward at full boost.</summary>
    [ExportGroup("Boost Handling")]
    [Export] public float BoostSpeedMult { get; set; } = 2.0f;

    /// <summary>Multiplies Acceleration at full boost. Higher = harder kick when boost starts.</summary>
    [Export] public float BoostAccelerationMult { get; set; } = 3.0f;

    /// <summary>Multiplies SteerRate (seconds to full lock) at full boost. Higher = slower steering wheel.</summary>
    [Export] public float BoostSteerTimeMult { get; set; } = 1.4f;

    /// <summary>Multiplies grip at full boost. Lower = longer, wider slides.</summary>
    [Export] public float BoostGripMult { get; set; } = 0.85f;

    /// <summary>Seconds to reach maximum steer angle.</summary>
    [ExportGroup("Vehicle Dimensions")]
    [Export] public float SteerRate { get; set; } = 0.25f;

    /// <summary>Distance between front and rear wheels in world units. Larger = wider turns.</summary>
    [Export] public float Wheelbase { get; set; } = 2.5f;

    /// <summary>Currently unused.</summary>
    [Export] public float GroundHeight { get; set; } = 1.0f;
}
