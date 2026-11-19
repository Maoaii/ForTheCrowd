using Godot;

namespace CarGame.Entities.Player.Weapons.Ballistics;

public readonly record struct FiringSolution(Vector3 AimPoint, Vector3 LaunchVelocity, Vector3 MuzzleDirection, float FlightTime);
