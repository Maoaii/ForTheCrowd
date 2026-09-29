using Godot;
using System;

[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public PackedScene WeaponScene { get; set; }

	private IWeapon _weapon;


	public override void _Ready()
	{
		if (WeaponScene == null)
			return;
		
		InstantiateWeapon();
	}

	private void InstantiateWeapon()
	{
		_weapon = WeaponScene.Instantiate<IWeapon>();
		AddChild(_weapon as Node3D, true);
        (_weapon as Node3D).Owner = this;
		_weapon.SetInitialBasis(Basis);
	}

	public void TryActivate()
	{
		_weapon.TryActivate(Vector3.Forward);
	}
}
