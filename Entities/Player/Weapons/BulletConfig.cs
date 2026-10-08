using System;
using Godot;

[GlobalClass]
public partial class BulletConfig : Resource
{
	[Export] public PackedScene BulletPackedScene;
	[Export] public float InitialSpeed = 100.0f;
	[Export] public float BulletLifetime = 5.0f;
}
