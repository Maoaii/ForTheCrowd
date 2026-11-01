using CarGame.Entities.Player.Weapons;
using CarGame.Systems.Blackboards;
using Godot;

public partial class Mower : Area3D, IDamageProvider, IWeapon
{
	public BlackboardComponent Blackboard { get; private set; }
	public MowerWeaponConfig WeaponConfig { get; private set; }
	private bool _isEnabled = true;
	public void Setup(WeaponConfig config, BlackboardComponent blackboard, bool enabled)
	{
		WeaponConfig = config as MowerWeaponConfig;
		Blackboard = blackboard;
		_isEnabled = enabled;
	}

	public bool TryGetDamage(out DamageInfo damageInfo)
	{
		damageInfo = default;
		if (!_isEnabled)
			return false;

		if (Blackboard.Kinematics.Speed < WeaponConfig.MinSpeed)
			return false;

		damageInfo = new DamageInfo()
		{
			Amount = WeaponConfig.Damage
		};

		return true;
	}


	private void OnAreaEntered(Area3D area)
	{
		Blackboard?.Events.OnEnemyRunOver?.Invoke();
	}
}
