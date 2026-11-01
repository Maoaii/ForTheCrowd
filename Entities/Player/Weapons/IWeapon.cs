using CarGame.Systems.Blackboards;

namespace CarGame.Entities.Player.Weapons;

public interface IWeapon
{
	void Setup(WeaponConfig config, BlackboardComponent blackboard, bool enabled);
}
