using Godot;
using System;

public interface IWeapon
{
	public void SetInitialPosition(Vector3 position);

	public bool TryActivate(Vector3 direction);
}
