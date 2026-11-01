using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class MowerWeaponConfig : WeaponConfig
{
	[Export] public float MinSpeed;
	[Export] public int Damage;
}
