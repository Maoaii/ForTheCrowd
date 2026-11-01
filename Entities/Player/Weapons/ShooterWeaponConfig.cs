using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class ShooterWeaponConfig : WeaponConfig
{
	[Export] public float Cooldown = 1.0f;
	[Export] public BulletConfig BulletConfig;
}
