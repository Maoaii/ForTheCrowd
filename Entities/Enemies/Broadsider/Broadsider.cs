using System;
using CarGame.Entities.Components;
using CarGame.Entities.Enemies.Swarm;
using Godot;

public partial class Broadsider : EnemyBase, ISwarmable, IVelocityHolder
{
	[ExportGroup("Health")]
	[Export] public HealthComponent HealthComponent;
	[Export] public HurtBox HurtBox;

	[ExportGroup("Visuals")]
	[Export] private GpuParticles3D _bloodParticles;
	[Export] private Decal _bloodDecal;

	public override void _Ready()
	{
		HurtBox.AddToGroup("TerrainTrackable");
		HurtBox.OnHit += HandleCollision;
		HealthComponent.HealthDepleted += Die;
		Swarm?.RegisterEntity(this);
	}

	private void HandleCollision(DamageInfo damageInfo)
	{
		HealthComponent.TakeDamage(damageInfo.Amount);
		SpawnBloodParticles();
	}

	private void Die()
	{
		SpawnBloodParticles();
		SpawnBloodDecal();
		QueueFree();
	}

	private void SpawnBloodParticles()
	{
		GpuParticles3D particles = _bloodParticles.Duplicate() as GpuParticles3D;
		GetTree().CurrentScene.AddChild(particles);
		particles.Owner = GetTree().CurrentScene;
		particles.GlobalPosition = _bloodParticles.GlobalPosition;
		particles.Emitting = true;

		GetTree().CreateTimer(particles.Lifetime).Timeout += particles.QueueFree;
	}

	private void SpawnBloodDecal()
	{
		Decal decal = _bloodDecal;
		_bloodDecal.Reparent(GetTree().CurrentScene);
		_bloodDecal.Owner = GetTree().CurrentScene;
		_bloodDecal.Visible = true;
		Tween tween = GetTree().CurrentScene.CreateTween();
		tween.TweenProperty(decal, "modulate:a", 0.0f, 3).SetTrans(Tween.TransitionType.Linear).SetEase(Tween.EaseType.InOut);

		tween.TweenCallback(Callable.From(decal.QueueFree));
	}
}
