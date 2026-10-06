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
		_weapon.SetInitialPosition(GlobalPosition);
	}

	public void TryActivate(Vector3 direction)
	{
		if (_weapon == null)
			return;

		_weapon.TryActivate(direction);
	}
}
