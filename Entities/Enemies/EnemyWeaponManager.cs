using System;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class EnemyWeaponManager : WeaponManager
{
	public override void _Ready()
	{
		base._Ready();
		for (int i = 0; i < WeaponConfigs.Count; i++)
			_slots[i].Equip(WeaponConfigs[i]);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint())
			return;

		base._PhysicsProcess(delta);

		foreach (WeaponSlot weaponSlot in _slots)
		{
			if (!AimingProvider.TryGetTarget(weaponSlot, out TargetInfo target))
				continue;

			weaponSlot.TryActivate(target, Vector3.Zero);
		}
	}

	public void SetWeapon(int slot, WeaponConfig config)
	{
		if (Engine.IsEditorHint())
			return;
		if (slot < 0 || slot >= _slots.Count)
			throw new ArgumentOutOfRangeException(nameof(slot));

		_slots[slot].Equip(config);
	}
}
