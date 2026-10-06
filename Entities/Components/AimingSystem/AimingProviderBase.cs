using CarGame.Entities.Components.AimingSystem;
using Godot;
using System;

public abstract partial class AimingProviderBase : Resource, IAimingProvider
{
    public abstract Vector3 GetAimingDirection(Node3D fromNode);
}
