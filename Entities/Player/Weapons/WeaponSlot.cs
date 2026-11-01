using CarGame.Entities.Components.AimingSystem;
using CarGame.Systems.Blackboards;
using Godot;

namespace CarGame.Entities.Player.Weapons;

[Tool]
[GlobalClass]
public partial class WeaponSlot : Node3D
{
	[Export] public bool Enabled = true;

	private Node3D _weapon;

	public void Equip(WeaponConfig config, BlackboardComponent blackboard = null)
	{
		Unequip();
		if (config?.WeaponScene == null)
			return;

		_weapon = config.WeaponScene.Instantiate<Node3D>();
		if (_weapon is IWeapon weapon)
			weapon.Setup(config, blackboard, Enabled);
		else
			GD.PushWarning($"{config.WeaponScene.ResourcePath} root does not implement IWeapon.");

		AddChild(_weapon);
		_weapon.GlobalPosition = GlobalPosition;
	}

	public void ShowPreview(WeaponConfig config)
	{
		Unequip();
		if (config?.WeaponScene == null)
			return;

		_weapon = config.WeaponScene.Instantiate<Node3D>();
		AddChild(_weapon);
	}

	public void Unequip()
	{
		if (_weapon == null)
			return;

		RemoveChild(_weapon);
		_weapon.QueueFree();
		_weapon = null;
	}

	public bool TryActivate(TargetInfo targetInfo, Vector3 ownerVelocity)
	{
		if (!Enabled)
			return false;
		if (_weapon is not ShooterWeaponBase shooter)
			return false;

		return shooter.TryActivate(targetInfo, ownerVelocity);
	}
}
