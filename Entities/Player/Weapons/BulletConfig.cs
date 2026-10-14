using System;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[GlobalClass]
public partial class BulletConfig : Resource
{
	[Export] public PackedScene BulletPackedScene;
	[Export] public BallisticsBase FiringSolver;
	[Export] public float BulletLifetime = 5.0f;
	[Export] public float TimeToFreeAfterCollision = 0.0f;
	[Export] public AfterCollisionConfig AfterCollisionConfig;
}
