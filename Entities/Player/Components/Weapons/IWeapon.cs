using Godot;
using System;

public interface IWeapon
{
	public void SetInitialBasis(Basis basis);

	public void TryActivate(Vector3 direction);
}
