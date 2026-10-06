using Godot;
using System;

public interface IWeapon
{
	public void SetInitialPosition(Vector3 position);

	public void TryActivate(Vector3 direction);
}
