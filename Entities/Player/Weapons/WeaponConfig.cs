using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class WeaponConfig : Resource
{
	[Export] public PackedScene WeaponScene;
}
