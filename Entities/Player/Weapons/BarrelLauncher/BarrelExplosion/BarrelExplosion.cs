using Godot;

namespace CarGame.Entities.Player.Weapons;

public partial class BarrelExplosion : Area3D
{
	[Export(PropertyHint.Range, "0.0,5.0,0.05")] public float PeakScaleLifetime;
	[Export(PropertyHint.Range, "0.0,5.0,0.05")] public float LowestAlphaLifetime;
	[Export] public MeshInstance3D mesh;
	[Export] public CollisionShape3D collisionShape;

	public override void _Ready()
	{
		Material meshMaterial = mesh.GetActiveMaterial(0);
		Scale = new Vector3(0.0f, 0.0f, 0.0f);
		Tween tween = CreateTween().SetEase(Tween.EaseType.Out).SetTrans(Tween.TransitionType.Back);

		tween.TweenProperty(this, "scale", new Vector3(1.0f, 1.0f, 1.0f), PeakScaleLifetime);
		tween.TweenCallback(Callable.From(() => collisionShape.Disabled = true));
		tween.TweenProperty(meshMaterial, "albedo_color:a", 0.0f, PeakScaleLifetime + LowestAlphaLifetime);
		tween.TweenCallback(Callable.From(QueueFree));
	}
}
