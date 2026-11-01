using System;
using System.Collections.Generic;
using CarGame.Entities.Components.AimingSystem;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class WeaponManager : Node3D
{
	private const int SlotCount = 4;

	[ExportToolButton("Generate weapon slots")]
	public Callable GenerateSlotsButton => Callable.From(GenerateSlots);

	[ExportCategory("References")]
	[Export]
	private BlackboardComponent _blackboard;

	[Export]
	private AimingProviderBase _aimingProvider;

	[ExportCategory("Weapons")]
	[Export] private WeaponConfig _slot1Config;
	[Export] private WeaponConfig _slot2Config;
	[Export] private WeaponConfig _slot3Config;
	[Export] private WeaponConfig _slot4Config;


	private readonly List<WeaponSlot> _slots = [];

	private WeaponConfig[] Configs => [_slot1Config, _slot2Config, _slot3Config, _slot4Config];

	public override void _Ready()
	{
		base._Ready();
		CollectSlots();

		if (Engine.IsEditorHint())
		{
			ShowPreviews();
			return;
		}

		WeaponConfig[] configs = Configs;
		for (int i = 0; i < _slots.Count; i++)
			_slots[i].Equip(configs[i], _blackboard);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (Engine.IsEditorHint())
			return;

		base._PhysicsProcess(delta);

		foreach (WeaponSlot weaponSlot in _slots)
		{
			if (!_aimingProvider.TryGetTarget(weaponSlot, out TargetInfo target))
				continue;

			weaponSlot.TryActivate(target, _blackboard.Kinematics.Velocity);
		}
	}

	public void SetWeapon(int slot, WeaponConfig config)
	{
		if (Engine.IsEditorHint())
			return;
		if (slot < 0 || slot >= _slots.Count)
			throw new ArgumentOutOfRangeException(nameof(slot));

		_slots[slot].Equip(config, _blackboard);
	}

	private void CollectSlots()
	{
		_slots.Clear();
		foreach (Node child in GetChildren())
		{
			if (child is WeaponSlot slot)
				_slots.Add(slot);
		}
	}

	private void ShowPreviews()
	{
		WeaponConfig[] configs = Configs;
		for (int i = 0; i < _slots.Count && i < configs.Length; i++)
			_slots[i].ShowPreview(configs[i]);
	}

	private void GenerateSlots()
	{
		for (int i = 1; i <= SlotCount; i++)
		{
			string slotName = $"Slot {i}";
			if (HasNode(slotName))
				continue;

			WeaponSlot slot = new() { Name = slotName };
			AddChild(slot, true);
			slot.Owner = GetTree().EditedSceneRoot;
		}

		CollectSlots();
		ShowPreviews();
	}
}
