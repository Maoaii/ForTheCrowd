using System;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class BulletConfig : Resource
{
	[Export] public PackedScene BulletPackedScene;
	[Export] public BallisticsBase FiringSolver;
	[Export] public int Damage = 1;
	[Export] public float BulletLifetime = 5.0f;
	[Export] public float TimeToFreeAfterCollision = 0.0f;
	[Export] public int MaxHits = 1;
	[Export] public PackedScene ImpactScene;
}
