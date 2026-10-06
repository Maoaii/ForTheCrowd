using CarGame.Entities.Components.AimingSystem;
using Godot;
using System;

public abstract partial class AimingProviderBase : Node3D, IAimingProvider
{
    public abstract Vector3 GetAimingDirection();
}
