using System;
using Godot;

namespace CarGame.Entities.Player;

[GlobalClass]
public partial class PlayerAnimationConfig : Resource
{
    [Export] public Vector3 DefaultScale { get; set; } = new Vector3(2.0f, 2.0f, 2.0f);
    [Export] public Vector3 StretchScale { get; set; } = new Vector3(1.8f, 2.2f, 1.8f);
    [Export] public Vector3 SquashScale { get; set; } = new Vector3(2.2f, 1.8f, 2.2f);

    [ExportGroup("Launch Stretch Tweens")]
    [Export] public Tween.TransitionType LaunchStretchTweenTrans { get; set; } = Tween.TransitionType.Back;
    [Export] public Tween.EaseType LaunchStretchTweenEase { get; set; } = Tween.EaseType.Out;
    [Export] public float LaunchStretchTweenDuration { get; set; } = 0.12f;

    [ExportGroup("Air Tweens")]
    [Export] public Tween.TransitionType AirTweenTrans { get; set; } = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType AirTweenEase { get; set; } = Tween.EaseType.Out;
    [Export] public float AirTweenDuration { get; set; } = 0.08f;

    [ExportGroup("Landing Squash Tweens")]
    [Export] public Tween.TransitionType LandingSquashTweenTrans { get; set; } = Tween.TransitionType.Quart;
    [Export] public Tween.EaseType LandingSquashTweenEase { get; set; } = Tween.EaseType.Out;
    [Export] public float LandingSquashTweenDuration { get; set; } = 0.10f;

    [ExportGroup("Recovery Tweens")]
    [Export] public Tween.TransitionType RecoveryTweenTrans { get; set; } = Tween.TransitionType.Back;
    [Export] public Tween.EaseType RecoveryTweenEase { get; set; } = Tween.EaseType.Out;
    [Export] public float RecoveryTweenDuration { get; set; } = 0.08f;
}