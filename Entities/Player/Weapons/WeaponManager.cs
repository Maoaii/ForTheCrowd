using System;
using System.Collections.Generic;
using CarGame.Entities.Components.AimingSystem;
using Godot;
using Godot.Collections;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class WeaponManager : Node3D
{
	[ExportToolButton("Generate weapon slots")]
	public Callable GenerateSlotsButton => Callable.From(GenerateSlots);

	[Export] public AimingProviderBase AimingProvider;
	[Export] public Array<WeaponConfig> WeaponConfigs;
	protected readonly List<WeaponSlot> _slots = [];

	public override void _Ready()
	{
		base._Ready();
		CollectSlots();

		if (Engine.IsEditorHint())
		{
			ShowPreviews();
			return;
		}
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
		for (int i = 0; i < WeaponConfigs.Count; i++)
			_slots[i].ShowPreview(WeaponConfigs[i]);
	}

	private void GenerateSlots()
	{
		for (int i = 1; i <= WeaponConfigs.Count; i++)
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