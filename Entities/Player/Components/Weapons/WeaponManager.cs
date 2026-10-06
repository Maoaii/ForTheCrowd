using CarGame.Entities.Components.AimingSystem;
using CarGame.Systems.Blackboards;
using Godot;
using System;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class WeaponManager : Node3D
{
	[ExportToolButton("Generate slot positions")]
	public Callable GenerateSlotPositionsButton => Callable.From(GenerateSlotPositions); 

	[ExportCategory("References")]
	[Export] 
	private BlackboardComponent _blackboard;

	[Export]
	private AimingProviderBase _aimingProvider;


	private readonly List<WeaponSlot> _slots = [];
	private WeaponSlot _slot1Marker;
	private WeaponSlot _slot2Marker;
	private WeaponSlot _slot3Marker;
	private WeaponSlot _slot4Marker;

    public override void _Ready()
    {
        base._Ready();
		if (Engine.IsEditorHint())
			return;
		
		CollectEditorSlots();
    }

    public override void _PhysicsProcess(double delta)
    {
		if (Engine.IsEditorHint())
			return;
		
        base._PhysicsProcess(delta);
		if (_blackboard.Input.WantsActivateWeapon(out int slot)) {
			if (slot == -1 || _slots.Count - 1 < slot)
				return;
			
			WeaponSlot weaponSlot = _slots[slot - 1];

			weaponSlot.TryActivate(_aimingProvider.GetAimingDirection(weaponSlot));
		}
    }

	private void CollectEditorSlots()
	{
		foreach (Node child in GetChildren(true))
		{
			_slots.Add(child as WeaponSlot);
		}
	}

	public void SetWeapon(int slot)
	{
		if (Engine.IsEditorHint())
			return;
		throw new NotImplementedException();
	}

	
	private void GenerateSlotPositions()
	{
		_slots.Clear();
		foreach (Node child in GetChildren(true))
		{
			RemoveChild(child);
			child.QueueFree();
		}
		
		_slot1Marker = new WeaponSlot();
		AddChild(_slot1Marker, true, InternalMode.Disabled);
		_slot1Marker.Owner = GetTree().EditedSceneRoot;
		_slot1Marker.Name = "Slot 1";
		_slots.Add(_slot1Marker);

		_slot2Marker = new WeaponSlot();
		AddChild(_slot2Marker, true, InternalMode.Disabled);
		_slot2Marker.Owner = GetTree().EditedSceneRoot;
		_slot2Marker.Name = "Slot 2";
		_slots.Add(_slot2Marker);

		_slot3Marker = new WeaponSlot();
		AddChild(_slot3Marker, true, InternalMode.Disabled);
		_slot3Marker.Owner = GetTree().EditedSceneRoot;
		_slot3Marker.Name = "Slot 3";
		_slots.Add(_slot3Marker);

		_slot4Marker = new WeaponSlot();
		AddChild(_slot4Marker, true, InternalMode.Disabled);
		_slot4Marker.Owner = GetTree().EditedSceneRoot;
		_slot4Marker.Name = "Slot 4";
		_slots.Add(_slot4Marker);
	}
}
