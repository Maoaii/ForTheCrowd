using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[GlobalClass]
public partial class WeaponConfig : Resource
{
	[Export] public float Cooldown = 1.0f;

	[ExportGroup("Bullet")]
	[Export] public BulletConfig BulletConfig;
}
