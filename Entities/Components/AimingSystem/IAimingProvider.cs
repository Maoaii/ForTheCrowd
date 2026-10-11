using System;
using Godot;

namespace CarGame.Entities.Components.AimingSystem;

public interface IAimingProvider
{
	public abstract (Vector3 direction, Vector3 targetPosition) GetAimingDirection(Node3D fromNode, Vector3 ownerVelocity);
}
