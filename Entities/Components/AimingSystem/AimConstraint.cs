using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

[Tool]
[GlobalClass]
public abstract partial class AimConstraint : Resource
{
	public abstract bool ApplyConstraint(FiringSolution solution, Node3D shooter, float restYaw);
}
