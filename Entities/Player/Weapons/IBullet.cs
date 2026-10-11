using System;
using Godot;

namespace CarGame.Entities.Player.Weapons;

public interface IBullet
{
	public void SetDirection(Vector3 direction, float speed);

	public void SetLifetime(float lifetime);
}
