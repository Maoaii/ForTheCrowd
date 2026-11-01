using System;
using Godot;

public readonly record struct DamageInfo(int Amount = 0);

public partial interface IDamageProvider
{
	bool TryGetDamage(out DamageInfo damageInfo);
}
