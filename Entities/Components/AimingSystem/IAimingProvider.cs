using Godot;
using System;

namespace CarGame.Entities.Components.AimingSystem;

public interface IAimingProvider
{
	public abstract Vector3 GetAimingDirection();
}
