using System;
using CarGame.Entities.Components;
using Godot;

[GlobalClass]
public partial class HurtBox : Area3D, IVelocityHolder
{
	public event Action<DamageInfo> OnHit;
	[Export] public float InvincibleTimeMs = 0.0f;
	[Export] public CharacterBody3D Owner;

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

	public Vector3 GetVelocity()
	{
		return Owner.Velocity;
	}
}
