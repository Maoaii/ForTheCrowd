using Godot;
using System;

[GlobalClass]
public partial class WeaponConfig : Resource
{
    [Export] public float InitialSpeed = 100.0f;
    [Export] public float Cooldown = 1.0f;
    [Export] public float BulletLifetime = 5.0f;
}
