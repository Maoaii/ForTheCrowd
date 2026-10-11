using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

public readonly record struct ShooterState(Vector3 MuzzlePosition, Vector3 Velocity);
