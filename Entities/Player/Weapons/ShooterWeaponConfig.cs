using System;
using CarGame.Entities.Components.AimingSystem;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class ShooterWeaponConfig : WeaponConfig
{
	[Export] public float Cooldown = 1.0f;
	[Export] public BulletConfig BulletConfig;

	[ExportGroup("Aiming")]
	[Export] public bool InheritOwnerVelocity = true;
	[Export] public Array<AimModifier> AimModifiers = new();
	[Export] public Array<AimConstraint> AimConstraints = new();
}
