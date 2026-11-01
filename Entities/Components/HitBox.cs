using System;
using Godot;

[GlobalClass]
public partial class HitBox : Area3D, IDamageProvider
{
	[Export] public int Damage;

	public bool TryGetDamage(out DamageInfo damageInfo)
	{
		damageInfo = new DamageInfo()
		{
			Amount = Damage
		};

		return true;
	}
}
