using Godot;
using System;

[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public PackedScene WeaponScene { get; set; }

	private WeaponBase _weapon;


	public override void _Ready()
	{
		if (WeaponScene == null)
			return;
		
		InstantiateWeapon();
	}

	private void InstantiateWeapon()
	{
		_weapon = WeaponScene.Instantiate<WeaponBase>();
		AddChild(_weapon);
        _weapon.Owner = this;
		_weapon.SetInitialPosition(GlobalPosition);
	}

	public bool TryActivate(Vector3 direction)
	{
		if (_weapon == null)
			return false;
		
		bool activated = _weapon.TryActivate(direction);
		
		if (activated)
			LookAt(GlobalPosition + direction);
		
		return activated;
	}
}
