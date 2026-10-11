using Godot;

namespace CarGame.Entities.Components.AimingSystem;

public readonly record struct TargetInfo(Vector3 Position, Vector3 Velocity, Vector3 GroundPosition);
