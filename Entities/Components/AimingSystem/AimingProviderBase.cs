using System;
using CarGame.Entities.Components.AimingSystem;
using Godot;

public abstract partial class AimingProviderBase : Resource, IAimingProvider
{
    public abstract (Vector3 direction, Vector3 targetPosition) GetAimingDirection(Node3D fromNode, Vector3 ownerVelocity);
}
