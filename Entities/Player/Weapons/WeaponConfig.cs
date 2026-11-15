using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class WeaponConfig : Resource
{
	[Export] public PackedScene WeaponScene;
	[Export(PropertyHint.Range, "0.0,180.0,1.0,radians_as_degrees")] public float RotationConeDegrees;
}
