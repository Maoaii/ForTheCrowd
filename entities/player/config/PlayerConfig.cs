using System;
using Godot;

namespace CarGame.Entities.Player;

public class MovementConfig
{
    public float Acceleration { get; init; }
    public float CoastingFriction { get; init; }
    public float AirFriction { get; init; }
    public float MaxSpeedForward { get; init; }
    public float MaxSpeedBackwards { get; init; }
    public float SpeedLowerThreshold { get; init; }
    public float JumpForce { get; init; }
    public float JumpBufferThreshold { get; init; }
    public float GravityForce { get; init; }
    public float FallMultiplier { get; init; }
    public float MaxSteerAngleDegrees { get; init; }

    // Drift
    public float MinDriftSpeed { get; init; }
    public float DriftBuildRate { get; init; }
    public float DriftDecayRate { get; init; }
    public float HighGripRate { get; init; }
    public float LowGripRate { get; init; }

    public float SteerRate { get; init; } // seconds to reach maximum steer angle
    public float Wheelbase { get; init; } // distance between front and rear wheels in world units
    public float GroundHeight { get; init; } // was borrowing DefaultScale.Y before
    public float ShoveItTrickDuration { get; init; } // duration of the Shove It trick in milliseconds
    public float KickflipTrickDuration { get; init; } // duration of the Kickflip trick in milliseconds
    public float BackManualTrickDuration { get; init; } // duration of the Back Manual trick in milliseconds

    // Trails/Tire marks
    public float TrailSampleDistance { get; init; }
    public float TrailWidth { get; init; }
    public float TrailMaxLifetime { get; init; }
    public float TrailDriftThreshold { get; init; }
    public float TrailCooldown { get; init; }
}

public class PlayerAnimationConfig
{
    public Vector3 DefaultScale { get; init; }
    public Vector3 StretchScale { get; init; }
    public Vector3 SquashScale { get; init; }

    public Tween.TransitionType LaunchStretchTweenTrans { get; init; }
    public Tween.EaseType LaunchStretchTweenEase { get; init; }
    public float LaunchStretchTweenDuration { get; init; }

    public Tween.TransitionType AirTweenTrans { get; init; }
    public Tween.EaseType AirTweenEase { get; init; }
    public float AirTweenDuration { get; init; }

    public Tween.TransitionType LandingSquashTweenTrans { get; init; }
    public Tween.EaseType LandingSquashTweenEase { get; init; }
    public float LandingSquashTweenDuration { get; init; }

    public Tween.TransitionType RecoveryTweenTrans { get; init; }
    public Tween.EaseType RecoveryTweenEase { get; init; }
    public float RecoveryTweenDuration { get; init; }
}

public static class MovementConfigs
{
    public static readonly MovementConfig Default = new()
    {
        Acceleration = 105.0f,
        CoastingFriction = 0.65f,
        AirFriction = 0.01f,
        MaxSpeedForward = 45.0f,
        MaxSpeedBackwards = 35.0f,
        SpeedLowerThreshold = 0.1f,
        JumpForce = 45.0f,
        JumpBufferThreshold = 0.2f,
        GravityForce = 9.81f * 5.0f,
        FallMultiplier = 3.0f,
        MaxSteerAngleDegrees = 15.0f,
        SteerRate = 0.25f,
        Wheelbase = 2.5f,
        GroundHeight = 1.0f,
        // Drift
        MinDriftSpeed = 30.0f,
        DriftBuildRate = 1.0f,
        DriftDecayRate = 0.1f,
        HighGripRate = 20.0f,
        LowGripRate = 6.0f,
        // Tricks
        ShoveItTrickDuration = 200.0f,
        KickflipTrickDuration = 200.0f,
        BackManualTrickDuration = 1000.0f,
        // Trails/Tire marks
        TrailSampleDistance = 0.15f,
        TrailWidth = 0.4f,
        TrailMaxLifetime = 4.0f,
        TrailDriftThreshold = 0.15f,
        TrailCooldown = 0.05f
    };
}

public static class PlayerAnimationConfigs
{
    public static readonly PlayerAnimationConfig Default = new()
    {
        DefaultScale = new Vector3(2.0f, 2.0f, 2.0f),
        StretchScale = new Vector3(1.8f, 2.2f, 1.8f),
        SquashScale = new Vector3(2.2f, 1.8f, 2.2f),

        LaunchStretchTweenTrans = Tween.TransitionType.Back,
        LaunchStretchTweenEase = Tween.EaseType.Out,
        LaunchStretchTweenDuration = 0.12f,

        AirTweenTrans = Tween.TransitionType.Sine,
        AirTweenEase = Tween.EaseType.Out,
        AirTweenDuration = 0.08f,

        LandingSquashTweenTrans = Tween.TransitionType.Quart,
        LandingSquashTweenEase = Tween.EaseType.Out,
        LandingSquashTweenDuration = 0.10f,

        RecoveryTweenTrans = Tween.TransitionType.Back,
        RecoveryTweenEase = Tween.EaseType.Out,
        RecoveryTweenDuration = 0.08f
    };
}