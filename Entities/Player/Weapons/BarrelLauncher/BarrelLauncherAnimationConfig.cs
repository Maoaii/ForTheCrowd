using System;
using CarGame.Entities.Player.Weapons;
using Godot;

[GlobalClass]
public partial class BarrelLauncherAnimationConfig : ShootAnimationConfig
{
	[ExportGroup("Tilt Animation")]
	[Export] public float TiltAngle = -70.0f;
	[Export] public float TiltTime = 0.15f;
	[Export] public float ReturnTime = 0.3f;
	[Export] public Tween.TransitionType TweenTransition = Tween.TransitionType.Cubic;
	[Export] public Tween.EaseType TweenEase = Tween.EaseType.In;
}
