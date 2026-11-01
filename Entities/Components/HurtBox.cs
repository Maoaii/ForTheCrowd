using System;
using Godot;

[GlobalClass]
public partial class HurtBox : Area3D
{
	public event Action<DamageInfo> OnHit;
	[Export] public float InvincibleTimeMs = 0.0f;

	private float _lastHitTime = 0.0f;
	public override void _Ready()
	{
		base._Ready();
		AreaEntered += GotHit;
	}

	private void GotHit(Area3D area)
	{
		if (area is not IDamageProvider)
			return;
		if (!(area as IDamageProvider).TryGetDamage(out DamageInfo damageInfo))
			return;

		float currentTime = Time.GetTicksMsec();
		if (currentTime - _lastHitTime < InvincibleTimeMs)
			return;

		_lastHitTime = currentTime;

		OnHit?.Invoke(damageInfo);
	}
}
