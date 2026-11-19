using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Components.AimingSystem;

public readonly record struct AimContext(
	Vector3 MuzzlePosition,
	Vector3 InheritedVelocity,
	TargetInfo Target,
	BallisticsBase Ballistics,
	RandomNumberGenerator Random);
