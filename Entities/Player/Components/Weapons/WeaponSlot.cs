using Godot;
using System;

[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public PackedScene WeaponScene { get; set; }

	private GattlingGun _weapon;


	public override void _Ready()
	{
		if (WeaponScene == null)
			return;
		
		InstantiateWeapon();
	}

	private void InstantiateWeapon()
	{
		_weapon = WeaponScene.Instantiate<GattlingGun>();
		AddChild(_weapon);
        _weapon.Owner = this;
		_weapon.SetInitialBasis(Basis);
	}

	public void TryActivate(Vector3 direction)
	{
		_weapon.TryActivate(direction);
	}
}
