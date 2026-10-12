using CarGame.Entities.Components.AimingSystem;
using CarGame.Entities.Player.Weapons.Ballistics;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public bool Enabled = true;
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

	public bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		if (!Enabled)
			return false;
		if (_weapon == null)
			return false;

		bool activated = _weapon.TryActivate(targetInfo, ownerVelocity, out FiringSolution solution);

		if (activated)
			LookAt(GlobalPosition + solution.MuzzleDirection, Vector3.Up, useModelFront: true);   // weapon models face +Z

		return activated;
	}
}
