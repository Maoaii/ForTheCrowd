using Godot;

namespace CarGame.Entities.Components;

public interface IVelocityHolder
{
	Vector3 GetVelocity();
}