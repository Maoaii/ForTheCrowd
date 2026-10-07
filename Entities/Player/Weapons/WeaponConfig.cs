using Godot;
using System;

[GlobalClass]
public partial class WeaponConfig : Resource
{
    [Export] public float InitialSpeed = 100.0f;
    [Export] public float Cooldown = 1.0f;
    [Export] public float BulletLifetime = 5.0f;

    [ExportGroup("Visuals")]
    [ExportSubgroup("Shoot Scale Animation")]
    [Export] public float ScaleUpTime = 0.1f;
    [Export] public float ScaleDownTime = 0.1f;
    [Export] public Vector3 ScaleUpAmount = new Vector3(1.2f, 1.2f, 1.2f);
    [Export] public Tween.TransitionType ScaleUpTweenTransition = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType ScaleUpTweenEase = Tween.EaseType.Out;
    [Export] public Tween.TransitionType ScaleDownTweenTransition = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType ScaleDownTweenEase = Tween.EaseType.In;

    [ExportSubgroup("Shoot Recoil Animation")]
    [Export] public float RecoilUpTime = 0.1f;
    [Export] public float RecoilDownTime = 0.1f;
    [Export] public float RecoilDistance = 0.1f;
    [Export] public Tween.TransitionType RecoilUpTweenTransition = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType RecoilUpTweenEase = Tween.EaseType.Out;
    [Export] public Tween.TransitionType RecoilDownTweenTransition = Tween.TransitionType.Sine;
    [Export] public Tween.EaseType RecoilDownTweenEase = Tween.EaseType.In;
    [Export] public float RecoilRotationAmount = 5.0f;
}
