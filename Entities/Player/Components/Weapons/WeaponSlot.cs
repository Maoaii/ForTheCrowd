using Godot;
using System;

[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public PackedScene WeaponScene;


	private IWeapon _weapon;


	public override void _Ready()
	{
		if (Engine.IsEditorHint())
			return;
		if (WeaponScene == null)
			return;
		
		_weapon = WeaponScene.Instantiate<IWeapon>();
		AddChild(_weapon as Node3D, true);
        (_weapon as Node3D).Owner = this;
		_weapon.SetInitialBasis(this.Basis);
	}

	public void TryActivate()
	{
		_weapon.TryActivate(Vector3.Forward);
	}
}
